using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playremote : MonoBehaviour {

    slagunity m_slagunity;

    private void Start()
    {
        m_slagunity = slagunity.Create(gameObject);
        m_slagunity.StartNetComm(slagremote.RUNMODE.NORMAL);
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
}
