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

        //#region システム
        //public static object F_Sleep(bool bHelp, object[] ol,StateBuffer sb)
        //{
        //    if (bHelp)
        //    {
        //        return "指定時間Sleepする。　フォーマット) Sleep(sec)";
        //    }

        //    kit.check_num_of_args(ol,1);

        //    var x = kit.get_number_at(ol,0);
        //    if (!number.IsNaN(x))
        //    { 
        //        x = x * 1000.0f;
        //    }
        //    else
        //    {
        //        x = 1000;
        //    }
        //    System.Threading.Thread.Sleep((int)x);

        //    return null;
        //}
        //#endregion

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

#region 数値操作
        //public static object F_RandomInt(bool bHelp,object[] ol,StateBuffer sb) // 引数 0 -- 最少数  1 -- 最大数
        //{
        //    if (bHelp)
        //    {
        //        return "Get a random integer." + NL + "format: RandomInt(min, max)";
        //    }

        //    kit.check_num_of_args(ol,2);

        //    var min = kit.get_number_at(ol,0);
        //    var max = kit.get_number_at(ol,1);
        //    var diff = max - min;

        //    var r = new System.Random(DateTime.Now.Millisecond);
        //    var i = r.Next((int)diff+1);
           
        //    return (number)(min + i);
        //}
        //public static object F_ToInt(bool bHelp,object[] ol,StateBuffer sb)
        //{
        //    if (bHelp)
        //    {
        //        return "Conver a number to an integer number.";
        //    }

        //    kit.check_num_of_args(ol,1);

        //    var x = kit.get_number_at(ol,0);
        //    if (!number.IsNaN(x))
        //    {
        //        var i = (int)x;
        //        return (number)i;
        //    }
        //    return 0;
        //}
        //public static object F_Float(bool bHelp,object[] ol, StateBuffer sb)
        //{
        //    if (bHelp)
        //    {
        //        return "Cast number to float.";
        //    }
        //    var x = kit.get_number_at(ol,0);
        //    return (float)x;
        //}
        //public static object F_CastInt32(bool bHelp,object[] ol, StateBuffer sb)
        //{
        //    if (bHelp)
        //    {
        //        return "Cast numner to int32";
        //    }
        //    var x = kit.get_number_at(ol,0);
        //    return (System.Int32)x;
        //}
        //public static object F_Int(bool bHelp,object[] ol, StateBuffer sb)
        //{
        //    if (bHelp)
        //    {
        //        return "Cast numner to int";
        //    }
        //    var x = kit.get_number_at(ol,0);
        //    return (System.Int32)x;
        //}
#endregion

#region 文字列操作
        //public static object F_Substring(bool bHelp, object[] ol, StateBuffer sb)
        //{
        //    if (bHelp)
        //    {
        //        return "Substring. see c# string.substring.";
        //    }

        //    if (ol==null) return null;
        //    if (ol.Length==2)
        //    {
        //        var s = ol[0].ToString();
        //        var n = kit.get_number_at(ol,1);
        //        return s.Substring((int)n);
        //    }
        //    else if (ol.Length==3)
        //    {
        //        var s = ol[0].ToString();
        //        var n = kit.get_number_at(ol,1);
        //        var c = kit.get_number_at(ol,2);
        //        return s.Substring((int)n,(int)c);
        //    }

        //    kit.error("Substring syntax error");

        //    return null;
        //}

#endregion

#region 配列操作
        //public static object F_ListSize(bool bHelp,object[] ol,StateBuffer sb)
        //{
        //    if (bHelp)
        //    {
        //        return "Get size of the list.";
        //    }
        //    kit.check_num_of_args(ol,1);

        //    var list = kit.get_list_at(ol,0);
        //    if (list!=null)
        //    { 
        //        return (number)list.Count;
        //    }
        //    return (number)0;
        //}
        //public static object F_ListCombine(bool bHelp, object[] ol,StateBuffer sb)
        //{
        //    if (bHelp)
        //    {
        //        return "Combine list or element." + NL +"ex) var a = ListCombine((1,2),(3,4));";
        //    }

        //    var nl = new List<object>();
        //    if (ol!=null) for(int i = 0; i<ol.Length; i++)
        //    {
        //        var al = kit.get_list_at(ol,i);
        //        if (al!=null)
        //        {
        //            nl.AddRange(al);
        //        }
        //        else
        //        {
        //            var o = kit.get_ol_at(ol,i);
        //            if (o!=null)
        //            {
        //                nl.Add(o);
        //            }
        //        }
        //    }
        //    return (object)nl;
        //}
        //public static object F_ListSelectRandom(bool bHelp,object[] ol,StateBuffer sb)
        //{
        //    if (bHelp)
        //    {
        //        return "Select a elemenet in the list at random" + NL + "var a = ListSelectRandom(1,2,3);";
        //    }

        //    kit.check_num_of_args(ol,1);

        //    var list = kit.get_list_at(ol,0);
        //    if (list!=null)
        //    { 
        //        var rand = new Random(DateTime.Now.Millisecond);
        //        var n = rand.Next();
        //        var a = n % list.Count;
             
        //        return list[a];
        //    }
        //    return null;
        //}
        //public static object F_ListRemove(bool bHelp,object[] ol,StateBuffer sb)
        //{
        //    if (bHelp)
        //    {
        //        return "Remove a element from a list." + NL + "Format: var l = ListRemove(list,element);";
        //    }
        //    kit.check_num_of_args(ol,2);

        //    var list = kit.get_list_at(ol,0);
        //    var s    = kit.get_ol_at(ol,1);
        //    if (list==null || s==null) return null;
        //    list.Remove(s);

        //    return list;
        //}
        //public static object F_ListShuffle(bool bHelp,object[] ol,StateBuffer sb)
        //{
        //    if (bHelp)
        //    {
        //        return "Shuffle a list.";
        //    }
        //    kit.check_num_of_args(ol,1);

        //    var list = kit.get_list_at(ol,0);
        //    if (list!=null)
        //    { 
        //        var nl = new List<object>();
        //        var rand = new Random();
        //        if (list!=null)
        //        { 
        //            while(list.Count>0)
        //            {
        //                var n = rand.Next() % list.Count;
        //                var s = list[n];
        //                list.RemoveAt(n);
        //                nl.Add(s);
        //            }
        //        }
        //        return nl;
        //    }
        //    return null;
        //}
        //public static object F_ListAt(bool bHelp, object[] ol,StateBuffer sb)
        //{
        //    if (bHelp)
        //    {
        //        return "Get a element at the number of the list.";
        //    }

        //    kit.check_num_of_args(ol,2);

        //    var list = kit.get_list_at(ol,0);
        //    var n    = kit.get_number_at(ol,1);
        //    if (list!=null && !number.IsNaN(n) && n < list.Count)
        //    {
        //        return list[(int)n];
        //    }
        //    return null;
        //}
        //public static object F_ListSort(bool bHelp, object[] ol, StateBuffer sb)
        //{
        //    if (bHelp)
        //    {
        //        return "Sort the list";
        //    }

        //    var src = kit.get_list_at(ol,0);
        //    if (src==null) kit.error("ListSort arg is not valid.");
        //    var l = new List<object>(src);
        //    if (l.Count>0 && l[0].GetType()==typeof(number))
        //    {
        //        l.Sort((a,b)=> (int)Math.Ceiling((number)a - (number)b));
        //    }
        //    else
        //    { 
        //        l.Sort((a,b)=>string.Compare(a.ToString(),b.ToString()));
        //    }
        //    return l;
        //}
#endregion

//#region Assembley
//        public static object F_GetAllAsm(bool bHelp, object[] ol, StateBuffer sb)
//        {
//            if (bHelp)
//            {
//                return "";
//            }
//            foreach(var asm in System.AppDomain.CurrentDomain.GetAssemblies())
//            {
//                Console.WriteLine(asm.FullName);
//            }
//            return null;
//        }
//        public static object F_FindType(bool bHelp, object[] ol, StateBuffer sb)
//        {
//            if (bHelp)
//            {
//                return "";
//            }

//            var _type      = kit.get_string_at(ol,0).ToUpper();
//#if nounity
//            foreach(var asm in System.AppDomain.CurrentDomain.GetAssemblies())
//            {
//                foreach(var ti in asm.DefinedTypes)
//                {
//                    if (ti.FullName.ToUpper() == _type)
//                    {
//                        Console.WriteLine("Found Type:" + ti.ToString());

//                        var typ = ti.AsType();

//                        foreach(var m in ti.GetMethods())
//                        {
//                            Console.WriteLine(m.ToString());
//                        }

//                        return ti.AsType();
//                    }
//                }
//            }
//#endif
//            return null;
//        }

//#endregion

    }
}
