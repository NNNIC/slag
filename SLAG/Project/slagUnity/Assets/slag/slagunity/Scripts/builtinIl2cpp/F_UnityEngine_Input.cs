using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F_UnityEngine_Input : MonoBehaviour {

    public static bool GetKey(KeyCode key)
    {
        return Input.GetKey(key);
    }
    public static bool GetKeyDown(KeyCode key)
    {
        return Input.GetKeyDown(key);
    }
}
