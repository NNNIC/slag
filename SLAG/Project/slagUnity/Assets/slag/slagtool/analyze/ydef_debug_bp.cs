using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ブレイクポインタ関連を集約

namespace slagtool
{
    public partial class YDEF_DEBUG
    {
        //ブレイクポインタ用
        public class BPITEM
        {
            public string filename;
            public int    hash;
            public List<int> lines = new List<int>();

            public BPITEM(int i_hash)
            {
                hash = i_hash;
            }
            public BPITEM(string i_filename)
            {
                filename = i_filename;
                hash     = GetFilenameHash(filename);
            }

            public void AddLine(int line)
            {
                if (!lines.Contains(line))
                {
                    lines.Add(line);
                }
            }
            public void DelLine(int line)
            {
                if (lines.Contains(line))
                {
                    lines.Remove(line);
                }
            }
        }

        public static  Dictionary<int,BPITEM> breakpoints;        //ブレイクポイント
        public static  int　                  cur_file_id;        //
        public static  string                 cur_filename
        {
            get {
                return m_filelist.GetFile(cur_file_id);
            }
        }

        #region  normalize breakpoint 
        private static void NormalizeBp()
        {
            if (breakpoints==null) return;
            for(var loop  = 0; loop <1000; loop++)
            {
                bool bNeedLoop = false;
                foreach(var k in breakpoints.Keys)
                {
                    var v = breakpoints[k];
                    if (v.lines==null || v.lines.Count==0)
                    {
                        breakpoints.Remove(k);
                        bNeedLoop = true;
                        break;
                    }
                }
                if (!bNeedLoop) break;
            }
            if (breakpoints.Count==0)
            {
                breakpoints = null;
            }
        }
        #endregion

        /// <summary>
        /// 名前でソートされたkeyを返す
        /// </summary>
        public static List<int> GetSortBpKeys()
        {
            if (breakpoints==null) return null;
            var filelist = new List<string>(); 
            foreach(var k in breakpoints.Keys)
            {
                filelist.Add(breakpoints[k].filename);
            }
            filelist.Sort();

            var keylist = new List<int>();
            foreach(var s in filelist)
            {
                foreach(var p in breakpoints)
                {
                    if (p.Value.filename == s)
                    {
                        keylist.Add(p.Key);
                        break;
                    }
                }
            }

            return keylist;
        }


        #region bp add|del
        private static BPITEM _findBpItemByFileName(string filename, bool bCreateIfNotExist)
        {
            BPITEM item = null;
            if (breakpoints!=null) {
                var hash = GetFilenameHash(filename);
                if (breakpoints.ContainsKey(hash))
                {
                    item = breakpoints[hash];
                }
            }

            if (item==null && bCreateIfNotExist)
            {
                item = new BPITEM(filename);
                if (breakpoints==null) breakpoints = new Dictionary<int, BPITEM>();
                breakpoints.Add(item.hash,item);
            }

            __saveCurFileId(item);

            return item;
        }
        private static void __saveCurFileId(BPITEM item)
        {
            cur_file_id = 0;
            try {
                if (item!=null)
                {
                    cur_file_id = m_filelist.files.FindIndex(i=>i.filename == item.filename);
                }
            } catch { }
            return;
        }
        private static BPITEM _findBpItemByFileID(int fileid, bool bCreateIfNotExist)
        {
            var filename = TMPFILENAME;
            if (m_filelist!=null) {
                filename = m_filelist.GetFile(fileid);
            }
            return _findBpItemByFileName(filename,bCreateIfNotExist);            
        }

        private static BPITEM FindBpItem(string filename_or_id, bool bCreateIfNotExist)
        {
            if (!string.IsNullOrEmpty(filename_or_id))
            {
                if (filename_or_id[0]=='\"')
                {
                    var filename = filename_or_id.Trim('\"');
                    return _findBpItemByFileName(filename,bCreateIfNotExist);
                }
                int fid = 0;
                if (int.TryParse(filename_or_id, out fid))
                {
                    return _findBpItemByFileID(fid,bCreateIfNotExist);
                }
            } 
            return _findBpItemByFileID(cur_file_id,bCreateIfNotExist);
        }

        public static void AddBreakpoint(int line, string filename_or_id=null )
        {
            /*
                filename_or_id が null時は、cur_file_idを使用
                m_filelistにてcur_file_id無効の場合は、filelistのトップを採用

                filename_or_idがダブルクゥオートに囲まれた文字列の場合に、それがファイル名
            */
            var item = FindBpItem(filename_or_id,true);
            item.AddLine(line);
        }
        public static bool DelBreakpoint(int line, string filename_or_id=null)
        {
            var item = FindBpItem(filename_or_id,false);
            if (item!=null)
            {
                item.DelLine(line);
            }
            NormalizeBp();
            return true;
        }
        #endregion

        public static void ResetAllBreakpoints()
        {
            breakpoints = null;
        }

        public static List<int> FindBpLinesList(int fid)
        {
            var bpitem = _findBpItemByFileID(fid,false);
            if (bpitem==null) return null;
            return bpitem.lines;
        }
        //--- 本クラス内使用のツール
        public static int GetFilenameHash(string filename)
        {
            return filename.ToUpper().GetHashCode();
        }
    }
}
