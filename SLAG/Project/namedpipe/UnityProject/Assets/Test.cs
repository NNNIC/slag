using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

    public NamedPipe m_pipe;

	// Use this for initialization
	void Start () {
        NamedPipe.Log = (s)=>Debug.Log(s);

        m_pipe = new NamedPipe("unity");	
        m_pipe.Start();
	}
	
	// Update is called once per frame
	void Update () {
        var msg = m_pipe.Read();
        if (msg!=null)
        {
            Debug.Log(msg);
        }
	}

    void OnGUI()
    {
        if (m_pipe.IsEnd())
        {
            GUILayout.Label("STOPED");
        }
        else
        { 
            if (GUILayout.Button("STOP SERVER"))
            {
                m_pipe.Terminate();
            }
        }
    }

    void OnDestroy()
    {
        m_pipe.Terminate();
    }
}
