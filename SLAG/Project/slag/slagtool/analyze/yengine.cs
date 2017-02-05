using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace slagtool
{
    public class yengine
    {
        string dbg_src;                 // デバッグ用途
        List<List<YVALUE>> dbg_src_list; //     〃
        List<List<YVALUE>> dbg_out_list; //     〃

        public List<List<YVALUE>> Lex(string src)
        {
            dbg_src = src;
            dbg_out_list = lexUtil.lexSource(src);
            return dbg_out_list;
        }

        public void Normalize(ref List<List<YVALUE>> src_list)
        {
            const int LOOPMAX = 10000;

            dbg_src_list = src_list;

            //無意味な行の削除
            for (int loop = 0; loop <= LOOPMAX; loop++)
            {
                if (loop == LOOPMAX) sys.error("Normalize LOOPMAX");

                bool bNeedLoop = false;
                for (int n = 0; n < src_list.Count; n++)
                {
                    var l = dbg_out_list[n];
                    if (l.TrueForAll(i => i.IsType(YDEF.EOL) || i.IsType(YDEF.SP) || i.IsType(YDEF.CMT)))
                    {
                        dbg_out_list.RemoveAt(n);
                        bNeedLoop = true;
                        break;
                    }
                }
                if (!bNeedLoop) break;
            }

            //ダブルクォーテーションで囲まれた文字以外、すべて大文字へ
            //スペースの全削除
            foreach (var l in src_list)
            {
                for (int loop = 0; loop <= LOOPMAX; loop++)
                {
                    if (loop == LOOPMAX) sys.error("Normalize LOOPMAX:2");

                    bool bNeedLoop = false;
                    for (int n = 0; n < l.Count; n++)
                    {
                        var v = l[n];
                        if (!v.IsType(YDEF.QSTR))
                        {
                            if (v.s != null) v.s = v.s.ToUpper();
                        }

                        if (v.IsType(YDEF.SP))
                        {
                            l.RemoveAt(n);
                            bNeedLoop = true;
                            break;
                        }

                        if (v.IsType(YDEF.EOF))
                        {
                            l.RemoveAt(n);
                            bNeedLoop = true;
                            break;
                        }

                        if (v.IsType(YDEF.CMT))
                        {
                            l.RemoveAt(n);
                            bNeedLoop = true;
                            break;
                        }
                    }
                    if (!bNeedLoop) break;
                }
            }

            return;
        }

        public List<List<YVALUE>> Make_one_line(List<List<YVALUE>> src)
        {
            List<YVALUE> dst = new List<YVALUE>();

            dst.Add(YVALUE.BOF());
            foreach(var l in src)
            {
                foreach(var v in l)
                {
                    if (v.IsType(YDEF.EOL)) continue;
                    dst.Add(v);
                }
            }
            if (dst.Count>0 && dst[dst.Count-1].type != YDEF.EOF) dst.Add(YVALUE.EOF());

            var final = new List<List<YVALUE>>();
            final.Add(dst);
            return final;
        }

        public List<List<YVALUE>> Interpret(List<List<YVALUE>> src)
        {
            dbg_src_list = src;
            var output = new List<List<YVALUE>>();
            foreach (var l in src)
            {
                var oline = new List<YVALUE>();
                if (!yanalyze.Analyze(l, out oline)) return null;
                output.Add(oline);
            }

            dbg_out_list = output;

            return output;
        }

        public bool IsExecuable(List<List<YVALUE>> list, out int errorline)
        {
            errorline = -1;
            var roottype = YDEF.get_syntax_root();
            foreach(var l in list)
            {
                if (l.Count>0)
                {
                    var typ = l[0].type;
                    if (typ == roottype) continue; //最終形態でＯＫ

                    errorline = l[0].get_dbg_line();
                    return false;
                }
            }
            return true;
        }

        // -- util --
        private static string gn(object[] o)
        {
            return YDEF.get_name(o);
        }
        private static int getyp(object[] o)
        {
            return YDEF.get_type(o);
        }
        private static YVALUE find(YVALUE v, object[] o)
        {
            var t = getyp(o);
            var fv = v.FindValueByTravarse(t);
            return fv;
        }
        private static YVALUE find(YVALUE v, int typ)
        {
            var fv = v.FindValueByTravarse(typ);
            return fv;
        }
        private static bool replace(YVALUE src, int typ, YVALUE dst)
        {
            return src.ReplaceValueByTravarse(typ,dst);
        }
        //ライン
        private static YVALUE find_l(List<YVALUE> l, int typ)
        {

            for (int i = 0; i < l.Count; i++)
            {
                var v = l[i];
                if (v.type == typ)
                {
                    return v;
                }
            }
            for (int i = 0; i < l.Count; i++)
            {
                var v = l[i];
                var f = find(v, typ);
                if (f != null) return f;
            }
            return null;
        }
        private static bool replace_l(List<YVALUE> l, int typ, YVALUE dst)
        {
            for (int i = 0; i < l.Count; i++)
            {
                var v = l[i];
                if (v.type == typ)
                {
                    l[i] = dst;
                    return true;
                }
            }
            for (int i = 0; i<l.Count; i++)
            { 
                var v = l[i];
                var b = replace(v, typ,dst);
                if (b) return true;
            }
            return false;
        }
    }
}

