using System;
using System.Collections.Generic;
using System.Text;
using number = System.Double;

/*
    要素ごとに解析を行う
    １．セパレータで分けた各要素に対して
    ２．ポインタ変数(ピリオドで接続された変数)要素に対して
    ３. 前置演算子に対して
    ４．後置演算子に対して
    ５．３項演算子に対して
    ６．SWITCHのCASE および DEFAULTに対して
    ７．セミコロンの句単位で
*/
namespace slagtool
{
    public partial class yanalyze
    {
        public enum TokenProvideMode
        {
            ALL_VALUES,
            POINTER_FUNCS_ARRAY,
            POINTER_VALUES,
            PREFIX_VALUES,
            POSTFIX_VALUES,
            TERNARY_VALUES,
            CASE_VALUES,
            CLAUSE_VALUES
        }

        public class TokenProvider
        {
            List<string>     m_separators_all;   //全分離要素
            List<string>     m_operators_plural; //2項及び３項演算子            

            TokenProvideMode m_mode;

            List<YVALUE>     m_target;
            List<YVALUE>     m_subtarget;
            int              m_index;
            int?             m_sample_start;
            int?             m_sample_end;

            public void Init()
            {
                m_separators_all= new List<string>(lexPrimitive.operators_all);
                m_separators_all.Add("NEW");
                m_separators_all.Add(";");
                
                m_operators_plural = new List<string>(lexPrimitive.operators_binary);
                m_operators_plural.AddRange(lexPrimitive.operators_ternay);
                m_operators_plural.Add("RETURN"); //returnの後ろを理解させるため。
            }

            public void Start(List<YVALUE> l, int ob, int cb, TokenProvideMode mode)
            {
                m_target      = extruct_list(l,ob,cb);
                m_subtarget   = null;
                m_index       = 0;
                m_sample_start= null;
                m_sample_end  = null;

                m_mode = mode;
            }

            public List<YVALUE> GetResult()
            {
                return m_target;
            }

            public bool Update()// return true if done
            {
                switch(m_mode)
                {
                    case TokenProvideMode.ALL_VALUES:          return _update_all_values();
                    case TokenProvideMode.POINTER_FUNCS_ARRAY: return _update_pointer_funcs_and_arrays();
                    case TokenProvideMode.POINTER_VALUES:      return _update_pointer_values();
                    case TokenProvideMode.PREFIX_VALUES:       return _update_prefix_values();
                    case TokenProvideMode.POSTFIX_VALUES:      return _update_postfix_values();
                    case TokenProvideMode.TERNARY_VALUES:      return _update_ternary_values();
                    case TokenProvideMode.CASE_VALUES:         return _update_case_values();
                    case TokenProvideMode.CLAUSE_VALUES:       return _update_clause_values();
                }
                return false;
            }

            bool _update_all_values()
            {
                /*
                   アップデート毎に１つ要素を指定して解析へ
                   m_index       : １回のアップデートで１つ進行
                   m_subtarget   : 解析対象=m_targetの要素のm_sample_start番目からm_sample_end番目まで
                */
                m_subtarget   =null;
                m_sample_start=null;
                m_sample_end  =null;

                int cnt = 0;
                for(int i = 0; i<m_target.Count; i++)        
                {
                    var v = getval(i);                         
                    var bSep = m_separators_all.Contains(v.s);
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

            bool _update_pointer_funcs_and_arrays() 
            {
                m_subtarget    = null;
                m_sample_start = null;
                m_sample_end   = null;

                for(int i = 0; i<m_target.Count; i++)
                {
                    var v0 = getval(i);
                    var v1 = getval(i+1);
                    var v2 = getval(i+2);

                    if (m_sample_start==null)
                    {
                        if (v0!=null && v1!=null && v2!=null && v0.s==".")
                        {
                            if (v1.IsType(YDEF.sx_expr) && v2.IsType(YDEF.sx_expr_bracket))
                            {
                                m_sample_start = i+1;
                                m_sample_end   = i+2;
                                break;
                            }
                            if (v1.IsType(YDEF.sx_expr) && v2.IsType(YDEF.sx_array_index))
                            {
                                m_sample_start = i+1;
                                m_sample_end   = i+2;
                                break;
                            }
                        }
                    }
                }
                if (m_sample_start!=null)
                {
                    m_subtarget = new List<YVALUE>();
                    for(int j = (int)m_sample_start; j<= (int)m_sample_end; j++) m_subtarget.Add(m_target[j]);
                    _analyze(ref m_subtarget);
                    replace_list(ref m_target,(int)m_sample_start,(int)m_sample_end, m_subtarget);
                    return false;
                }
                return true;
            }
            bool _update_pointer_values()
            {
                m_subtarget    = null;
                m_sample_start = null;
                m_sample_end   = null;

                for(int i = 0; i<m_target.Count; i++)
                {
                    var v0 = getval(i);
                    var v1 = getval(i+1);
                    var v2 = getval(i+2);
                    
                    if (m_sample_start==null)
                    { 
                        if (v0!=null && v1!=null && v2!=null)
                        {
                            if (v0.IsType(YDEF.sx_expr) &&  v1.s == "." && v2.IsType(YDEF.sx_expr))
                            {
                                m_sample_start = i;
                                m_sample_end   = i+2;
                                continue;
                            }
                        }
                    }
                    else
                    {
                        if (v0.s=="." && v1.IsType(YDEF.sx_expr))
                        {
                            m_sample_end = i+1;
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                if (m_sample_start!=null)
                {
                    m_subtarget = new List<YVALUE>();
                    for(int j = (int)m_sample_start; j<= (int)m_sample_end; j++) m_subtarget.Add(m_target[j]);
                    _analyze(ref m_subtarget);
                    replace_list(ref m_target,(int)m_sample_start,(int)m_sample_end, m_subtarget);
                    return false;
                }

                return true;
            }
            bool _update_prefix_values()
            {
                m_subtarget = null;
                m_sample_start=null;
                m_sample_end  =null;

                for(int i = m_index; i<m_target.Count; i++)
                {
                    var v = getval(i);
                    var bPreOp =  Array.IndexOf(lexPrimitive.operators_prefix,v.s)>=0; //前置演算子？
                    if (bPreOp)
                    { 
                        if (i==0) //先頭でかつexprが後続
                        {
                            if (isExpr(i+1))
                            {
                                m_sample_start= i;                                    
                                m_sample_end  = i+1;
                            }
                            else
                            {
                                throw new SystemException("This operator follows something.");
                            }
                        }
                        else //直前が他のオペレータでかつexprが後続
                        {
                            if (isOtherOp(i-1))
                            {
                                if (isExpr(i+1))
                                {
                                    m_sample_start = i;
                                    m_sample_end   = i+1;
                                }
                                else
                                {
                                    throw new SystemException("This operator follows something.");
                                }
                            }
                        }
                    }

                    m_index++;

                    if (m_sample_start!=null)
                    {
                        m_subtarget = new List<YVALUE>();
                        for(int j = (int)m_sample_start; j<= (int)m_sample_end; j++) m_subtarget.Add(m_target[j]);
                        _analyze(ref m_subtarget);
                        replace_list(ref m_target,(int)m_sample_start,(int)m_sample_end, m_subtarget);
                        return false;
                    }
                }
                return true;
            }
            bool _update_postfix_values()
            {
                m_subtarget = null;
                m_sample_start=null;
                m_sample_end  =null;

                for(int i = m_index; i<m_target.Count; i++)
                {
                    var v = getval(i);
                    var bPreOp = Array.IndexOf(lexPrimitive.operators_postfix,v.s) >= 0; // 後置演算子？
                    if (bPreOp)
                    { 
                        if (isExpr(i-1))
                        {
                            m_sample_start = i-1;
                            m_sample_end   = i;
                        }
                        else
                        {
                            throw new SystemException("This operator follows something.");
                        }
                    }

                    if (m_sample_start!=null)
                    {
                        m_subtarget = new List<YVALUE>();
                        for(int j = (int)m_sample_start; j<= (int)m_sample_end; j++) m_subtarget.Add(m_target[j]);
                        _analyze(ref m_subtarget);
                        replace_list(ref m_target,(int)m_sample_start,(int)m_sample_end, m_subtarget);
                        return false;
                    }

                    m_index++;
                }
                return true;
            }
            bool _update_ternary_values()
            {
                m_subtarget   = null;
                m_sample_start= null;
                m_sample_end  = null;

                for(int i = 0; i<m_target.Count; i++)
                {
                    if (
                        isExpr(i)
                        &&
                        isStringOp(i+1,"?")
                        &&
                        isExpr(i+2)
                        &&
                        isStringOp(i+3,":")
                        &&
                        isExpr(i+4)
                        )
                    {
                        m_sample_start = i;
                        m_sample_end   = i + 4;
                        
                        break;   
                    }
                }

                if (m_sample_start!=null)
                {
                    m_subtarget = new List<YVALUE>();
                    for(int j = (int)m_sample_start; j<= (int)m_sample_end; j++) m_subtarget.Add(m_target[j]);
                    _analyze(ref m_subtarget);
                    replace_list(ref m_target,(int)m_sample_start,(int)m_sample_end, m_subtarget);
                    return false;
                }

                return true;
            }
            bool _update_case_values()
            {
                m_subtarget   = null;
                m_sample_start= null;
                m_sample_end  = null;

                for(int i = 0; i<m_target.Count; i++)
                {
                    if (
                        isStringOp(i,"CASE")
                        &&
                        isExpr(i+1)
                        &&
                        isStringOp(i+2,":")
                        )
                    {
                        m_sample_start = i;
                        m_sample_end   = i + 2;
                        
                        break;   
                    }
                    if (
                        isStringOp(i,"DEFAULT")
                        &&
                        isStringOp(i+2,":")
                        )
                    {
                        m_sample_start = i;
                        m_sample_end   = i + 1;
                        
                        break;   
                    }

                }

                if (m_sample_start!=null)
                {
                    m_subtarget = new List<YVALUE>();
                    for(int j = (int)m_sample_start; j<= (int)m_sample_end; j++) m_subtarget.Add(m_target[j]);
                    _analyze(ref m_subtarget);
                    replace_list(ref m_target,(int)m_sample_start,(int)m_sample_end, m_subtarget);
                    return false;
                }

                return true;
            }
            bool _update_clause_values()
            {
                /*
                    セミコロン（含）を区切りとした要素を取り出して解析へ
                    一要素の解析が終わるとfalseで戻る。
                    終端記号のセミコロンが無くなればtrueで処理終了(true)

                    ※clauseとなったものは対象外
                */
                m_subtarget    = null;
                m_sample_start = null;
                m_sample_end   = null;

                if (m_target.TrueForAll(v=>v.s!=";")) //終末記号の';'が全くない場合、処理なし
                {
                    return true;
                }

                for(int i = m_target.Count-1; i>=0; i--)
                {
                    var v = getval(i);
                    if (m_sample_end==null)
                    {
                        if (v.s == ";")
                        {
                            m_sample_end  = i;
                            m_sample_start = i;
                        }
                        continue;
                    }
                    if (v.IsType(YDEF.sx_sentence) || v.IsType(YDEF.sx_sentence_list) ||v.s==";")
                    {
                        break;
                    }
                    m_sample_start = i;
                }

                if (m_sample_start!=null)
                {
                    m_subtarget = new List<YVALUE>();
                    for(int j = (int)m_sample_start; j<= (int)m_sample_end; j++) m_subtarget.Add(m_target[j]);
                    _analyze(ref m_subtarget);
                    replace_list(ref m_target,(int)m_sample_start,(int)m_sample_end, m_subtarget);
                    return false;
                }

                return true;
            }
            // --- tool for this class
            private YVALUE getval(int i)
            {
                if (i<0 || i>=m_target.Count) return null;
                return m_target[i];
            }
            private bool isStringOp(int i, string s)
            {
                var v = getval(i);
                return (v!=null && !isExpr(i) && v.s == s);
            }
            private bool isExpr(int i)
            {
                if (i<0 || i>=m_target.Count) return false;
                return m_target[i].IsType(YDEF.sx_expr);
            }
            private bool isOtherOp(int i)
            {
                if (i<0 || i>=m_target.Count) return false;
                return m_operators_plural.Contains(m_target[i].s);
            }
        }
    }
}