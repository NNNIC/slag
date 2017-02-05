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
using System.Runtime.Serialization.Formatters.Binary;

namespace slagmon
{
    public partial class FormConfig : Form
    {
        public FormConfig()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _save();
            this.Close();
        }

        private void FormConfig_Load(object sender, EventArgs e)
        {
            _load();
        }


        #region SAVE LOAD
        private void _save()
        {
            var data = new SAVEFORMAT();
            data.EditorPath = textBox1.Text;

            util.Save(data);
        }
        private void _load()
        {
            var data = util.Load();
            
            if (data!=null)
            {  
                textBox1.Text = data.EditorPath;
            }
        }
        #endregion

    }
}
