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

namespace slagmon
{
    public partial class Form1 : Form
    {
        public static string m_work_path = @"N:\Project\test";
        
        public static Form1 V;

        public static bool m_bDebug;

        Queue<string> m_log;
        public FilePipe m_pipe;
        public int?     m_focus;//デバッグ用フォーカスライン

        public string   m_curfile;

        public Form1()
        {
            InitializeComponent();

            V = this;

            m_log = new Queue<string>();

            FilePipe.Log = (s) => {
                lock(m_log)
                {
                    m_log.Enqueue(s);
                }
            };

            m_pipe = new FilePipe("mon");
            m_pipe.Start();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_pipe.Terminate();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
        }

        public void WriteLog(string s)
        {
            textBox1_log.AppendText(s + System.Environment.NewLine);
            if (s.Contains("[SS$L"))
            {
                var begin = s.IndexOf("[SS$L");
                var end   = s.IndexOf("]",begin+5);
                var wd = s.Substring(begin,end-begin+1);
                util.Jump(this,wd);
            }
        }
        private void WriteVar(string s)
        {
            if (s.ToUpper().StartsWith("@STOP"))
            {
                textBoxVar.Text="";
            }

            textBoxVar.Text += s.Substring(1) + Environment.NewLine;
        }
        private void WritePlayText(string s)
        {
            try { 
                var base64 = s.Substring(6);
                var bytes = Convert.FromBase64String(base64);

                
                //textBox2_src.Text = Encoding.UTF8.GetString(bytes).Replace("\n",Environment.NewLine);
                util.WriteTextToSrcDG( Encoding.UTF8.GetString(bytes));

                m_curfile = "?TEXT?";

            } catch {
                WriteLog("GetPlayText got unknown text");
            }

        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            for(var loop= 0; loop<5000; loop++)
            { 
                var s = m_pipe.Read();
                if (s!=null&&!string.IsNullOrEmpty(s))
                { 
                    bool bConverted = false;

                    if (s[0]=='@')
                    {
                        bConverted = true;
                        WriteVar(s);
                    }
                    else if (s.StartsWith("?TEXT?"))
                    {
                        bConverted = true;
                        WritePlayText(s);
                    }
                    else if (s.StartsWith("[FILELIST:"))
                    {
                        bConverted = true;
                        var ns = s.Substring(10);
                        var toks = ns.Split(',');
                        var list = new List<string>();
                        foreach(var i in toks)
                        {
                            var fn = i.TrimEnd(']');
                            list.Add(fn);
                        }
                        __loadMultiScript(list);
                    }
                    else if (s.StartsWith("[BPLIST:"))
                    {
                        bConverted = true;
                        util_bp.ReadBpListLog(s);
                        util_bp.Refresh();
                    }

                    if (m_bDebug)
                    {
                        WriteLog(s);
                    }                            
                    else
                    {
                        if (!bConverted)
                        {                   
                            WriteLog(s);
                        }
                    }
                }
                else
                {
                    break;
                }
            }

            lock(m_log)
            {
                while(m_log.Count>0)
                {
                    var s = m_log.Dequeue();
                    textBox1_log.AppendText(s + Environment.NewLine);
                }
            }

            //フォーカス
            try { 
                if (m_focus!=null)
                {
                    util.FocusSrc(this,(int)m_focus);
                    m_focus = null;
                }
            } catch (SystemException ec)
            {
                System.Diagnostics.Debug.WriteLine(ec.Message);
            }
        }
        private void textBox3_input_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                string cmd = null;

                var topindex = textBox3_input.GetFirstCharIndexOfCurrentLine();
                var buf = textBox3_input.Text.Substring(topindex);
                if (string.IsNullOrWhiteSpace(buf)) return;
                var endindex = buf.IndexOf('\xa');
                if (endindex < 0)
                {
                    cmd = buf.TrimStart('>').TrimEnd();
                }
                else
                {
                    cmd = buf.Substring(0, endindex).TrimStart('>').TrimEnd();
                    e.KeyChar= '\x00';
                }

                if (!string.IsNullOrWhiteSpace(cmd))
                {
                    string newcmd = null;
                    if (_get_load_inc(cmd,out newcmd))
                    {
                        textBox1_log.AppendText("[展開 :" + cmd + "]" + Environment.NewLine);
                        cmd = newcmd;
                    }
                    textBox1_log.AppendText("Send Command : " + cmd + Environment.NewLine);

                    _if_help_add(cmd);

                    m_pipe.Write(cmd, "unity");

                    _loadScriptWhenCmdHas(cmd);
                }
            }
        }
        private bool _get_load_inc(string cmd, out string ncmd)
        {
            ncmd = null;

            List<string> readlist = null;
            var tokens = cmd.Split(' ');
            if (tokens==null || tokens.Length<2 || tokens[0].ToLower()!="load" || !tokens[1].ToLower().EndsWith(".inc") ) return false;
            try { 
                readlist = util.Get_inc_files(tokens[1]);  //File.ReadAllLines(Path.Combine(m_work_path, tokens[1]),Encoding.UTF8);
            } catch { return false; }

            if (readlist==null || readlist.Count==0) return false;

            foreach(var nl in readlist)
            {
                ncmd = ncmd==null ? "load " : (ncmd + " ");
                ncmd += nl;
            }

            return ncmd!=null;
        }
        private void _if_help_add(string i)
        {
            if (i.ToUpper()!="HELP") return;

            var NL  =  Environment.NewLine;
            var msg =        "# モニターショートカット #" 
                      + NL + ":[ ショートカット ]" 
                      + NL + ": F6   : Step over (ソース画面)  "
                      + NL + ": F7   : Step in   (ソース画面)  "
                      + NL + ": F9   : Set or reset Breakpoint    (ソース画面)  "
                      + NL + "" + NL;

            textBox1_log.AppendText(msg);
        }


        private void textBox3_input_KeyDown(object sender, KeyEventArgs e)
        {
            Form1_KeyDown(sender,e);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox3_input.AppendText(">");
            Text = "slag monitior - work dir: " + m_work_path;
            
        }

        private void textBox3_input_TextChanged(object sender, EventArgs e)
        {
            var txt = textBox3_input.Text;


            if (txt.Length>1)
            {
                if (txt[txt.Length-1]=='\x0a')
                {
                    textBox3_input.AppendText(">");
                }
            }
        }


        private void button1clear_Click(object sender, EventArgs e)
        {
            textBox1_log.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try { 
                var filename = comboBoxFiles.Items[comboBoxFiles.SelectedIndex].ToString().Substring(3);
                util.StartEditor(Path.Combine(m_work_path , filename));
            }
            catch
            {
                ;
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            this.textBox3_input.Focus();
        }

        private void buttonConfig_Click(object sender, EventArgs e)
        {
            var fm = new FormConfig();
            fm.ShowDialog();
        }

        private void buttonFolder_Click(object sender, EventArgs e)
        {
            util.OpenFolder(@"N:\Project\test");
        }

        private void comboBoxFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            _SetSource(comboBoxFiles.SelectedIndex);
        }

        //---
        private void _loadScriptWhenCmdHas(string cmd)
        {
            if (string.IsNullOrWhiteSpace(cmd)) return;
            var tokens = cmd.Split(' ');
            if (tokens[0].Trim().ToUpper()=="LOAD" && tokens.Length>=2)
            {
                comboBoxFiles.Items.Clear();
                var list = new List<string>();

                for(int i = 0; i<tokens.Length-1;i++)
                {
                    var tok = tokens[i+1]; 
                    var filename = tok.Trim();
                    list.Add(filename);
                }

                __loadMultiScript(list);
            }
        }
        private void __loadMultiScript(List<string> list)
        {
            comboBoxFiles.Items.Clear();
            for(int i = 0; i<list.Count; i++)
            {
                comboBoxFiles.Items.Add((i+1).ToString("00") + " " + list[i]);
            }
            _SetSource(0);
        }
        private void _SetSource(int index)
        {
            try { 
                if (comboBoxFiles.Items.Count>index)
                {
                    var filename = comboBoxFiles.Items[index].ToString().Substring(3);
                    var path = @"N:\Project\test\" + filename;
                    m_curfile = filename;
                    util.WriteTextToSrcDG("");

                    if (File.Exists(path))
                    {
                        var ext = Path.GetExtension(path).ToLower();
                        if (ext==".js")
                        { 
                            var text = File.ReadAllText(path,Encoding.UTF8);
                            util.WriteTextToSrcDG(text);
                        }
                        
                        comboBoxFiles.SelectedIndex = index;
                    }
                }
            }
            catch { }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.F6)
            {
                var cmd = "step";
                m_pipe.Write(cmd, "unity");
                if (m_bDebug)
                {
                    WriteLog(cmd);
                }
            }
            if (e.KeyCode == Keys.F7)
            {
                var cmd = "step i";
                m_pipe.Write(cmd, "unity");
                if (m_bDebug)
                {
                    WriteLog(cmd);
                }
            }
            if (e.KeyCode == Keys.F9)
            {
                util_bp.SetBp();
            }
        }
        private void dataSource_KeyDown(object sender, KeyEventArgs e)
        {
            Form1_KeyDown(sender,e);
        }


        #region drag and drop
        private void dataSource_DragEnter(object sender, DragEventArgs e)
        {
            //ref http://dobon.net/vb/dotnet/control/droppedfile.html

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
        private void dataSource_DragDrop(object sender, DragEventArgs e)
        {
            var fileNames = (string[]) e.Data.GetData(DataFormats.FileDrop, false);
            
            if (fileNames.Length==0) return;
            var file = Path.GetFullPath( fileNames[0]);
            
            if (!file.StartsWith( Path.GetFullPath( m_work_path)))
            {
                MessageBox.Show("ファイルがワークディレクトリにありません");
                return;
            }
            var refpath = file.Substring(m_work_path.Length + 1);

            List<string> list = null;

            if (Path.GetExtension(refpath)==".inc")
            {
                list = util.Get_inc_files(file);
            }
            else
            {
                list = new List<string>();
                list.Add(refpath);
            }
            
            __loadMultiScript(list);

        }
        #endregion

    }
}
