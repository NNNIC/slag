using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menu : MonoBehaviour {

    slagunity m_slagunity;

	void Start () {
		m_slagunity = slagunity.Create(gameObject,false);
        m_slagunity.StartNetComm();
	}

	void Update () {
		
	}
    private void OnGUI()
    {
        var gh = GUILayout.Height(100);
        GUILayout.BeginArea(new Rect(Screen.width / 4, 0,Screen.width /2, Screen.height));
        {
            GUILayout.Label("slag ver."+slagunity.version);
            if (GUILayout.Button("Load text file and play\nテキストファイルのロードと実行",gh))
            {
                SceneManager.LoadScene("playtext");
            }
            if (GUILayout.Button("Load binary and play\nバイナリファイルのロードと実行",gh))
            {
                SceneManager.LoadScene("playslag");
            }
#if UNITY_STANDALONE_WIN
            if (GUILayout.Button("Remote load and play\nリモートによるロードと実行",gh))
            {
                SceneManager.LoadScene("remote");
            }
#endif
            if (GUILayout.Button("Unit Test\n単体テスト",gh))
            {
                SceneManager.LoadScene("unittest");
            }
            if (GUILayout.Button("Open Wiki\nWiki表示-",gh))
            {
                Application.OpenURL("https://github.com/NNNIC/slag/wiki");
            }


        }
        GUILayout.EndArea();
    }
}
