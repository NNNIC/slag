using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Reflection;
using LIST = System.Collections.Generic.List<object>;
using number = System.Double;

namespace slagtool.runtime
{
    // Ｃ＃依存部分
    public class sub_pointervar_clause //ピリオド区切りの文字列に対しての処理
    {
        public static StateBuffer run(YVALUE v, StateBuffer sb, PointervarMode mode = PointervarMode.GET) 
        {
            var nsb = sb;
            var item = new PointervarItem(); //先行アイテム。中身なし
            item.mode = mode;

            nsb.m_pvitem = item;

            var size = v.list_size();
            for(int i = 0 ; i<size ; i++)
            {
                var vn = v.list_at(i);
                if (vn==null) throw new SystemException("Unexpected");
                
                if (vn.IsType(YDEF.PERIOD)) continue;

                item = nsb.m_pvitem;
                item.setter = null;
                item.getter = null;

                nsb = run_script.run(vn,nsb.curnull());
                
                item = nsb.m_pvitem;
                if (i<size-1 && item.o == null) { //最後尾前のnull確認。 最後尾のNULLは容認。
                    if (sys.DEBUGMODE)
                    {
                        sys.logline("Null Pointer, but ignored at line:" + v.get_dbg_line(true) +" file:" + v.get_dbg_file());
                    }

                    break;                               //最近の流行りを取り入れてnullだったら後ろは処理しない
                }

                if (i<size-1)
                {
                    if (item.getter!=null)
                    {
                        item.o = item.getter();
                        nsb.m_pvitem =item;
                    }
                }
            }

            item = nsb.m_pvitem;

            if (item.mode == PointervarMode.SET)
            {
                nsb.m_cur = item;//.setter;
            }
            else if (item.mode == PointervarMode.GET)
            { 
                if (item.getter!=null)
                {
                    nsb.m_cur = item.getter();
                }
                else
                {
                    nsb.m_cur = item.o;
                }
            }
            else if (item.mode == PointervarMode.NEW)
            {
                nsb.m_cur = item.o;  
            }
            else if (item.mode == PointervarMode.ITEM)
            {
                nsb.m_cur = item;
            }
            nsb.pvitemnull();

            return nsb;
        }
        public static StateBuffer run_new_func(YVALUE v, StateBuffer sb) 
        {
            var nsb = sb;
            var item = new PointervarItem(); //先行アイテム。中身なし
            item.mode =  PointervarMode.NEW;

            nsb.m_pvitem = item;
            
            nsb = run_script.run(v,nsb.curnull());

            nsb.m_cur = nsb.m_pvitem.o;
            nsb.pvitemnull();

            return nsb;
        }
        public static StateBuffer run_new_array_var(YVALUE v, StateBuffer sb)
        {
            var nsb = sb;
            var item = new PointervarItem(); //先行アイテム。中身なし
            item.mode = PointervarMode.NEW;

            nsb.m_pvitem = item;

            nsb = run_script.run(v,nsb.curnull());

            nsb.m_cur = nsb.m_pvitem.o;
            nsb.pvitemnull();

            return nsb;
        }

        public static StateBuffer run_name(YVALUE v, StateBuffer sb)
        {
            var nsb = sb;
            var name = v.GetString();
            PointervarItem item = nsb.m_pvitem;
            var preobj = item.o; //先行ロケーションアイテムの値
            if (preobj == null) //先行値がないのでNAMEとしてバッファを検索し、なければリテラルとして処理を以降に任せる
            {
                if (nsb.exist(name))
                { 
                    item.o = nsb.get(name);
                }
                else
                {
                    var literal = new Literal();
                    literal.s = name;
                    item.o = literal;
                }
                nsb.m_pvitem = item;
                
                return nsb;                
            }
            var pretype = preobj.GetType();

            if (pretype == typeof(Literal))
            {
                var literal = (Literal)preobj;
                item = GetObj(literal.s,name, item);
                nsb.m_pvitem =item;
                return nsb;
            }

            if (pretype == typeof(Hashtable))
            {
                var ht = (Hashtable)preobj;
                var nameo = name.ToUpper();
                item.o = ht[nameo];
                item.getter = ()=>ht[nameo];
                item.setter_parametertype = null;
                item.setter = (x)=>ht[nameo]=x;
                nsb.m_pvitem = item;
                return nsb;
            }

            item = GetObj(preobj,name, item);
            nsb.m_pvitem = item;
            return nsb;
        }
        public static StateBuffer run_func(YVALUE v, StateBuffer sb, string name, List<object> ol)
        {
            var nsb = sb;
            var item = nsb.m_pvitem;
            var preobj = item.o; //先行ロケーションアイテムの値
            if (preobj == null)  //先行値がない場合はRUNTYPEがv内にある。
            {
                var vr = v.FindValueByTravarse(YDEF.RUNTYPE);
                if (vr!=null && vr.o is Type)
                {
                    var ti = (Type)vr.o;
                    item.o = runtime.sub_reflection.InstantiateType(ti,ol.ToArray());
                    nsb.m_pvitem = item;
                    return nsb;
                }
                var fv = (YVALUE)nsb.get_func(name);
                if (fv!=null)
                {
                    nsb.m_pvitem = null;
                    nsb = util.CallFunction(fv,ol,nsb.curnull());
                    item.o = nsb.m_cur;
                    nsb.m_pvitem = item;
                    return nsb;
                }
                else
                {
                    if (builtin.builtin_func.IsFunc(name))
                    {
                        nsb.m_pvitem = null;
                        nsb.m_cur = builtin.builtin_func.Run(name,ol.ToArray(),nsb.curnull());
                        item.o= nsb.m_cur;
                        nsb.m_pvitem = item;
                        return nsb;
                    }
                    //util._error("function is not defined:" + name);
                }

                throw new SystemException("unexpected");
            }
            var pretype = preobj.GetType();
            if (pretype == typeof(Literal))
            {
                var literal = (Literal)preobj;
                item = ExecuteFunc(literal.s,name,ol,item);
                nsb.m_pvitem =item;
            }
            else if (pretype == typeof(Hashtable))
            {
                var ht = (Hashtable)preobj;
                var n  = name.ToUpper();
                var funcobj = ht[n];
                var nol = new LIST();
                nol.Add(ht);
                if (ol!=null) nol.AddRange(ol);

                if (funcobj!=null)
                {
                    if (funcobj is YVALUE)
                    {
                        var fv = (YVALUE)funcobj;
                        nsb.m_pvitem = null;
                        nsb = util.CallFunction(fv,nol,nsb);
                        item.o = nsb.m_cur;
                        nsb.curnull();
                        nsb.m_pvitem = item;
                        return nsb;
                    }
                }
                util._error("ハッシュテーブル内に関数がありません : " + name);
            }
            else
            {
                item = ExecuteFunc(preobj,name,ol,item);
            }
            return nsb;
        }
        public static StateBuffer run_num(YVALUE v, StateBuffer sb)
        {
            var nsb = sb;
            var item = nsb.m_pvitem;
            item.o = v.GetNumber();
            nsb.m_pvitem =item;
            return nsb;
        }
        public static StateBuffer run_qstr(YVALUE v, StateBuffer sb)
        {
            var nsb = sb;
            var item = nsb.m_pvitem;
            item.o =v.GetString();
            nsb.m_pvitem = item;
            return nsb;
        }
        public static StateBuffer run_array_var(YVALUE v, StateBuffer sb, string name, object index_o)
        {
            var nsb  = sb;
            var item = nsb.m_pvitem;
            var preobj = item.o; 
            if (preobj == null)
            {
                if (item.mode == PointervarMode.NEW && v.list_at(0).type == YDEF.RUNTYPE)
                {
                    var type = (Type)v.list_at(0).o;
                    item = ExecuteArrayVar(type,index_o,item); // tbc
                    nsb.m_pvitem = item;
                    return nsb;
                }

                item.o =nsb.get(name,index_o);
                nsb.m_pvitem = item;
                return nsb;

                //throw new SystemException("unexpected");
            }

            var pretype = preobj.GetType();
            if (pretype == typeof(Literal))
            {
                var literal = (Literal)preobj;
                item = ExecuteArrayVar(literal.s,name, index_o ,item);
                nsb.m_pvitem =item;
            }
            else
            {
                item = ExecuteArrayVar(preobj,name,index_o,item); // tbc
                nsb.m_pvitem =item;
            }
            return nsb;
        }
        public static StateBuffer run_array_value(StateBuffer sb, YVALUE v, object index_o)
        {
            var nsb = sb;
            nsb = run_script.run(v,nsb);
            var item = nsb.m_pvitem;
            if (item.getter!=null) item.o = item.getter();
            if (item.o!=null)
            {
                if (item.o.GetType().IsArray)
                {
                    var i = (int)util.ToNumber(index_o);
                    var a = (Array)item.o;
                    item.o = a.GetValue(i);

                    nsb.m_pvitem = item;
                    return nsb;
                }
                if (item.o.GetType() == typeof(LIST))
                {
                    var i = (int)util.ToNumber(index_o);
                    var l = (LIST)item.o;
                    item.o = l[i];

                    nsb.m_pvitem = item;
                    return nsb;
                }
                if (item.o.GetType() == typeof(Hashtable))
                {
                    var ht = (Hashtable)item.o;
                    item.o = ht[index_o];

                    nsb.m_pvitem = item;
                    return nsb;
                }
                if (item.o is string)
                {
                    var i = (int)util.ToNumber(index_o);
                    var s = (string)item.o;
                    item.o = s[i];

                    nsb.m_pvitem = item;
                    return nsb;
                }
            }
            util._error("unexpected");
            return null;
        }
        public static StateBuffer run_runtype(YVALUE v, StateBuffer sb)
        {
            var nsb = sb;
            var item = nsb.m_pvitem;
            item.o = v.o;
            nsb.m_pvitem = item;
            return nsb;
        }
        // -- tool for this class
        private static PointervarItem GetObj(string pre, string cur, PointervarItem item)
        {
            //アセンブリ調査 --- set/get不明なので直前の形で返す
            var searchname = (pre + "." + cur).ToUpper();
            var ti = find_typeinfo(searchname);
            if (ti!=null)
            {
                item.o = ti;
                return item;
            }
            //ない場合は、ピリオドで結合してリテラルとして返す
            var literal = new Literal();
            literal.s = pre + "." + cur;
            item.o = literal;
            return item;
        }
        private static PointervarItem GetObj(object o, string cur,PointervarItem item)
        {
            var name = cur.ToUpper();
            Type type = null;
            if (o is Type)
            {
                type = (Type)o;
            }
            object obj  = null;
            if (type!=null)
            {
                obj  = null;
            }
            else
            {
                type = o.GetType();
                obj  = o;
            }
#if !test
            var mem1 = type.GetDefaultMembers();
            var mem2 = type.GetMembers();
            var find_mi = Array.Find(type.GetMembers(),mi=>mi.Name.ToUpper()==name);
            if (find_mi!=null)
            { 
                if (find_mi.MemberType == MemberTypes.Property)
                { 
                    var pi = type.GetProperty(find_mi.Name);
                    item.getter = ()=>pi.GetValue(obj,null);
                    item.setter_parametertype = pi.PropertyType;
                    item.setter = (x)=> pi.SetValue(obj,x,null);
                    return item;
                }
                if (find_mi.MemberType == MemberTypes.Field)
                {
                    var fi = type.GetField(find_mi.Name);
                    item.getter = ()=> fi.GetValue(obj); 
                    item.setter_parametertype = fi.FieldType;
                    item.setter = (x)=> fi.SetValue(obj,x); 
                    return item;
                }
                
                throw new System.Exception("unknown");
            }
#endif
            return _GetObjMissing(type,obj,name,item);
        }
        private static PointervarItem _GetObjMissing(Type type, object obj, string name, PointervarItem item) //IL2CPP対策
        {
            var subtypename = "F_" + type.FullName.Replace('.','_');
            var subtype = find_typeinfo(subtypename);
            if (subtype!=null)
            {
                var mts = subtype.GetMethods(BindingFlags.Static| BindingFlags.Public);

                var searchname_set = "__SET__" + name;
                var find_set =Array.Find(mts, m=>m.Name.ToUpper()==searchname_set);
                if (find_set!=null)
                {
                    item.setter = (x) => find_set.Invoke(null,new object[2] { obj, x });
                }

                var searchname_get = "__GET__" + name;
                var find_get =Array.Find(mts, m=>m.Name.ToUpper()==searchname_get);
                if (find_get!=null)
                {
                    item.getter = () => find_get.Invoke(null,new object[1] { obj });
                }

                return item;
            }
            util._error("Unknown Name : " + name + "(" + type.Name + ")");
            return null;
        }



        private static PointervarItem ExecuteFunc(string pre, string cur, List<object> param, PointervarItem item)
        {
            if (item!=null && item.mode == PointervarMode.NEW)
            {
                var searchname = (pre + "." + cur).ToUpper();
                var ti = find_typeinfo(searchname);
                if (ti!=null)
                {
                    item.o = runtime.sub_reflection.InstantiateType(ti,param.ToArray());//Activator.CreateInstance(ti,args:param.ToArray());
                }
                return item;
            }
            throw new SystemException("unexpected");
        }
        private static PointervarItem ExecuteFunc(object o, string cur, List<object> param, PointervarItem item)
        {
            item.o = sub_reflection.ExecuteFunc(o,cur,param.ToArray());
            return item;
        }
        private static PointervarItem ExecuteArrayVar(string pre, string cur, object index_o, PointervarItem item)
        {
            var index = (int)util.ToNumber(index_o);
            if (item != null && item.mode == PointervarMode.NEW)
            {
                var searchname = (pre + "." + cur).ToUpper();
                var ti = find_typeinfo(searchname);
                if (ti != null)
                {
                    item.o =  Array.CreateInstance(ti, index);
                }
                return item;
            }
            throw new SystemException("unexpected");
        }
        private static PointervarItem ExecuteArrayVar(Type type,object index_o, PointervarItem item)
        {
            var index = (int)util.ToNumber(index_o);
            if (item != null && item.mode == PointervarMode.NEW)
            {
                item.o =  Array.CreateInstance(type, index);
                return item;
            }
            throw new SystemException("unexpected");
        }
        private static PointervarItem ExecuteArrayVar(object o, string cur, object index_o, PointervarItem item)
        {
            if (index_o==null) throw new SystemException("index is null");
            var name = cur.ToUpper();

            int index = -1;
            if (util.IsNumeric(index_o.GetType())) index = (int)util.ToNumber(index_o);

            Type otype = null;
            if (o is Type)
            {
                otype = (Type)o;
            }
            object obj = null;
            if (otype!=null)
            {
                obj = null;
            }
            else
            {
                otype = o.GetType();
                obj = o;
            }

            if (otype == typeof(Hashtable))
            {
                var ht = (Hashtable)obj;
                var val = ht[name];
                if (val==null)
                {
                    item.getter = null;
                    item.setter_parametertype = null;
                    item.setter = null;
                    item.o = null;
                    return item;
                }
                
                if (val.GetType()==typeof(LIST))
                {
                    var l= (LIST)val;
                    item.getter = ()=>l[index];
                    item.setter_parametertype = null;
                    item.setter = (x)=>l[index]=x;
                    item.o = l;
                    return item;
                }

                if (val.GetType()==typeof(Hashtable))
                {
                    var ht2 = (Hashtable)val;
                    item.getter = ()=>ht2[index_o];
                    item.setter_parametertype = null;
                    item.setter = (x)=>ht2[index_o] = x;
                    item.o = ht2;
                    return item;
                }

            }
            var mem1 = otype.GetDefaultMembers();
            var mem2 = otype.GetMembers();
            var find_mi = Array.Find(otype.GetMembers(),mi=>mi.Name.ToUpper()==name);
            if (find_mi!=null)
            { 
                if (find_mi.MemberType == MemberTypes.Property)
                { 
                    var pi = otype.GetProperty(find_mi.Name);
                    item.getter = ()=>  { return pi.GetValue(obj,new object[1] { index }); };
                    item.setter_parametertype = pi.PropertyType;
                    item.setter = (x)=> { pi.SetValue(obj,x,new object[1] {index });      };
                    return item;
                }
                if (find_mi.MemberType == MemberTypes.Field)
                {
                    var fi = otype.GetField(find_mi.Name);
                    item.getter = ()=>  {
                        var z = fi.GetValue(obj);
                        if (otype.IsArray)
                        {
                            var a = (Array)z;
                            return a.GetValue(index);
                        }
                        if (otype.IsGenericType && true)
                        {
                            if (otype is IList )
                            { 
                                var a = (IList)z;
                                return a[index];
                            }
                        }
                        return null;
                    };
                    item.setter = (x)=> {
                        var z = fi.GetValue(obj);
                        if (otype.IsArray)
                        {
                            var a = (Array)z;
                            a.SetValue(x,index);
                        }
                        if (otype.IsGenericType && true)
                        {
                            if (otype is IList )
                            { 
                                var a = (IList)z;
                                a[index]=x;
                            }
                        }
                    };
                    return item;
                }
                throw new System.Exception("unknown");
            }
            return item;

        }


#region タイプ検索
        internal class AllAssemblies
        {
            Dictionary<string,Type> m_dic;
            public AllAssemblies() {
                m_dic = new Dictionary<string, Type>();
                foreach(var asm in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    var types= asm.GetTypes();

                    foreach(var ti in asm.GetTypes())
                    {
                        var n = ti.FullName.ToUpper();
                        if (!m_dic.ContainsKey(n))
                        { 
                            m_dic.Add(n,ti);
                        }
                    }
                }
            }

            internal Type Find(string name)
            {
                name = name.ToUpper();
                if (m_dic.ContainsKey(name))
                {
                    return m_dic[name];
                }
                return null;
            }
        }
        private static AllAssemblies m_allAssemblies = new AllAssemblies(); //起動時に初期化

        public static Type find_typeinfo(string searchname)
        {
            return m_allAssemblies.Find(searchname);
        }
#endregion
    }
}
