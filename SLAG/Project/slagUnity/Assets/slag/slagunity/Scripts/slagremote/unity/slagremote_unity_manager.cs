using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using slagremote;

/*
    2017/3/12 作成
    新リモート用オブジェクト

    常駐型

*/

public class slagremote_unity_manager : MonoBehaviour {

    #region インスタンス
    const string m_name = "slagremote_unity_manager";
    public static slagremote_unity_manager V;
    public static void Create(slagunity p_slagunity)
    {
        if (V==null)
        {
            var go = new GameObject(m_name);
            GameObject.DontDestroyOnLoad(go);

            V = go.AddComponent<slagremote_unity_manager>();

            V.m_start_done  = false;

            V.m_bReqAbort   = false;
            V.m_abort_done  = false;
            V.m_seq = new StateSequencer();
        }

        V.m_slagunity   = p_slagunity;
    }
    #endregion

    #region フレームワーク
    public slagunity m_slagunity { get; private set; }
    private StateSequencer m_seq;
    void Start () {
	}
	
	void Update () {
        if (m_seq!=null) m_seq.Update();
	}
    #endregion

    //コマンド制限用の実行モード
    slagremote.RUNMODE m_runmode;

    //Resetコマンド時のコールバック
    public Action      m_reset_callback;

    // 以下ステート
    public netcomm m_netcomm;
    bool   m_bReqAbort = false; //停止リクエストあり

    bool   m_start_done;
    void   S_START(float f) //開始
    {
        if (f==0)
        {
            StartCoroutine(_start_co());
        }
        if (m_start_done)
        {
            m_seq.Done();
        }
    }

    bool  m_abort_done;
    void S_ABORT(float f) //停止
    {
        if (f==0)
        {
            m_abort_done = false;
            m_bReqAbort  = true;
        }
        if (m_abort_done)
        {
            m_abort_done = false;
            m_seq.Done();
        }
    }

    IEnumerator _start_co()
    {
        m_start_done  = false;

        m_bReqAbort   = false;
        m_abort_done  = false;

        m_netcomm = new netcomm(m_runmode);
        m_netcomm.Start();

        slagremote.cmd.init();

        m_start_done = true; //スタート完了通知

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
        m_bReqAbort  = false;

        if (m_netcomm!=null)
        {
            m_netcomm.Terminate();
            while(!m_netcomm.IsEnd()) yield return null;
            m_netcomm = null;
        }
        m_abort_done = true;
        m_start_done = false;
    }

    //リクエスト
    public void StartCom(RUNMODE runmode,  Action cb=null) //スタート
    {
        m_runmode = runmode;

        if (m_seq.IsEmpty())  //シーケンスに次のリクエストがない状態で
        {         
            if (m_start_done) //既に実行済みにつき、すぐに終了
            {
                if (cb != null) cb();
                return;
            }
        }
        var last_statename = m_seq.LastStateNameInQueue();
        if (last_statename!=null && last_statename.StartsWith("start")) //多重要求禁止
        {
            if (cb != null) cb();
            return;
        }
        m_seq.Command("start0",S_START);
        if (cb!=null)
        {
            var cb2 = cb; //キャプチャ
            m_seq.Command("start1",
                f=> {
                cb2();
                m_seq.Done();
            });
        }
    }
    public void AbortCom(Action cb=null) //停止
    {
        if (m_seq.IsEmpty())            //次のリクエストがない状態で
        {
            if (m_start_done == false) // スタートしていないので、すぐ終了
            {
                if (cb != null) cb();
                return;
            }
        }
        var statename = m_seq.LastStateNameInQueue();
        if (statename!=null && statename.StartsWith("abort")) //多重要求禁止
        {
            if (cb != null) cb();
            return;
        }

        m_seq.Command("abort0",S_ABORT);
        if (cb!=null)
        {
            var cb2 = cb;
            m_seq.Command("abort1",f=> {
                cb2();
                m_seq.Done();
            });
        }
    }
}
