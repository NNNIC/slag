using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace slagtool
{
    // 構文解析
    public class yanalyze
    {
        //  概要
        //
        //  字句解析からの結果を入力とし構文解析を行う。最終結果は『１つの要素』となる。
        //  構文解析は構文の優先順位順に対象データの走査を繰り返す。
        //  走査は括弧（丸括弧、波カッコ）内と区分けした要素を優先的に行う。
        //  
        const int LOOPMAX = 10000;

        public static bool Analyze(List<YVALUE> src, out List<YVALUE> dst)
        {
            dst = new List<YVALUE>(src);

            var vp = new ValProvider();
            vp.Init(dst);

            for(int loop = 0; loop<=LOOPMAX; loop++)
            {
                if (loop == LOOPMAX) sys.error("Analyze LoopMax:1"); 
                if (vp.Update())
                {
                    dst = vp.GetResult();
                    break;
                }
            }
            vp = null;
            return true; 
        }

        #region 優先要素抽出
        public class ValProvider
        {
            public enum MODE
            {
                FIND_DEEPEST_BRACKETS,
                CHECK_INSIDE_BRACKETS,
                CHECK_INSIDE_BRACKETS_UPDATE,
                CHECK_WITH_BRACKETS,
                END
            }

            List<YVALUE>  m_target;
            List<YVALUE>  m_subtarget;
                          
            int           m_brackets_open_index;
            int           m_brackets_close_index;
                          
            MODE          m_mode = MODE.FIND_DEEPEST_BRACKETS;

            TokenProvider m_tp;

            public void Init(List<YVALUE> org)
            {
                m_target = org;
            }
            public List<YVALUE> GetResult()
            {
                return m_target;
            }
            public bool Update() // return true, if done.
            {
                switch(m_mode)
                {
                    case MODE.FIND_DEEPEST_BRACKETS:        FindDeepestBrackets();       return false; 
                    case MODE.CHECK_INSIDE_BRACKETS:        CheckInsideBrackets();       return false;
                    case MODE.CHECK_INSIDE_BRACKETS_UPDATE: CheckInsideBracketsUpdate(); return false;
                    case MODE.CHECK_WITH_BRACKETS:          CheckWithBracket();          return false;
                    default:                                End();                       return true;
                }
            }

            private void End()
            {
                //Console.WriteLine("END");
            }


            // 対象の括弧を検索
            private void FindDeepestBrackets()
            {
                var b = set_start_end(m_target,out m_brackets_open_index, out m_brackets_close_index);
                if (!b)
                {
                    m_mode = MODE.END;
                }
                else
                {
                    m_subtarget = extruct_list(m_target,m_brackets_open_index,m_brackets_close_index);
                    if (m_brackets_open_index + 1 == m_brackets_close_index)
                    {
                        m_mode = MODE.CHECK_WITH_BRACKETS;
                    }
                    else
                    {
                        m_mode = MODE.CHECK_INSIDE_BRACKETS;
                    }
                }
            }
            
            // 括弧内を分割子で分けられた要素ごとに変換
            private void CheckInsideBrackets()
            {
                m_tp = new TokenProvider();
                m_tp.Init(m_subtarget,1,m_subtarget.Count-2);
                m_mode = MODE.CHECK_INSIDE_BRACKETS_UPDATE;
            }
            private void CheckInsideBracketsUpdate()
            {
                var bDone = m_tp.Update();
                if (bDone)
                {
                    var result = m_tp.GetResult();
                    m_tp = null;

                    replace_list(ref m_subtarget,1,m_subtarget.Count-2,result);
                    m_mode = MODE.CHECK_WITH_BRACKETS;
                }
            }

            // 括弧を含めて変換
            private void CheckWithBracket()
            {
                _analyze(ref m_subtarget);
                replace_list(ref m_target,m_brackets_open_index,m_brackets_close_index,m_subtarget);
                m_mode = MODE.FIND_DEEPEST_BRACKETS;
            }

        }

        public class TokenProvider
        {
            List<string> m_separators;  //new string[] {".",",",";"};
            List<YVALUE> m_target;
            List<YVALUE> m_subtarget;
            int          m_index;
            int?         m_sample_start;
            int?         m_sample_end;

            public void Init(List<YVALUE> l, int ob, int cb)
            {
                m_separators = new List<string>(lexPrimitive.operators);
                m_separators.Add(";");
                m_target = extruct_list(l,ob,cb);
                m_index = 0;
            }

            public bool Update() // return true if done
            {
                m_subtarget = null;
                m_sample_start=null;
                m_sample_end  =null;

                int cnt = 0;
                for(int i = 0; i<m_target.Count; i++)
                {
                    var v = m_target[i];
                    var bSep = m_separators.Contains(v.s);
                    if (cnt == m_index)
                    {
                        if (m_subtarget==null) m_subtarget = new List<YVALUE>();
                        if (!bSep)
                        {
                            m_subtarget.Add(v);
                            if (m_sample_start==null) m_sample_start = i;
                            m_sample_end = i;
                        }
                    }
                    if (bSep) cnt++;
                }

                m_index++;

                if (m_subtarget!=null)
                { 
                    if (m_subtarget.Count>0)
                    { 
                        _analyze(ref m_subtarget);
                        replace_list(ref m_target,(int)m_sample_start,(int)m_sample_end, m_subtarget);
                    }
                    return false; //continue;
                }
                else
                {
                    return true; // done
                }
            }

            public List<YVALUE> GetResult()
            {
                return m_target;
            }
        }
        #endregion 優先要素抽出

        #region  解析
        private static bool _analyze(ref List<YVALUE> dst)
        {
            if (slagtool.sys.DEBUGLEVEL>=2)
            { 
                sys.logline("==================");
                sys.logline("= Analyze target =");
                YDEF_DEBUG.PrintLineAndCol(dst); sys.logline();
                YDEF_DEBUG.PrintListValue(dst); 
                sys.logline("==================");
            }
            var syntax_order = YDEF.get_syntax_order();

            bool bNeedLoop=false;

            for(var loop = 0; loop <= LOOPMAX; loop++ )
            { 
                if (loop == LOOPMAX) sys.error("Analyze LoopMax:2"); 

                bNeedLoop = false;
                for(int i = 0; i < syntax_order.Count; i++)
                {
                    var syntax = syntax_order[i];
                    var tslist = YDEF.get_syntax_set(syntax);

                    if (slagtool.sys.DEBUGLEVEL==3) Console.WriteLine(syntax[0].ToString());

                    foreach (var ts in tslist)
                    {
                        if (_check_syntax(dst,ts))
                        {
                            bNeedLoop = true;
                            break;
                        }
                    }
                    if (bNeedLoop) break; //最初から
                }

                if (bNeedLoop)
                { 
                    continue;
                }
                else
                {
                    if (slagtool.sys.DEBUGLEVEL>=2)
                    { 
                        YDEF_DEBUG.DumpLine_detail(dst,true);
                    }
                    return true;
                }
            }
            return false;
        }

        private static bool _check_syntax(List<YVALUE> list, YDEF.TreeSet ts)
        {
            for (var i = 0; i<list.Count; i++) // 走査
            {
                if (_isMatchAndMake(list,i,ts))
                {
                    if (slagtool.sys.DEBUGLEVEL==1)
                    { 
                        sys.logline("match ..." + ": list[" + i + "] " + ts.ToString());
                    }
                    else
                    {
                        sys.logline("\n match ..." + ": list[" + i + "] " + ts.ToString() +">\n" + YDEF_DEBUG.PrintValue(list[i]));
                    }
                    return true;
                }
            }
            return false;
        }
        private static bool _isMatchAndMake(List<YVALUE> list, int index, YDEF.TreeSet ts)
        {
            Func<int,YVALUE> get = (n) => {
                if (n >= list.Count) return null;
                return list[n];
            };
            List<YVALUE> args = new List<YVALUE>();
            int removelength = ts.list.Count;

            for (int i = 0; i<ts.list.Count; i++)
            {
                var v = get(index + i);
                if (v==null) return false;

                if (i==0 && ts.list.Count==1 && v.IsType(ts.type)) return false; //既に変換済み

                int tstype = 0;
                var o = ts.list[i];
                YVALUE nv = null;
                if (o.GetType() == typeof(string))
                {
                    if (v.GetString() == (string)o)
                    { 
                        nv = v.GetTerminalValue_ascent();
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    tstype = (int)o;
                    if (tstype == YDEF.REST) // ※RESTは特殊処理。EOLまでのすべて(除EOL)が入る
                    {
                        removelength--; //本VALUE分を事前に引く

                        var restv = new YVALUE();
                        restv.type = YDEF.REST;
                        restv.list = new List<YVALUE>();

                        for (int j = index + i; j < list.Count; j++)
                        {
                            var v2 = get(j);
                            if (v2 != null && !v2.IsType(YDEF.EOL))
                            {
                                restv.list.Add(v2);
                                removelength++;
                            }
                            else
                            {
                                break;
                            }
                        }

                        args.Add(restv);
                        break;
                    }
                    else
                    {
                        if (v.IsType(tstype))
                        { 
                            nv = v.FindValueByTravarse(tstype);
                        }
                        else
                        {
                            return false;
                        }
                    }
                }

                if (nv!=null)
                {
                    args.Add(nv);
                }
                else
                {                 
                    args.Add(v);
                }
            }

            //Yes, they match! then Make it.

            var makefunc = (Func<int, YVALUE[], int[], YVALUE>)ts.make_func;

            var newv = ts.make_func(ts.type,args.ToArray(),ts.make_index.ToArray());

            list.RemoveRange(index, removelength);
            list.Insert(index,newv);

            return true;
        }

        #endregion

        #region tool for this class
        private static bool set_start_end(List<YVALUE> l , out int start, out int end)  // valid return ture;
        {
            start = -1;
            end   = -1;
            start = find_deepest(l);
            end   = find_end(l,start);

            if (start < 0) start= 0;
            if (end < 0)   end = l.Count-1;

            return !(start == 0 && end ==0);
        }
        private class bracket_data
        {
            public int    index;
            public string value;
            public int    nestcount;
        }
        private static string openbrackets = "[({";
        private static string closebrackets= "])}";
        private static int find_deepest(List<YVALUE> l)
        {
            bracket_data deepest = null;
            List<bracket_data> stack = new List<bracket_data>();
            for(int i = 0; i<l.Count;i++)
            {
                var v = l[i];
                var s = v.s;
                if (s==null) continue;
                var idx_open = openbrackets.IndexOf(s);
                if (idx_open >= 0)
                {
                    var nest = stack.Count+1;
                    var d = new bracket_data() { index=i, value = s, nestcount=nest};
                    stack.Insert(0,d);
                    if (deepest==null || deepest.nestcount < nest)
                    {
                        deepest = d;
                    }
                    continue;
                }
                var idx_close = closebrackets.IndexOf(s);
                if (idx_close >=0)
                {
                    if (stack.Count==0||stack[0].value != openbrackets[idx_close].ToString())
                    {
                        throw new SystemException("Pair of Branckets is invalid at " );
                    }
                    stack.RemoveAt(0);
                    continue;
                }
            }
            if (stack.Count > 0)
            {
                throw new SystemException("Pair of Branckets is invalid. Check " );
            }

            if (deepest!=null)
            {
                return deepest.index;
            }
            return -1;
        }
        private static int find_end(List<YVALUE> l, int start)
        {
            if (start < 0) return -1;
            var open = l[start];
            var idx = openbrackets.IndexOf(open.s);
            var close = closebrackets[idx].ToString();
            var end = l.FindIndex(start,v=>v.s==close);
            return end;
        }
        private static void replace_list(ref List<YVALUE> target, int start, int end, List<YVALUE> val)
        {
            target.RemoveRange(start, end - start + 1);
            target.InsertRange(start, val);
        }
        private static List<YVALUE> extruct_list(List<YVALUE> src, int start, int end)
        {
            var result = new List<YVALUE>();
            for(int i = start; i <= end; i++)
            {
                result.Add(src[i]);
            }
            
            return result;
        }
        #endregion
    }
}
