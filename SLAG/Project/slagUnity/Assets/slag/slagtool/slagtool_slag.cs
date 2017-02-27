//using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System;

using number = System.Double;
using slagtool.runtime;
using slagtool.runtime.builtin;

namespace slagtool
{
    /// <summary>
    /// 
    /// slagクラス
    /// 
    /// [使い方]
    /// 
    /// 1. 準備 : ログ関数・ユーザ組込関数・ユーザ演算オペレーション関数登録・デバッグレベル設定
    /// 
    /// util.SetLogFunc((s)=>Debug.Log(s));  
    /// util.SetBuitIn(ユーザ組込関数のクラス,説明);
    /// util.SetCalcOp(ユーザ演算オペレーション関数);
    /// util.SetDebugLevel({0|1|2});    0 - なし   2 - 厳密
    /// 
    /// 1. 基本形：ロード→実行→関数実行
    /// 
    /// var slag = new slag();
    /// slag.Load(ファイル);   --- テキストファイル(.js), バイナリファイル(.bin), Base64ファイル(.base64)
    /// slag.Run();            --- 実行
    /// slag.CallFunc(関数名); --- 関数呼出し ※Run()前には実行不可。
    /// 
    /// 2. コンパイル→セーブ
    /// 
    /// var slag = new slag();
    /// slag.Load("hoge.js");
    /// slag.SaveBase64("hoge.base64");  または slag.SaveBun("hoge.bin");
    /// 
    /// 3. 複数テキストファイルロード
    /// 
    /// var slag = new slag();
    /// slag.LoadJSFiles(new string[]{"hoge1.js","hoge2.js"});
    /// 
    /// </summary>
    public class slag
    {
        public static slag m_curslag;

        public object m_owner; //本クラスを所有するオブジェクト

        public string[] m_idlist;

        private List<YVALUE> m_exelist;
        private StateBuffer m_statebuf;

        public slag(object owner)
        {
            m_curslag = this;
            m_owner   = owner;
        }

        #region ロード&セーブ
        /// <summary>
        /// ファイルロード
        /// xx.js or xx.txt     : ソースとしてロード
        /// xx.bin or xx.base64 : バイナリとしてロード
        /// 
        /// id : デバッグ時の認識に利用。null時はファイル名となる
        /// </summary>
        public void LoadFile(string filename, string id = null)
        {
            m_curslag = this;
            var ext = Path.GetExtension(filename);
            switch (ext.ToUpper())
            {
                case ".JS":
                    {
                        LoadJSFiles(new string[1] { filename });
                    }
                    break;
                case ".BASE64":
                    {
                        var b64txt = File.ReadAllText(filename);
                        LoadBase64(b64txt);
                    }
                    break;
                case ".BIN":
                    {
                        var bin = File.ReadAllBytes(filename);
                        LoadBin(bin);
                    }
                    break;
                default:
                    throw new SystemException("unexpected");
            }
        }
        /// <summary>
        /// 拡張子JSのファイル（複数）をロード
        /// </summary>
        /// <param name="filenames"></param>
        public void LoadJSFiles(string[] filenames)
        {
            m_curslag = this;
            //m_id = Path.GetFileNameWithoutExtension(filenames[0]);
            //m_filename = filenames[0];
            var ids = new List<string>();
            var sources = new List<string>();
            Array.ForEach(filenames, f =>
            {
                sources.Add(File.ReadAllText(f));
                ids.Add(Path.GetFileNameWithoutExtension(f));
            });

            m_idlist = ids.ToArray();
            m_exelist = util_sub.Compile(sources);
        }
        /// <summary>
        /// テキストソースロード
        /// id はデバッグ時の認識に利用
        /// </summary>
        public void LoadSrc(string src, string id = null)
        {
            m_curslag = this;
            m_idlist = id != null ? new string[] { id } : null;
            m_exelist = util_sub.Compile(src);
        }
        /// <summary>
        /// バイナリロード
        /// </summary>
        public void LoadBin(byte[] bin)
        {
            m_curslag = this;
            var d = deserialize(bin);
            m_idlist = d.ids;
            m_exelist = d.list;
        }
        /// <summary>
        /// Base64ロード
        /// </summary>
        public void LoadBase64(string base64str)
        {
            m_curslag = this;
            var bin = Convert.FromBase64String(base64str);
            LoadBin(bin);
        }
        public void SaveBin(string filename)
        {
            m_curslag = this;
            var bytes = GetBin();
            File.WriteAllBytes(filename, bytes);
        }
        public void SaveBase64(string base64File)
        {
            m_curslag = this;
            var src = GetBase64();
            File.WriteAllText(base64File, src);
        }
        public byte[] GetBin()
        {
            m_curslag = this;
            return serialize(m_exelist, m_idlist);
        }
        public string GetBase64()
        {
            m_curslag = this;
            var bytes = GetBin();
            return Convert.ToBase64String(bytes);
        }

        [System.Serializable]
        public class SaveFormat
        {
            public string[] ids;
            public List<YVALUE> list;
        }

        private SaveFormat deserialize(byte[] bin)
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream(bin))
            {
                return (SaveFormat)bf.Deserialize(ms);
            }
        }
        private byte[] serialize(List<YVALUE> list, string[] ids)
        {
            var d = new SaveFormat();
            
            d.ids = ids;
            d.list = list;
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, d);
                return ms.ToArray();
            }
        }
        #endregion

        public void Run()
        {
            if (m_exelist==null || m_exelist.Count==0 )
            {
                return;
            }

            m_curslag = this;
            m_statebuf = new StateBuffer(this);
            runtime.builtin.builtin_func.Init();

#if SLAGRELEASE
            run_script.run(m_exelist[0], m_statebuf);
#else
    #if UNITY_5
            var save = UnityEngine.Time.realtimeSinceStartup;
            run_script.run(m_exelist[0], m_statebuf);
            UnityEngine.Debug.Log("Elapsed time :"+(UnityEngine.Time.realtimeSinceStartup - save));
    #else
            throw new NotImplementedException();
    #endif
#endif
        }

#region 関数関連
        public bool ExistFunc(string funcname)
        {
            m_curslag = this;
            return builtin_func.IsFunc(funcname);
        }
        public YVALUE FindFunc(string funcname)
        {
            var fv = (YVALUE)m_statebuf.get_func(funcname);
            return fv;
        }
        public object CallFunc(string funcname, object[] param = null)
        {
            m_curslag = this;
            var fv = (YVALUE)m_statebuf.get_func(funcname);
            if (fv == null)
            {
                if (builtin_func.IsFunc(funcname))
                {
                    return builtin_func.Run(funcname, param, m_statebuf);
                }
                throw new SystemException("CallFunc : ファンクションがありません : " + funcname);
            }
            return CallFunc(fv, param);

        }
        public object CallFunc(YVALUE func, object[] param = null)
        {
            if (func!=null)
            { 
                m_curslag = this;
                List<object> ol = param!=null ? new List<object>(param) : null;
                m_statebuf = runtime.util.CallFunction(func,ol,m_statebuf);
                return m_statebuf.m_cur;
            }

            throw new SystemException("CallFunc : ファンクションがありません : " + func.GetFunctionName());
        }
        private int _countParamsInBracket(YVALUE v)
        {
            if (v==null || v.list==null || v.list.Count<2 || v.type!=YDEF.get_type(YDEF.sx_expr_bracket)) return 0;
            //            c  n
            // "()"     : 2->0         n = ((c-2)+1) / 2       c=2 n=0.5...=0      
            // "(x)"    : 3->1         
            // "(x,y)"  : 5->2
            // "(x,y,z)": 7->3 

            int num = ((v.list.Count - 2) + 1) / 2;
                               
            return num;
        }
#endregion

#region 変数関連
        public bool ExistVal(string name)
        {
            name = name.ToUpper();
            if (m_statebuf != null && m_statebuf.m_root_dic != null)
            {
                return m_statebuf.m_root_dic.ContainsKey(name);
            }
            return false;
        }
        public object GetVal(string name)
        {
            var ret = _getval(name);
            if (ret == null) throw new SystemException("GetVal : 変数が見つかりません : " + name);
            return ret;
        }
        private object _getval(string name)
        {
            name = name.ToUpper();
            if (m_statebuf != null && m_statebuf.m_root_dic != null)
            {
                var dic = m_statebuf.m_root_dic;
                if (dic.ContainsKey(name))
                {
                    return dic[name];
                }
            }
            return null;
        }
        public number GetNumVal(string name)
        {
            var ret = _getval(name);
            if (ret == null) throw new SystemException("GetNumVal : 変数が見つかりません : " + name);
            if (ret.GetType() != typeof(number)) throw new SystemException("GetNumVal : 変数が Number ではありません : " + name);

            return (number)ret;

        }
        public string GetStrVal(string name)
        {
            var ret = _getval(name);
            if (ret == null) throw new SystemException("GetStrVal : 変数が見つかりません : " + name);
            if (ret.GetType() != typeof(string)) throw new SystemException("GetStrVal : 変数が String ではありません : " + name);

            return (string)ret;
        }
        public void SetVal(string name, object val, bool bCreateIfNotExist = true)
        {
            if (!_setval(name, val, bCreateIfNotExist))
            {
                throw new SystemException("SetVal : Setに失敗しました ; " + name);
            }
        }
        private bool _setval(string name, object val, bool bCreateIfNotExist)
        {
            name = name.ToUpper();
            if (ExistVal(name))
            {
                m_statebuf.m_root_dic[name] = val;
                return true;
            }
            if (bCreateIfNotExist)
            {
                m_statebuf.m_root_dic[name] = val;
                return true;
            }
            return false;
        }
        public void SetNumVal(string name, number val, bool bCreateIfNotExist = true)
        {
            if (!_setval(name, val, bCreateIfNotExist))
            {
                throw new SystemException("SetNumVal : Setに失敗しました ; " + name);
            }
        }
        public void SetStrVal(string name, string val, bool bCreateIfNotExist = true)
        {
            if (!_setval(name, val, bCreateIfNotExist))
            {
                throw new SystemException("SetStrVal : Setに失敗しました ; " + name);
            }
        }
#endregion

#region デバッグ
        public string GetFileName(int id, bool base1=false)
        {
            id = base1 ? id-- : id;

            if (id>=0 && m_idlist!=null && id < m_idlist.Length)
            {
                return m_idlist[id];
            }
            return null;
        }
#endregion
    }
}