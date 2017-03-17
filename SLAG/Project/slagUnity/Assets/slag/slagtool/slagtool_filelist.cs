using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/*
    ファイルリスト

    ファイル名保持のため

    リモート環境を考慮して、ルートフォルダと関連パスとする
    
*/

namespace slagtool
{
    [System.Serializable]
    public class Filelist {


        public string root;
        public List<string> files = new List<string>();

        #region constructor
        public Filelist(string file=null)
        {
            if (file!=null) files.Add(file);
        }
        public Filelist(string iroot, string file)
        {
            root = iroot;
            files.Add(file);
        }
        #endregion

        #region access 
        public int Count { get { return (files==null) ? 0 : files.Count;   }  }
        public string this [int i]
        {
            get { var n = GetFile(i); return n!=null ? Path.Combine(root,n) : null;  }
        }
        public string GetFile(int idx)
        {
            return idx>=0 && idx<Count ? files[idx] : null;
        }
        #endregion
    }
}
