using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

/*
    ファイルリスト

    ファイル名保持のため

    リモート環境を考慮して、ルートフォルダと関連パスとする
    
*/

namespace slagtool
{
    [System.Serializable]
    public class Filelist {

        #region fileItem 
        [System.Serializable]
        public class Item {
            public string filename;
            public int    hash;
        }
        public List<Item> files = new List<Item>();
        public void filesAdd(string filename)
        {
            var i = new Item();
            i.filename = filename;
            i.hash     = YDEF_DEBUG.GetFilenameHash(filename);
            files.Add(i);
        }
        #endregion

        public string root;

        #region constructor
        public Filelist(string file=null)
        {
            if (file!=null)
            {
                filesAdd(file);
            }
        }
        public Filelist(string iroot, string file)
        {
            root = iroot;
            filesAdd(file);
        }
        #endregion

        #region access 
        public int Count { get { return (files==null) ? 0 : files.Count;   }  }
        //public string this [int i]
        //{
        //    get { var n = GetFile(i); return n!=null ? Path.Combine(root,n) : null;  }
        //}
        public string GetFullPath(int idx)
        {
            var n = GetFile(idx);
            return n!=null ? Path.Combine(root,n) : null;
        }
        public string GetFile(int idx)
        {
            return files!=null && idx>=0 && idx<Count ? files[idx].filename : null;
        }
        public int GetHash(int idx)
        {
            return idx>=0 && idx<Count ? files[idx].hash : 0;
        }
        #endregion
    }
}
