using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

/*
 * プリミティブ構文解析 
 *  .lexファイルを作らずに処理自体をここに記載。
 * 
 */
namespace slagtool
{
    public class lexPrimitive
    {
        //計算記号  2文字を先に。
        public static string[] operators = {"++","--","==","!=","<=",">=","+=","-=","*=","/=","%=","&&","||","=","+","-","*","/","%",">","<","!",",","."}; //未サポート含む

        string m_src;
        string[] m_lines;
        int      m_l;
        int      m_c;

        List<YVALUE> m_curline_value;
        public List<List<YVALUE>> m_value_list;  //行単位

        public void Init(string src)
        {
            m_src = src;
            m_lines = m_src.Split('\x0a');
            m_l = 0;
            m_c = 0;
            m_value_list = new List<List<YVALUE>>();
        }

        void add(YVALUE v)
        {
            if (m_curline_value == null) m_curline_value = new List<YVALUE>();
            m_curline_value.Add(v);
        }

        void end_line()
        {
            if (m_curline_value!=null) m_value_list.Add(m_curline_value);
            m_curline_value = null;
        }

        public bool CheckOne() //no more then return false;
        {
            if (m_l >= m_lines.Length)
            {
                var vtmp = new YVALUE();
                vtmp.type = YDEF.EOF;
                add(vtmp);
                end_line();
                return false;
            }
            
            int wdlen;
            var v = lexUtil.GetWord(out wdlen, m_c, m_lines[m_l],m_l);
            m_c += wdlen;

            add(v);

            if (v.type == YDEF.ERROR)
            {
                sys.error(string.Format("L:{0}C:{1}>{2}",v.dbg_line,v.dbg_col,v.s));
                return false;
            }
            
            if (v.type == YDEF.EOL)
            {
                end_line();
                m_l++;
                m_c=0;
            }
            
            return true;
        }
    }

    public class lexUtil //汎用で使う。
    {
        public static List<List<YVALUE>> lexSource(string src)
        {
            const int LOOPMAX = 10000;

            var lex = new lexPrimitive();
            lex.Init(src);
            for(var loop = 0; loop<=LOOPMAX; loop++)
            {
                if (loop == LOOPMAX)  sys.error("lexSource LOOP MAX");
                if (!lex.CheckOne())
                { 
                    break;
                }
            }

            return lex.m_value_list;
        }

        public static YVALUE GetWord(out int wdlen, int col, string i_line, int dbg_line = -1)
        {
            var v= new YVALUE();

            v.type = YDEF.UNKNOWN;
            v.dbg_col  = col;
            v.dbg_line = dbg_line;

            wdlen = 0;
            if (string.IsNullOrEmpty(i_line)) return any_return(v,YDEF.EOL);

            var line = i_line.TrimEnd();
            if (string.IsNullOrEmpty(line) || col >= line.Length) return any_return(v,YDEF.EOL);

            var ls = i_line.Substring(col);
            if (string.IsNullOrEmpty(ls)) return any_return(v,YDEF.EOL);

            //コメントは全部
            if (ls.StartsWith(YDEF.CMTSTR))
            {
                wdlen = ls.Length;
                return any_return(v,YDEF.CMT,null,ls);
            }

            //ダブルクォーテーションで囲まれた文字列はそのまま
            if (ls.StartsWith(YDEF.DQ))
            {
                if (ls.Length > 1)
                {
                    var idx = ls.IndexOf(YDEF.DQ, 1);
                    if (idx < 0)
                    {
                        return err_return(v, "End of Double Quatation is not found:1");
                    }
                    wdlen = idx + 1;
                    return any_return(v,YDEF.QSTR,null,ls.Substring(0, idx + 1));
                }
                else
                {
                    return err_return(v,"End of Double Quation is not found:2");
                }
            }

            //連続したスペース・タブはそのまま
            if (ls[0] <= ' ')
            {
                for (var i = 0; i < ls.Length; i++)
                {
                    if (ls[i] > ' ')
                    {
                        wdlen = i;
                        return any_return(v,YDEF.SP,null," ");
                    }
                }
                return any_return(v,YDEF.EOL);
            }

            //数字
            if (IsNumberElement(ls[0]))
            {
                wdlen = 0;
                string s = "" + ls[0];
                for (int i = 1; i < ls.Length; i++)
                {
                    if (IsNumberElement(ls[i]))
                    {
                        s += ls[i];
                    }
                    else
                    {
                        wdlen = i;
                        break;
                    }
                }
                if (wdlen == 0) wdlen = ls.Length;
                double d;
                if (double.TryParse(s, out d))
                {
                    return any_return(v, YDEF.NUM, d, s);
                }
                else if (s==".")
                {
                    return any_return(v,YDEF.PERIOD,null,".");
                }
                else 
                {
                    return err_return(v,s);
                }
            }

            //名前
            if (IsNameElement(ls[0]))
            {
                wdlen = 0;
                string s = "" + ls[0];
                for (int i = 1; i < ls.Length; i++)
                {
                    if (IsNameElement(ls[i], true))
                    {
                        s += ls[i];
                    }
                    else
                    {
                        wdlen = i;
                        break;
                    }
                }
                if (wdlen == 0) wdlen = ls.Length;

                switch(s.ToUpper())
                {
                case "FUNCTION": return any_return(v,YDEF.FUNCTION,null,s);
                case "VAR"     : return any_return(v,YDEF.VAR,null,s);
                case "IF"      : return any_return(v,YDEF.IF,null,s);
                case "ELSE"    : return any_return(v,YDEF.ELSE,null,s);
                case "FOR"     : return any_return(v,YDEF.FOR,null,s);
                case "WHILE"   : return any_return(v,YDEF.WHILE,null,s);
                case "SWITCH"  : return any_return(v,YDEF.SWITCH,null,s);
                case "CASE"    : return any_return(v,YDEF.CASE,null,s);
                case "DEFAULT" : return any_return(v,YDEF.DEFAULT,null,s);
                case "BREAK"   : return any_return(v,YDEF.BREAK,null,s);
                case "CONTINUE": return any_return(v,YDEF.CONTINUE,null,s);
                case "RETURN"  : return any_return(v,YDEF.RETURN,null,s);
                case "NEW"     : return any_return(v,YDEF.NEW,null,s);

                case "TRUE"    : return any_return(v,YDEF.BOOL,null,s);
                case "FALSE"   : return any_return(v,YDEF.BOOL,null,s);

                case "NULL"    : return any_return(v,YDEF.NULL,null,s);
                }

                return any_return(v,YDEF.NAME,null,s);
            }
            
            //計算記号  2文字を先に。 "="は除外
            string[] ops = lexPrimitive.operators;//{"++","--","==","!=","<=",">=","+=","-=","*=","/=","%=","&&","||","=","+","-","*","/","%",">","<","!",",","."}; //未サポート含む
            foreach(var op in ops)
            {
                if (op.Length==2 && ls.Length>=2)
                {
                    var s = ls.Substring(0,2);
                    if (op == s)
                    {
                        wdlen = 2;
                        return op_return(v,s);
                    }
                }
                if (op.Length==1 && ls.Length >= 1)
                {
                    var s = ls.Substring(0,1);
                    if (op == s)
                    {
                        wdlen = 1;
                        return op_return(v,s);
                    }
                }
            }

            //その他　記号とみなす
            wdlen = 1;

            return any_return(v,YDEF.OTR,null,""+ls[0]);
        }

        // -- util for this clas --
        static YVALUE err_return(YVALUE v, string s)
        {
            v.type = YDEF.ERROR;
            v.o = v.s = s;
            return v;
        }
        static YVALUE op_return(YVALUE v, string s)
        {
            // 参照 wikipedia:「演算子の優先順位」

            v.s = s;
            v.o = v.s;
            int type = YDEF.UNKNOWN;

            string[] op3  = {"*","/","%" };
            string[] op4  = {"+","-" };
            string[] op6  = {"<","<=",">",">="};
            string[] op7  = {"==","!="};
            string[] op11 = {"||"};
            string[] op12 = {"&&"};
            //string[] op14 = {"," };

            string[] incop  = { "++", "--"};
            string[] asinop = { "=","+=","-=","*=","/=","%=" };

            string   comma  = ",";
            string   period = ".";  


            if (type==YDEF.UNKNOWN && Array.FindIndex(op3, i=>i==s)>=0)   type =YDEF.OP3;
            if (type==YDEF.UNKNOWN && Array.FindIndex(op4, i=>i==s)>=0)   type =YDEF.OP4;
            if (type==YDEF.UNKNOWN && Array.FindIndex(op6, i=>i==s)>=0)   type =YDEF.OP6;
            if (type==YDEF.UNKNOWN && Array.FindIndex(op7, i=>i==s)>=0)   type =YDEF.OP7;
            if (type==YDEF.UNKNOWN && Array.FindIndex(op11,i=>i==s)>=0)   type =YDEF.OP11;
            if (type==YDEF.UNKNOWN && Array.FindIndex(op12,i=>i==s)>=0)   type =YDEF.OP12;
            //if (type==YDEF.UNKNOWN && Array.FindIndex(op14,i=>i==s)>=0)   type =YDEF.OP14;
            if (type==YDEF.UNKNOWN && Array.FindIndex(incop,i=>i==s)>=0)  type =YDEF.INCOP;
            if (type==YDEF.UNKNOWN && Array.FindIndex(asinop,i=>i==s)>=0) type =YDEF.ASINOP;
            if (type==YDEF.UNKNOWN)
            {
                if      (s==period) type = YDEF.PERIOD;                 
                else if (s==comma)  type = YDEF.COMMA;  
                else                type = YDEF.OP;
            }

            v.type = type;

            return v;
        }
        static YVALUE any_return(YVALUE v, int type, double? n=null, string s=null)
        {
            v.type = type;
            if (n!=null)
            {
                v.o = v.n = (double)n;
            }
            if (s!=null)
            {
                v.s = s;
                if (v.o==null) v.o = v.s;
            }
            if (type == YDEF.BOOL)
            {
                v.o = (bool)(s.ToUpper()=="TRUE");
            }
            if (v.o==null) v.o = type;
            return v;
        }

        static bool IsNumberElement(char c)
        {
            return (c >= '0' && c <= '9') || (c == '.');
        }
        static bool IsNameElement(char c, bool bNumberInclude = false)
        {
            if (c == '_') return true;
            if (c >= 'a' && c <= 'z') return true;
            if (c >= 'A' && c <= 'Z') return true;
            if (bNumberInclude && ((c >= '0' && c <= '9'))) return true;

            return false;
        }
    }
}
