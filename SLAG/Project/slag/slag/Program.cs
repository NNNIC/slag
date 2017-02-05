using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;


namespace slag
{
    class Program
    {                 
        static void Main(string[] args)
        {
            TcpPipe pipe = new TcpPipe("127.0.0.1",2002);
            pipe.Start();

            while(true)
            {
                var msg = pipe.Read();
                if (msg!=null)
                {
                    Console.WriteLine(msg);
                }
                else
                {
                    var cmd = Console.ReadLine();
                    if (!string.IsNullOrEmpty(cmd))
                    {
                        pipe.Write("127.0.0.1",2001,cmd);
                    }
                }
            }
        }
    }
}
