using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using slagtool;

namespace slagremote
{
    public class cmd_sub
    {
        // 便宜
        private static string             TMPFILENAME { get { return slag.TMPFILENAME;                                                                } } 
        private static slagunity          m_slagunity { get { return slagremote_unity_manager.V!=null  ? slagremote_unity_manager.V.m_slagunity:null; } }
        private static Filelist           m_fileList  { get { return slag.m_latest_slag!=null ? slag.m_latest_slag.m_filelist:null;                   } }

        private static  Dictionary<int,YDEF_DEBUG.BPITEM> m_breakpoints { get { YDEF_DEBUG.NormalizeBp();  return YDEF_DEBUG.breakpoints;   } }

        public static slagtool.slag Load(string path, string[] files)
        {
            var file_list = new slagtool.Filelist();//new List<string>();
            file_list.root = path;

            for (var i = 0; i<files.Length; i++)
            {
                var file = files[i];
                if (!file.ToUpper().EndsWith(".JS"))
                {
                    wk.SendWriteLine("ERROR:File is not JS :" + file );
                    return null;
                }
                file_list.filesAdd(file);
            }

            if (slagtool.sys.USETRY)
            {
                try
                {
                    m_slagunity.LoadJSFiles(file_list);
                }
                catch(SystemException e)
                {
                    wk.SendWriteLine("-- EXCEPTION --");
                    wk.SendWriteLine(e.Message);
                    wk.SendWriteLine("---------------");
                    return null;
                }
            }
            else
            {
                m_slagunity.LoadJSFiles(file_list);
            }
            wk.SendWriteLine("Loaded.");

            wk.SendWriteLine("Checksum:" + m_slagunity.GetMD5());

            return  m_slagunity.m_slag;
        }
        public static slagtool.slag Load(string path, string file)
        {
            string fullpath = null;
            try
            {
                fullpath = Path.Combine(path,file);
            }
            catch
            {
                wk.SendWriteLine("ERROR:Unexpcted path name");
                return null;
            }

            if (fullpath==null)
            {
                wk.SendWriteLine("ERROR:File name is null!");
                return null;
            }
            var ext = Path.GetExtension(fullpath).ToUpper();
            if (ext!=".JS" && ext!=".BIN" && ext!=".BASE64")
            {
                wk.SendWriteLine("ERROR:File name is not allowed");
                return null;
            }
            if (!File.Exists(fullpath))
            {
                wk.SendWriteLine("ERROR:File does not exist!");
            }

            bool bJS = ext == ".JS";

            //m_slagunity = null;

            if (slagtool.sys.USETRY)
            {
                try
                {
                    if (bJS)
                    { 
                        m_slagunity.LoadJSFiles(new slagtool.Filelist(path,file));
                    }
                    else
                    {
                        m_slagunity.LoadFile(fullpath);
                    }
                }
                catch(SystemException e)
                {
                    wk.SendWriteLine("-- 例外発生 --");
                    wk.SendWriteLine(e.Message);
                    wk.SendWriteLine("---------------");
                    return null;
                }
            }
            else
            {
                if (bJS)
                { 
                    m_slagunity.LoadJSFiles(new slagtool.Filelist(path,file));
                }
                else
                {
                    m_slagunity.LoadFile(fullpath);
                }
            }
            wk.SendWriteLine("Loaded.");

            wk.SendWriteLine("Checksum:" + m_slagunity.GetMD5());

            return m_slagunity.m_slag;
        }

        public static void SaveBin(string path, string file)
        {
            if (m_slagunity==null || m_slagunity.m_slag==null)
            {
                wk.SendWriteLine("データがありません");
                return;
            }
            try { 
                var fp = Path.Combine(path,file);
                m_slagunity.m_slag.SaveBin(Path.Combine(path,file));
                wk.SendWriteLine("セーブしました ファイル:"+ fp);
            } catch (SystemException e)
            {
                wk.SendWriteLine("-- 例外発生 --");
                wk.SendWriteLine(e.Message);
                wk.SendWriteLine("---------------");
            }
        }
        public static void SaveBase64(string path, string file)
        {
            if (m_slagunity==null || m_slagunity.m_slag==null)
            {
                wk.SendWriteLine("データがありません");
                return;
            }
            try { 
                var fp = Path.Combine(path,file);
                m_slagunity.m_slag.SaveBase64(Path.Combine(path,file));
                wk.SendWriteLine("セーブしました ファイル:"+ fp);
            } catch (SystemException e)
            {
                wk.SendWriteLine("-- 例外発生 --");
                wk.SendWriteLine(e.Message);
                wk.SendWriteLine("---------------");
            }
        }


        public static void Run()
        {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            if (slagtool.sys.USETRY)
            {
                try { 
                    m_slagunity.m_slag.Run();
                } 
                catch(SystemException e)
                {
                    wk.SendWriteLine("-- 例外発生 --");
                    wk.SendWriteLine(e.Message);
                    if (slagtool.YDEF_DEBUG.current_v!=null) wk.SendWriteLine("Stop at Line:" + slagtool.YDEF_DEBUG.current_v.get_dbg_line(true).ToString() );
                    wk.SendWriteLine("---------------");
                }
            }
            else
            {
                m_slagunity.m_slag.Run();
            }
            sw.Stop();
            wk.SendWriteLine("! The program exection time : " + ((float)sw.ElapsedMilliseconds / 1000f).ToString("F3") + "sec !");

        }

        public static void Reset()
        {
            slagtool.YDEF_DEBUG.ResetAllBreakpoints();//BPクリア
            slagtool.YDEF_DEBUG.bPausing = false;     //ポーズOFF

            if (slagremote_unity_manager.V.m_reset_callback!=null)
            {
                slagremote_unity_manager.V.m_reset_callback();
            }
        }
        #region BP
        //public static int? m_curFild_id=null; //base 0
        public static void BP(string[] plist)
        {
            var NL = Environment.NewLine;

            if (plist==null||plist.Length==0)
            {
                BP_List();
                return;
            }

            var p0 = plist[0].ToLower();
            string p1 = plist.Length > 1 ? plist[1].ToLower() : null;
            string p2 = plist.Length > 2 ? plist[2].ToLower() : null;
            if (Array.FindIndex(new string[] {"?","h","help"}, i=>i==p0) >= 0)
            {
                var helpmsg = 
                                "bp - ブレイクポインタのリスト表示                         " + NL +
                                "bp c|clear|r|reset - ブレイクポインタのクリア             " + NL +
                                "bp d NUM [FID|\"ファイル名\"]- NUM行のブレイクポインタ削除                    " + NL +
                                "bp NUM [FID|\"ファイル名\"]　- カレントファイルのnum行目にブレイクポインタ設定  " + NL +
                                "bp f FID - file FIDのファイルに変更                       " + NL +
                                "bp f - BP設定対象のファイル名表示                         " + NL +
                                "                                                          " + NL +
                                "bpを設定すると debug 1も同時設定される。                  " + NL ;

                wk.SendWriteLine(helpmsg);
                return;
            }

            if (Array.FindIndex(new string[] {"c","clear","r","reset" }, i=>i==p0) >=0)
            {
                slagtool.YDEF_DEBUG.ResetAllBreakpoints();
                wk.SendWriteLine("全てのブレイクポイントをクリアしました。");
                return;
            }

            if (p0 == "f")
            {
                if (p1==null)
                {
                    wk.SendWriteLine("カレントファイル:" + slagtool.YDEF_DEBUG.cur_filename);
                    return;
                }
                var num = intparse(p1);
                if (num==null||(int)num<=0)
                {
                    wk.SendWriteLine("削除の行番号が不正です。");
                    return;
                }
                var dnum = (int)num - 1;
                slagtool.YDEF_DEBUG.cur_file_id = dnum;
                wk.SendWriteLine("カレントファイルを変更しました。カレントファイル:" + slagtool.YDEF_DEBUG.cur_filename);
                return;
            }

            if (p0 == "d" && !string.IsNullOrEmpty(p1))
            {
                var num = intparse(p1);
                if (num==null||(int)num<=0)
                {
                    wk.SendWriteLine("削除の行番号が不正です。");
                    return;
                }
                int dnum = (int)num - 1;
                var b = slagtool.YDEF_DEBUG.DelBreakpoint(dnum,p2);
                if (b)
                {
                    wk.SendWriteLine("削除しました。");
                }
                else
                {
                    wk.SendWriteLine("削除の入力が不正です。");
                }
                return;
            }

            if (!string.IsNullOrEmpty(p0))
            {
                var num = intparse(p0);
                if (num==null||(int)num<=0)
                {
                    wk.SendWriteLine("設定行番号が不正です。");
                    return;
                }
                int dnum = (int)num - 1;

                slagtool.YDEF_DEBUG.AddBreakpoint(dnum,p2);
                wk.SendWriteLine("設定しました。");

                if (slagtool.sys.DEBUGLEVEL==0)
                {
                    slagtool.util.SetDebugLevel(1);
                    wk.SendWriteLine("デバッグレベル１を設定しました。");
                }

                return;
            }
        }
        private static void BP_List()
        {
            if (slagtool.YDEF_DEBUG.breakpoints==null || slagtool.YDEF_DEBUG.breakpoints.Count==0)
            {
                wk.SendWriteLine("ブレイクポインタは設定されていません。");
                return;
            }
            var keylist = slagtool.YDEF_DEBUG.GetSortBpKeys(); // new List<int>(slagtool.YDEF_DEBUG.breakpoints.Keys);
            if (keylist==null)
            {
                wk.SendWriteLine("ブレイクポインタは設定されていません。");
                return;
            }
            for(int i = 0; i<keylist.Count; i++)
            {
                var k = keylist[i];
                var item = slagtool.YDEF_DEBUG.breakpoints[k];
                wk.SendWriteLine("====" + i.ToString("00") + ":" + item.filename );
                var lines = item.lines;   //new List<int>(slagtool.YDEF_DEBUG.breakpo xsints[k]);
                lines.Sort();
                for(int n = 0; n<lines.Count;n++)
                {
                    wk.SendWriteLine("Line:" + (lines[n]+1));
                }
                wk.SendWriteLine("===");
            }
        }
        #endregion

        #region STOP and RESUME
        public static void Stop()
        {
            slagtool.YDEF_DEBUG.bPausing = true;
        }
        public static void Resume()
        {
            slagtool.YDEF_DEBUG.bPausing = false;
        }
        public static void Step(string p)
        {
            slagtool.YDEF_DEBUG.stepMode = !string.IsNullOrEmpty(p) && p.ToUpper()[0]=='I' ? slagtool.YDEF_DEBUG.STEPMODE.StepIn : slagtool.YDEF_DEBUG.STEPMODE.StepOver;
            slagtool.YDEF_DEBUG.bPausing = false;
        }
        #endregion


        public static void Test()
        {

            wk.SendWriteLine("...Test returned!");
        }

        public static void Debug(string p)
        {
            int x = -1;
            if (!string.IsNullOrEmpty(p) && int.TryParse(p,out x) && x>=0 && x<=2)
            {
                wk.SendWriteLine("Set Debug Level : " + x);
                slagtool.util.SetDebugLevel(x);
#if UNITY_5
                UnityEngine.Debug.logger.logEnabled = (x>0);
#endif
            }
            else
            {
                wk.SendWriteLine("Current Debug Level : " + slagtool.util.GetDebugLevel());
            }
        }

        public static void Help()
        {
            var s = slagremote.cmd_data_table.GetHelpAll();
            s  += slagtool.runtime.builtin.builtin_func.Help();
            wk.SendWriteLine(s);
        }

        public static void GetPlayText()
        {
            var s = slagunity.m_script;
            if (!string.IsNullOrEmpty(s))
            { 
                var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(s));
                wk.SendWriteLine(TMPFILENAME + base64);
            }
            else
            {
                wk.SendWriteLine("No Text");
            }
        }

        //[BPLIST:[ファイル名:行,行,・・]|[ファイル名:行,行,・・]|[ファイル名:行,行,・・]|・・・]
        public static void GetBp()
        {
            var bplist = m_breakpoints;
            if (bplist==null)
            {
                wk.SendWriteLine("[BPLIST:]");
                return;
            }

            string sf = null;
            foreach(var p in  bplist)
            {
                var item = p.Value;

                string ls = null;
                foreach(var l in item.lines)
                {
                    if (ls!=null) ls += ",";
                    ls += (l+1).ToString();
                }
                    
                if (sf!=null) sf += "|";
                sf += string.Format("[{0}:{1}]",item.filename,ls);
            }
            wk.SendWriteLine("[BPLIST:" + sf + "]");
        }

        //
        public static void ListFile()
        {
            try {
                var list = slagtool.slag.m_latest_slag.m_filelist;

                wk.SendWriteLine("== TARGET FILES ==");
                for(int i = 0; i<list.Count; i++)
                {
                    var s = string.Format("{0}:{1}",(i+1).ToString(), list.GetFile(i));
                    wk.SendWriteLine(s);
                }
                wk.SendWriteLine("==================");
            }
            catch (SystemException e)
            {
                wk.SendWriteLine("Error ListFile: " + e.Message);
            }
        }

        public static void TransferFileData() //リモートに対象のファイルデータを送る
        {
            try {
                var list = slagtool.slag.m_latest_slag.m_filelist;
                
                if (
                    list==null
                        ||
                    list.Count==0
                        ||
                    (list!=null && list.Count==1 && list.GetFile(0) == TMPFILENAME)
                    )
                {
                    GetPlayText();  //対象のスクリプトを送る
                    return;
                }

                var s = "[FILELIST:";
                for(int i = 0; i<list.Count; i++)
                {
                    s+=list.GetFile(i);
                    if (i<list.Count-1) s+=",";
                }
                s+="]";

                wk.SendWriteLine(s);

            } catch (SystemException e)
            {
                wk.SendWriteLine("Error ListFile: " + e.Message);
            }
        }

        public static void ListFileCom() // リモート伝達用
        {
            try {
                var list = slagtool.slag.m_latest_slag.m_filelist;
                
                if (
                    list==null
                        ||
                    list.Count==0
                        ||
                    (list!=null && list.Count==1 && list.GetFile(0) == TMPFILENAME)
                    )
                {
                    return; //ファイルが存在しない
                }

                var s = "[FILELIST:";
                for(int i = 0; i<list.Count; i++)
                {
                    s+=list.GetFile(i);
                    if (i<list.Count-1) s+=",";
                }
                s+="]";

                wk.SendWriteLine(s);
            }
            catch (SystemException e)
            {
                wk.SendWriteLine("Error ListFile: " + e.Message);
            }
        }
        //--- tool for this class
        private static int? intparse(string s)
        {
            int n;
            if (int.TryParse(s, out n))
            {
                return n;
            }
            return null;
        }
    }
}
