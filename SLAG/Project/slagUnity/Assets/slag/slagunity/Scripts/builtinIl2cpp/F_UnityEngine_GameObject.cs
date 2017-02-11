using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class F_UnityEngine_GameObject {

    //メンバー
    public static object  __GET__transform(object o)             { return  ((GameObject)o).transform;     }

    public static object  __GET__name(object o)                  { return  ((GameObject)o).name;          }
    public static void    __SET__name(object o, object v)        { ((GameObject)o).name = (string)v;      }

    //コンストラクタ
    public static GameObject __NEW__()
    {
        return new GameObject();
    }
    public static GameObject __NEW__(string s)
    {
        return new GameObject(s);
    }

    //static メソッド
    public static GameObject CreatePrimitive(PrimitiveType type)
    {
        return GameObject.CreatePrimitive(type);
    }

    //メソッド
    public static object AddComponent(GameObject o, Type type)
    {
        return ((GameObject)o).AddComponent(type);
    }

}
