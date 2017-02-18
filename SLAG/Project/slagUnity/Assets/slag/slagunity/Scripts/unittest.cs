using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;

public class unittest : MonoBehaviour {

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
                
            string os;
            _run(src, out os);

            log(os);
            yield return new WaitForSeconds(0.5f);
        }

        m_sm.Goto(S_CHECK);
    }

    void _run(string src, out string so)
    {
        so = null;
        var slag = slagunity.Create(gameObject);
        
        string outputstr = null;

        //表示の切り替え
        slagtool.runtime.builtin.builtin_sysfunc.m_printFunc   = (s)=> { Debug.Log(s); guiDisplay.Write(s);      outputstr+=s;     };
        slagtool.runtime.builtin.builtin_sysfunc.m_printLnFunc = (s)=> { Debug.Log(s); guiDisplay.WriteLine(s);  outputstr+=s+"\n";};

        try
        {
            slag.LoadSrc(src);
            slag.Run();
        }
        catch (SystemException e)
        {
            slagtool.runtime.builtin.builtin_sysfunc.m_printLnFunc(e.Message);
        }

        so = outputstr;
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
