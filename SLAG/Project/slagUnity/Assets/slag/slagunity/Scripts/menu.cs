using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menu : MonoBehaviour {

	void Start () {
		
	}
	void Update () {
		
	}
    private void OnGUI()
    {
        var gh = GUILayout.Height(100);
        GUILayout.BeginArea(new Rect(Screen.width / 4, 0,Screen.width /2, Screen.height));
        {
            if (GUILayout.Button("Load binary and play",gh))
            {
                SceneManager.LoadScene("playslag");
            }
            if (GUILayout.Button("Load text file and play",gh))
            {
                SceneManager.LoadScene("playtext");
            }
#if UNITY_STANDALONE_WIN
            if (GUILayout.Button("Remote load and play",gh))
            {
                SceneManager.LoadScene("remote");
            }
#endif
        }
        GUILayout.EndArea();
    }
}
