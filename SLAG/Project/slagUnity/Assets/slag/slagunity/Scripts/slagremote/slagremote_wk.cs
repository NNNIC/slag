using UnityEngine;
using System.Collections;

namespace slagremote
{
#if UNITY_5_3_OR_NEWER
    public class wk
    { 
        public static void SendWrite(string s)
        {
            slagremote.unity.wk_.SendWrite(s);
        }
        public static void SendWriteLine(string s=null)
        {
            slagremote.unity.wk_.SendWriteLine(s);
        }

        public static void Log(string s)
        {
            slagremote.unity.wk_.Log(s);
        }

        public static void Update()
        {
            slagremote.unity.wk_.Update();
        }
    }
#else
    public class wk
    { 
        public static void SendWrite(string s)
        {
        }
        public static void SendWriteLine(string s=null)
        {
        }

        public static void Log(string s)
        {
        }

        public static void Update()
        {
        }
    }

#endif
}