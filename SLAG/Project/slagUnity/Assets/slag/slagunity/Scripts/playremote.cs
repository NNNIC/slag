using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playremote : MonoBehaviour {

    slagunity m_slagunity;

    private void Start()
    {
        m_slagunity = slagunity.Create(gameObject);
        m_slagunity.StartNetComm(slagremote.RUNMODE.NORMAL);
        m_slagunity.SetResetCallback(Reset);
    }

    private void Update()
    {
    }
    private void OnDestroy()
    {
        if (m_slagunity!=null)
        {
            m_slagunity.TerminateNetComm();
        }
    }

    [ContextMenu("Reset")]
    public void Reset()
    {
        if (m_slagunity!=null)
        {
            m_slagunity.TerminateNetComm(()=> {
                SceneManager.LoadScene("remotereset");
            });
        }
    }
}
