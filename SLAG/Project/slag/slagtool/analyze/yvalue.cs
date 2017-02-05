﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace slagtool
{
    [System.Serializable]
    public class YVALUE
    {
        public double n;
        public string s;
        public object o;

        public int type;
        public List<YVALUE> list;
        public bool IsType(int itype)
        {
            if (itype == type) return true;
            if (list != null && list.Count == 1)//保持要素が１つの場合は、そのタイプもチェック対象
            {
                return list[0].IsType(itype);
            }
            return false;
        }
        public bool IsType(string s)
        {
            int tp = YDEF.get_type(s);
            return IsType(tp);
        }
        public bool IsType(object[] o)
        {
            int tp= YDEF.get_type(o);
            return IsType(tp);
        }
        public string GetString()
        {
            if (o != null) return o.ToString();
            if (list != null && list.Count == 1)
            {
                return list[0].GetString();
            }
            return null;
        }
        public double GetNumber()
        {
            if (o!=null && o.GetType()==typeof(double))
            {
                return (double)o;
            }
            if (list !=null && list.Count==1)
            {
                return list[0].GetNumber();
            }
            return 0;
        }
        public bool GetBool()
        {
            if (o!=null && o.GetType()==typeof(bool))
            {
                return (bool)o;
            }
            if (list !=null && list.Count==1)
            {
                return list[0].GetBool();
            }
            return false;
        }
        public string GetTerminal()
        {
            if (s!=null) return s;
            if (o!=null && o.GetType()==typeof(double)) return o.ToString();
            return null;
        }
        public object GetTerminalObject()
        {
            if (o != null)
            {
                var t = o.GetType();
                if (t == typeof(string) || t==typeof(double))
                {
                    return o;
                }
            }
            return null;
        }
        public object GetTerminalObject_ascent()//遡って取得
        {
            var o = GetTerminalObject();
            if (o!=null) return o;
            if (list != null && list.Count == 1)
            {
                o = list[0].GetTerminalObject_ascent();
            }
            return o;
        }
        public YVALUE GetTerminalValue_ascent()
        {
            if (list!=null && list.Count==1)
            {
                return list[0].GetTerminalValue_ascent();
            }
            return this;
        }

        #region //デバッグ
        public int dbg_line;
        public int dbg_col;

        public override string ToString()
        {
            string s = null;

            s += type.ToString() + ":" + YDEF.get_name(type);

            return s + ":" + (o != null ? o.ToString() : "null");
        }
        public string get_type_name()
        {
            return YDEF.get_name(type);
        }
        public string get_ascent_types() //タイプを遡って纏めて文字列化。listの先頭のみが対象
        {
            string s = null;
            Action<YVALUE> printtype = null;
            printtype = (v) =>
            {
                if (v==null) return;
                if (s!=null) s+="-";
                s+= YDEF.get_name(v.type);
                if (v.list != null && v.list.Count > 0)
                {
                    printtype(v.list[0]);
                }
            };

            printtype(this);

            return s;
        }
        public string get_all_terminals() //全終端記号を出力
        {
            string s = null;
            Action<YVALUE> print_terminals = null;
            print_terminals = (v) => {
                var n = v.GetTerminal();
                if (n != null)
                {
                    if (s != null) s += ",";
                    s += n;
                }
                else
                {
                    if (v.list != null) foreach (var v2 in v.list)
                    {
                        print_terminals(v2);
                    }
                }
            };
            print_terminals(this);
            return s;
        }
        public int get_dbg_line(int depth = 0) //depth==0 最後まで確認
        {
            int line = -1;
            int cnt = 0;
            Travarse(v => {
                if (Enum.IsDefined(typeof(TOKEN),v.type))
                {
                    line = v.dbg_line;
                    return true;
                }
                if (depth>0 && cnt>=depth)
                { 
                   return true;
                }
                return false;
            },depth);
            return line;
        }
        public int get_debug_column(int depth = 0) //depth==0 最後まで確認
        {
            int line = -1;
            int cnt = 0;
            Travarse(v => {
                if (Enum.IsDefined(typeof(TOKEN),v.type))
                {
                    line = v.dbg_col;
                    return true;
                }
                if (depth>0 && cnt>=depth)
                { 
                   return true;
                }
                return false;
            },depth);
            return line;
        }
        #endregion

        #region //実行時用
        public YVALUE FindValueByTravarse(int itype) //指定タイプをトラバースして検索　(listを辿りながら)
        {
            if (itype == type) return this;
            if (list==null) return null;

            for(int i = 0; i<list.Count;i++)//１．中のタイプのみを確認
            {
                if (list[i].type == itype) return list[i];
            }
            for(int i = 0; i<list.Count;i++)//２．一つずつ中を検索
            {
                var v = list[i].FindValueByTravarse(itype);
                if (v!=null) return v;
            }

            return null;
        }
        public YVALUE FindValueByTravarse(object[] o) //指定タイプをトラバースして検索　(listを辿りながら)
        {
            var type = YDEF.get_type(o);
            return FindValueByTravarse(type);
        }
        public bool ReplaceValueByTravarse(int itype, YVALUE dst)//トラバースして、最初に見つけたのを入れ替える。
        {
            if (itype==type)//自身の入れ替えはＮＧ
            {
                sys.logline("Unexpected. Cannot replace self");
                return false;
            }
            if (list == null) return false;

            for (int i = 0; i<list.Count; i++)
            {
                if (list[i].type == itype)
                {
                    list[i] = dst;
                    return true;
                }
            }
            for(int i = 0; i<list.Count;i++)
            {
                if (list[i].ReplaceValueByTravarse(itype,dst))
                {
                    return true;
                }
            }
            return false;
        }

        public YVALUE get_child(int index)
        {
            if (index >= list.Count) { sys.logline("get_child index exceeded"); return null; }
            var v = list[index];
            return v;
        }

        public void Travarse(Func<YVALUE,bool> func,int depth=0)//汎用トラバース  funcの戻り値がtrue時は、以降の確認をしない。
        {
            int cnt = 0;

            bool bDone = false;
            Action<YVALUE> work = null;
            work = (v) => {
                if (!bDone)
                {
                    bDone = func(v);
                    if (bDone) return;
                }
                if (v.list != null) {
                    for (int i = 0; i < v.list.Count; i++)
                    {
                        bDone = func(v.list[i]);
                        if (bDone) return;
                    }

                    cnt++;
                    if (depth==0 || cnt < depth)
                    { 
                        for (int i = 0; i < v.list.Count; i++)
                        {
                            work(v.list[i]);
                            if (bDone) return;
                        }
                    }
                }
            };
            work(this);
        }
        public YVALUE list_at(int n)
        {
            if (n<0 || n>=list_size()) return null;
            return list[n];
        }
        public int list_size()
        {
            if (list == null) return 0;
            return list.Count;
        }
        #endregion
        #region static 
        public static YVALUE BOF()
        {
            var v = new YVALUE();
            v.type = YDEF.BOF;
            v.s = "BOF";
            v.o = v.s;
            return v;
        }
        public static YVALUE EOF()
        {
            var v = new YVALUE();
            v.type = YDEF.EOF;
            v.s = "EOF";
            v.o = v.s;
            return v;
        }
        #endregion
    }
}
