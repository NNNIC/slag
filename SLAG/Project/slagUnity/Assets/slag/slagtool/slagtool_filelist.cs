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
    public class filelist {

        public int Count { get { return (files==null) ? 0 : files.Count;   }  }
        public string this [int i]
        {
            get { return i>=0 && i<Count ?  Path.Combine(root,files[i]) : null; }
        }

        public string root;
        public List<string> files = new List<string>();
        public filelist(string file=null, string iroot = null)
        {
            root = iroot;
            if (file!=null) files.Add(file);
        }
    }
}
