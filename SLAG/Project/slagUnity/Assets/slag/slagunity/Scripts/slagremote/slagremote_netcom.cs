using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace slagremote
{
    public class netcomm
    {
        public static Action<string>      Log           = (s)=>{ System.Diagnostics.Debug.WriteLine(s);  };// デバッグログ　※依存のため外出し

        /// <summary>
        /// コマンドを先処理します。
        /// 注）別スレッドで実行されます。
        /// 　　そのためスレッド依存関数は使用不可(※UnityAPIは依存関数になります)
        /// </summary>
        public static Func<string,string> PreProcessCmd = (s)=>{ return s;};                               // コマンド受取り直後に処理

        string m_myname  = "unity";

        string m_toname  = "mon";

        FilePipe       m_pipe;
        Queue<string>  m_sendlog;
        Thread         m_thread;
        
        object        m_mtx;
        string        m_cmd;

        bool          m_bReqAbort;
        bool          m_bEnd;

        public void Start()
        {
            //Log("netcomm:start");

            m_bReqAbort = false;
            m_bEnd      = false;

            m_mtx = new object();

            m_pipe   = new FilePipe(m_myname);
            m_pipe.Start();

            m_sendlog   = new Queue<string>();
            m_thread = new Thread(Work);
            m_thread.IsBackground = true;
            m_thread.Priority = ThreadPriority.AboveNormal;
            m_thread.Start();

        }

        public void Terminate()
        {
            if (m_pipe!=null)
            {
                m_pipe.Terminate();
            }
        }

        public bool IsEnd()
        {
            if (m_pipe!=null)
            {
                if (m_pipe.IsEnd())
                {
                    m_bReqAbort = true;
                    m_pipe = null;
                }
                return false;
            }

            if (m_bEnd && m_thread!=null)
            {
                m_thread.Join();
                m_thread = null;
                return false;
            }

            return m_bEnd;
        }

        private void Work()
        {
            try { 
                while(true)
                {
                    _update();

                    var cmd = m_pipe.Read();
                    if (cmd!=null)
                    { 
                        record(cmd);
                    }
                    else
                    { 
                        Thread.Sleep(33);
                    }
                    if (m_bReqAbort) break;
                }
            }
            catch (SystemException e) { Log(e.Message);   }

            m_bEnd = true;
        }

        private void record(string cmd)
        {
            lock(m_mtx)
            { 
                m_cmd = PreProcessCmd(cmd);
            }
        }

        #region ログ
        public void SendMsg(string s)
        {
            Log(s);
            lock(m_sendlog)
            {
                m_sendlog.Enqueue(s);
            }
        }
        private void _update()
        {
            string s = null;
            lock(m_sendlog)
            {
                while(m_sendlog.Count>0)
                {
                    if (m_bReqAbort) break;

                    if (s!=null) s+=Environment.NewLine;
                    s+=m_sendlog.Dequeue();
                }
            }
            if (!m_bReqAbort)
            { 
                if (s!=null) m_pipe.Write(s,m_toname);
            }
        }
        #endregion

        #region cmd
        public string GetCmd()
        {
            string s = null;
            lock(m_mtx)
            {
                s= m_cmd;
                m_cmd = null;
            }
            return s;
        }
        #endregion
    }
}
