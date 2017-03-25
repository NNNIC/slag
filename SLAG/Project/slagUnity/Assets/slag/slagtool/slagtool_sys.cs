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

        #region コンソール出力
        internal  static Action<string> m_conWrite=null;
        internal  static Action<string> m_conWriteLine = null;

        internal  static Action<string> m_con_remoteWrite = null;           //ネットログ用
        internal  static Action<string> m_con_remoteWriteLine = null;       //ネットログ用

        private   static void conWrite(string s)
        {
            s = numbase.convert_log(s);

            Console.Write(s);
            if (m_conWrite!=null) m_conWrite(s);
            if (m_con_remoteWrite!=null) m_con_remoteWrite(s);
        }
        private   static void conWriteLine(string s)
        {
            s = numbase.convert_log(s);

            Console.WriteLine(s);
            if (m_conWriteLine!=null) m_conWriteLine(s);
            if (m_con_remoteWriteLine!=null) m_con_remoteWriteLine(s);
        }
        #endregion

        internal static void error(string s, YVALUE v = null)
        {
            int line = -1;
            if (v!=null) line = v.get_dbg_line(true);
            
            string es = "ERROR"+ (line>=0 ? "(L:" + line.ToString() + ")" : "") + ":" + s;

            conWriteLine(es);

            throw new SystemException(es);
        }

        internal  static void log(string s,bool bForce = false)
        {
            if (DEBUGMODE||bForce)
            {
                conWrite(s);
            }
        }

        internal static void logline(string s=null,bool bForce=false)
        {
            if (DEBUGMODE||bForce)
            { 
                conWriteLine(s);
            }
        }

        internal static void log_stopinfo()
        {
            logline(YDEF_DEBUG.RuntimeErrorInfo(),true);
        }
    }
}
