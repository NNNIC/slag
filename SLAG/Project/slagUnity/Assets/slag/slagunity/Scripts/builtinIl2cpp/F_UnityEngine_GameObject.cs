using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F_UnityEngine_GameObject {

    public static object  __GET__transform(object o)             { return  ((GameObject)o).transform;          }


    public static GameObject __NEW__()
    {
        return new GameObject();
    }
    public static GameObject __NEW__(string s)
    {
        return new GameObject(s);
    }

    public static GameObject CreatePrimitive(PrimitiveType type)
    {
        return GameObject.CreatePrimitive(type);
    }
}
