using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System;

using number = System.Double;
using slagtool.runtime;
using slagtool.runtime.builtin;

namespace slagtool
{
    public class util
    {

        #region 組込関数設定
        public static void SetBuiltIn(Type type, string name = null)
        {
            var catname = !string.IsNullOrEmpty(name) ? name : type.ToString();
            runtime.builtin.builtin_func.Subscribe(type, catname);
        }
        public static void SetCalcOp(Func<object, object, string, object> user_calc_op)
        {
            runtime.util.User_Calc_op = user_calc_op;
        }
        public static void SetItemsForIL2CPP(runtime.runtime_sub_missings user_missing_class) //IL2CPPにて、不明となったメソッド対策 Constuctors・Methods・Members
        {
            
        }
 
        #endregion


        #region ログ設定
        public static void SetLogFunc(Action<string> writeline, Action<string> write = null)
        {
            sys.m_conWrite = write;
            sys.m_conWriteLine = writeline;
        }
        #endregion

        #region デバッグレベル設定・取得
        public static void SetDebugLevel(int debugLevel)
        {
            sys.set_debugLevel(debugLevel);
        }
        public static int GetDebugLevel()
        {
            return sys.DEBUGLEVEL;
        }
        #endregion


        public static void Error(string msg)
        {
            slagtool.runtime.util._error(msg);
        }
    }

    internal class util_sub
    {
        internal static List<YVALUE> Compile(List<string> multiple_srces)
        {
            var engine = new yengine();

            // 終末記号に分類
            var lex_output = new List<List<YVALUE>>();
            for (int i = 0; i < multiple_srces.Count; i++)
            {
                lex_output.AddRange(engine.Lex(multiple_srces[i], i));
            }

            //スペース・コメント削除。"文字列"以外大文字化。
            engine.Normalize(ref lex_output);
            sys.logline("\n*lex_output");
            YDEF_DEBUG.DumpList(lex_output, true);

            //改行を無くしBOFとEOFを追加
            var one_line = engine.Del_LF_And_Add_BOF_EOF(lex_output);

            //実行用リスト作成(解析)
            var analyzed = engine.Interpret(one_line);
            var executable_value_list = analyzed[0];


            //ダンプ
            sys.logline("\n[executable_value_list]\n");

            YDEF_DEBUG.PrintListValue(executable_value_list);

            sys.logline("\n");

            //リストの整合性テスト
            List<int> errorline;
            if (YDEF_DEBUG.IsExecutable(executable_value_list, out errorline))
            {
                sys.logline("スクリプト実行準備完了");
            }
            else
            {
                string s = null;
                errorline.ForEach(i =>
                {
                    if (s != null) s += ",";
                    s += (i + 1);
                });
                sys.error("スクリプト実行不可、次を確認：" + s);
            }

            //最適化
            executable_value_list = preruntime.process.Convert(executable_value_list);

            return executable_value_list;
        }
        internal static List<YVALUE> Compile(string src)
        {
            var multiple_srces = new List<string>();
            multiple_srces.Add(src);

            return Compile(multiple_srces);
        }
    }
}