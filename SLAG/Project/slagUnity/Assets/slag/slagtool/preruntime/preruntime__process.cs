using System;
using System.Collections;
using System.Collections.Generic;
using slagtool;

/*
    省略された型名を完全名にする。

    1. Pointer Value
    　　　File.ReadAllText()
           |  
           |  １．補完
           |       System.IO.File.ReadAllText()
           |  ２．タイプ分離
           |       type = System.IO.File
           |       Api  = ReadAllText()
           |  ３．再構成
           |       list[0]= YDEF.TYPE v.s = typename v.o = Type格納
           |       list[1]= sx_func( ReadAllText() )
           v
          (Type:System.File.ReadAllText())

    2. Name
          int
           |
           |   1. タイプ取得を試みる
           |         int, string等々
           |   2. 再構成
           |　       v.type = YDEF.TYPE  v.s=type.name  v.o=Type格納
           v
        　(Type:System.Int32)

    3. NEW + FuncName
          new HashTable()
           |
           |  1. 補完
           |

*/

namespace slagtool.preruntime
{ 
    public class process  {

        private static List<string> m_prefix_list;

        internal static List<YVALUE> Convert(List<YVALUE> list)
        {
            m_prefix_list = new List<string>();

            var v = list[0];

            _convert(v);

            list[0] = v;            

            return list;
        }

        internal static YVALUE _convert(YVALUE v)
        {
            if (v.type == YDEF.get_type(YDEF.sx_expr_clause) && v.list_size()==2 && v.list_at(0).IsType(YDEF.QSTR))// "using ..."節か？
            {
                var s = v.FindValueByTravarse(YDEF.QSTR).GetString();
                sys.logline(s);

                if (!string.IsNullOrEmpty(s))
                {
                    s = s.Trim('"');
                    var tokens = s.Split(' ');
                    if (tokens.Length>=2 && tokens[0]=="using")
                    {
                        var a = tokens[1];
                        if (!m_prefix_list.Contains(a))
                        { 
                            m_prefix_list.Add(a);//格納
                        }
                    }
                }
                return v;
            }

            if (v.type == YDEF.get_type(YDEF.sx_pointervar_clause))//ポインタ値節か？
            {
                //中身確認
                //リスト先頭が１要素でNAMEであれば、そのNAMEを調査する
                var vt =v.list_at(0);
                vt = Checktype.ChangeIfType(vt, m_prefix_list);
                v.list[0] = vt;

#if obs
                //リスト最後尾がfunc要素であれば、引数を対象に
                var ve =   v.list_at(v.list.Count-1);
                if (ve!=null && ve.IsType(YDEF.sx_func))
                {
                    var vf = ve.FindValueByTravarse(YDEF.sx_func);
                    var v2 = vf.list_at(1);
                    if (v2!=null)
                    { 
                        v2 = _convert(v2);
                        vf.list[1] = v2;
                    }
                }
#endif
                //リスト中にfunc要素があれば、引数を対象に。
                for(int i = 0; i<v.list_size(); i++)
                {
                    var ve = v.list_at(i);
                    if (ve!=null && ve.IsType(YDEF.sx_func))
                    {
                        var vf = ve.FindValueByTravarse(YDEF.sx_func);
                        var v2 = vf.list_at(1);
                        if (v2!=null)
                        { 
                            v2 = _convert(v2);
                            vf.list[1] = v2;
                        }
                    }
                }

                return v;
            }
            if (v.type == YDEF.get_type(YDEF.sx_def_func_clause))//ファンクション宣言節か？
            {
                //ファンクション名を対象とせず、本体を対象へ
                var nv = v.list_at(2);
                if (nv!=null) _convert(nv);
                return v;
            }
            if (v.type == YDEF.get_type(YDEF.sx_def_var_clause))//変数宣言節か？
            {
                //  宣言名は対象とせず、アサイン値があれば対象へ
                //var pv = v.list_at(0);
                //if (pv!=null) _convert(pv);
                var nv = v.list_at(2);
                if (nv!=null) _convert(nv);
                return v;
            }
            if (v.type == YDEF.get_type(YDEF.sx_expr) && (v.list_size()==2 && v.list_at(0).type == YDEF.NEW))//NEW節か？
            {
                //タイプ名かを確認
                var nv = v.list_at(1);
                if (nv.IsType(YDEF.sx_func))
                {
                    var fv = nv.FindValueByTravarse(YDEF.sx_func);
                    var namev = fv.list_at(0);
                    namev = Checktype.ChangeIfType(namev, m_prefix_list);
                    fv.list[0] = namev;
                    fv.list[1] = _convert(fv.list[1]);
                }
                else if (nv.IsType(YDEF.sx_pointervar_clause))
                {
                    var pv = nv.FindValueByTravarse(YDEF.sx_pointervar_clause);
                    pv = _convert(pv);
                }
                else if (nv.IsType(YDEF.sx_array_var))
                {
                    var av = nv.FindValueByTravarse(YDEF.sx_array_var);
                    var namev = av.list_at(0);
                    namev = Checktype.ChangeIfType(namev, m_prefix_list);
                    av.list[0] = namev;
                    av.list[1] = _convert(av.list[1]);
                }
                return v;
            }
            //if (v.type == YDEF.get_type(YDEF.sx_func))  ---- ポインタ値に属さない関数は内部または組込関数のみなので、評価外。
            //{
            //
            //}
            if (v.type == YDEF.NAME)
            {
                //タイプ名かを確認

                v = Checktype.ChangeIfType(v, m_prefix_list);

                return v;
            }

            // vの内部構成が変わるため、インデックスとサイズを確認しながら走査する。
            int index = 0;
            while(v.list!=null)
            {
                if (v.list.Count <= index) break;
                var vn = v.list_at(index);
                if (vn==null) continue;

                vn = _convert(vn);
                v.list[index] = vn;

                index++;
            }
            return v;
        }
    }
}