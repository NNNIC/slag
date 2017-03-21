using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
    ブレイクポインタ関連
*/
namespace slagmon
{
    /*
        * unity側からbpを受け取るには？

        bplistがnullであれば、要求。
        - 要求タイミング
        　１．comboのファイルが更新されたとき
          ３．bp設定完了後の通信

        * bpのon/offについて
        
        bp x 行 "ファイル名"　を使用      


    */

    class util_bp
    {
        // 便宜
        private static void pipe_write(string s)
        {
            Form1.V.m_pipe.Write(s,"unity");
        }
        private static string m_curfile
        {
            get { return Form1.V.m_curfile; }
        }


        public class BPITEM
        {
            public string file;
            public List<int> lines;
        }
        static List<BPITEM> m_bpList;

        // Bp Set or Reset
        public static void SetBp()
        {
            var ds = Form1.V.dataSource;
            
            var rows = ds.SelectedRows;
            if (rows==null || rows.Count==0) return;

            try {
                var linestr = rows[0].Cells["Line"].Value.ToString();
                var s = string.Format("bp x {0} \"{1}\"",linestr,m_curfile);
                pipe_write(s);
            } catch {}
        }

        //Read BP List log
        public static void ReadBpListLog(string s)
        {
            try {
                m_bpList = new List<BPITEM>();

                //01234567
                //[BPLIST:
                var ns = s.Substring(8);
                var tokens = ns.Split('|');
                foreach(var t in tokens)
                {
                    var m = t.Trim('[',']');
                    var ms = m.Split(':');
                    var file  = ms[0];
                    var lines = ms[1].Split(',');

                    var item = new BPITEM();
                    item.file = file;
                    item.lines = new List<int>();
                    Array.ForEach(lines,i=>item.lines.Add(int.Parse(i)));

                    m_bpList.Add(item);
                }
            }
            catch
            {
                m_bpList = new List<BPITEM>();
            }
        }

        public static void Refresh()
        {
            var ds = Form1.V.dataSource;

            // clear all
            for(int i = 0; i<ds.RowCount; i++)
            {
                ds.Rows[i].Cells["bp"].Value = "";
            }

            var findfile = m_curfile;
            if (string.IsNullOrWhiteSpace(findfile))
            {
                findfile = "?TEXT?";
            }

            var finditem = m_bpList.Find(i=>i.file == findfile);
            if (finditem!=null)
            {
                foreach(var line in finditem.lines)
                {
                    if (line < ds.RowCount)
                    {
                        ds.Rows[line].Cells["bp"].Value = "BP";
                    }
                }
            }
        }

    }
}
