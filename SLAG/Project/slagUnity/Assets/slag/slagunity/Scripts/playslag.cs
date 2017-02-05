﻿using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playslag : MonoBehaviour {

    StateManager m_sm;
    Action       m_guiFunc;

    List<string> files = null;
    //slagtool.slag m_slag;
    slagunity m_slagunity;

	void Start () {
        m_sm = new StateManager();
        m_sm.Goto(S_INIT);
	}
	
	void Update () {
		m_sm.Update();
	}

    private void OnGUI()
    {
        if (m_guiFunc!=null) m_guiFunc();        
    }

    void S_INIT(bool bFirst)
    {
        if (bFirst)
        {
            var fileinfos = new DirectoryInfo(Application.dataPath + "/slag/slagunity/Resources/bin").GetFiles("*.bytes");
            files = new List<string>();
            Array.ForEach(fileinfos,f=> { files.Add(f.FullName); Debug.Log(f.Name); });

            //slagtool.util.SetDebugLevel(0);
            //slagtool.util.SetBuitIn(typeof(slagunity_builtinfunc));
            //slagtool.util.SetCalcOp(slagunity_builtincalc_op.Calc_op);

            //m_slag = new slagtool.slag(this);

            //slagunity_root.SLAG = m_slag;
            

            m_slagunity = slagunity.Create(gameObject);

            m_sm.Goto(S_TESTMENU);
        }
    }

    #region S_TESTMENU
    void S_TESTMENU(bool bFirst)
    {
        if (bFirst)
        {
            m_guiFunc = testMenuGui;
        }
    }
    Vector2 m_pos;
    void testMenuGui()
    {
        var gh = GUILayout.Height(50);

        GUILayout.BeginArea(new Rect(0,Screen.height / 2, Screen.width/2,Screen.height / 2));
        m_pos = GUILayout.BeginScrollView(m_pos);
        for(int i = 0; i<files.Count; i++)
        {
            var fn = Path.GetFileNameWithoutExtension(files[i]);
            if (GUILayout.Button(fn,gh))
            {
                var bytes = Resources.Load<TextAsset>("bin/" + fn).bytes;
                m_slagunity.LoadBin(bytes);
                m_slagunity.Run();
                m_sm.Goto(S_PLAYING);
            }
        }
        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }
    #endregion

    void S_PLAYING(bool bFirst)
    {
        if (bFirst)
        {
            m_guiFunc = playingGUI;
        }
    }
    void playingGUI()
    {  
        var rect = new Rect((float)Screen.width/3f,0,(float)Screen.width/3f,50);
        if (GUI.Button(rect,"RESET"))
        {
            SceneManager.LoadScene("playslagreset");
        }
    }
}
