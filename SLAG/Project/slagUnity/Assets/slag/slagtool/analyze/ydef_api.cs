using System;
using System.Collections.Generic;
using System.Reflection;
using number = System.Double;

namespace slagtool
{
    public partial class YDEF
    {
        public static string get_name(object[] o)
        {
            if (o.Length > 0) return (string)o[0];
            return null;
        }
        public static string get_name(int type)
        {
            if (Enum.IsDefined(typeof(TOKEN), type))
            {
                var n = (TOKEN)type;
                return n.ToString();
            }
            var list = get_syntax_order();
            var find = list.Find(i => get_type(i) == type);
            if (find != null) return get_name(find);
            return null;
        }
        public static int get_type(object[] o)
        {
            if (o.Length > 1) return (int)o[1];
            return -1;
        }
        public static int get_type(string syntax)
        {
            var list = get_syntax_order();
            var find = list.Find(i => get_name(i) == syntax);
            if (find != null) return get_type(find);
            return YDEF.ERROR;
        }

        //構文ツリー設定格納用
        public class TreeSet 
        {
            public object[] syntax_tree; //分解元            ※利用時に便宜
            public string name;          //文法名            ※　　〃
            public int type;             //トークンタイプ　　※　　〃

            public List<object> list;        //トークンタイプ、または、文字列
            public Func<int, YVALUE[], int[], YVALUE> make_func;
            public List<int> make_index;

            public override string ToString()
            {
                string s = null;
                list.ForEach(i=> {
                    if (s!=null) s+=",";
                    if (i.GetType()==typeof(int))
                    {
                        s+=YDEF.get_name((int)i);
                    }
                    else
                    {
                        s+=i.ToString();
                    }
                });
                return name +":"+s;
            }
        }

        // ydef.csの構文ツリーを解析用に変換
        public static List<TreeSet> get_syntax_set(object[] syntax_tree)
        {
            List<TreeSet> list = new List<TreeSet>();
            TreeSet ts = null;
            bool bMakeOrTree = false;

            string name = (string)syntax_tree[0];
            int type = (int)syntax_tree[1];

            for (int i = 2; i < syntax_tree.Length; i++)
            {
                var o = syntax_tree[i];
                if (o.GetType() == typeof(int) && (int)o == YDEF.__OR__)
                {
                    list.Add(ts);
                    ts = null;
                    bMakeOrTree = false;
                    continue;
                }

                if (o.GetType() == typeof(int) && (int)o == YDEF.__MAKE__)
                {
                    bMakeOrTree = true;
                    ts.make_index = new List<int>();
                    continue;
                }

                if (bMakeOrTree)
                {
                    if (ts.make_func == null)
                    {
                        ts.make_func = (Func<int, YVALUE[], int[], YVALUE>)o;
                        continue;
                    }
                    else
                    {
                        ts.make_index.Add((int)o);
                        continue;
                    }
                }
                else
                {
                    if (ts == null)
                    {
                        ts = new TreeSet();
                        ts.syntax_tree = syntax_tree;
                        ts.name = name;
                        ts.type = type;
                        ts.list = new List<object>();
                    }
                    var newo = o;
                    if (o.GetType() == typeof(string))
                    {
                        int temp = get_type((string)o);
                        if (temp >= 0) newo = (object)temp;
                    }
                    ts.list.Add(newo);
                    continue;
                }
            }
            if (ts != null)
            {
                list.Add(ts);
            }
            return list;
        }

        private static List<FieldInfo> __sx_members;
        public static List<FieldInfo> get_syntax_list()
        {
            if (__sx_members != null) return __sx_members;

            var type = typeof(YDEF);

            __sx_members = new List<FieldInfo>();
            foreach (var i in type.GetFields())
            {
                if (i.Name.StartsWith("sx_")) __sx_members.Add(i);
            }

            return __sx_members;
        }

        private static Dictionary<int,object[]> __sx_dic;
        public static Dictionary<int,object[]> get_sx_dic()
        {
            if (__sx_dic!=null) return __sx_dic;
            __sx_dic = new Dictionary<int, object[]>();
            var infos = get_syntax_list();

            foreach(var i in infos)
            {
                var o = (object[])i.GetValue(null);
                __sx_dic.Add((int)o[1],o);
            }

            return __sx_dic;
        }

        private static List<object[]> __syntax_order;
        public static List<object[]> get_syntax_order()
        {
            if (__syntax_order != null) return __syntax_order;
            var infos = get_syntax_list();
            __syntax_order = new List<object[]>();

            foreach (var i in infos)
            {
                var o = (object[])i.GetValue(null);
                __syntax_order.Add(o);
            }

            __syntax_order.Sort((a, b) => get_type(b) - get_type(a));

            return __syntax_order;
        }

        public static int get_syntax_root() //ルートとなる文法タイプ
        {
            var order = get_syntax_order();
            var lowest = order[order.Count-1];

            return get_type(lowest);
        }

        public static void insert_rest_children_if_use(ref List<YVALUE> list)// リスト中にRESTがある場合、RESTの子供を上位に挿入し、RESTを消す
        {
            bool bDone = false;

            Action<List<YVALUE>> work = null;
            work = (l) =>
            {
                if (bDone) return;
                for (int i = 0; i < l.Count; i++)
                {
                    var v = l[i];
                    if (v.type == YDEF.REST)
                    {
                        l.RemoveAt(i);
                        l.InsertRange(i, v.list);
                        bDone = true;
                        return;
                    }
                }
            };
            work(list);
        }
    }
}
