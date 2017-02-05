using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace monitor1
{
    public partial class Form1 : Form
    {
        NamedPipe m_pipe;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            NamedPipe.Log = s =>textBox1.AppendText(s + Environment.NewLine);
            m_pipe = new NamedPipe("mon");
            m_pipe.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var msg = m_pipe.Read();
            if (msg!=null)
            {
                textBox1.AppendText(msg + Environment.NewLine);
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r' && !string.IsNullOrWhiteSpace(textBox2.Text))
            {
                m_pipe.Write(textBox2.Text,"unity");
                textBox2.Text="";
            }
        }
    }
}
