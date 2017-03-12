using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;

namespace slagremote
{
    class cmd
    {
        public enum COMMAND
        {
            NONE,
            WD,     //Set Working Directory

            READ,   //Read FILENAME (.js or .inc)
            LOAD,   //Load FILENAME (.js, .inc, .bin or .base64)
            LOADRUN,//Load and run FILENAME (.js or .txt)
            LOADBIN,//Load binary file
            LOADBASE64, //Load Base64 file
            SAVETMPBIN, //Save the current to tmp.bin
            LOADTMPBIN, //Load tmp.bin
            SAVETMPBASE64,//Save the current to tmp.base64
            LOADTMPBASE64,//Load tmp.base64
            
            RUN,    //Run
            RUNTEXT,//Run text
            STEP,   //Step in or out
            BP,     //Set breakpoint          
            PRINT,  //Print variable
            STOP,   //Stop next line          --- 実行中OK
            RESUME, //Resume
            TEST,   //Test
            DEBUG,  //Debug [0|1|2]
            HELP, 
            QUIT,   //Quit and Close          --- 実行中OK
            RESET,  //Same as Quit
        }

        public static string m_workDir = @"N:\Project\test";
        public static void init()
        {
            netcomm.PreProcessCmd = preexecute;
        }
        
        private static string m_nextcmd = null;
        public static string GetNextCmd()
        {
            var s = m_nextcmd;
            m_nextcmd = null;
            return s;
        }

        public static string preexecute(string cmdbuf)        //※UnityAPI使用不可
        {
            if (!slagtool.YDEF_DEBUG.bPausing) return cmdbuf; //ポーズ外・・通過。

            string[] plist;
            COMMAND cmd = GetCmd(cmdbuf,out plist);
            string p1 = plist!=null && plist.Length>0 ? plist[0] : null;

            switch (cmd)
            {
                case COMMAND.BP:    cmd_sub.BP(plist);      return null; 
                case COMMAND.STEP:  cmd_sub.Step(p1);       return null;

                case COMMAND.RUN:
                case COMMAND.RESUME:cmd_sub.Resume();       return null;

                case COMMAND.QUIT:
                case COMMAND.RESET: cmd_sub.BP(new string[1] {"c"});  cmd_sub.Resume(); m_nextcmd = "reset";  return null;

                case COMMAND.TEST:         cmd_sub.Test();                                             break;
            }

            return cmdbuf;
        }

        public static void execute(string cmdbuff)
        {
            string[] plist;
            COMMAND cmd = GetCmd(cmdbuff,out plist);
            string p1 = plist!=null && plist.Length>0 ? plist[0] : null;
            switch(cmd)
            {
                case COMMAND.WD:           if (!string.IsNullOrEmpty(p1)) Set_WorkingDirectoy(p1);     break;
                case COMMAND.READ:         cmd_sub.Read(m_workDir,p1);                                 break;
                case COMMAND.LOAD:         if (plist!=null)
                                           { 
                                               if (plist.Length == 1) { cmd_sub.Load(m_workDir,p1);    break; }
                                               if (plist.Length>1)    { cmd_sub.Load(m_workDir,plist); break; }
                                           }
                                           wk.SendWriteLine("file name was not specified.");
                                           break;

                case COMMAND.SAVETMPBIN:   cmd_sub.SaveBin(m_workDir,"tmp.bin");                       break;
                case COMMAND.SAVETMPBASE64:cmd_sub.SaveBase64(m_workDir,"tmp.base64");                 break;
                case COMMAND.LOADTMPBIN:   cmd_sub.Load(m_workDir,"tmp.bin");                          break;
                case COMMAND.LOADTMPBASE64:cmd_sub.Load(m_workDir,"tmp.base64");                       break;


                case COMMAND.RUN:          cmd_sub.Run();                                              break;
                case COMMAND.STEP:         break;
                case COMMAND.BP:           cmd_sub.BP(plist);                               break;
                case COMMAND.PRINT:        break;
                case COMMAND.STOP:         cmd_sub.Stop();                                             break;
                case COMMAND.RESUME:       break;
                case COMMAND.TEST:         cmd_sub.Test();                                             break;
                case COMMAND.DEBUG:        cmd_sub.Debug(p1);                                          break;

                case COMMAND.RESET:
                case COMMAND.QUIT:         cmd_sub.Reset();                                            break;
                                                                                                     
                case COMMAND.HELP:         cmd_sub.Help();                                             break;

                default:                   wk.SendWriteLine("ignore:" + cmdbuff); break;
            }
        }

        // --- tool for this class
        private static COMMAND GetCmd(string cmdbuff,out string[] parameters)
        {
            parameters = null;
            var token  = cmdbuff.Split(' ');

            var p0    = token[0].ToUpper();
            var list  = new List<string>();
            if (token.Length>1) for(var i = 1; i< token.Length; i++)
            { 
                list.Add(token[i]);
            }
            parameters = list.ToArray();

            // コマンド確認
            if (!Enum.IsDefined(typeof(COMMAND),p0))
            { 
                wk.SendWriteLine("Unknow command:" + cmdbuff);
                return COMMAND.NONE;
            }
            var cmd = (COMMAND)Enum.Parse(typeof(COMMAND),p0);
            return cmd;
        }
        private static void Set_WorkingDirectoy(string dir)
        {
            if (dir!=null)
            {
                m_workDir = dir;
            }
            wk.SendWriteLine("Current Working Directory : " + m_workDir);
        }
    }
}
