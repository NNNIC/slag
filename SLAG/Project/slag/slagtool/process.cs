using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace slagtool
{
    public class process
    {
        public static void SetLogFunc(Action<string> write, Action<string> writeline )
        {
            sys.m_conWrite = write;
            sys.m_conWriteLine = writeline;
        }

        public static void Run(string src, bool bCompileOnly=false,string outbinfile = null)
        {
            var engine = new yengine();

            // 終末記号に分類
            var lex_output = engine.Lex(src);

            //スペース・コメント削除。"文字列"以外大文字化。
            engine.Normalize(ref lex_output);                             sys.logline("\n*lex_output");           YDEF_DEBUG.DumpList(lex_output, true);

            //１行化
            var one_line = engine.Make_one_line(lex_output);

            //実行用リスト作成(解析)
            var analyzed = engine.Interpret(one_line);       
            var executable_value_list = analyzed[0];
            

            //ダンプ
            sys.logline("\n[executable_value_list]\n");

            YDEF_DEBUG.PrintListValue(executable_value_list);

            sys.logline("\n");

            //リストの整合性テスト
            List<int> errorline;
            if (YDEF_DEBUG.IsExecutable(executable_value_list,out errorline))
            {
                sys.logline("This script is ready to excute.");
            }
            else
            {
                string s = null;
                errorline.ForEach(i=> {
                    if (s!=null) s+=",";
                    s += (i+1);
                });
                sys.error("This script is not executable. Check syntax at " + s);
            }

            //SAVE
            YSAVELOAD.Save(executable_value_list,slagtool.runtime.CFG.TMPBIN);
            if (outbinfile!=null)
            {
                YSAVELOAD.Save(executable_value_list,outbinfile);
            }

            if (!bCompileOnly)
            { 
                //LOAD
                executable_value_list = YSAVELOAD.Load(slagtool.runtime.CFG.TMPBIN);

                //実行
                sys.logline("\n\n*Execute! \n");

                runtime.builtin.builtin_func.Init();
                runtime.run_script.Run(executable_value_list[0]);
            }
            sys.logline("\n*end");
        }

        public static void Run_from_savefile(string file = null)
        {
            if (file==null)
            {
                file = slagtool.runtime.CFG.TMPBIN;
            }

            //LOAD
            var executable_value_list = YSAVELOAD.Load(file);

            //実行
            sys.logline("\n\n*Execute! \n");

            runtime.builtin.builtin_func.Init();
            runtime.run_script.Run(executable_value_list[0]);

            sys.logline("\n*end");
        }
    }
}
