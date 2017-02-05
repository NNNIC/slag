using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FilePipeMonitor
{
    public partial class Form1 : Form
    {
        FilePipe m_pipe;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            m_pipe = new FilePipe("EARTH");
            m_pipe.Start(s=>System.Diagnostics.Debug.WriteLine(s));
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            m_pipe.Update();
            var msg = m_pipe.Read();
            if (msg!=null)
            {
                textBox1.AppendText(msg + Environment.NewLine);
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode== Keys.Enter)
            {
                var cmd = textBox2.Text;
                textBox2.Text = null;
                m_pipe.Write(cmd,"MOON");
                m_pipe.Write(cmd,"MOON");
            }
        }
    }
}
