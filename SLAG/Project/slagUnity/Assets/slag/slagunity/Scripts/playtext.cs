using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class playtext : MonoBehaviour {

    public guiDisplay m_guiDisplay;

    StateManager m_sm;
    Action       m_guiFunc;
    static string       m_src;

    private void Start()
    {
        if (m_src==null) m_src = "//　スクリプトを入力するか、Loadボタンを押してください。";
        m_sm = new StateManager();
        m_guiDisplay.gameObject.SetActive(false);
        m_sm.Goto(S_EDIT);
    }

    private void Update()
    {
        m_sm.Update();
    }

    private void OnGUI()
    {
        if (m_guiFunc!=null) m_guiFunc();
    }

    #region edit
    private void S_EDIT(bool bFirst)
    {
        if (bFirst)
        {
            m_guiFunc = gui_edit;
        }
    }
    private void gui_edit()
    {
        var w = Screen.width /3;
        if (GUI.Button(new Rect(0,0,w,50),"Load"))
        {
            m_sm.Goto(S_LOAD);
        }
        if (GUI.Button(new Rect(w,0,w,50),"Run"))
        {
            m_sm.Goto(S_RUN);
        }
        if (GUI.Button(new Rect(w*2,0,w,50),"[MENU]"))
        {
            SceneManager.LoadScene("0.Menu");
        }
        m_src = GUI.TextArea(new Rect(0,50,Screen.width,Screen.height-50),m_src);
    }
    #endregion

    #region load
    List<string> m_filelist;
    private void S_LOAD(bool bFirst)
    {
        if (bFirst)
        {
            var list = ((TextAsset)Resources.Load("txt/_list")).text.Split('\n');
            m_filelist = new List<string>();
            Array.ForEach(list,i=> {
                var s = i.Trim();
                if (!string.IsNullOrEmpty(s))
                { 
                    s = s.Replace(".txt","");
                    m_filelist.Add(s);
                }
            });
            m_guiFunc = gui_load;
        }

    }
    Vector2 m_pos;
    private void gui_load()
    {
        var gh = GUILayout.Height(50);

        GUI.Label(new Rect(0,0,Screen.width,30), "※IL2CPPモードではtest53以降は動作不可");

        GUILayout.BeginArea(new Rect(Screen.width/4,30,Screen.width/2,Screen.height-30));
        {
            m_pos = GUILayout.BeginScrollView(m_pos);
            foreach(var s in m_filelist)
            { 
                if (GUILayout.Button(s,gh))
                {
                    m_src = ((TextAsset)Resources.Load("txt/" + s,typeof(TextAsset))).text;
                    m_sm.Goto(S_EDIT);
                }
            }
            GUILayout.EndScrollView();
        }
        GUILayout.EndArea();
    }
    #endregion

    #region run
    slagunity m_slagunity;
    private void S_RUN(bool bFirst)
    {
        if (bFirst)
        {
            if (m_slagunity == null)
            {
                m_slagunity = slagunity.Create(gameObject);
            }

            m_guiDisplay.gameObject.SetActive(true);
            m_guiFunc = playingGUI;
        }
        else
        {
            m_slagunity.LoadSrc(m_src);
            m_slagunity.Run();
            m_sm.Goto(S_RUNNING);
        }
    }
    void playingGUI()
    {  
        var rect = new Rect((float)Screen.width/3f,0,(float)Screen.width/3f,50);
        if (GUI.Button(rect,"RESET"))
        {
            SceneManager.LoadScene("playtextreset");
        }
    }
    private void S_RUNNING(bool bFirst)
    {
    }
    #endregion
}
