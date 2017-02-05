using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;

/// <summary>
/// NamedPipe : 
/// 　　
///    .net 2.0 モードで使用
///    
///    var pipe = new NamedPipe("myname");
///    pipe.Start();
///    
///    white(true)
///    {
///        var msg = pipe.Read();
///        if (msg!=null)
///        {
///            DoSomething(msg);
///            pipe.Write("message","distination name");
///        }
///        Sleep(33);
///    }
/// 
/// </summary>
public class NamedPipe
{
    public static Action<string> Log = (s)=> { System.Diagnostics.Debug.WriteLine(s); };



    string m_name; //自名            
    
    Thread m_thread;

    Queue<string> m_req_list;

    string m_serverfile { get{ return get_filename(m_name); } }

    bool m_force_end;
    bool m_bEnd;

    public NamedPipe(string myname)
    {
        m_name = myname;
    }

    public void Start()
    {
        Log("Pipe Start Thread");

        m_force_end = false;
        m_bEnd      = false;

        m_thread = new Thread(server);
        m_thread.IsBackground = true;
        m_thread.Priority = ThreadPriority.AboveNormal;
        m_thread.Start();

        m_req_list = new Queue<string>();
    }
    public void Terminate()
    {
        m_force_end = true;
        if (m_thread!=null)
        { 
            m_thread.Interrupt();
            m_thread.Join();
            m_thread = null;
        }
    }
    public bool IsEnd()
    {
        if (m_force_end && m_bEnd &&  m_thread!=null)
        {
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
    public void Write(string msg,string to_name)
    {
        send_client(msg,to_name);
    }

    #region サーバー
    private void server()
    {
        Log("NAMEDPIPEサーバ開始");

        var file = m_serverfile;
        
        try { 
            //var ps = new NamedPipeServerStream(file, PipeDirection.In,1,PipeTransmissionMode.Byte,PipeOptions.Asynchronous);
            using (var ps = new NamedPipeServerStream(file, PipeDirection.In,1,PipeTransmissionMode.Byte))
            {
                Log("サーバ:" + file);

                Log("Connecting...");
                while(true)
                {
                    bool bWaitDone = false;
                    ps.BeginWaitForConnection(r =>
                    {
                        bWaitDone = true;
                        //if (m_force_end) return;
                        try { ps.EndWaitForConnection(r); } catch (SystemException e) { Log(e.Message);}
                    },
                    null);
                    while (!bWaitDone)
                    {
                        Thread.Sleep(100);
                    }

                    if (m_force_end)
                    {
                        break;
                    }
                    if (!ps.IsConnected)
                    {
                        Log("Reconnecting...");
                        continue;
                    }

                    while(true)
                    {
                        Log("Reading...");
                        var ss =new StreamString(ps);
                        var cmd = ss.ReadString();
                        Log("-->" + cmd);
                        if (cmd == "#END")
                        {
                            ps.Disconnect();
                            break;
                        }
                        m_req_list.Enqueue(cmd);
                    }       
                }
                if (ps.IsConnected) ps.Disconnect();
                ps.Close();
                ps.Dispose();
            }
        }
        catch (SystemException e)
        {
            Log("CreatePipe Error :" + e.Message);
        }
        m_bEnd = true;
        Log("PIPEサーバ終了");
    }
    #endregion

    #region クライアント
    private void send_client(string msg, string to_name)
    {
        if (string.IsNullOrEmpty(msg))
        {
            Log("メッセージが設定されていません");
            return;
        }

        var file = get_filename(to_name);

        using (var ps = new NamedPipeClientStream(".", file, PipeDirection.InOut, PipeOptions.None))
        {
            Log("Connecting to server...");
            ps.Connect();
            Log("Connected");

            var ss = new StreamString(ps);
            ss.WriteString(msg);
            ss.WriteString("#END");
            ps.Close();
        }
    }
    #endregion

    // --- tool for this class ---
    private string get_filename(string name)
    {
        return "namedpipe_"+name; 
    }

    // Defines the data protocol for reading and writing strings on our stream
    public class StreamString
    {
        private Stream ioStream;
        private UnicodeEncoding streamEncoding;

        public StreamString(Stream ioStream)
        {
            this.ioStream = ioStream;
            streamEncoding = new UnicodeEncoding();
        }

        public string ReadString()
        {
            int len = 0;

            len = ioStream.ReadByte() * 256;
            len += ioStream.ReadByte();
            byte[] inBuffer = new byte[len];
            ioStream.Read(inBuffer, 0, len);

            return streamEncoding.GetString(inBuffer);
        }

        public int WriteString(string outString)
        {
            byte[] outBuffer = streamEncoding.GetBytes(outString);
            int len = outBuffer.Length;
            if (len > UInt16.MaxValue)
            {
                len = (int)UInt16.MaxValue;
            }
            ioStream.WriteByte((byte)(len / 256));
            ioStream.WriteByte((byte)(len & 255));
            ioStream.Write(outBuffer, 0, len);
            ioStream.Flush();

            return outBuffer.Length + 2;
        }
    }
}

