using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace slagruntime
{
    class Program
    {
        public static communicate m_comm;
        static void Main(string[] args)
        {
            m_comm = new communicate();
            m_comm.Start();

            slagtool.process.SetLogFunc(util.Log,util.LogLine);

            while(true)
            {
                var cmd = m_comm.GetCmd();
                if (cmd==null)
                {
                    Thread.Sleep(33);
                    continue;
                }   
                command.execute(cmd);
            }
        }
    }
    class util
    {
        private static string m_tmp;
        public static void Log(string s)
        {
            Console.Write(s);

            m_tmp += s;
        }
        public static void LogLine(string s=null)
        {
            Console.WriteLine(s);
            m_tmp += s;
            m_tmp = null;
            Program.m_comm.Log(s);
        }
    }
}
