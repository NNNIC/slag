using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using LIST = System.Collections.Generic.List<object>;
using number = System.Double;

namespace slagtool.runtime
{
    public class util
    {
        internal static YVALUE GetOptimize(YVALUE v)
        {
            YVALUE findV = v;

            Action<YVALUE> trv = null;
            trv = (w)=> {
                if (w.list_size()==1)
                {
                    findV = w;
                    trv(w);
                }
                else
                {
                    return;
                }
            };

            return findV;
        }

        internal static bool is_paramlist(YVALUE v)
        {
            if (v.type == YDEF.get_type(YDEF.sx_expr))
            {
                if (v.list.Count>=3)
                { 
                    for(int i = 1; i < v.list.Count; i+=2)
                    {
                        if (v.list_at(i).GetString()!=",") return false;            
                    }
                    return true;
                }
            }
            return false;
        }
        internal static YVALUE normalize_func_bracket(YVALUE v)
        {
            if (v.type != YDEF.get_type(YDEF.sx_expr_bracket)) _error("unexpected");

            Func<YVALUE,YVALUE> comb = null;
            comb = (w) => {
                if (!is_paramlist(w))
                {
                    return w;                    
                }
                var x = comb(w.list_at(0));
                var c = w.list_at(1); //, comma
                var y = w.list_at(2); //
                if (is_paramlist(x))
                {
                    w.list.Clear();
                    w.list.Add(x.list_at(0));
                    w.list.Add(x.list_at(1));
                    w.list.Add(x.list_at(2));
                    w.list.Add(c);
                    w.list.Add(y);
                }
                return w;
            };
            
            if (v.list.Count==3)
            {
                var nv = comb(v.list_at(1));
                return nv;    
            }
            v.list.Clear();
            return v;        
        }

        internal static YVALUE check_switch_sentence_block(YVALUE v)
        {
            if (v.type != YDEF.get_type(YDEF.sx_sentence_block)) throw new System.Exception("unexpected switch block #1");
            var inblock = v.list_at(1);
            if (inblock.type == YDEF.get_type(YDEF.sx_sentence))
            {
                check_case(inblock);
                return v;
            }
            if (inblock.type == YDEF.get_type(YDEF.sx_sentence_list))
            {
                var list = inblock.list_at(0);
                for(int i = 0; i<list.list.Count; i++)
                {
                    check_case(list.list_at(i));
                }
                return v;
            }
            _error("unexpected switch block #2");
            return null;
        }
        private static void check_case(YVALUE v)
        {
            if (v.IsType(YDEF.sx_case_clause))
            {
                // ok!  なんでもＯＫとする。

#if obs
                var expr = v.list_at(1);                
                if (expr.IsType(YDEF.QSTR) || expr.IsType(YDEF.NUM))
                {
                    ;//ok
                }
                else
                {
                    _error("unexpected case sentence");
                }
#endif
            }
            else if (v.IsType(YDEF.sx_default_clause))
            {
                ; //ok
            }
            else if (v.IsType(YDEF.sx_sentence))
            {
                ;//ok
            }
            else
            {
                _error("unexpected switch senetence");
            }
        }
        internal static string DelDQ(string i)
        {
            var s = i;
            if (string.IsNullOrEmpty(i)) return "";
            if (s.StartsWith("\"")) s=s.Substring(1);
            if (s.EndsWith("\""))   s=s.Substring(0,s.Length-1);
            return s;
        }

        internal static Func<object, object, string, object> User_Calc_op = null; //ユーザ用
        internal static object Calc_op(object a, object b, string op)
        {
            if (a!=null && b==null)
            {
                switch(op)
                {
                    case "+": return a;
                    case "==": return false;
                    case "!=": return true;
                }
                _error("unexpected null");
                return null;
            }
            if (a==null && b!=null)
            {
                switch(op)
                {
                    case "+": return b;
                    case "==": return false;
                    case "!=": return true;
                }
                _error("unexpected null");
                return null;
            }
            if (a==null && b==null)
            {
                switch(op)
                {
                    case "+": return null;
                    case "==": return true;
                    case "!=": return false;
                }
                _error("unexpected null");
                return null;
            }
            if (a.GetType()==typeof(string))
            {
                var x = a.ToString();
                var y = b.ToString();
                switch(op)
                {
                    case "+":   return x + y;
                    case "==":  return (bool)(x==y);
                    case "!=":  return (bool)(x!=y);
                    default:    _error("unexpected string operaion");   break;
                }
            }
            else if (util.IsNumeric(a.GetType()))  // if (a.GetType()==typeof(number))
            {
                switch(op)
                {
                    case "+":   
                    case "-":   
                    case "*":   
                    case "/":   
                    case "%":   return _calc_numeric(a,b,op);
                }

                var x = util.ToNumber(a);
                var y = util.ToNumber(b);

                switch(op)
                { 
                    case "==":  return (bool)(x==y);
                    case "!=":  return (bool)(x!=y);
                    case ">":   return (bool)(x>y);
                    case ">=":  return (bool)(x>=y);
                    case "<":   return (bool)(x<y);
                    case "<=":  return (bool)(x<=y);
                    default:    _error("unexpected number operaion:" + op);   break;                 
                }
            }
            else if (a.GetType()==typeof(bool))
            {
                var x = (bool)a;
                var y = (bool)b;

                switch(op)
                {
                    case "==":  return (bool)(x==y);
                    case "!=":  return (bool)(x!=y);
                    case "||":  return (bool)(x||y);
                    case "&&":  return (bool)(x&&y);
                    default:    _error("unexpected bool operaion:" + op);   break;                
                }
            }
            else if (a.GetType()==typeof(LIST))
            {
                if (b.GetType()==typeof(LIST))
                {
                    var ary_a = (LIST)a;
                    var ary_b = (LIST)b;
                    switch(op)
                    {
                        //case "+": ary_a.AddRange(ary_b); return ary_a;
                        case "==": return ary_a.SequenceEqual(ary_b);
                        default:    _error("unexpected bool operaion:" + op);   break;                
                    }
                }
                //else
                //{
                //    var ary_a = (LIST)a;
                //    switch(op)
                //    {
                //        case "+": ary_a.Add(b); return ary_a;
                //        default:    _error("unexpected bool operaion:" + op);   break;                
                //    }
                //}
                _error("unexpected bool operaion:" + op);
            }

            if (User_Calc_op!=null)
            {
                return User_Calc_op(a,b,op);
            }

            _error("unexpected calc op:"+op);  
            return null;                 
        }

#region calc numeric ... for optimize
        private static object _calc_numeric(object a, object b, string op)
        {
            var atype = a.GetType();
            var btype = b.GetType();
            object b2= atype==btype ? b : Convert.ChangeType(b,atype);
            if (a is System.Byte)   return __calc_num((System.Byte)a , (System.Byte)b2,op);
            if (a is System.SByte)  return __calc_num((System.SByte)a, (System.SByte)b2,op);

            if (a is System.Int16)  return __calc_num((System.Int16)a, (System.Int16)b2,op);
            if (a is System.UInt16) return __calc_num((System.UInt16)a,(System.UInt16)b2,op);

            if (a is System.Int32)  return __calc_num((System.Int32)a, (System.Int32)b2,op);
            if (a is System.UInt32) return __calc_num((System.UInt32)a,(System.UInt32)b2,op);

            if (a is System.Int64)  return __calc_num((System.Int64)a, (System.Int64)b2,op);
            if (a is System.UInt64) return __calc_num((System.UInt64)a,(System.UInt64)b2,op);

            if (a is System.Single) return __calc_num((System.Single)a, (System.Single)b2,op);
            if (a is System.Double) return __calc_num((System.Double)a, (System.Double)b2,op);

            return null;

        }
        private static object __calc_num(System.Byte a, System.Byte b, string op)
        {
            switch (op)
            {
                case "+": return a + b;
                case "-": return a - b;
                case "*": return a * b;
                case "/": return a / b;
                case "%": return a % b;
            }
            _error("unexpected number operaion:" + op);
            return null;
        }
        private static object __calc_num(System.SByte a, System.SByte b, string op)
        {
            switch (op)
            {
                case "+": return a + b;
                case "-": return a - b;
                case "*": return a * b;
                case "/": return a / b;
                case "%": return a % b;
            }
            _error("unexpected number operaion:" + op);
            return null;
        }
        private static object __calc_num(System.Int16 a, System.Int16 b, string op)
        {
            switch (op)
            {
                case "+": return a + b;
                case "-": return a - b;
                case "*": return a * b;
                case "/": return a / b;
                case "%": return a % b;
            }
            _error("unexpected number operaion:" + op);
            return null;
        }
        private static object __calc_num(System.UInt16 a, System.UInt16 b, string op)
        {
            switch (op)
            {
                case "+": return a + b;
                case "-": return a - b;
                case "*": return a * b;
                case "/": return a / b;
                case "%": return a % b;
            }
            _error("unexpected number operaion:" + op);
            return null;
        }
        private static object __calc_num(System.Int32 a, System.Int32 b, string op)
        {
            switch (op)
            {
                case "+": return a + b;
                case "-": return a - b;
                case "*": return a * b;
                case "/": return a / b;
                case "%": return a % b;
            }
            _error("unexpected number operaion:" + op);
            return null;
        }

        private static object __calc_num(System.UInt32 a, System.UInt32 b, string op)
        {
            switch (op)
            {
                case "+": return a + b;
                case "-": return a - b;
                case "*": return a * b;
                case "/": return a / b;
                case "%": return a % b;
            }
            _error("unexpected number operaion:" + op);
            return null;
        }
        private static object __calc_num(System.Int64 a, System.Int64 b, string op)
        {
            switch (op)
            {
                case "+": return a + b;
                case "-": return a - b;
                case "*": return a * b;
                case "/": return a / b;
                case "%": return a % b;
            }
            _error("unexpected number operaion:" + op);
            return null;
        }

        private static object __calc_num(System.UInt64 a, System.UInt64 b, string op)
        {
            switch (op)
            {
                case "+": return a + b;
                case "-": return a - b;
                case "*": return a * b;
                case "/": return a / b;
                case "%": return a % b;
            }
            _error("unexpected number operaion:" + op);
            return null;
        }
        private static object __calc_num(System.Single a, System.Single b, string op)
        {
            switch (op)
            {
                case "+": return a + b;
                case "-": return a - b;
                case "*": return a * b;
                case "/": return a / b;
                case "%": return a % b;
            }
            _error("unexpected number operaion:" + op);
            return null;
        }
        private static object __calc_num(System.Double a, System.Double b, string op)
        {
            switch (op)
            {
                case "+": return a + b;
                case "-": return a - b;
                case "*": return a * b;
                case "/": return a / b;
                case "%": return a % b;
            }
            _error("unexpected number operaion:" + op);
            return null;
        }
#endregion

        internal static bool IsNumeric(Type type)
        {
            return (
                type == typeof(System.Byte)    ||   type == typeof(System.SByte)
                ||
                type == typeof(System.Int16)   ||   type == typeof(System.UInt16)
                ||
                type == typeof(System.Int32)   ||   type == typeof(System.UInt32)
                ||
                type == typeof(System.Int64)   ||   type == typeof(System.UInt64)
                ||
                type == typeof(System.Single)  ||   type == typeof(System.Double)
                );
        }
        internal static number ToNumber(object o, bool ErrorAsNan=false)
        {
            if (o==null)
            {
                if (ErrorAsNan) return number.NaN;
                util._error("number is null");
            }
            var ot =o.GetType();
            if (!IsNumeric(ot))
            {
                if (ErrorAsNan) return number.NaN;
                util._error("it is not numeric");
            }
            if (ot != typeof(number))
            {
                return (number)Convert.ChangeType(o,typeof(number));
            }
            return (number)o;
        }

        #region call script function
        internal static StateBuffer CallFunction(YVALUE fv,List<object> ol, StateBuffer sb)
        {
            var nsb = sb;
            YDEF_DEBUG.funcCntSmp++;
            nsb.set_funcwork();
            {
                var fvbk = util.normalize_func_bracket(fv.list_at(1).list_at(1)); //ファンクション定義部の引数部分
                if (
                    ( (fvbk!=null && ol!=null) && (((fvbk.list.Count + 1) / 2) != ol.Count) )
                   )
                {
                    util._error("number of arguments in valid.");
                }
                int n = 0;
                if (fvbk!=null) for(int i = 0; i<fvbk.list.Count; i+=2)
                {
                    var varname = fvbk.list_at(i).GetString();//定義側の変数名
                    object o = ol!=null && n < ol.Count ? ol[n] : null;
                    nsb.define(varname, o);
                    n++;
                }
                nsb = runtime.run_script.run(fv.list_at(2),nsb);
                nsb.breaknone();
            }
            nsb.reset_funcwork();
            YDEF_DEBUG.funcCntSmp--;

            return nsb;
        }
        #endregion

        # region 配列の値取得
        internal static bool GetValueInArray(out object ret, object v, object index, string cmtname=null/*コメント用名前*/)
        {
            ret = null;
            if (v!=null)
            {
                var t = v.GetType();
                if (t==typeof(LIST) || t.IsArray)
                { 
                    var i = (int)util.ToNumber(index);
                    if (t==typeof(LIST))
                    {
                        var l = (LIST)v;
                        if (i < 0 || i >= l.Count)  util._error( cmtname + "["+index+"] is out of range");
                        ret = l[i];
                        return true;
                    }
                    else
                    {
                        var l = (Array)v;
                        if (i<0 || i >= l.Length)   util._error( cmtname + "["+index+"] is out of range");
                        ret = l.GetValue(i);
                        return true;
                    }
                }
                if (t==typeof(Hashtable))
                {
                    var ht = (Hashtable)v;
                    if (ht.ContainsKey(index))
                    {
                        ret = ht[index];
                    }
                    ret = null;
                    return true;
                }
                if (t==typeof(string))
                {
                    var i = (int)util.ToNumber(index);
                    var l = (string)v;
                    if (i<0 || i >= l.Length)   util._error( cmtname + "["+index+"] is out of range");
                    ret = l[i];                        
                    return true;
                }
            }
            return false;          
        }
        #endregion





        // Error
        internal static void _error(string cmt)
        {
            var s = cmt + "\n" + YDEF_DEBUG.RuntimeErrorInfo();
            throw new SystemException(s);
        }
        //Assert
        internal static void _assert(bool condition)
        {
#if UNITY_5
            UnityEngine.Assertions.Assert.IsTrue(condition);
#else
            System.Diagnostics.Debug.Assert(condition);
#endif
        }
    }
}
