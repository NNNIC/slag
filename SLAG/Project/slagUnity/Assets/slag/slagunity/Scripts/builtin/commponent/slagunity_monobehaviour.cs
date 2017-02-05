using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using slagtool;
using slagtool.runtime;
using slagtool.runtime.builtin;

public class slagunity_monobehaviour : MonoBehaviour {

    public slagtool.slag  m_slag {get;private set; }

    [System.NonSerialized]    public  YVALUE m_startFunc;
    [System.NonSerialized]    public  YVALUE m_updateFunc;
    [System.NonSerialized]    public  YVALUE m_onDestroyFunc;
    [System.NonSerialized]    public  YVALUE m_onMouseUpAsButtonFunc;
    [System.NonSerialized]    private Hashtable m_msgfunctable;

    public object    m_usrobj;                    //ユーザ用。必要に応じて使用してください。
    public Hashtable m_usrtbl = new Hashtable();  //ユーザ用。必要に応じて使用してください。

    public void Init(slagtool.slag slag)
    {
        m_slag = slag;
    }

	void Start () {
        callfunc(m_startFunc);
	}
	
	void Update () {
        callfunc(m_updateFunc);
	}

    void OnDestroy()
    {
        callfunc(m_onDestroyFunc);
    }

    void OnMouseUpAsButton()
    {
        callfunc(m_onMouseUpAsButtonFunc);
        
    }

    #region Send Message
    /*
        UnityのSendMessage似た動作をする機能

        スクリプト

        function $_hoge($bhv) //引数bhvは本クラス
        {
            PrintLn("called");
        }

        var $go = new GameObject();
        var $hv = AddBehaviour($go);       ---- 当コンポネント追加
        $hv.AddMsgFunc("xyz",$_hoge);      ---- $_hoge関数を "xyz"として登録
         :
         :
        SendMsg($go, "xyz");　　       --- GameObjectに対してSendMessageを送信。xyz名で定義された関数($_hoge)が呼び出される
    */
    public void AddMsgFunc(string name, YVALUE func)
    {
        if (m_msgfunctable==null) m_msgfunctable = new Hashtable();
        name = name.ToUpper();
        m_msgfunctable[name] = func;
    }
    public void SendMessageSocket(object o)
    {
        if (o==null || !(o is List<object>))
        {
            throw new System.Exception("unexpected");
        }

        var ol = (List<object>)o;
        string name = null;
        if (ol.Count > 0)
        {
            name = ol[0].ToString().ToUpper().Trim('\"');

            if (m_msgfunctable == null || !m_msgfunctable.ContainsKey(name))
            {
                slagtool.sys.logline("関数登録がありません : " + name);
                return;
            }
            var func = m_msgfunctable[name];
            if (func!=null && func is YVALUE)
            {
                ol.RemoveAt(0); //先頭のnameを削除

                callfunc((YVALUE)func,ol);
                return;
            }
            else
            { 
                slagtool.sys.logline("関数登録がNULLです : " + name);
                return;
            }
        }
        throw new System.Exception("unexpected");
    }
    #endregion

    //-- util for this class
    void callfunc(YVALUE func)
    {
        if (func!=null && m_slag!=null)
        { 
            if (slagtool.sys.USETRY)
            {
                try {  
            	    m_slag.CallFunc(func,new object[1] { this });
                } catch (System.Exception e)
                {
                    slagtool.sys.logline("--- 例外発生 ---");
                    slagtool.sys.logline(e.Message);
                    slagtool.sys.logline("----------------");
                }
            }
            else
            {
            	m_slag.CallFunc(func,new object[1] { this });
            }
        }
    }
    void callfunc(YVALUE func, object o)
    {
        if (o==null)
        {
            callfunc(func);
            return;
        }

        List<object> ol = null;
        if (o is List<object>)
        {
            ol = (List<object>)o;
        }
        else
        {
            ol = new List<object>();
            ol.Add(o);
        }
        
        ol.Insert(0,this);

        var oary = ol.ToArray();

        if (func!=null && m_slag!=null)
        { 
            if (slagtool.sys.USETRY)
            {
                try {  
            	    m_slag.CallFunc(func,oary);
                } catch (System.Exception e)
                {
                    slagtool.sys.logline("--- 例外発生 ---");
                    slagtool.sys.logline(e.Message);
                    slagtool.sys.logline("----------------");
                }
            }
            else
            {
            	m_slag.CallFunc(func,oary);
            }
        }
    }
}
