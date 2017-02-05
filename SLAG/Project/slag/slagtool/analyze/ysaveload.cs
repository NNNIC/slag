using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace slagtool
{
    public static class YSAVELOAD
    {
        public static void Save(List<YVALUE> l, string path)
        {
            var bf = new BinaryFormatter();
            var ms = new MemoryStream();

            bf.Serialize(ms,l);

            try { 
                File.WriteAllBytes(path,ms.ToArray());
            } catch
            {
                sys.error("Faild to save!");
            }
        }

        public static List<YVALUE> Load(string path)
        {
            if (!File.Exists(path))
            {
                sys.error("File does not exist! .. " + path);
            }
            var bytes = File.ReadAllBytes(path);
            var ms = new MemoryStream(bytes);

            List<YVALUE> l = null;
            try {
                var bf = new BinaryFormatter();
                l = (List<YVALUE>)bf.Deserialize(ms);
            } catch
            {
                sys.error("Faild to load!");
            }
            return l;
        }
    }
}
