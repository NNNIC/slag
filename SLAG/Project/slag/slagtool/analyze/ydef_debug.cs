using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace slagtool
{
    public class YDEF_DEBUG
    {
        public static int                  save_startIndex;    //解析時の開始インデックス
        public static int                  save_endindex;      //解析時の終了インデックス
        public static YVALUE               current_v;          //実行中のＶ
        public static runtime.StateBuffer  current_sb;         //ステートバッファ
        public static List<int>            breakpoints;        //ブレイクポイント 
        public static bool                 bForceToStop;       //
        public static int                  stoppedLine;        

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
            foreach(var l in list)
            {
                DumpLine_detail(l,bOmitTerminalType);
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
        public static void PrintListValue(List<YVALUE> l)
        {
            var s = "";
            l.ForEach(v=>s+=PrintValue(v)); 
            sys.logline(s);
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
        public static void PrintLineAndCol(List<YVALUE> l)
        {
            if (l==null || l.Count==0) { sys.log("Line:?,Col:?"); return; }
            var v = l[0];
            var line = v.get_dbg_line();
            var col  = v.get_dbg_line();

            sys.log( string.Format("Line:{0},Col:{1}",line,col));
        }
        #endregion

        #region runtime error_info
        public static string RuntimeErrorInfo()
        {
            if (current_v==null) return null;

            string s = null;

            s += "Error at line:" + (current_v.get_dbg_line() + 1);
             
            return s;            
        }
        #endregion
    }
}
