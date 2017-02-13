using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using number = System.Double;
using System.Reflection;

namespace slagtool.runtime.builtin
{
    public class builtin_sysfunc
    {
        static string NL = kit.NL;

        public static object F_Hashtable(bool bHelp, object[] ol,StateBuffer sb)
        {
            if (bHelp)
            {
                return "新規ハッシュテーブルの作成。" +NL + "例) var t = hashtable();  t.a = 123; t.b=234;";
            }
            return new System.Collections.Hashtable();
        }

        #region 変換
        public static object F_ToNumber(bool bHelp, object[] ol,StateBuffer sb=null)
        {
            if (bHelp)
            {
                return "文字列またはNumber以外数値型の値をNumber型の数値に変換。" + NL + "例)var a = ToNumber(\"123\");";
            }

            kit.check_num_of_args(ol,1);

            var x = kit.get_number_at(ol,0);
            if (!number.IsNaN(x))
            {
                return x;
            }
            return (number)0;
        }
        public static object F_Typeof(bool bHelp, object[] ol,StateBuffer sb=null)
        {
            if (bHelp)
            {
                return "タイプ型を取得。例)var t = typeof(System.Int32);";
            }

            kit.check_num_of_args(ol,1);

            var s = kit.get_string_at(ol,0).ToUpper();
            var type = slagtool.runtime.sub_pointervar_clause.find_typeinfo(s);

            return type;
        }
        public static object F_Cast(bool bHelp, object[] ol, StateBuffer sb = null)
        {
            if (bHelp)
            {
                return "値を指定タイプへキャスト。 例)var i = Cast(\"System.Int16\",j);  または Cast(type,j); ";
            }

            kit.check_num_of_args(ol,2);

            var p0 = ol[0];
            Type type = null;
            if(p0 is Type)
            {
                type = (Type)p0;
            }
            if (type == null)
            { 
                var s = kit.get_string_at(ol,0).ToUpper();
                type  = kit.FindType(s);// slagtool.runtime.sub_pointervar_clause.find_typeinfo(s);
            }

            var o = ol[1];
            return Convert.ChangeType(o,type);
        }
        public static object F_ToArray(bool bHelp, object[] ol, StateBuffer sb)
        {
            if (bHelp)
            {
                return "配列要素を指定タイプの要素へ変換する。" +NL + 
                       "フォーマット) var new_l1 = ToArray(\"System.Int16\",l);" + NL +
                       "        var type = typeof(\"System.Int16\"); var new_l2 = ToArray(\"System.Int16\",l);" + NL +
                       "補足: 返還後は C#での Array<タイプ>へ変換される。";
            }
            kit.check_num_of_args(ol,2);

            Type type = null;
            if (ol[0] is Type)
            {
                type = (Type)ol[0];
            }
            else
            { 
                var s = kit.get_string_at(ol,0).ToUpper();
                type = slagtool.runtime.sub_pointervar_clause.find_typeinfo(s);
            }

            var l = ol[1];
            if (l==null) return null;
            var lt = l.GetType();
            if (l is IList)
            {
                var rl = (IList)l;
                var oa = Array.CreateInstance(type,rl.Count);
                for(var i = 0; i<rl.Count; i++)
                {
                    var e = rl[i];
                    if (e!=null &&  e.GetType()!=type)
                    { 
                        e = Convert.ChangeType(e,type);
                    }
                    oa.SetValue(e,i);
                }
                return oa;
            }
            if (lt.IsArray)
            {
                var rl = (Array)l;
                var oa = Array.CreateInstance(type,rl.Length);
                for(var i = 0; i<rl.Length;i++)
                {
                    var e = rl.GetValue(i);
                    if (e!=null && e.GetType()!=type)
                    {
                        e = Convert.ChangeType(e,type);
                    }
                    oa.SetValue(e,i);
                }
                return oa;
            }
            util._error("Unexpected");
            return null;
        }
        #endregion

        #region Print/Dump

        public static Action<string> m_printFunc    =(s)=>System.Console.Write(s); 
        public static Action<string> m_printLnFunc  =(s)=>System.Console.WriteLine(s);

        public static object F_Println(bool bHelp,object[] ol,StateBuffer sb)
        {
            if (bHelp)
            {
                return "改行付き表示。例) PrintLn(\"hoge!\");";
            }
        
            var s = "";
            if (ol!=null&&ol.Length>0)
            { 
                kit.check_num_of_args(ol,1);
                var o = kit.get_ol_at(ol,0);
                s = kit.convert_escape(o);
            }
            //Debug.Log(s);
            //guiDisplay.WriteLine(s);
            m_printLnFunc(s);

            return null;
        }
        public static object F_Print(bool bHelp,object[] ol,StateBuffer sb)
        {
            if (bHelp)
            {
                return "表示。 例) Print(\"hoge!\");";
            }
            kit.check_num_of_args(ol,1);
            var o = kit.get_ol_at(ol,0);
            var s = kit.convert_escape(o);

            //Debug.Log(s);
            //guiDisplay.Write(s);
            m_printFunc(s);

            return null;
        }
        public static object F_Dump(bool bHelp,object[] ol,StateBuffer sb)
        {
            if (bHelp)
            {
                return "変数のDump。 例) Dump(x);";
            }

            if (ol==null) return "-null-";

            Func<object,string> tostr = null;
            Func<List<object>,string> join_list = (l)=> {
                string t= null;
                foreach(var e in l)
                {
                    if (t!=null) t+=",";
                    t+= tostr(e);
                }
                return t;
            };
            Func<Array,string> join_array = (l)=> {
                string t= null;
                foreach(var e in l)
                {
                    if (t!=null) t+=",";
                    t+= tostr(e);
                }
                return t;
            };
            Func<Hashtable,string> join_hashtable = (l)=> {
                string t = null;
                foreach(var k in l.Keys)
                {
                    if (t!=null) t+=",";
                    t+= k.ToString() + ":" + tostr(l[k]);
                }
                return t;
            };

            tostr = (a) => {
                if (a==null) return "-null-";
                if (a is List<object>)
                {
                    var l = (List<object>)a;
                    return "[" + join_list(l) + "]";
                }
                if (a.GetType().IsArray)
                {
                    var l = (Array)a;
                    return "(" + join_array(l) +")";
                }
                if (a is Hashtable)
                {
                    var l = (Hashtable)a;
                    return "{" + join_hashtable(l) + "}";
                }
                return a.ToString();
            };

            string s = null;
            foreach(var o in ol)
            {
                if (s!=null) s+=",";
                s += tostr(o);
            }

            //UnityEngine.Debug.Log(s);
            //guiDisplay.WriteLine(s);
            m_printLnFunc(s);

            return s;
        }



        #endregion





    }
}
