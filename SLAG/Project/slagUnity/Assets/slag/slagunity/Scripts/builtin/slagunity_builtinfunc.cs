using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using number = System.Double;
using slagtool.runtime;
using slagtool.runtime.builtin;


public class slagunity_builtinfunc {
    static string NL = Environment.NewLine;

    static string m_readtext;
    public static object F_ReadLineStart(bool bHelp, object[] ol, StateBuffer sb)
    {
        if (bHelp) return "GUI入力開始";

        m_readtext = null;
        var label = kit.get_string_at(ol,0);
        guiDisplay.GetInput(label,"",(s)=>m_readtext=s);
        return null;
    }
    public static object F_ReadLineDone(bool bHelp, object[] ol, StateBuffer sb)
    {
        if (bHelp) return "GUI入力取得";

        return m_readtext;
    }

    public static object F_AddBehaviour(bool bHelp, object[] ol, StateBuffer sb)
    {
        if (bHelp)
        {
            return "slagunity_monobehaviourコンポネントを追加" + NL +
                   "フォーマット) var bhv = AddBehaviour([GameObject]);"+NL +
                   "　　　　　　　GameObject指定がない場合、slag実行メインのGameObjectに追加";
        }
        GameObject go = null;
        if (ol.Length==0)
        {
            if (sb.m_slag!=null&& sb.m_slag.m_owner!=null && sb.m_slag.m_owner is slagunity && ((slagunity)sb.m_slag.m_owner).m_root!=null)
            {
                go = ((slagunity)sb.m_slag.m_owner).m_root.gameObject;
            }
            else        //slagunity_root.V.gameObject;
            {
                throw new SystemException("slagの初期化時のownerを確認せよ");
            }
        }
        else if (ol[0] is GameObject)
        {
            go = (GameObject)ol[0];
        }
        var mono = go.AddComponent<slagunity_monobehaviour>();
        mono.Init(sb.m_slag);

        return mono;
    }

#region ステートマシン
    public static object F_StateManager(bool bHelp, object[] ol, StateBuffer sb)
    {
        if (bHelp)
        {
            return "ステート管理作成。" +NL+ 
                   "フォーマット) var sm = StateManager([GameObject]);" + NL +
                   "slagunity_statemanagerクラスに詳細あり";
        }

        slagunity_statemanager sm = null;
        if (ol.Length == 0)
        {
            GameObject go = null;
            if (sb.m_slag!=null&& sb.m_slag.m_owner!=null && sb.m_slag.m_owner is slagunity && ((slagunity)sb.m_slag.m_owner).m_root!=null)
            {
                go = ((slagunity)sb.m_slag.m_owner).m_root.gameObject;
            }
            else
            {
                throw new SystemException("slag初期化時のownerを確認せよ");
            }
            sm = go.AddComponent<slagunity_statemanager>();
            sm.Init(sb.m_slag);
        }
        else if (ol[0] is GameObject)
        {
            sm = ((GameObject)ol[0]).AddComponent<slagunity_statemanager>();
        }
        else
        {
            util._error("StateManager関数のパラメータが不正です");
        }

        sm.Init(sb.m_slag);

        return sm;
    }

#endregion

    public static object F_SendMsg(bool bHelp, object[] ol, StateBuffer sb)
    {
        if (bHelp)
        {
            return "GameObjectにメッセージを送る。メッセージ受信先が存在すれば指定関数を実行。"+NL+
                   "フォーマット) SendMsg(GameObject,名前[,パラメータ・・・])" + NL +
                   "slagunity_monoehaviourクラス内に詳細あり ";
        }
        GameObject go   = null;
        if (ol.Length>0)
        {
            go   = (GameObject)ol[0];
        }
        List<object> plist=null;
        if (ol.Length>1)
        {
            plist = new List<object>();
            for(int i = 1; i<ol.Length; i++)
            {
                plist.Add(ol[i]);
            }
        }
        if (plist==null)
        {
            util._error("SendMsgの引数が正しくありません");
        }
        go.SendMessage("SendMessageSocket",plist,SendMessageOptions.DontRequireReceiver);

        return null;
    }

    public static object F_GetObjectAtScreenPoint(bool bHelp, object[] ol, StateBuffer sb)
    {
        if (bHelp)
        {
            return "スクリーンポジションの位置にあるオブジェクトを返す"+NL+
                   "フォーマット) GetObjectAtScreenPoint(Vector3[,Camera])" +NL+
                   "例)　var go = GetObjectAtScreenPoint(new Vector3(100,100,0));";
        }

        Vector3 pos=Vector3.zero;
        Camera cam = null;
        if (ol.Length>=1 && ol[0] is Vector3)
        {
            pos = (Vector3)ol[0];
            cam = Camera.main;
        }
        if (ol.Length>=2 && ol[1] is Camera)
        {
            cam = (Camera)ol[1];
        }
        if (cam==null)
        {
            util._error("GetObjectAtScreenPoint:引数が正しくありません");
        }
        
        var ray = cam.ScreenPointToRay(pos);
        RaycastHit hit;
        if (Physics.Raycast(ray,out hit))
        {
            if (hit.collider!=null)
            {
                return hit.collider.gameObject;
            }
        }

        return null;
    }

    public static object F_FromHexColor(bool bHelp, object[] ol, StateBuffer sb)
    {
        if (bHelp)
        {
            return "16進カラー文字列をColorへ変換する" +NL +
                   "フォーマット) var col = FromHexColor(\"16進６桁または８桁\");" + NL +
                   "例）var col = FromColor(\"ffc0cb\"); //ピンク" ;
        }

        kit.check_num_of_args(ol,1);

        var s = kit.get_string_at(ol,0);

        if (s==null || (s.Length!=6 && s.Length!=8)) util._error("FromHexColor:６桁または８桁の１６進文字列を指定してください");
        var v = s.ToUpper().ToCharArray();
        if (!Array.TrueForAll(v,n=>((n>='0' && n<='9')||(n>='A' && n<='F'))))
        {
            util._error("FromHexColor:１６進文字を指定してください");
        }

        byte r,g,b,a;

        r = (byte)Convert.ToInt32(s.Substring(0,2),16);
        g = (byte)Convert.ToInt32(s.Substring(2,2),16);
        b = (byte)Convert.ToInt32(s.Substring(4,2),16);
        a = (s.Length==8) ? (byte)Convert.ToInt32(s.Substring(5,2),16) : (byte)255;
                    
        return (Color)(new Color32(r,g,b,a));
    }

}
