using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System;

/*
    ユーザインターフェースにて、行番号とファイルインデックスが、１ベースとなり、
    内部では０ベースとなる。

    それを吸収する

*/

namespace slagtool {
    public class numbase {

        public static string convert_cmd(string cmd)  // コマンドを読込む際に利用。 1ベースの引数を０ベースとする
        {
            if (string.IsNullOrEmpty(cmd)) return "";
            var s = cmd.Trim();
            if (string.IsNullOrEmpty(s)) return "";

            var tokens = cmd.Split(' ');
            
            bool bCheck = false;
            if (tokens[0].ToLower() == "bp")
            {
                bCheck = true;
            }
            
            if (!bCheck) return cmd; //対象外

            string newcmd = null;

            for(var i = 0; i<tokens.Length; i++)
            {
                if (i==0)
                {
                    newcmd = tokens[0];
                    continue;
                }

                newcmd += " ";

                int x;
                if (int.TryParse(tokens[i],out x))
                {
                    x--;
                    newcmd += x.ToString();
                    continue;
                }

                newcmd += tokens[i];
            }

            return newcmd;
        }

        public static string convert_log(string log) // ログ内の <L数字> と <F数字>を 1ベースの数字に変更
        {
            if (string.IsNullOrEmpty(log)) return "";
            var s=log.Trim();
            if (string.IsNullOrEmpty(s)) return "";

            
            Action<string> conv = (ptn) => {
                var rx = new Regex(ptn);
                for(var loop=0;loop<100;loop++)
                {
                    var matches = rx.Matches(s);
                    if (matches.Count==0) break;
                    var text = matches[0].Value.Substring(2).Trim('>');
                    var n = int.Parse(text);

                    s = s.Replace(matches[0].Value, (n+1).ToString());
                }
            };

            conv(@"<L(\d+)>");
            conv(@"<F(\d+)>");

            return s;
        }

    }

}