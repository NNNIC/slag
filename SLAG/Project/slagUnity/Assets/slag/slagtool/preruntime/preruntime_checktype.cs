using System;
using System.Collections.Generic;
using slagtool;

namespace slagtool.preruntime
{ 
    public class Checktype {
        private static Type Check(string s, List<string> prefixlist)
        {
            Type find = null;
            foreach(var pre in prefixlist)
            {
                var ss = pre + "." + s;
                var ti = slagtool.runtime.sub_pointervar_clause.find_typeinfo(ss);
                if (ti!=null)
                {
                    if (find!=null)
                    {
                        sys.error("The type name is ambiguous : " + s);
                    }
                    else
                    {
                        find =ti;
                    }
                }
            }
            return find;
        }

        internal static YVALUE ChangeIfType(YVALUE v, List<string> prefixlist)
        {
            if (v.IsType(YDEF.NAME))
            {
                var vname = v.FindValueByTravarse(YDEF.NAME);
                var n = vname.GetString();
                Type type = GetPrimitiveType(n); 
                if (type==null)
                { 
                    type = Check(vname.GetString(),prefixlist);
                }
                if (type!=null)
                {
                    vname.type = YDEF.RUNTYPE;
                    vname.o = type;
                    vname.s = type.ToString();
                }
            }
            return v;
        }

        private static Type GetPrimitiveType(string s)
        {
            // ref https://msdn.microsoft.com/en-us/library/ms228360(v=vs.90).aspx

            s = s.ToLower();

            switch(s)
            {
                case "byte"   : return typeof(byte);
                case "sbyte"  : return typeof(sbyte);
                case "int"    : return typeof(int);
                case "uint"   : return typeof(uint);
                case "short"  : return typeof(short);
                case "ushort" : return typeof(ushort);
                case "long"   : return typeof(long);                     
                case "ulong"  : return typeof(ulong);
                case "float"  : return typeof(float);
                case "double" : return typeof(double);
                case "char"   : return typeof(char);
                case "bool"   : return typeof(bool);
                case "object" : return typeof(object);
                case "string" : return typeof(string);
                case "decimal": return typeof(decimal);
            }
        
            return null;
        }


    }
}
