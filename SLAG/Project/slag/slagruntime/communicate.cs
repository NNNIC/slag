using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace slagruntime
{
    internal class communicate
    {
        string m_self_ip   = "127.0.0.1";
        int    m_self_port = 2001;

        string m_to_ip   = "127.0.0.1";
        int    m_to_port = 2002;

        TcpPipe       m_pipe;
        Queue<string> m_log;
        Thread        m_thread;
        
        object        m_mtx;
        string        m_cmd;

        static communicate V;

        public void Start()
        {
            V = this;
            m_mtx = new object();

            m_pipe   = new TcpPipe(m_self_ip,m_self_port);
            m_pipe.Start();

            m_log    = new Queue<string>();
            m_thread = new Thread(Work);
            m_thread.Start();
        }
        
        private void Work()
        {
            while(true)
            {
                _update();

                var cmd = m_pipe.Read();
                record(cmd);
                Thread.Sleep(33);
            }
        }

        private void record(string cmd)
        {
            lock(m_mtx)
            { 
                m_cmd = cmd;
            }
        }

        #region ログ
        public void Log(string s)
        {
            lock(m_log)
            {
                m_log.Enqueue(s);
            }
        }
        private void _update()
        {
            string s = null;
            lock(m_log)
            {
                while(m_log.Count>0)
                {
                    if (s!=null) s+=Environment.NewLine;
                    s+=m_log.Dequeue();
                }
            }
            m_pipe.Write(s,m_to_ip,m_to_port);
        }
        #endregion

        #region cmd
        public string GetCmd()
        {
            string s = null;
            lock(V.m_mtx)
            {
                s= V.m_cmd;
                V.m_cmd = null;
            }
            return s;
        }
        #endregion

    }
}
