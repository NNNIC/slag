using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class guiDisplay : MonoBehaviour
{
    static guiDisplay V;

#region フレームワーク
    void Start()
    {
        V = this;

        //WriteLine("slag monitor からコマンドを入力して下さい。"+Environment.NewLine);
    }


    void OnGUI()
    {
        guishow();
    }
#endregion

    public class Item {
        public bool           bReadOrWrite;
        public string         text;
        public bool           bLF;
    };

    List<Item> m_list;

    Vector2 m_pos;

    string m_inputtext;
    string m_inputlabel;
    Action<string> m_inputcallback;

    void guishow()
    {
        if (m_list==null) return;
        GUILayout.BeginArea(new Rect(0,0,Screen.width,Screen.height));
        m_pos = GUILayout.BeginScrollView(m_pos);

        foreach(var i in m_list)
        {
            if (i.bReadOrWrite)
            {
                GUILayout.Label("== [INPUT] ==");
                if (m_inputlabel!=null) GUILayout.Label(m_inputlabel);
                m_inputtext = GUILayout.TextField(m_inputtext);
                if (GUILayout.Button("OK"))
                {
                    if (m_inputcallback!=null) m_inputcallback(m_inputtext);
                    i.text = m_inputlabel + ":" + m_inputtext + Environment.NewLine;
                    i.bReadOrWrite = false;
                }
            }
            else
            {
                GUILayout.Label(i.text);
            }
        }
        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    #region write
    public static void WriteLine(string s) { if (V!=null) V.writeLine(s); }
    public static void Write(string s)     { if (V!=null) V.write(s);     }
    void writeLine(string s)
    {
        if (m_list == null) m_list = new List<Item>();
        m_list.Add(new Item() { bReadOrWrite=false, text =s , bLF = true});
        m_pos.y = float.MaxValue;
    }
    void write(string s)
    {
        if (m_list == null) m_list = new List<Item>();
        if (m_list.Count==0)
        {
            m_list.Add(new Item() { bReadOrWrite=false, text =s, bLF = false });
        }
        else
        {
            var l = m_list[m_list.Count-1];
            if (!l.bLF)
            { 
                l.text += s;
                m_list[m_list.Count-1] = l;
            }
            else
            {
                m_list.Add(new Item() { bReadOrWrite=false, text =s, bLF = false });
            }
        }
        m_pos.y = float.MaxValue;
    }
    #endregion

    #region input
    public static void GetInput(string label, string initialtext, Action<string> cb)
    {
        if (V!=null) V.getinput(label,initialtext,cb);
    }
    void getinput(string label, string initialtext, Action<string> cb)
    {
        m_inputlabel = label;
        m_inputtext = initialtext!=null ? initialtext : "?";
        m_inputcallback = cb;
        if (m_list == null) m_list = new List<Item>();
        m_list.Add(new Item() { bReadOrWrite=true, text =null });
        m_pos.y = float.MaxValue;
    }
    #endregion

}
