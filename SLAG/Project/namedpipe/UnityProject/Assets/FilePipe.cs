using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

/// <summary>
/// FilePipe : ファイルを使ってパイプ通信
/// 
/// Tcp通信中はデバッガが使えなかったための措置　故に ip,portの記述あり
/// 
// 
/// 使い方
/// 
/// var pipe = new FilePipe("myname");
/// pipe.Start(s=>Console.WriteLine(s)); //開始。ログ出力にをコンソール指定
/// 
/// while(true)
/// {
///     var msg = pipe.Read();                 //読み込み
///     if (msg!=nll) 
///     {
///          DoSomething(msg);
///          pipe.Write("some","who_sendto"); //書き込み
///     }
///     pipe.Update();         
///     Sleep(33);
/// }
/// 
/// 
/// 制限
///   
///   テキストのみ
/// 
/// </summary>
public class FilePipe
{
    public static Action<string> Log = (s)=> { System.Diagnostics.Debug.WriteLine(s); };


    string m_name; //自名            
    int    m_port; //自ポート  ・・・TcpPipeとの互換性

    Thread m_thread;

    Queue<string> m_req_list;

    string m_serverfile { get{ return get_filename(m_name,m_port); } }

    bool m_force_exit;
    bool m_bEnd;

    public FilePipe(string myname, int myport=2000)
    {
        m_force_exit = false;
        m_bEnd       = false;

        m_name = myname;
        m_port = myport;
    }

    public void Start(Action<string> logfunc=null)
    {
        m_thread = new Thread(server);
        m_thread.IsBackground = true;
        m_thread.Priority = ThreadPriority.AboveNormal;
        m_thread.Start();

        m_req_list = new Queue<string>();
    }
    public void Terminate()
    {
        m_force_exit = true;
    }
    public bool IsEnd()
    {
        if (m_bEnd && m_thread!=null)
        {
            m_thread.Abort();
            m_thread.Join();
            m_thread = null;
            return false;
        }

        return  m_bEnd;
    }

    public string Read()
    {
        if (m_req_list==null) throw new SystemException("Call Start() before this function.");
        lock(m_req_list)
        { 
            if (m_req_list.Count>0)
            {
                var s = m_req_list.Dequeue();
                return s;            
            }
        }
        return null;
    }    
    public void Write(string msg,string ip, int port=2000)
    {
        send_client(msg,ip,port);
    }

    #region サーバー
    private void server()
    {
        Log("PIPEサーバ開始");

        var file = m_serverfile;
        
        while (File.Exists(file)) {
            if (m_force_exit) break;

            try { File.Delete(file); } catch { }
            Thread.Sleep(10);
        }

        if (!m_force_exit)
        { 
            using (var fs = File.Open(file, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                Log("PIPEサーバファイル:" + file);

                var data = new List<byte>();
                while(true)
                { 
                    if (m_force_exit)
                        break;

                    try { 
                        byte[] temp = null;
                        var templen = fs.Length - fs.Position;
                        if (templen <= 0)
                        {
                            Thread.Sleep(100);
                            continue;
                        }
                        temp = new byte[templen];
                        fs.Read(temp,0,(int)templen);
                        data.AddRange(temp);
                        if (data.Contains((byte)'\n'))
                        {
                            _accumelate(ref data);
                        }
                    } catch (SystemException e)
                    {
                        Log("PIPEサーバーファイルエラー:" + e.Message);
                        System.Diagnostics.Debug.WriteLine("FilePipe Exception:"+e.Message);
                        break;
                    }
                }
                fs.Close();
            }
        }
        Log("PIPEサーバ終了");

        m_bEnd = true;
    }
    void _accumelate(ref List<byte> data)
    {
        var sp = (byte)'\n';
        while(data.Contains(sp))
        {
            var idx = data.IndexOf(sp);
            var tmp = new List<byte>(); for(int i = 0; i<idx; i++) tmp.Add(data[i]); //idxまで取得
            var cmd = Encoding.UTF8.GetString(tmp.ToArray());
            m_req_list.Enqueue(cmd);
            data.RemoveRange(0,idx+1);//取得済み分を削除
        }
    }
    #endregion

    #region クライアント
    private void send_client(string msg, string to_name, int to_port)
    {
        if (string.IsNullOrEmpty(msg))
        {
            Log("メッセージが設定されていません");
            return;
        }

        if (msg[msg.Length-1]!='\n') msg+="\n";

        var file = get_filename(to_name,to_port);
        if (!File.Exists(file))
        {
            Log("接続先が存在しません:" + to_name +":" + to_port);
            return;
        }
        var data = Encoding.UTF8.GetBytes(msg);
        using (var fs = File.Open(file, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
        {
            fs.Seek(0,SeekOrigin.End);
            fs.Write(data,0,data.Length);
            fs.Close();
        }
    }
    #endregion

    // --- tool for this class ---
    private string get_filename(string name, int port)
    {
        return Path.Combine(Path.GetTempPath(),"~filepipe_"+name+"_"+port+".txt"); 
    }
}
