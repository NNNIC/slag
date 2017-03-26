using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;

public class slagunittest : MonoBehaviour {

    public guiDisplay m_guiDisplay;

    StateManager m_sm;
    Action       m_guiFunc;


    void Start () {
        m_sm = new StateManager();
        m_guiDisplay.gameObject.SetActive(false);
        m_sm.Goto(S_START);		
	}
	
	void Update () {
		m_sm.Update();
	}

    private void OnGUI()
    {
        if (m_guiFunc!=null) m_guiFunc();
    }

    void S_START(bool bFirst)
    {
        if (bFirst)
        {
            m_guiFunc = ()=>
            {
                if (GUILayout.Button("\n\n\ntest01～test1Xまで\n自動実行\n\n\n"))
                {
                    m_sm.Goto(S_RUN);
                }
            };
        }
    }

    #region RUN
    string m_output;
    List<string> m_filelist;
    void S_RUN(bool bFirst)
    {
        if (bFirst)
        {
            m_guiFunc = null;
            m_output  = null;
            m_guiDisplay.gameObject.SetActive(true);

            var list = ((TextAsset)Resources.Load("slag/txt/_list")).text.Split('\n');
            m_filelist = new List<string>();
            Array.ForEach(list,i=> {
                var s = i.Trim();
                if (!string.IsNullOrEmpty(s))
                { 
                    s = s.Replace(".txt","");
                    m_filelist.Add(s);
                }
            });

            StartCoroutine(_run_co());
        }
    }
    string m_outputstr;
    IEnumerator _run_co()
    {
        Action<string> log = (s)=> { guiDisplay.WriteLine(s); m_output += s + "\n"; };

        foreach(var f in m_filelist)
        {
            var nstr = f.Substring(4,2);
            int n = 0;
            if (!int.TryParse(nstr,out n)) continue;
            if (n>=20) continue;

            var src = ((TextAsset)Resources.Load("slag/txt/" + f,typeof(TextAsset))).text;

            log("###########################[test"+n.ToString("00") + "]");
            log("\n");
        
            
            // 実行
            bool bStartDone=false;
            var slag = _startSlag(()=>bStartDone=true);
            while(!bStartDone) yield return null;
                    
            m_outputstr = null;
            _change_output();

            _loadsrc(slag,src);

            yield return null;

            _run(slag, src);

            bool bEndDone=false;
            _endSlag(slag, ()=>bEndDone=true);
            while(!bEndDone) yield return null;

            log(m_outputstr);

            //実行終了

            yield return new WaitForSeconds(0.5f);
        }

        m_sm.Goto(S_CHECK);
    }

    slagunity _startSlag(Action cb)
    {
        var slag = slagunity.Create(gameObject);
        slag.StartNetComm( slagremote.RUNMODE.RunLimit,cb);
        return slag;
    }

    void _change_output()
    {
        //表示の切り替え
        slagtool.runtime.builtin.builtin_sysfunc.m_printFunc   = (s)=> { Debug.Log(s); guiDisplay.Write(s);      m_outputstr+=s;     };
        slagtool.runtime.builtin.builtin_sysfunc.m_printLnFunc = (s)=> { Debug.Log(s); guiDisplay.WriteLine(s);  m_outputstr+=s+"\n";};
    }

    void _loadsrc(slagunity slag, string src)
    {
        try
        {
            slag.LoadSrc(src);
            slag.TransferFileData();
            //slag.TransferBPList(); ファイル表示が正しくなくなるため、ＣＯ
        }
        catch (SystemException e)
        {
            slagtool.runtime.builtin.builtin_sysfunc.m_printLnFunc(e.Message);
        }
    }

    void _endSlag(slagunity slag, Action cb)
    {
        slag.TerminateNetComm(cb);
    }

    void _run(slagunity slag, string src)
    {
        try
        {
            slag.Run();
        }
        catch (SystemException e)
        {
            slagtool.runtime.builtin.builtin_sysfunc.m_printLnFunc(e.Message);
        }
    }
    #endregion

    void S_CHECK(bool bFirst)
    {
        if (bFirst)
        { 
            m_guiDisplay.gameObject.SetActive(false);

            var answer = ((TextAsset)Resources.Load("slag/unittest/answer",typeof(TextAsset))).text;

            if (answer.Trim() == m_output.Trim())
            {
                m_guiFunc = () => {
                    if (GUILayout.Button("\n\n結果一致\n問題ありません\n\n"))
                    {
                        SceneManager.LoadScene("0.Menu");
                    }
                };
            }
            else
            {
                m_guiFunc = () => {
                    if (GUILayout.Button("\n\n<color=red>結果不一致</color>\n\n\n"))
                    {
                        m_sm.Goto(S_FAILD);
                    }
                };
            }
        }
    }

    void S_FAILD(bool bFirst)
    {
        if (bFirst)
        {
            var bEditor = Application.platform.ToString().Contains("Editor");

            m_guiFunc = () => {
                GUILayout.Label("\n\n<color=red>結果不一致</color>\n\n\n");
                if (GUILayout.Button("再実行"))
                {
                        m_sm.Goto(S_START);
                }
                if (GUILayout.Button("メニューに戻る"))
                {
                        SceneManager.LoadScene("0.Menu");
                }
                if (bEditor)
                {
                    if (GUILayout.Button("結果を上書き?"))
                    {
                        m_sm.Goto(S_OVERWRITE);
                    }
                }
            };
        }
    }

    void S_OVERWRITE(bool bFirst)
    {
        if (bFirst)
        {
            m_guiFunc = () => {
                GUILayout.Label("\n\n<color=red>結果不一致</color>\n\n\n");
                if (GUILayout.Button("\n\n再確認\n\n結果を上書き?\n\n"))
                {
                    File.WriteAllText(Path.Combine(Application.dataPath, @"slag\slagunity\Resources\slag\unittest\answer.txt"),m_output);        
                    m_sm.Goto(S_DONE);
                }
                if (GUILayout.Button("\n\nキャンセル\n\n"))
                {
                    m_sm.Goto(S_START);
                }
            };
        }
    }
    void S_DONE(bool bFirst)
    {
        if (bFirst)
        {
#if UNITY_EDITOR
            m_guiFunc = () => { 
                GUILayout.Label("\n\n<color=red>結果不一致</color>\n\n\n");
                if (GUILayout.Button("結果を反映するため終了"))
                {
                    UnityEditor.EditorApplication.isPlaying = false;
                }
            };
#endif
        }
    }


}
