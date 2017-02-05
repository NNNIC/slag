using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace FilePipeTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //テスト　エコーサーバ
            FilePipe m_pipe = new FilePipe("MOON");
            m_pipe.Start(s=>Console.WriteLine(s));
            while(true)
            {
                m_pipe.Update();

                var cmd = m_pipe.Read();
                if (cmd==null)
                {
                    Thread.Sleep(33);
                    continue;
                }
                Console.WriteLine("got: ([" + cmd +"])");
                m_pipe.Write("MOON replay by " + cmd,"EARTH");
            }
        }
    }
}
