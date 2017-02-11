using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class F_UnityEngine_Transform : MonoBehaviour {

    public static void    __SET__LocalPosition(object t, object pos) { ((Transform)t).localPosition = (Vector3)pos;   }
    public static object  __GET__LocalPosition(object t)             { return  ((Transform)t).localPosition;          }

    public static void    __SET__LocalScale(object t, object scale)  { ((Transform)t).localScale = (Vector3)scale;    }
    public static object  __GET__LocalScale(object t)                { return  ((Transform)t).localScale;             }




}
