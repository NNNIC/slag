using System;
using System.Collections.Generic;
using System.Text;
using number = System.Double;

namespace slagtool
{
    public class YCODE
    {
        //※ 汎用 DO_NEW DO_ADD DO_ADDHEAD DO_COMBINE
        // 
        // DO_NEW     : 新規作成
        // DO_ADD     : 既存リストの最後に追加
        // DO_ADDHEAD : 既存リストの先頭に挿入
        // DO_COMBINE : 既存リストの最後にリスト追加
        
        public static Func<int,YVALUE[],int[],YVALUE> DO_NEW = (type, args,idx) => {
            var v = new YVALUE();
            v.type = type;
            v.list = new List<YVALUE>();
            foreach(var i in idx)
            {
                if (i>=0&&i<args.Length) v.list.Add(args[i]);
            }
            return v;
        };

        public static Func<int, YVALUE[],int[], YVALUE> DO_ADD = (type, args,idx) => {
            var v = args[idx[0]];
            if (v.list==null) v.list = new List<YVALUE>();
            for(int i=1; i<idx.Length; i++)
            {
                var n = idx[i];
                if (n>=0 && n<args.Length) v.list.Add(args[idx[i]]);
            }            
            return v;
        };
        public static Func<int, YVALUE[],int[], YVALUE> DO_ADDHEAD = (type, args,idx) => {
            var v = args[idx[0]];
            if (v.list==null) v.list = new List<YVALUE>();
            for(int i=idx.Length-1; i>=1; i--)
            {
                var n = idx[i];
                if (n>=0 && n<args.Length) v.list.Insert(0,args[idx[i]]);
            }            
            return v;
        };

        public static Func<int, YVALUE[],int[],YVALUE> DO_COMBINE = (type,args,idx) =>
        {
            var v = args[idx[0]];
            if (v.list==null) v.list = new List<YVALUE>();
            for(int i=1; i<idx.Length; i++)
            {
                var n = idx[i];
                if (n>=0 && n<args.Length)
                {
                    var nv = args[idx[i]];
                    if (nv!=null)
                    {
                        if (nv.list!=null)
                        {
                            nv.list.ForEach(j=>v.list.Add(j));
                        }
                        else
                        {
                            v.list.Add(nv);
                        }
                    }
                }
            }      
            v.type = type;      
            return v;
        };

        // tools for this class

        static number _getnum(object[] o,int n)
        {
            if (n>=0 &&  n < o.Length)
            {
                var v = (YVALUE)o[n];
                return v.n;
            }
            return 0;
        }
        static string _getstr(object[] o, int n)
        {
            if (n>=0 && n < o.Length )
            {
                var v = (YVALUE)o[n];
                return v.s;
            }
            return null;
        }
    }
}
