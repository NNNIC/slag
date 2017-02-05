using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace slagruntime
{
    class command_exec
    {
        public static void Load(string file)
        {
            if (file==null)
            {
                util.LogLine("ERROR:File name is null!");
                return;
            }
            var ext = Path.GetExtension(file).ToUpper();
            if (ext!=".JS" && ext!=".BIN")
            {
                util.LogLine("ERROR:File name is not allowed");
                return;
            }
            if (!File.Exists(file))
            {
                util.LogLine("ERROR:File does not exist!");
            }

            var raw = File.ReadAllText(file,Encoding.UTF8);

            try
            {
                slagtool.process.Run(raw,true);
            } catch (SystemException e)
            {
                util.LogLine("-- EXCEPTION --");
                util.LogLine(e.Message);
                util.LogLine("---------------");
                return;
            }
            util.LogLine(".. Read.");
        }
    }
}
