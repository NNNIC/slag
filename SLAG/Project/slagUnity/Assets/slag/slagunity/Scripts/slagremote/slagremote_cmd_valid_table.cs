using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace slagremote { 
    internal struct valid_item { 
        internal bool        bRunLimit; //playtext,playslag等の実行制限下で有効か？     

        public valid_item(bool _bValidExecInLimit)
        {
            bRunLimit = _bValidExecInLimit;
        }
    }

    public enum RUNMODE
    {
        NORMAL,
        RunLimit
    }

    internal struct valid_table
    {
        static Dictionary<cmd.COMMAND, valid_item> list;
        
        static valid_table()
        {
            list = new Dictionary<cmd.COMMAND, valid_item>();
            
            // enum COMMANDでの定義順に記述。他の要素が出てきたときに利用予定
                                                          //bRunLimit 
            list.Add(cmd.COMMAND.NONE,         new valid_item(false    ));
            list.Add(cmd.COMMAND.WD  ,         new valid_item(false    ));
                                                                       
            //list.Add(cmd.COMMAND.READ,         new valid_item(false    ));
            list.Add(cmd.COMMAND.LOAD,         new valid_item(false    ));
            list.Add(cmd.COMMAND.LOADRUN,      new valid_item(false    ));
            list.Add(cmd.COMMAND.LOADBIN,      new valid_item(false    ));
            list.Add(cmd.COMMAND.LOADBASE64,   new valid_item(false    ));
            list.Add(cmd.COMMAND.SAVETMPBIN,   new valid_item(false    ));
            list.Add(cmd.COMMAND.LOADTMPBIN,   new valid_item(false    ));
            list.Add(cmd.COMMAND.SAVETMPBASE64,new valid_item(false    ));
            list.Add(cmd.COMMAND.LOADTMPBASE64,new valid_item(false    ));
                                                                       
            list.Add(cmd.COMMAND.RUN,          new valid_item(false    ));
            list.Add(cmd.COMMAND.RUNTEXT,      new valid_item(false    ));
            list.Add(cmd.COMMAND.STEP,         new valid_item(true     ));
            list.Add(cmd.COMMAND.BP,           new valid_item(true     ));
            list.Add(cmd.COMMAND.PRINT,        new valid_item(true     ));
            list.Add(cmd.COMMAND.STOP,         new valid_item(true     ));
            list.Add(cmd.COMMAND.RESUME,       new valid_item(true     ));
            list.Add(cmd.COMMAND.DEBUG,        new valid_item(true     ));
            list.Add(cmd.COMMAND.HELP,         new valid_item(true     ));
            list.Add(cmd.COMMAND.QUIT,         new valid_item(true     ));
            list.Add(cmd.COMMAND.RESET,        new valid_item(false    ));

            list.Add(cmd.COMMAND.GETPLAYTEXT,  new valid_item(true     ));
        }

        internal static bool IsValid(RUNMODE mode, cmd.COMMAND command)
        {
            if (list!=null && list.ContainsKey(command))
            {
                var item = list[command];
                switch(mode)
                {
                    case RUNMODE.RunLimit: return item.bRunLimit; 
                }
            }
            return true;
        }
    }
}