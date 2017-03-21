using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace slagremote { 
    internal struct cmd_data_item { 
        internal string      help;
        internal bool        bNormal;
        internal bool        bRunLimit; //playtext,playslag等の実行制限下で有効か？     

        public cmd_data_item(string _help,  bool _bNormal,   bool _bValidExecInLimit)
        {
            help      = _help;
            bNormal   = _bNormal;
            bRunLimit = _bValidExecInLimit;
        }
    }

    public enum RUNMODE
    {
        NORMAL,
        RunLimit
    }

    internal class cmd_description
    {
        public static readonly string NONE    = "";
        public static readonly string WD      = "ワーキングディレクトリ設定";

        public static readonly string LOAD       = "ファイルロード(*.js|*.inc|*.bin|*.base64)";

        public static readonly string SAVETMPBIN = "[テスト]バイナリセーブ";
        public static readonly string LOADTMPBIN = "[テスト]バイナリロード";

        public static readonly string SAVETMPBASE64 = "[テスト]Base64セーブ";
        public static readonly string LOADTMPBASE64 = "[テスト]Base64ロード";

        public static readonly string RUN    = "実行";
        public static readonly string STOP   = "停止";
        public static readonly string RESUME = "停止からの復旧";

        public static readonly string QUIT   = "リセット(RESETと同じ)";
        public static readonly string RESET  = "リセット(QUITと同じ)";

        public static readonly string DEBUG  = "デバッグモード取得・設定";
        public static readonly string STEP   = "ステップ実行";
        public static readonly string BP     = "ブレイクポインタ設定。詳細：bp help";
        public static readonly string PRINT  = "変数ダンプ";

        public static readonly string TEST   = "通信確認用";
        public static readonly string HELP   = "コマンドヘルプ表示";

        public static readonly string GETTEXT  = "実行中のテキスト表示";
        public static readonly string GETBP    = "ブレイクポインタの表示";
        public static readonly string LISTFILE = "コンパイル対象のファイル一覧";
    }


    internal struct cmd_data_table
    {
        static Dictionary<cmd.COMMAND, cmd_data_item> list;
        
        static cmd_data_table()
        {
            list = new Dictionary<cmd.COMMAND, cmd_data_item>();
            
            // enum COMMANDでの定義順に記述。他の要素が出てきたときに利用予定
                                                                                                    //bRunLimit 
            list.Add(cmd.COMMAND.NONE,         new cmd_data_item(cmd_description.NONE,          true, false   ));
            list.Add(cmd.COMMAND.WD  ,         new cmd_data_item(cmd_description.WD,            true, false   ));
                                                                        
            list.Add(cmd.COMMAND.LOAD,         new cmd_data_item(cmd_description.LOAD,          true, false   ));

            list.Add(cmd.COMMAND.SAVETMPBIN,   new cmd_data_item(cmd_description.SAVETMPBIN,    true, false   ));
            list.Add(cmd.COMMAND.LOADTMPBIN,   new cmd_data_item(cmd_description.LOADTMPBIN,    true, false   ));
            list.Add(cmd.COMMAND.SAVETMPBASE64,new cmd_data_item(cmd_description.SAVETMPBASE64, true, false   ));
            list.Add(cmd.COMMAND.LOADTMPBASE64,new cmd_data_item(cmd_description.LOADTMPBASE64, true,false    ));

            list.Add(cmd.COMMAND.RUN,          new cmd_data_item(cmd_description.RUN,           true,false    ));
            list.Add(cmd.COMMAND.STEP,         new cmd_data_item(cmd_description.STEP,          true,true     ));
            list.Add(cmd.COMMAND.BP,           new cmd_data_item(cmd_description.BP,            true,true     ));
            list.Add(cmd.COMMAND.PRINT,        new cmd_data_item(cmd_description.PRINT,         true,true     ));
            list.Add(cmd.COMMAND.STOP,         new cmd_data_item(cmd_description.STOP,          true,true     ));
            list.Add(cmd.COMMAND.RESUME,       new cmd_data_item(cmd_description.RESUME,        true,true     ));
            list.Add(cmd.COMMAND.DEBUG,        new cmd_data_item(cmd_description.DEBUG,         true,true     ));
            list.Add(cmd.COMMAND.HELP,         new cmd_data_item(cmd_description.HELP,          true,true     ));
            list.Add(cmd.COMMAND.QUIT,         new cmd_data_item(cmd_description.QUIT,          true,false    ));
            list.Add(cmd.COMMAND.RESET,        new cmd_data_item(cmd_description.RESET,         true,false    ));

            list.Add(cmd.COMMAND.GETTEXT,      new cmd_data_item(cmd_description.GETTEXT,       true,true     ));
            list.Add(cmd.COMMAND.GETBP,        new cmd_data_item(cmd_description.GETBP,         true,true     ));

            list.Add(cmd.COMMAND.LISTFILE,     new cmd_data_item(cmd_description.LISTFILE,      true,true     ));
        }

        internal static bool IsValid(RUNMODE mode, cmd.COMMAND command)
        {
            if (list!=null && list.ContainsKey(command))
            {
                var item = list[command];
                switch(mode)
                {
                    case RUNMODE.NORMAL:   return item.bNormal;
                    case RUNMODE.RunLimit: return item.bRunLimit; 
                }
            }
            return true;
        }

        internal static string GetHelp(cmd.COMMAND command)
        {
            if (list!=null && list.ContainsKey(command))
            {
                var item = list[command];
                return item.help;
            }
            return null;         
        }

        internal static string GetHelpAll()
        {
            var enums = Enum.GetValues(typeof(cmd.COMMAND));
            
            string s = "::: モニターコマンド :: \n";
            foreach(var i in enums)
            {
                var cmd = (cmd.COMMAND)i;
                var help = GetHelp(cmd);
                if (string.IsNullOrEmpty(help)) continue;

                s += string.Format("{0,-13} {1}\n",cmd.ToString().ToLower(),help);
            }
            s+="\n";            
            return s;
        }
    }
}