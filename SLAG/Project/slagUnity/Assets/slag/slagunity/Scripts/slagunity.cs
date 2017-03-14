using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using System;
using slagremote;

public class slagunity {

    public static string version // = "0.6.20170228";  // X.X.年月日
    {
        get { return slagunity_version.version;  }
    }

    public slagtool.slag  m_slag   {get; private set; }
    public slagunity_root m_root   {get; private set; }
    public static string           m_script;
    
    public static netcomm m_netcomm;

    private slagunity()
    { }

    #region Initialization
    private static List<Type>   m_customBuitInList;
    public  static void AddBuiltIn(Type t) 
    {
        if (m_customBuitInList==null) m_customBuitInList = new List<Type>();
        m_customBuitInList.Add(t);
    }
    bool m_bInitialized = false;
    public void Init(GameObject go,bool bCompileOnly=false)
    {
        if (m_bInitialized) return;

        m_bInitialized = true;

#if SRAGRELEASE
        slagtool.util.SetDebugLevel(0);
#else
        slagtool.util.SetDebugLevel(1);
#endif

        //表示の切り替え
        slagtool.runtime.builtin.builtin_sysfunc.m_printFunc   = (s)=> { Debug.Log(s); guiDisplay.Write(s);     };
        slagtool.runtime.builtin.builtin_sysfunc.m_printLnFunc = (s)=> { Debug.Log(s); guiDisplay.WriteLine(s); };

        //デバッグログ
        slagtool.util.SetLogFunc(Debug.Log,Debug.Log);

        slagtool.util.SetBuiltIn(typeof(slagunity_builtinfunc));
        slagtool.util.SetCalcOp(slagunity_builtincalc_op.Calc_op);

        if (m_customBuitInList!=null)
        {
            foreach(var t in m_customBuitInList)
            {
                slagtool.util.SetBuiltIn(t);
            }
        }

        m_slag = new slagtool.slag(this);

        if (!bCompileOnly)
        { 
            if (go!=null)
            { 
                m_root = go.GetComponent<slagunity_root>();
                if (m_root==null)
                { 
                   m_root = go.AddComponent<slagunity_root>();
                }
            }
        }
    }
    #endregion

    #region netcomm
    public void StartNetComm(RUNMODE mode,  Action cb=null)
    {
        slagremote_unity_manager.Create(this);
        slagremote_unity_manager.V.StartCom(mode, cb);
    }
    public void TerminateNetComm(Action cb=null)
    {
        slagremote_unity_manager.V.AbortCom(cb);
    }
    #endregion

    /// <summary>
    /// 指定パスよりロード
    /// ファイル：*.js | *.bin | *.base64 | *.inc
    /// </summary>
    public void LoadFile(string path)
    {
        if (Path.GetExtension(path).ToLower()==".inc")
        { 
            var filelist = convert_inc(path);
            m_slag.LoadJSFiles(filelist);
        }
        else
        { 
            m_slag.LoadFile(path);
        }
    }
    string[] convert_inc(string f)
    {
        List<string> filelist = new List<string>();

        string[] readlist = null;
        try { 
            readlist = File.ReadAllLines(f,Encoding.UTF8);
        } catch { return null; }

        if (readlist==null || readlist.Length==0) return null;

        foreach(var l in readlist)
        {
            var nl = l.Trim();
            if (string.IsNullOrEmpty(nl) || nl.StartsWith("//") ) continue;
            filelist.Add(Path.Combine(Path.GetDirectoryName(f),nl));
        }
        return filelist.ToArray();
    }
    /// <summary>
    /// 拡張子JSファイル（複数）をロード
    /// </summary>
    public void LoadJSFiles(string[] files)
    {
        m_slag.LoadJSFiles(files);
    }
    /// <summary>
    /// テキストソースロード
    /// </summary>
    public void LoadSrc(string src)
    {
        m_slag.LoadSrc(src);
    }
    #region スクリプト格納とコンパイル
    /// <summary>
    /// ファイルを読み込み、m_scriptに格納する
    /// </summary>
    public void ReadScript(string file)
    {
        m_script = File.ReadAllText(file,Encoding.UTF8);
    }
    /// <summary>
    /// m_scriptをコンパイル
    /// </summary>
    public void CompileScript()
    {
        m_slag.LoadSrc(m_script);
    }
    #endregion
    /// <summary>
    /// バイナリロード
    /// </summary>
    public void LoadBin(byte[] bin)
    {
        m_slag.LoadBin(bin);
    }
    /// <summary>
    /// Base64テキストロード
    /// </summary>
    public void LoadBase64(string base64str)
    {
        m_slag.LoadBase64(base64str);
    }
    /// <summary>
    /// バイナリファイルとしてセーブ
    /// </summary>
    public void SaveBin(string path)
    {
        m_slag.SaveBin(path);
    }
    /// <summary>
    /// Base64としてセーブ
    /// </summary>
    public void SaveBase64(string path)
    {
        m_slag.SaveBase64(path);
    }
    /// <summary>
    /// バイナリとして取得
    /// </summary>
    public byte[] GetBin()
    {
        return m_slag.GetBin();
    }
    /// <summary>
    /// Base64文字列として取得
    /// </summary>
    public string GetBase64()
    {
        return m_slag.GetBase64();
    }
    
    /// <summary>
    /// チェックサム(MD5)を取得
    /// </summary>
    public string GetMD5()
    {
        //ref http://dobon.net/vb/dotnet/string/md5.html
        var bytes = GetBin();
        var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        var bs = md5.ComputeHash(bytes);
        md5.Clear();

        var result = new System.Text.StringBuilder();
        foreach(var b in bs)
        {
            result.Append(b.ToString("x2"));
        }

        return result.ToString();
    }

    /// <summary>
    /// 実行
    /// </summary>
    public void Run()
    {
        m_slag.Run();
    }    

    /// <summary>
    /// 関数実行
    /// </summary>
    /// <param name="funcname">関数名</param>
    /// <param name="param">引数</param>
    public object CallFunc(string funcname, object[] param = null)
    {
        return m_slag.CallFunc(funcname,param);
    }

    /// <summary>
    /// 関数存在確認
    /// </summary>
    public bool ExistFunc(string funcname)
    {
        return m_slag.ExistFunc(funcname);
    }

    /// <summary>
    /// 生成
    /// </summary>
    public static slagunity Create(GameObject go,bool bCompileOnly=false)
    {
        var p = new slagunity();
        p.Init(go,bCompileOnly);
        return p;
    }
}
