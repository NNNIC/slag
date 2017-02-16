using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Diagnostics;
using System.IO;
using System.Text;

public class slageditortool : MonoBehaviour {

    [MenuItem("slag/monitior")]
    static void CallMonitor()
    {
        var path = Path.Combine(Application.dataPath,@"..\..\slagMonitor\m2\slagmon\slagmon\bin\Debug\slagmon.exe");
        UnityEngine.Debug.Log("path="+path);
        Process.Start(path);
    }

    [MenuItem("slag/test/コンパイル結果をリソースBINへ出力")]
    static void CompileTestFiles()
    {
        string   wd  = "N:/Project/test";
        string[] list = new string[] {
            "test01.js",
            "test02.js",
            "test03.js",
            "test04.js",
            "test05.js",

            "test06.js",
            "test07.js",
            "test08.js",
            "test09.js",
            "test10.js",

            "test11.js",
            "test12.js",
            "test13.js",
            "test14.js",

            "test51.js",
            "test52.js",
            "test53.js",
            "test54.js",
            "test55.js",

            "test55.js",

            "test91.js",
            "test92.inc",
            "test93.inc"
        };   

        string savefolder = Application.dataPath + "/slag/slagunity/Resources/bin";

        //slagtool.slag slag = new slagtool.slag(null);
        var slag = slagunity.Create(null,false);
        foreach(var f in list)
        {
            slag.LoadFile(Path.Combine(wd,f));
            slag.SaveBin(Path.Combine(savefolder,Path.GetFileNameWithoutExtension(f) + ".bytes"));

            UnityEngine.Debug.Log("Compiled .. "+ Path.GetFileNameWithoutExtension(f) + ",checksum:" + slag.GetMD5());
        }
    }

    #region 外部コンパイル実行


    [MenuItem("slag/test/バッチ実行テスト")]
    /// <summary>
    /// Unityバッチ起動によるコンパイル実行
    /// 実行前に次の環境変数を定義
    ///  "FILELIST"  ---- ソースファイル名リストを格納したテキストファイル。改行区切り。
    ///  "SRCDIR"    ---- ソースファイルの格納ディレクトリ (デフォルト：N:\Project\Test)
    ///  "DSTDIR"    ---- 出力ディレクトリ（デフォルト：N:\Project\Test\out）
    ///  "DSTEXT"    ---- 出力ファイルの拡張子
    ///  　　　　　　　　 "bin"または"bytes"を指定するとバイナリを出力
    ///  　　　　　　　　 "base64"または"txt"を指定するとBase64テキスト出力
    ///  　　　　　　　　 (デフォルト：bin)
    /// </summary>
    public static void COMPILE()
    {
        var filelist = System.Environment.GetEnvironmentVariable("FILELIST");

        var srcdir   = System.Environment.GetEnvironmentVariable("SRCDIR");
        if (string.IsNullOrEmpty(srcdir)) srcdir = "N:/Project/Test";

        var dstdir   = System.Environment.GetEnvironmentVariable("DSTDIR");
        if (string.IsNullOrEmpty(dstdir)) dstdir = "N:/Project/Test/out";

        var dstext   = System.Environment.GetEnvironmentVariable("DSTEXT");
        if (string.IsNullOrEmpty(dstext)) dstext = "bin";

        bool bCompileBinOrBase64 =false;
        dstext = dstext.ToLower();
        switch(dstext)
        {
            case "bin":
            case "bytes":   bCompileBinOrBase64 = true; break;
            case "base64":
            case "txt":     bCompileBinOrBase64 = false; break;
            default:
                throw new System.Exception("DSTEXTを確認せよ");
        }

        try
        {
            compile(bCompileBinOrBase64,filelist,srcdir,dstdir,dstext);
        }
        catch(System.Exception e)
        {
            UnityEngine.Debug.Log(e.Message);
        }
    }
    private static void compile(bool bCompileBinOrBase64, string filelist, string srcdir, string dstdir, string dstext)
    {
        var list = File.ReadAllLines(filelist,Encoding.UTF8);
        foreach(var l in list)
        {
            var s = l.Trim();
            if (string.IsNullOrEmpty(s) || s.StartsWith("//"))
            {
                continue;
            }

            var path = Path.Combine(srcdir,s);
            if (!File.Exists(path))
            { 
                UnityEngine.Debug.Log("ファイルが存在しない:" + path);
                continue;
            }

            var slag = slagunity.Create(null,false);

            slag.LoadFile(path);

            var dstpath = Path.Combine(dstdir, Path.GetFileNameWithoutExtension(path) + "." + dstext);

            if (bCompileBinOrBase64)
            {
                slag.SaveBin(dstpath);
            }
            else
            {
                slag.SaveBase64(dstpath);
            }

            UnityEngine.Debug.Log("Compiled .. "+ Path.GetFileNameWithoutExtension(path) + ",checksum:" + slag.GetMD5());
        }
    }
    #endregion
}
