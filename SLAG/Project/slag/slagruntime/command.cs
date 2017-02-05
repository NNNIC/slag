using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace slagruntime
{
    class command
    {
        public enum CMD
        {
            NONE,
            LOAD,   //Load FILENAME (.js or .txt)
            LOADBIN,//Load TEXT
            RUN,    //Run
            STEP,   //Step in or out
            BP,     //Set breakpoint          
            PRINT,  //Print variable
            STOP,   //Stop next line          --- 実行中OK
            RESUME, //Resume
            QUIT    //Quit and Close          --- 実行中OK
        }

        public static void execute(string cmdbuff)
        {
            string p1;
            CMD cmd = GetCmd(cmdbuff,out p1);
            switch(cmd)
            {
                case CMD.LOAD:    break;
                case CMD.LOADBIN: break;
                case CMD.RUN:     break;
                case CMD.STEP:    break;
                case CMD.BP:      break;
                case CMD.PRINT:   break;
                case CMD.STOP:    break;
                case CMD.RESUME:  break;
                case CMD.QUIT:    break;
                default: util.LogLine("ignore:" + cmdbuff); break;
            }
        }

        public static void execute_in_running(string cmdbuff)
        {
            string p1;
            CMD cmd = GetCmd(cmdbuff,out p1);
            switch(cmd)
            {
                case CMD.STOP: break;
                case CMD.QUIT: break;
                default: util.LogLine("ignore:" + cmd.ToString()); break;
            }
        }

        // --- tool for this class
        private static CMD GetCmd(string cmdbuff,out string p1)
        {
            var token = cmdbuff.Split(' ');
            string p0 = token[0].ToUpper();
            p1        = token.Length>1 ? token[1] : null;

            CMD cmd = CMD.NONE;
            if (!Enum.TryParse<CMD>(p0,out cmd))
            {
                util.LogLine("Unknow command:" + cmdbuff);
            }
            return cmd;
        }
    }
}
