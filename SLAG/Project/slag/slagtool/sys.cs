using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace slagtool
{
    public static class sys
    {
        public static bool DEBUGMODE { get { return DEBUGLEVEL!=0; }  set { DEBUGLEVEL = value ? 1 : 0;  } }
        public static int  DEBUGLEVEL = 0;

        public static Action<string> m_conWrite=null;
        public static Action<string> m_conWriteLine = null;

        public static void error(string s, YVALUE v = null)
        {
            int line = -1;
            if (v!=null) line = v.get_dbg_line();
            
            string es = "ERROR"+ (line>=0 ? "(L:" + (line+1).ToString() + ")" : "") + ":" + s;

            Console.WriteLine(es);
            if (m_conWriteLine!=null) m_conWriteLine(es);

            throw new SystemException(es);
        }

        public static void log(string s)
        {
            if (DEBUGMODE)
            {
                if (m_conWrite!=null)
                {
                    m_conWrite(s);
                }
            }
        }

        public static void logline(string s=null)
        {
            if (DEBUGMODE)
            { 
                if (m_conWriteLine!=null)
                {
                    m_conWriteLine(s);
                }
            }
        }
    }
}
