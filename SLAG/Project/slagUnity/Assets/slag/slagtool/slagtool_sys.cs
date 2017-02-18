using System;
using System.Collections.Generic;
using System.Text;
using number = System.Double;

namespace slagtool
{
    public static class sys
    {
        internal  static bool DEBUGMODE { get { return DEBUGLEVEL!=0; }  set { DEBUGLEVEL = value ? 1 : 0;  } }

        private static int _debuglevel = 0;
        internal  static int  DEBUGLEVEL
        {
            get { return _debuglevel;}
            private set
            {
                USETRY = (value != 2);
                _debuglevel = value;
            }
        }
        internal static void set_debugLevel(int x) { DEBUGLEVEL = x; }  // util.SetDebugLevelのみに呼出しを制限

        internal  static bool USETRY = true;

        internal  static Action<string> m_conWrite=null;
        internal  static Action<string> m_conWriteLine = null;

        internal  static void error(string s, YVALUE v = null)
        {
            int line = -1;
            if (v!=null) line = v.get_dbg_line(true);
            
            string es = "ERROR"+ (line>=0 ? "(L:" + line.ToString() + ")" : "") + ":" + s;

            Console.WriteLine(es);
            if (m_conWriteLine!=null) m_conWriteLine(es);

            throw new SystemException(es);
        }

        internal  static void log(string s,bool bForce = false)
        {
            if (DEBUGMODE||bForce)
            {
                if (m_conWrite!=null)
                {
                    m_conWrite(s);
                }
            }
        }

        internal static void logline(string s=null,bool bForce=false)
        {
            if (DEBUGMODE||bForce)
            { 
                if (m_conWriteLine!=null)
                {
                    m_conWriteLine(s);
                }
            }
        }

        internal static void log_stopinfo()
        {
            logline(YDEF_DEBUG.RuntimeErrorInfo(),true);
        }
    }
}
