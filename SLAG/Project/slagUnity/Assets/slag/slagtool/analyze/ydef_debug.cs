using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using LIST = System.Collections.Generic.List<object>;
using number = System.Double;

namespace slagtool
{
    public partial class YDEF_DEBUG
    {
        // 便宜
        static string   TMPFILENAME { get { return slag.TMPFILENAME; } }

        static slag     m_slag      { get { return slag.m_latest_slag;  } }
        static Filelist m_filelist  { get { return (m_slag!=null) ? m_slag.m_filelist : null;   } }

        #region [  ANALIZE  ]
        #region Check executable
        public static bool IsExecutable(List<YVALUE> list, out List<int> errorline)
        {
            errorline = null;
            if (list.Count==1 && list[0].IsType(YDEF.get_type(YDEF.sx_main_block)))
            {
                return true;
            }

            List<int> errlist = new List<int>();

            for (var i = save_startIndex; i<save_endindex; i++)
            {
                var v = list[i];
                if (v.type == YDEF.BOF || v.type == YDEF.EOF) continue;
                if (v.type < (int)TOKEN.MAX)
                {
                    var el = v.get_dbg_line();
                    errlist.Add(el);
                    continue;
                }
                if (
                    v.IsType(YDEF.sx_sentence_block) || v.IsType(YDEF.sx_sentence_list) || v.IsType(YDEF.sx_sentence)
                    )
                {
                    continue;
                }
                else
                {
                    var el = v.get_dbg_line();
                    errlist.Add(el);
                    continue;
                }
            }

            errorline = errlist.Distinct().ToList();
            
            return errlist.Count == 0;
        }
        #endregion

        #region Dump
        public static void DumpList(List<List<YVALUE>> list, bool bOmitTerminalType = false)
        {
            if (YDEF_DEBUG.level >=2)
            {
                foreach(var l in list)
                {
                    DumpLine_detail(l,bOmitTerminalType);
                }
            }
        }
        public static void DumpLine_detail(List<YVALUE> l,bool bOmitTerminalType=false)
        {
            // [type|?|0[]1[]2[]
            string s =null;
            Action<YVALUE> work = null;
            work = (v) => {
                var tm = v.GetTerminal();
                var tn = v.get_type_name();
                if ((v.type < (int)TOKEN.MAX) && bOmitTerminalType)
                { 
                    tn = "";
                }

                if (tn=="sx_sentence") s+=Environment.NewLine;
                s += "[";
                s +=  tn.Replace("sx_","") + (tm!=null ? ("`" + tm + "`") :"");

                if (v.list!=null)
                {
                    for(int i = 0; i<v.list.Count; i++)
                    {
                        if (v.list.Count>1) s+=i.ToString();
                        work(v.list[i]);
                    }
                }

                s +="]";
                if (tn=="sx_sentence") s+=Environment.NewLine;
            };
            
            l.ForEach(i=>work(i));

            sys.logline(s);
        }
        #endregion

        #region Print
        public static void PrintListValue(List<YVALUE> l,bool bForce=false)
        {
            if (YDEF_DEBUG.level>=2)
            {
                var s = GetStringListValue(l);
                sys.logline(s,bForce);
            }
        }
        public static string GetStringListValue(List<YVALUE> l)
        {
            var s = "";
            if (l==null) return "GetStringListValue:引数にNULLが設定されています";
            l.ForEach(v=>s+=PrintValue(v)); 
            return s;
        }
        public static string PrintValue(YVALUE v)
        {
            foreach(var e in Enum.GetValues(typeof(TOKEN)))
            {
                var i = (int)e;
                if (i>0 && i<(int)TOKEN.MAX)
                {
                    YVALUE find = v.IsType(i) ? v.FindValueByTravarse(i) : null;
                    if (find!=null)
                    {
                        if (i==(int)TOKEN.BOF || i==(int)TOKEN.EOF) return "----" + NL; 
                        return find.o.ToString();
                    }
                }
            }

            Func<string,string> conv = (j) => {
                if (j==";") return j  + NL;
                if (j=="{") return NL + j + NL;
                if (j=="}") return j  + NL;
                if (string.IsNullOrEmpty(j)) return j;
                return  j.StartsWith(" ") ? j : " " + j ; 
            };

            string s = "";
            if (v.list!=null)
            {
                v.list.ForEach(i=>s+=conv(PrintValue(i)));
            }
            return s;
        }
        public static void PrintLineAndCol(List<YVALUE> l,bool bForce=false)
        {
            sys.logline(GetLineAndCol(l),bForce);
        }
        public static string GetLineAndCol(List<YVALUE> l)
        {
            if (l==null || l.Count == 0) return GetLineAndCol((YVALUE)null);
            var v = l[0];
            return GetLineAndCol(v);
        }
        public static string GetLineAndCol(YVALUE v)
        {
            if (v==null) return "Line:?,Col:?";
            var line = v.get_dbg_line(true);
            var col  = v.get_dbg_col(true);
            var file = v.get_dbg_file();

            return string.Format("Line:{0},Col:{1} in {2}",line,col,file);
        }
        #endregion

        #region Error Hint
        public static string ErrorHint(List<YVALUE> l)
        {
            if (l==null) return null;
            //lの要素がsentence_listでないもの
            foreach(var v in l)
            {
                if (v.IsType(YDEF.BOF)) continue;
                if (v.IsType(YDEF.sx_sentence_list)) continue;
                if (v.IsType(YDEF.sx_sentence)) continue;
                return "Error Hint:" + GetLineAndCol(v);
            } 
            return null;       
        }


        #endregion

        #endregion

        #region [  RUNTIME  ]

        public static bool                      bEnable { get { return sys.DEBUGMODE;  } } //便宜
        public static int                       level   { get { return sys.DEBUGLEVEL; } } //便宜

        public static int                       save_startIndex;    //解析時の開始インデックス
        public static int                       save_endindex;      //解析時の終了インデックス
        public static YVALUE                    current_v;          //実行中のＶ
        public static runtime.StateBuffer       current_sb;         //ステートバッファ


        public static bool                      bPausing;           //
        public static int                       stoppedLine;        //

        public enum STEPMODE
        {
            None    =0,
            StepIn   ,
            StepOver
        }
        public static STEPMODE             stepMode;           //ステップ実行中
        public static int                  funcCntSmp {        //ファンクション用カウンタセマフォ 
            get {
                if (stepMode!= STEPMODE.StepOver)
                {
                    __funcntsmp = 0;
                }
                return __funcntsmp;
            }
            set
            {
                if (stepMode!= STEPMODE.StepOver)
                {
                    __funcntsmp = 0;
                }
                else
                {
                    __funcntsmp = value;
                }
                return;
            }
        }
        private static int                 __funcntsmp;          
        public static bool                 bRequestAbort;      //停止リクエスト

        public static string NL { get {return Environment.NewLine; } }

        #region Set Breakpoint
        //public static void AddBreakpoint(int line, int file_id)
        //{
        //    if (breakpoints==null) breakpoints = new Dictionary<int, List<int>>();
        //    List<int> linelist = breakpoints.ContainsKey(file_id) ? breakpoints[file_id] : new List<int>();
        //    if (linelist.Contains(line)) return;
        //    linelist.Add(line);
        //    linelist.Sort();
        //    if (breakpoints.ContainsKey(file_id)) {
        //        breakpoints[file_id] = linelist;
        //    }
        //    else
        //    {
        //        breakpoints.Add(file_id,linelist);
        //    }
        //}
        //public static bool DelBreakpoint(int line, int file_id)
        //{
        //    if (breakpoints==null) return false;
        //    if (!breakpoints.ContainsKey(file_id)) return false;
        //    var list = breakpoints[file_id];
        //    if (list.Contains(line))
        //    {
        //        list.Remove(line);
        //        return true;
        //    }
        //    return false;
        //}
        //public static void ResetAllBreakpoints()
        //{
        //    breakpoints = null;
        //}
        #endregion

        public static void DumpCurrentVariables(runtime.StateBuffer bf)
        {
            var depth = 0;
            Action<Hashtable> trvs = null;
            trvs = (t)=> {
                depth++;
                string title = null;
                if (t==bf.m_root_dic) { title = depth.ToString("00") + " ROOT ";}
                else if (depth==1) { title = "01 FRONT"; }
                else {title = depth.ToString("00"); }

                sys.logline("@--- [" + title + "] ---");
                
                foreach(var k in t.Keys)
                {
                    var ks = k.ToString();
                    if (ks[0]=='!') continue;
                    _dumpObj(ks,t[k]);
                }
                var po = t[runtime.StateBuffer.KEY_PARENT];
                if (po!=null)
                {
                    trvs((Hashtable)po);
                }
            };

            trvs(bf.m_front_dic);
        }


        private static void _dumpObj(string name,object o)
        {
            if (o is IList)
            {
                string s = null;
                var l = (IList)o;
                foreach(var i in l)
                {
                    if (s!=null)
                    {
                        s +=",";
                    }
                    s+= i!=null ? i.ToString() : "-null-";
                }
                sys.logline("@"+ name+" = " + s);
                return;
            }
            if (o is Hashtable)
            {
                var t = (Hashtable)o;
                foreach(var k in t.Keys)
                {
                    var c = t[k];
                    string s = c!=null ? c.ToString() : "-null-";
                    sys.logline("@" + name + "." + k.ToString() + " = " + s );
                }
                return;
            }
            string a = o!=null ? o.ToString() : "-null-";
            sys.logline("@" + name + " = " + a);
        }
        #endregion//[RUNTIME]

        #region runtime error_info
        public static string RuntimeErrorInfo()
        {
            if (current_v==null) return null;

            var fid = current_v.dbg_file_id;
            string file = null;
            try { 
                file = m_slag.m_filelist.GetFile(fid); //　　　　 _slag!=null &&  fid>=0 && m_slag.m_idlist.Length > fid ? m_slag.m_idlist[fid] : "";
            }
            catch { file = null; }

            string s = null;

            s += "Error at line:" + (current_v.get_dbg_line(true)) + " in File:" + file + NL + RuntimeSyncInfo();
             
            return s;            
        }
        #endregion

        #region runtime sync source info
        public static string RuntimeSyncInfo() //デバッガ同期用
        {
            if (current_v==null) return null;
            int line, fileid;
            current_v.get_dbg_id_line(out fileid,out line);
            if (fileid>=0 && line>=0)
            { 
                string fn = null;
                if (slag.m_latest_slag!=null && slag.m_latest_slag.m_filelist!=null)
                { 
                    fn = slag.m_latest_slag.m_filelist.GetFile(fileid);
                }

                //line ++;

                if (fn==TMPFILENAME && m_slag!=null)
                {
                    return TMPFILENAME + m_slag.m_script_base64 + "\n" + "[SS$L:<L"+line+ ">]";
                }

                return "[SS$L:<L"+line + ">,N:" + fn +"]";
            }
            return null;
        }
        #endregion
    }
}
