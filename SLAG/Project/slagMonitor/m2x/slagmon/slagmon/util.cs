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
    [Serializable]
    public class SAVEFORMAT
    {
        public string EditorPath;
    }
    public class util
    {
        static string m_savepth {
            get {
                return Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"),@"AppData\LocalLow\slag\slagmon.config");
            }
        }

        public static void Save(SAVEFORMAT data)
        {
            //var data = new SAVEFORMAT();
            //data.EditorPath = textBox1.Text;

            var ms = new MemoryStream();
            var bf = new BinaryFormatter();
            bf.Serialize(ms,data);

            if (!Directory.Exists(Path.GetDirectoryName(m_savepth)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(m_savepth));
            }
            try { 
                File.WriteAllBytes(m_savepth,ms.ToArray());
            }
            catch (SystemException e)
            {
                MessageBox.Show(e.Message);
            }
        }
        public static SAVEFORMAT Load()
        {
            if (!File.Exists(m_savepth))
            {
                //MessageBox.Show("File not found :" + m_savepth);
                System.Diagnostics.Debug.WriteLine("File not found :" + m_savepth);
                return null;
            }

            try { 
                var bin = File.ReadAllBytes(m_savepth);
                var ms = new MemoryStream(bin);
                var bf = new BinaryFormatter();
                var data = (SAVEFORMAT)bf.Deserialize(ms);

                return data;
            } catch (SystemException e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }
        public static void StartEditor(string file)
        {
            string editorpath = "notepad.exe";
            var data = Load();
            if (data!=null && !string.IsNullOrWhiteSpace(data.EditorPath))
            {
                editorpath = data.EditorPath.Trim().Trim('"');
            }
            
            try { 
                System.Diagnostics.Process.Start(editorpath, file);
            }
            catch
            {
                ;
            }

        }
        public static void OpenFolder(string folder)
        {
            try { 
                System.Diagnostics.Process.Start(folder);
            }
            catch
            {
                ;
            }
        }
        #region Jump    
                                                       //      0123456
        public static void Jump(Form1 form, string wd) //wd = "[SS$L:6,F:1]"
        {
            System.Diagnostics.Debug.WriteLine(wd);

            try { 
                var nwd = wd.Substring(4).TrimEnd(']'); // L:6,F:1
                var tokens = nwd.Split(',');
                if (tokens.Length!=2) return;

           
                int line = 0;
                {
                    var w = tokens[0]; //"L:6"
                    line = int.Parse(w.Substring(2));
                    line --;
                }
                int fid = 0;
                {
                    var w = tokens[1]; //F:1
                    fid = int.Parse(w.Substring(2));
                    fid --;
                }

                form.comboBoxFiles.SelectedIndex = fid;

                form.m_focus = line;

            } catch (SystemException e)
            {
               System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }
        #endregion

        #region Focus Source
        static int? m_changedIndex = null;
        public static void FocusSrc(Form1 form, int focusline)
        {
            var text = form.textBox2_src.Text;

            if (m_changedIndex!=null)
            {   
                var ci = (int)m_changedIndex;
                if (text.Length > ci && text[ci]=='>')
                {
                    var tmptext = new StringBuilder(form.textBox2_src.Text);   //var tmptext = form.textBox2_src.Text;
                    tmptext[ci] = ':';
                    form.textBox2_src.Text = tmptext.ToString();
                    text = form.textBox2_src.Text;
                }
            }

            var line = focusline + 1;

            int  cur = 0;
            int? focus_index=null;
            for(var idx=0; idx < text.Length;idx++)
            {
                if (cur == line)
                {
                    focus_index = idx;
                    break;
                }

                if (text[idx] == '\n')
                {
                    cur++;
                }
            }
            if (focus_index!=null)
            {
                var begin = (int)focus_index;
                var end   = text.IndexOf(':',begin + 1);
                if (begin>=0 && end>=begin)
                {
                    m_changedIndex = end;
                    
                    //form.textBox2_src.Text[end] = '>'; --- 割り当て不可！
                    var tmptext = new StringBuilder(form.textBox2_src.Text); 
                    tmptext[end] = '>';
                    form.textBox2_src.Text = tmptext.ToString();

                    form.textBox2_src.Select(begin,(end+1)-begin);
                    form.textBox2_src.Focus();  
                    form.textBox2_src.ScrollToCaret();              
                }
            }
        }
        #endregion
    }
}
