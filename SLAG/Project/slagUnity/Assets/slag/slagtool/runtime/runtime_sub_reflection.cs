using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Text;
using System.Diagnostics;
using LIST = System.Collections.Generic.List<object>;
using number = System.Double;

// constructorのタイプを調べる
//https://msdn.microsoft.com/en-us/library/h93ya84h(v=vs.110).aspx


namespace slagtool.runtime
{
    public class sub_reflection
    {
        internal static object ExecuteFunc(object o, string api, object[] parameters )
        {
            var name = api.ToUpper();

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

            var paramtypes = GetObjectsType(parameters);
            var mts = type.GetMethods();

            MethodInfo find_m = null;
            var mlist = cache_util.GetFuncCache(name,type,paramtypes);
            mlist.AddRange(mts);

            foreach(var m in mlist)
            {
                if (m.Name.ToUpper() != name) continue;
                var pis = m.GetParameters();
                if (_isMatchTypes(paramtypes,pis))
                {
                    find_m = m;
                    break;
                }                
            }

            if (find_m!=null)
            {
                cache_util.RecordCache(name,type,paramtypes,find_m);
                var p2 = ChangeObjs(parameters,find_m.GetParameters());
                if (obj==null && !find_m.IsStatic)
                { 
                    if (name == "TOSTRING")
                    {
                        return type.ToString();
                    }
                    throw new System.Exception("methods requires class pointer but it's null.");
                }
                else
                { 
                    return find_m.Invoke(obj,p2);
                }
            }

            throw new SystemException("Cannot find method : " + type + "." + name + "(API is none or parameter typs not match.)");
        }
        private static bool _isMatchTypes(Type[] paramtypes, ParameterInfo[] pis)
        {
            var bNull_paramtypes = __isNullOrNothing(paramtypes);
            var bNull_pis        = __isNullOrNothing(pis);

            //両方nullは適合
            if (bNull_paramtypes && bNull_pis) return true;  
            //片方nullは不適合
            if ( bNull_paramtypes ^ bNull_pis) return false;
            //引数の数が異なるは不適合
            if (paramtypes.Length != pis.Length) return false;


            bool bOk = true;

            ////全型一致検査
            //for(int i = 0; i<paramtypes.Length ; i++)
            //{
            //    var p = paramtypes[i];
            //    var f = pis[i].ParameterType;

            //    if (p==null && !f.IsValueType) continue; //Null許容はＯＫ
            //    if (p==f) continue;
            //    if (__isFloat(p) && __isFloat(f)) continue;//フロート型はdouble/single許容
            //    //if (util.IsNumeric(p) && util.IsNumeric(f)) continue;
            //    if (p.IsSubclassOf(f)) continue; //ベース一致

            //    bOk = false;
            //    break;
            //}
            //if (bOk) return true;
            
            //bOk = true;

            //全数値型を同一とみなす
            for(int i = 0; i<paramtypes.Length ; i++)
            {
                var p = paramtypes[i];
                var f = pis[i].ParameterType;

                if (p==null && !f.IsValueType) continue; //Null許容はＯＫ
                if (p==f) continue;
                if (util.IsNumeric(p) && util.IsNumeric(f)) continue;
                if (p.IsSubclassOf(f)) continue; //ベース一致

                bOk = false;
                break;
            }

            return bOk;
        }
        private static bool __isNullOrNothing<T>(T[] x)
        {
            return (x==null || x.Length==0);
        }
        private static bool __isFloat(Type t)
        {
            return (t==typeof(Single) || t==typeof(Double));
        }

        #region タイプ収取
        private static Type[] GetObjectsType(object[] args)
        {
            if (args==null || args.Length==0) return null;
            var tlist = new List<Type>();
            foreach(var a in args)
            {
                if (a!=null)
                {
                    tlist.Add(a.GetType());
                }
                else
                {
                    tlist.Add(null);
                }
            }
            return tlist.ToArray();
        }
        #endregion

        #region オブジェクトのタイプ変換
        private static object[] ChangeObjs(object[] ol, ParameterInfo[] pis)
        {
            util._assert(ol!=null&&pis!=null&&ol.Length==pis.Length);

            for(int i = 0;i<pis.Length; i++)
            {
                var o  = ol[i];
                var pi = pis[i];
                if (o==null) continue;
                var ot= o.GetType();
                if (ot==pi.ParameterType) continue;
                if (ot.IsEnum) continue;
                if (ot.IsSubclassOf(pi.ParameterType)) continue; 

                ol[i] = Convert.ChangeType(o,pi.ParameterType);
            }

            return ol;
        }
        #endregion

        internal static object InstantiateType(Type type, object[] parameters)
        {
            var paramtypes = GetObjectsType(parameters);
            var cts = type.GetConstructors();
            if (cts==null) return null;

            ConstructorInfo find_c = null;
            var clist = cache_util.GetNewCache(type,paramtypes);
            clist.AddRange(cts);
            
            foreach(var c in clist)
            {
                var pis = c.GetParameters();
                if (_isMatchTypes(paramtypes,pis))
                {
                    find_c = c;
                    break;
                }
            }

            if (find_c==null) util._error("the constractor can not find " + type.ToString() );
            
            cache_util.RecordCache(type,paramtypes);
            var p2 = ChangeObjs(parameters,find_c.GetParameters());
            return Activator.CreateInstance(type,args:p2);
        }

    }

    public class cache_util
    {
        private static Dictionary<object,LIST> m_hash;
        internal static LIST GetCache(object key)
        {
            if (m_hash!=null &&  m_hash.ContainsKey(key))
            {
                return m_hash[key];
            }
            return null;
        }
        internal static void RecordCache(object key, object val)
        {
            if (m_hash == null)
            {
                m_hash = new Dictionary<object, LIST>();
            }

            LIST vlist = m_hash.ContainsKey(key) ? m_hash[key] : new LIST();
            if (!vlist.Contains(val))
            {
                vlist.Add(val);
            }
            m_hash[key] = vlist;
        }

        #region Method Info用
        internal static List<MethodInfo> GetFuncCache(string name,Type type, Type[] tlist)
        {
            var key =_makekey_func(name,type,tlist);
            var vlist = GetCache(key);

            var ml = new List<MethodInfo>();
            if (vlist!=null) { 
                vlist.ForEach(i=> {
                    if (i is MethodInfo)
                    {
                        ml.Add((MethodInfo)i);
                    }
                });
            }
            return ml;         
        }
        internal static List<ConstructorInfo> GetNewCache(Type type, Type[] tlist)
        {
            var key =_makekey_new(type,tlist);
            var vlist = GetCache(key);

            var cl = new List<ConstructorInfo>();
            if (vlist!=null) { 
                vlist.ForEach(i=> {
                    if (i is ConstructorInfo)
                    {
                        cl.Add((ConstructorInfo)i);
                    }
                });
            }
            return cl;         
        }
        internal static void RecordCache(string name,Type type, Type[] tlist, MethodInfo m)
        {
            var key = _makekey_func(name,type,tlist);
            RecordCache(key,m);
        }
        internal static void RecordCache(Type type, Type[] tlist, ConstructorInfo c)
        {
            var key = _makekey_new(type,tlist);
            RecordCache(key,c);
        }
        private static string _makekey_func(string name,Type type, Type[] tlist)
        {
            return _makekey("mi_",name,type,tlist);
        }
        private static string _makekey_new(Type type, Type[] tlist)
        {
            return _makekey("ci_","",type,tlist);
        }

        private static string _makekey(string id, string name,Type type, Type[] tlist)
        {
            string s = id + name + type.ToString();
            if (tlist!=null)
            {
                foreach(var t in tlist)
                {
                    s += t!=null ? t.ToString() : "null";
                }
            } 
            return s;
        }
        #endregion
    }
}
