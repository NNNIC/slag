using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;

namespace slagremote
{
    public class cmd_sub
    {
        private static slagunity m_slagunity
        {
            get { return slagremote_unity_manager.V.m_slagunity; }
        }
        public static slagtool.slag Load(string path, string[] files)
        {
            //m_slagunity = slagunity.Create(slagremote_unity_main.V.gameObject);

            var fullpath_files = new List<string>();
            for (var i = 0; i<files.Length; i++)
            {
                var file = files[i];
                if (!file.ToUpper().EndsWith(".JS"))
                {
                    wk.SendWriteLine("ERROR:File is not JS :" + file );
                    return null;
                }
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

                fullpath_files.Add(fullpath);
            }

            if (slagtool.sys.USETRY)
            {
                try
                {
                    //m_slag = new slagtool.slag(null);
                    //m_slag.LoadJSFiles(fullpath_files.ToArray());

                    //m_slagunity = slagunity.Create(slagremote_unity_main.V.gameObject);
                    m_slagunity.LoadJSFiles(fullpath_files.ToArray());
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
                //m_slag = new slagtool.slag(null);
                //m_slag.LoadJSFiles(fullpath_files.ToArray());
                //m_slagunity = slagunity.Create(slagremote_unity_main.V.gameObject);
                m_slagunity.LoadJSFiles(fullpath_files.ToArray());
            }
            wk.SendWriteLine("Loaded.");

            wk.SendWriteLine("Checksum:" + m_slagunity.GetMD5());

            return  m_slagunity.m_slag;
        }
        //public static void Read(string path, string file)
        //{
        //    string fullpath = null;
        //    try
        //    {
        //        fullpath = Path.Combine(path,file);
        //    }
        //    catch
        //    {
        //        wk.SendWriteLine("ERROR:Unexpcted path name");
        //        return;
        //    }
        //    m_slagunity.ReadScript(fullpath);
        //}
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

            //m_slagunity = null;

            if (slagtool.sys.USETRY)
            {
                try
                {
                    //m_slag = new slagtool.slag(null);
                    //m_slag.LoadFile(fullpath);
                    //m_slagunity = slagunity.Create(slagremote_unity_main.V.gameObject);
                    m_slagunity.LoadFile(fullpath);
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
                //m_slag = new slagtool.slag(null);
                //m_slag.LoadFile(fullpath);
                //m_slagunity = slagunity.Create(slagremote_unity_main.V.gameObject);
                m_slagunity.LoadFile(fullpath);
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
            m_updateFunc = null;

            slagtool.YDEF_DEBUG.ResetAllBreakpoints();//BPクリア
            slagtool.YDEF_DEBUG.bPausing = false;     //ポーズOFF

            var main_go = UnityEngine.GameObject.Find("main");
            if (main_go!=null)
            { 
                main_go.SendMessage("Reset");
            }
        }
        #region BP
        public static int? m_curFild_id=null; //base 0
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
            if (Array.FindIndex(new string[] {"?","h","help"}, i=>i==p0) >= 0)
            {
                var helpmsg = 
                                "bp - ブレイクポインタのリスト表示                         " + NL +
                                "bp c|clear|r|reset - ブレイクポインタのクリア             " + NL +
                                "bp d NUM - NUM行のブレイクポインタ削除                    " + NL +
                                "bp NUM - カレントファイルのnum行目にブレイクポインタ設定  " + NL +
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

            if (p0 == "d" && !string.IsNullOrEmpty(p1))
            {
                var num = intparse(p1);
                if (num==null||(int)num<=0)
                {
                    wk.SendWriteLine("削除の行番号が不正です。");
                    return;
                }
                int dnum = (int)num - 1;
                var b = slagtool.YDEF_DEBUG.DelBreakpoint(dnum,(m_curFild_id!=null ? (int)m_curFild_id: 0));
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


            if (p0=="f" && !string.IsNullOrEmpty(p1))
            {
                var num = intparse(p1);
                if (num==null || (int)num<=0)
                {
                    wk.SendWriteLine("ファイルＩＤが不正です。");
                    return;
                }
                int dnum = (int)num - 1;
                var file = m_slagunity.m_slag.GetFileName(dnum);
                if (file==null)
                {
                    wk.SendWriteLine("ファイルＩＤが不正です。");
                    return;
                }
                wk.SendWriteLine("カレントファイル(" + num + "):" + file );
                m_curFild_id = dnum;
                return;
            }
            if (p0=="f" && string.IsNullOrEmpty(p1))
            {
                int dnum = m_curFild_id!=null ? (int)m_curFild_id : 0;
                var file = m_slagunity.m_slag.GetFileName(dnum);
                if (file==null)
                {
                    wk.SendWriteLine("ファイルが取得できません。");
                    return;
                }
                wk.SendWriteLine("カレントファイル(" + (dnum+1) + "):" + file );
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
                var fileid = (int)(m_curFild_id!=null ? (int)m_curFild_id: 0);
                var file = m_slagunity.m_slag.GetFileName(fileid);
                if (file == null)
                {
                    wk.SendWriteLine("設定ファイルが不正です。");
                    return;
                }

                slagtool.YDEF_DEBUG.AddBreakpoint(dnum,fileid);
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
            var keylist = new List<int>(slagtool.YDEF_DEBUG.breakpoints.Keys);
            keylist.Sort();
            foreach(var k in keylist)
            {
                if (slagtool.YDEF_DEBUG.breakpoints[k]==null||slagtool.YDEF_DEBUG.breakpoints[k].Count==0) continue;
                var file = m_slagunity.m_slag.GetFileName(k);
                wk.SendWriteLine("====" + (k+1).ToString("00") + ":" + file );
                var lines = new List<int>(slagtool.YDEF_DEBUG.breakpoints[k]);
                lines.Sort();
                for(int n = 0; n<lines.Count;n++)
                {
                    wk.SendWriteLine("Line:" + (lines[n]+1));
                }
                wk.SendWriteLine("===");
            }
        }

        private static void AddBreakPoint(string[] plist) //p0 = line , p1 = fileid   
        {
            int line   = -1;
            int fileid = -1;
            if (plist==null && plist.Length<=2)   {  wk.SendWriteLine("Breakpoint needs parameters"); return; }

            if (!int.TryParse(plist[0],out line))
            {
                wk.SendWriteLine("Breakpoint: the first parameter should be interger.");
                return;
            }
            if (!int.TryParse(plist[1],out fileid))
            {
                wk.SendWriteLine("Breakpoint: the sencond parameter should be interger or should no be specified as using previous id.");
                return;
            }
            if (line!=-1 && fileid!=-1)
            { 
                line--;
                fileid--;
                slagtool.YDEF_DEBUG.AddBreakpoint(line,fileid);
            }
            else
            {
                wk.SendWriteLine("Breakpoint needs parameters");
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
            var s = slagtool.runtime.builtin.builtin_func.Help();
            wk.SendWriteLine(s);
        }

        public static void GetPlayText()
        {
            var s = slagunity.m_script;
            if (!string.IsNullOrEmpty(s))
            { 
                var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(s));
                wk.SendWriteLine("?TEXT?" + base64);
            }
            else
            {
                wk.SendWriteLine("No Text");
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


        //-- Update用
        private static List<string> m_updateFunc;
        [Obsolete]
        public static void UpdateClear()
        {
            m_updateFunc = null;
        }
        [Obsolete]
        public static void UpdateAddFunc(string func)
        {
            if (m_updateFunc==null) m_updateFunc = new List<string>();
            m_updateFunc.Add(func);
        }
        [Obsolete]
        public static void UpdateExec()
        {
            if (slagtool.sys.USETRY)
            {
                try
                {
                    _updateExec();
                }
                catch (SystemException e)
                {
                    wk.SendWriteLine("-- EXCEPTION --");
                    wk.SendWriteLine(e.Message);
                    wk.SendWriteLine("Stop at Line:" + slagtool.YDEF_DEBUG.current_v.get_dbg_line(true).ToString() );
                    wk.SendWriteLine("---------------");
                    //m_sm = null;
                    m_updateFunc = null;
                }
            }
            else
            {
                _updateExec();
            }
        }
        [Obsolete]
        private static void _updateExec()
        {
            //if (m_sm!=null) m_sm.Update();

            if (m_slagunity.m_slag==null) return;
            if (m_updateFunc==null) return;

            var s = new System.Diagnostics.Stopwatch();
            s.Start();
            foreach (var f in m_updateFunc)
            {
                m_slagunity.m_slag.CallFunc(f);
            }
            s.Stop();
        }

        //-- StateMachine用
        [Obsolete]
        public class StateMachine
        {
            string m_cur;
            string m_next;

            int    m_waitcnt;

            [Obsolete]
            public void Goto(string func) { m_next     = func;  }
            [Obsolete]
            public void Wait(int c)       { m_waitcnt  = c;}

            [Obsolete]
            public void Update()
            {
                if (m_waitcnt>0)
                {
                    m_waitcnt--;
                    return;
                }
                bool bFirst = false;
                if (m_next!=null)
                {
                    m_cur  = m_next;
                    m_next = null;
                    bFirst = true;
                }
                if (m_slagunity.m_slag!=null &&  m_cur!=null) m_slagunity.m_slag.CallFunc(m_cur,new object[1] { bFirst });
            }
        }
    }
}
