using UnityEngine;
using System.Collections;
using System.Threading;
using slagremote;
using UnityEngine.SceneManagement;


public class slagremote_unity_main : MonoBehaviour {

    public static slagremote_unity_main V; //veridical pointer ... self pointer
    public static netcomm m_netcomm;

    bool m_bReqAbort;
    bool m_bEnd;

    void Awake()
    {
        V = this;
        
    }

	IEnumerator Start () {

        m_bReqAbort = false;
        m_bEnd      = false;

        UnityEngine.Debug.logger.logEnabled = false;

        //netcomm.Log = (s)=>Debug.Log(s);
        //FilePipe.Log= (s)=>Debug.Log(s);

        m_netcomm = new netcomm();
        m_netcomm.Start();

        slagtool.util.SetLogFunc(wk.SendWriteLine,wk.SendWrite);
        //slagtool.util.SetDebugLevel(0);
        //slagtool.util.SetBuitIn(typeof(slagunity_builtinfunc));
        //slagtool.util.SetCalcOp(slagunity_builtincalc_op.Calc_op);

        slagremote.cmd.init();

        yield return null;

        guiDisplay.WriteLine("slag monitor からコマンドを入力して下さい。"+System.Environment.NewLine);

        while(true)
        {
            if (m_bReqAbort) break;

            yield return null;
     
            if (m_bReqAbort) break;

            var cmd = slagremote.cmd.GetNextCmd();
            if (cmd==null) cmd = m_netcomm.GetCmd();
            
            if (cmd==null)
            {
                continue;
            }
            slagremote.cmd.execute(cmd);
        }
        m_bEnd = true;
    }

    void Update()
    {
        if (!m_bReqAbort) wk.Update();
        //if (!m_bReqAbort) slagremote.cmd_sub.UpdateExec();
    }

    void Reset()
    {
        StartCoroutine(_reset_co());
    }
    IEnumerator _reset_co()
    {

        Debug.Log("RESET!");

        if (m_netcomm!=null)
        {
            m_netcomm.Terminate();
            while(!m_netcomm.IsEnd()) yield return null;
            m_netcomm = null;
        }

        m_bReqAbort = true;

        while(!m_bEnd) yield return null;

        SceneManager.LoadScene("remotereset");
    }

}
