//using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Threading;
using System.Net.Sockets;
using System.Net;

/*
    TCP pipe通信
     テキストのみ

    使い方

    var pipe = new TcpPipe("自IP",自PORT);
    pipe.start();

    while(true)
    {
        var msg = pipe.Read(); //読み込み 
        if (msg==null)
        {
            Sleep(10);
            continue;
        }

        pipe.Write("相手IP",相手PORT,"I GOT MSG(" + msg + ").");//書き込み
    }
*/

public class TcpPipe {

    string m_ip;
    int    m_port;

    Thread m_thread;
    log    m_log;

    Queue<string> m_req_list;

    bool   m_force_exit;

    public TcpPipe(string my_ip, int my_port=2000)
    {
        m_ip = my_ip;
        m_port = my_port;

        m_force_exit = false;
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
        send_client(ip,port,msg);
    }

    #region サーバー
    private void server()
    {
        while(true)
        {
            if (m_force_exit) return;
            _server();
        }
    }

    private void _server()
    { 
        var ipString = m_ip; 
        var ipAdd = IPAddress.Parse(ipString);

        var port = m_port;

        var listener =  new TcpListener(ipAdd, port);

        listener.Start();
        m_log.WriteLine(string.Format("Listenを開始しました({0}:{1})。",
            ((IPEndPoint)listener.LocalEndpoint).Address,
            ((IPEndPoint)listener.LocalEndpoint).Port));

        while(!listener.Pending())
        {
            Thread.Sleep(33);
            if (m_force_exit)
            {
                listener.Stop();
                return;
            }
        }

        var client = listener.AcceptTcpClient();
        m_log.WriteLine(string.Format("クライアント({0}:{1})と接続しました。",
            ((IPEndPoint)client.Client.RemoteEndPoint).Address,
            ((IPEndPoint)client.Client.RemoteEndPoint).Port));

        var ns = client.GetStream();

        ns.ReadTimeout = 10000;
        ns.WriteTimeout = 10000;

        var enc = System.Text.Encoding.UTF8;
        var disconnected = false;
        var ms = new MemoryStream();
        var resBytes = new byte[256];
        var resSize = 0;
        do
        {
            resSize = ns.Read(resBytes, 0, resBytes.Length);
            if (resSize == 0)
            {
                disconnected = true;
                m_log.WriteLine("クライアントが切断しました。");
                break;
            }
            ms.Write(resBytes, 0, resSize);
            //まだ読み取れるデータがあるか、データの最後が\nでない時は、
            // 受信を続ける
        } while (ns.DataAvailable || resBytes[resSize - 1] != '\n');

        //受信したデータを文字列に変換
        var resMsg = enc.GetString(ms.GetBuffer(), 0, (int)ms.Length);
        ms.Close();

        //末尾の\nを削除
        resMsg = resMsg.TrimEnd('\n');
        m_log.WriteLine(resMsg);

        lock(m_req_list)
        {
            m_req_list.Enqueue(resMsg);
        }

        if (!disconnected)
        {
            //クライアントにデータを送信する
            //クライアントに送信する文字列を作成
            var sendMsg = resMsg.Length.ToString();
            //文字列をByte型配列に変換
            var sendBytes = enc.GetBytes(sendMsg + '\n');
            //データを送信する
            ns.Write(sendBytes, 0, sendBytes.Length);
            m_log.WriteLine(sendMsg);
        }

        //閉じる
        ns.Close();
        client.Close();
        m_log.WriteLine("クライアントとの接続を閉じました。");

        //リスナを閉じる
        listener.Stop();
        m_log.WriteLine("Listenerを閉じました。");
    }
    #endregion

    #region クライアント
    private void send_client(string to_ip, int to_port, string msg)
    {
        //サーバーに送信するデータを入力してもらう
        //Console.WriteLine("文字列を入力し、Enterキーを押してください。");
        var sendMsg = msg; //Console.ReadLine();
        //何も入力されなかった時は終了
        if (sendMsg == null || sendMsg.Length == 0)
        {
            return;
        }

        //サーバーのIPアドレス（または、ホスト名）とポート番号
        var ipOrHost = to_ip;         //string ipOrHost = "localhost";
        var port     = to_port;

        //TcpClientを作成し、サーバーと接続する
        TcpClient tcp=null;
        try { 
            tcp = new TcpClient(ipOrHost, port);
        }
        catch (SystemException e)
        {
            m_log.WriteLine(e.Message);
            return;
        }
        m_log.WriteLine(string.Format("サーバー({0}:{1})と接続しました({2}:{3})。",
            ((IPEndPoint)tcp.Client.RemoteEndPoint).Address,
            ((IPEndPoint)tcp.Client.RemoteEndPoint).Port,
            ((IPEndPoint)tcp.Client.LocalEndPoint).Address,
            ((IPEndPoint)tcp.Client.LocalEndPoint).Port));

        //NetworkStreamを取得する
        var ns = tcp.GetStream();

        //読み取り、書き込みのタイムアウトを10秒にする
        //デフォルトはInfiniteで、タイムアウトしない
        //(.NET Framework 2.0以上が必要)
        ns.ReadTimeout  = 10000;
        ns.WriteTimeout = 10000;

        //サーバーにデータを送信する
        //文字列をByte型配列に変換
        var enc       = System.Text.Encoding.UTF8;
        var sendBytes = enc.GetBytes(sendMsg + '\n');
        //データを送信する
        ns.Write(sendBytes, 0, sendBytes.Length);
        m_log.WriteLine(sendMsg);

        //サーバーから送られたデータを受信する
        var ms       = new MemoryStream();
        var resBytes = new byte[256];
        var resSize  = 0;
        do
        {
            //データの一部を受信する
            resSize = ns.Read(resBytes, 0, resBytes.Length);
            //Readが0を返した時はサーバーが切断したと判断
            if (resSize == 0)
            {
                m_log.WriteLine("サーバーが切断しました。");
                break;
            }
            //受信したデータを蓄積する
            ms.Write(resBytes, 0, resSize);
            //まだ読み取れるデータがあるか、データの最後が\nでない時は、
            // 受信を続ける
        } while (ns.DataAvailable || resBytes[resSize - 1] != '\n');
        //受信したデータを文字列に変換
        var resMsg = enc.GetString(ms.GetBuffer(), 0, (int)ms.Length);
        ms.Close();
        //末尾の\nを削除
        resMsg = resMsg.TrimEnd('\n');
        m_log.WriteLine(resMsg);

        //閉じる
        ns.Close();
        tcp.Close();
        m_log.WriteLine("切断しました。");

        //Console.ReadLine();
    }
    #endregion

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
