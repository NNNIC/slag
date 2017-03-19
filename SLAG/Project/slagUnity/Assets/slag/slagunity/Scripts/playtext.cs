using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Text;

public class playtext : MonoBehaviour {

    public static string test = "OK";
    public static string WORKINGDIR = @"N:\Project\Test";

    public guiDisplay m_guiDisplay;

    StateManager  m_sm;
    Action        m_guiFunc;
    string m_src { get { return slagunity.m_script; } set { slagunity.m_script = value;  } }

    static bool   m_resOrWorkDir = true;

    private void Start()
    {
        if (m_src==null) m_src = "//　スクリプトを入力するか、Loadボタンを押してください。\n";
        m_sm = new StateManager();
        m_guiDisplay.gameObject.SetActive(false);

        m_sm.Goto(S_START);

    }

    private void Update()
    {
        m_sm.Update();
    }

    private void OnGUI()
    {
        if (m_guiFunc!=null) m_guiFunc();
    }

    private void OnDestroy()
    {
        if (m_slagunity!=null)
        {
            m_slagunity.TerminateNetComm();
        }
    }

    void S_START(bool bFirst)
    {
        if (bFirst)
        {
            m_slagunity = slagunity.Create(gameObject);
            m_slagunity.StartNetComm( slagremote.RUNMODE.RunLimit);    // 終了時は OnDestroyにて TerminateNetCommを呼び出し

            m_sm.Goto(S_EDIT);
        }
    }
    #region edit
    private void S_EDIT(bool bFirst)
    {
        if (bFirst)
        {
            m_guiFunc = gui_edit;
            m_slagunity.TransferFileData();
            //slagremote.cmd_sub.GetPlayText();
            //m_slagunity.LoadSrc(m_src); //事前コンパイル
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
            _renewList();
            m_guiFunc = gui_load;
        }

    }
    private void _renewList()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        if (m_resOrWorkDir)
        {
            __renewList_Resources();
        }
        else
        {
            __renewList_WorkingDir();
        }
#else
        m_resOrWorkDir = true;
        __renewList_Resources();
#endif
    }
    private void __renewList_Resources()
    {
        var objs =  Resources.LoadAll("slag/txt",typeof(TextAsset));
        m_filelist = new List<string>();
        Array.ForEach(objs,i=> { 
            var file = i.name;
            var ext  = Path.GetExtension(file);
            var name = Path.GetFileNameWithoutExtension(file);
            if (name.Length==6 && name.StartsWith("test"))
            {
                if (ext==".js")  m_filelist.Add(file);
                if (ext==".inc") m_filelist.Add(file);
            }
        });
        m_filelist.Sort();
    }
    private void __renewList_WorkingDir()
    {
        m_filelist = new List<string>();
        if (!Directory.Exists(WORKINGDIR)) return;
        Array.ForEach((new DirectoryInfo(WORKINGDIR)).GetFiles("test??.js") ,i=>m_filelist.Add(i.Name));
        Array.ForEach((new DirectoryInfo(WORKINGDIR)).GetFiles("test??.inc"),i=>m_filelist.Add(i.Name));
    }
    string _getsrc(string file)
    {
        if (m_resOrWorkDir)
        {
            var ta = ((TextAsset)Resources.Load("slag/txt/" + file,typeof(TextAsset)));
            if (ta==null) return null;
            return ta.text;
        }
        else
        {
            var text = File.ReadAllText(Path.Combine(WORKINGDIR,file),Encoding.UTF8);
            return text;
        }
    }
    Vector2 m_pos;
    private void gui_load()
    {
        var gh = GUILayout.Height(50);

        GUI.Label(new Rect(0,0,Screen.width,30), "※IL2CPPモードではtest53以降は動作不可");

        GUILayout.BeginArea(new Rect(Screen.width/4,30,Screen.width/2,Screen.height-30));
        {
            var srcdir = m_resOrWorkDir ? "Resources/slag/txt":WORKINGDIR;
            if (GUILayout.Button("Source Directory\n"+srcdir))
            {
                m_resOrWorkDir = !m_resOrWorkDir;
                _renewList();
            }

            m_pos = GUILayout.BeginScrollView(m_pos);
            foreach(var s in m_filelist)
            { 
                if (GUILayout.Button(s,gh))
                {
                    if (s.EndsWith(".js"))
                    { 
                        m_src = _getsrc(s);  //((TextAsset)Resources.Load("slag/txt/" + s,typeof(TextAsset))).text;
                    }
                    else if (s.EndsWith(".inc"))
                    {
                        var inc = _getsrc(s).Split('\n'); //((TextAsset)Resources.Load("slag/txt/" + s,typeof(TextAsset))).text.Split('\n');
                        m_src = null;
                        foreach(var i in inc)
                        {
                            if (string.IsNullOrEmpty(i)) continue;
                            if (i.StartsWith("//")) continue;
                            var f = i.Trim();
                            if (string.IsNullOrEmpty(f)) continue;

                            var text = _getsrc(f); //var ta = ((TextAsset)Resources.Load("slag/txt/" + f,typeof(TextAsset)));
                            if (text==null) continue;
                            m_src += "//### include file : " + f +"\n";
                            m_src += text;
                            m_src += "\n\n";
                        }                       
                    }

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
            m_guiDisplay.gameObject.SetActive(true);
            m_guiFunc = playingGUI;
        }
        else
        {
            try { 
                m_slagunity.LoadSrc(m_src);
                m_slagunity.Run();
            }
            catch (SystemException e)
            {
                guiDisplay.Write(e.Message);    
                if (m_slagunity!=null) m_slagunity.WriteNetLog(e.Message);                   
            }
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
