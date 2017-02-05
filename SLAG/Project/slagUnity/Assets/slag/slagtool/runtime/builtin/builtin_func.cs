using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Reflection;
using number = System.Double;

namespace slagtool.runtime.builtin
{
    public class builtin_func
    {
        public class item
        {
            public string category;
            public string name;
            
            public MethodInfo mi;

            public string Help()
            {
                var s =  mi.Invoke(null,new object[] {true,null,null});
                return s.ToString();
            }
            public object Exec(object[] ol, StateBuffer sb=null)
            {
                return mi.Invoke(null,new object[] {false,ol,sb });
            }
        }

        static bool m_bReady = false;

        public static Hashtable m_hash;

        public class CategoryData { public string categoryname; public Type type; }
        public static List<CategoryData> m_categoryList = new List<CategoryData>();

        public static void Subscribe(Type type, string name)
        {
            if ( m_categoryList.Find(i=>i.type == type)==null )
            { 
                m_categoryList.Add(new CategoryData() { type = type, categoryname = name });
            }
        }

        public static void Init()
        {
            if (!m_bReady)
            {
                m_categoryList.Insert(0,new CategoryData() { type = typeof(builtin_sysfunc), categoryname = "システム" });
                m_categoryList.ForEach(d=>RegisterFunctions(d));
                m_bReady = true;
            }
        }
        private static void RegisterFunctions(CategoryData d)
        {
            if (m_hash == null)
            {
                m_hash = new Hashtable();
            }
            foreach (var m in d.type.GetMethods())
            {
                var n = m.Name;
                if (!n.StartsWith("F_")) continue;
                n = n.Substring(2);
                m_hash[n.ToUpper()] = new item() { category = d.categoryname, name =n, mi = m };
            }
        }

        public static bool IsFunc(string name)
        {
            if (m_hash==null) return false;
            var i = (item)m_hash[name.ToUpper()];
            return (i!=null);
        }
        public static object Run(string name, object[] ol,StateBuffer sb)
        {
            var i = (item)m_hash[name.ToUpper()];
            if (i ==null) return null;

            return i.Exec(ol,sb);
        }
        public static string Help()
        {
            if (m_hash==null) Init();

            string NL = Environment.NewLine;
            
            string s = null;

            foreach(var cat in m_categoryList)
            {
                s += string.Format("== {0} Functions ==",cat.categoryname) + NL;
                foreach(var k in m_hash.Keys)
                {
                    var i = (item)m_hash[k];
                    if (i.category == cat.categoryname)
                    {
                        s+= helpFormat(i) + NL; 
                    }
                }
            }
            
            return s;
        }
        private static string helpFormat(item i)
        {
            string NL = Environment.NewLine;
            string s = string.Format("{0,-20}", i.name) + " : " ;
            var help = i.Help();
            if (string.IsNullOrEmpty(help))
            {
                return s;
            }
            var lines= help.Split('\x0a');
            s += lines[0];
            if (lines.Length==1) return s;
            for(int n = 1; n < lines.Length; n++)
            {
                var t = lines[n].Trim();
                if (string.IsNullOrEmpty(t)) continue;
                s += NL + new string(' ',23) + t;
            }
            return s;
        }
    }
}
