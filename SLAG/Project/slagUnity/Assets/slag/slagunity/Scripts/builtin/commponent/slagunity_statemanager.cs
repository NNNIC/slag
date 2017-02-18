using UnityEngine;
using System.Collections;
using slagremote;
using slagtool;

/*
    ステートマシンを提供

    スクリプト例:

    function $_START(sm,bFirst)   // smは StateManager, bFirstはbooleanで初回のみtrueで呼ばれる
    {
        if (bFirst)
        {
            PrintLn("START");
            sm.Goto($_SECOND);            
        }
    }    
    function $_SECOND(sm,bFirst)
    {
        if (bFirst)
        {
            PrintLn("SECOND");
        }
        else
        {
            PrintLn("something");
        }
    }

    var $m_sm = StateManager();   --- ステートマネージャを作成
    $m_sm.Goto($_START);          --- $_STARTへ

*/

public class slagunity_statemanager : MonoBehaviour {

    public class StateManager
    {
        public slagunity_statemanager m_owner  { get; private set;           }
        public slagtool.slag          m_slag { get {return m_owner.m_slag; } }

        YVALUE m_cur;
        YVALUE m_next;

        int    m_waitcnt;
        float  m_waittime;

        float  dbg_elapsedtime=0; //時間計測

        public void Goto(YVALUE func)      { m_next     = func; }
        public void WaitCount(int c)       { m_waitcnt  = c;    }   //カウント分待つ
        public void WaitTime(float time)   { m_waittime = time; }   //指定時間（秒）待つ
        public void WaitCancel()           {m_waitcnt = 0; m_waittime=0; } //待ち中断

        public void Init(slagunity_statemanager owner)
        {
            m_owner = owner;
        }

        public void Update(float deltaTime)
        {
            if (m_waitcnt>0)
            {
                m_waitcnt--;
                return;
            }
            if (m_waittime>0)
            {
                m_waittime -= deltaTime;
                return;
            }

            bool bFirst = false;
            if (m_next!=null)
            {
                if (m_cur!=null) wk.Log("!" + m_cur + " elapsed " + dbg_elapsedtime +" sec ! (wo synctime)");
                dbg_elapsedtime = 0;
                m_cur  = m_next;
                m_next = null;
                bFirst = true;
            }
            if (m_slag!=null &&  m_cur!=null) {
                var save = Time.realtimeSinceStartup;
                if (sys.USETRY)
                {
                    try {  
            	       m_slag.CallFunc(m_cur,new object[2] { m_owner, bFirst });
                    }
                    catch (System.Exception e)
                    {
                        slagtool.sys.logline("--- 例外発生 ---");
                        slagtool.sys.logline(e.Message);
                        slagtool.sys.log_stopinfo();
                        slagtool.sys.logline("----------------");
                    }
                }
                else
                { 
                    m_slag.CallFunc(m_cur,new object[2] { m_owner,bFirst });
                }
                dbg_elapsedtime += Time.realtimeSinceStartup - save;
            }
        }
    }

    StateManager m_sm;
    public slagtool.slag m_slag {get;private set; }

	public void Init (slagtool.slag slag) {
        m_slag = slag;

        if (m_sm==null)
        {   
            m_sm = new StateManager();
            m_sm.Init(this);
        }
	}

    void Start()
    {
        //Init();
    }
	
	void Update () {
	   m_sm.Update(Time.deltaTime);
	}

    public void Goto(slagtool.YVALUE v)
    {
        m_sm.Goto(v);
    }

    public void WaitCount(int cnt)
    {
        m_sm.WaitCount(cnt);
    }

    public void WaitTime(float time)
    {
        m_sm.WaitTime(time);
    }

    public void WaitCancel()
    {
        m_sm.WaitCancel();
    }
    public slagunity_monobehaviour  bhv {  get { return GetComponent<slagunity_monobehaviour>();}  }
    //便宜: ユーザオブジェ 
    public object usrobj;
}
