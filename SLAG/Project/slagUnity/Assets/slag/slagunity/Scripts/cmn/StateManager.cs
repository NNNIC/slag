using UnityEngine;
using System.Collections;
using System;

public class StateManager {
    Action<bool> m_curstate;
    Action<bool> m_nextstate;
    
    //リクエスト
    public void Goto(Action<bool> func) 
    { 
        m_nextstate = func;  
    }
    
    //更新
    public void Update()
    {
        if (m_nextstate!=null)
        {
            m_curstate = m_nextstate;
            m_nextstate = null;
            m_curstate(true);
        }
        else
        {
            m_curstate(false);
        }
    }

    //確認
    public bool Check(Action<bool> state)
    {
        return m_curstate == state;
    }
}
