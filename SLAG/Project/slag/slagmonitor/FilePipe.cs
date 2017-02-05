using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Diagnostics;

public class FilePipe
{
    string m_name; //自名            
    int    m_port; //自ポート  ・・・TcpPipeとの互換性

    Thread m_thread;
    log    m_log;

    Queue<string> m_req_list;

    string m_serverfile { get{ return get_filename(m_name,m_port); } }

    bool m_force_exit;

    public FilePipe(string myname, int myport=2000)
    {
        m_name = myname;
        m_port = myport;
    }

    public void Start(Action<string> logfunc=null)
    {
        m_thread = new Thread(server);
        m_thread.Start();

        m_log = new log(logfunc);

        m_req_list = new Queue<string>();
    }
    public void Update()
    {
        m_log.Update();
    }
    public void Tenminate()
    {
        m_force_exit = true;
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
        m_log.WriteLine("サーバ開始");

        var file = m_serverfile;
        
        while (File.Exists(file)) {
            if (m_force_exit) return;

            try { File.Delete(file); } catch { }
            Thread.Sleep(10);
        }

        using (var fs = File.Open(file, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite))
        {
            m_log.WriteLine("サーバファイル:" + file);

            var data = new List<byte>();
            while(true)
            { 
                if (m_force_exit) return;

                try { 
                    byte[] temp = null;
                    var templen = fs.Length - fs.Position;
                    if (templen <= 0)
                    {
                        Thread.Sleep(33);
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
                    m_log.WriteLine("サーバーファイルエラー:" + e.Message);
                }
           }
        }
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
        if (msg == null)
        {
            var errmsg = "メッセージが設定されていません";
            m_log.WriteLine(errmsg);
            Debug.WriteLine(errmsg);
            return;
        }

        if (msg[msg.Length-1]!='\n') msg+="\n";

        var file = get_filename(to_name,to_port);
        if (!File.Exists(file))
        {
            var errmsg = string.Format("接続先が存在しません:" + to_name +":" + to_port);
            m_log.WriteLine(errmsg);
            Debug.WriteLine(errmsg);
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

    // --- For logging class
    public class log
    {
        Action<string> m_logFunc;

        object m_mutex;
        string m_tmp; //途中
        string m_out; //出力用

        public log(Action<string> logfunc) { m_mutex = new object(); m_logFunc = logfunc;}
        public void Update()
        {
            lock(m_mutex)
            {
                if (m_out!=null)
                {
                    if (m_logFunc!=null) m_logFunc(m_out);
                    m_out = null;
                }
            }
        }
        public void Write(string s)
        {
            m_tmp +=s;
        }
        public void WriteLine(string s=null)
        {
            m_tmp += s + Environment.NewLine;
            lock(m_mutex)
            {
                m_out += m_tmp;
                m_tmp = null;
            }
        }
    }
}
