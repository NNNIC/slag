using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace slagmonitor
{
    public partial class Form1 : Form
    {
        FilePipe m_pipe;
        List<string> m_cmdlog;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            m_pipe = new FilePipe("127.0.0.1",22002);
            m_pipe.Start(s=>textBox1.AppendText(s));

            _cmdlog_init();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            m_pipe.Update();
            string s= null;
            for(var loop=0; loop<100;loop++)
            {
                var msg = m_pipe.Read();
                if (msg==null) break;
                if (s!=null) s+=Environment.NewLine;
                s+=msg;
            }
            if (s!=null) textBox1.AppendText(s + Environment.NewLine);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
        
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                var s = textBox2.Text;
                if (!string.IsNullOrEmpty(s))
                { 
                    _recordcmd(s);
                    m_pipe.Write(s,"127.0.0.1",22001);
                }
                textBox2.Text = null;
            }
            if (e.KeyCode == Keys.Up)
            {
                var s = _getoldcmd(-1);
                if (s!=null) textBox2.Text = s;
            }
            if (e.KeyCode == Keys.Down)
            {
                var s = _getoldcmd(1);
                if (s!=null) textBox2.Text = s;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_pipe.Tenminate();
        }

        //--- コマンドログ
        private string _cmdlogfile {get { return Path.Combine(Path.GetTempPath(),"~slagmonitorlog.txt");} }
        private string _savecmd;
        private void _cmdlog_init()
        {
            var file = _cmdlogfile;
            try
            {
                var l = File.ReadAllLines(file,Encoding.UTF8);
                m_cmdlog = new List<string>(l);
            } catch(SystemException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                m_cmdlog = new List<string>();
            }
        }
        private void _recordcmd(string s)
        {
            if (m_cmdlog.Contains(s))
            {
                m_cmdlog.Remove(s);
            }
            m_cmdlog.Add(s);
            _savecmd = s;
            try
            {
                File.WriteAllLines(_cmdlogfile,m_cmdlog.ToArray());
            }
            catch(SystemException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }
        private string _getoldcmd(int adv)
        {
            var idx = m_cmdlog.FindIndex(i=>i==_savecmd);
            if (adv<0)
            { 
                if (idx < 0)
                {
                    _savecmd = (m_cmdlog.Count>0) ? m_cmdlog[m_cmdlog.Count-1] : null;
                    return _savecmd;
                }
                else
                {
                    idx--;
                    _savecmd = (idx>=0 && idx < m_cmdlog.Count) ? m_cmdlog[idx] : null;
                    return _savecmd;
                }
            }
            else
            {
                idx++;
                _savecmd = (idx>=0 && idx < m_cmdlog.Count) ? m_cmdlog[idx] : null;
                return _savecmd;
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            textBox2.Focus();
        }
    }
}
