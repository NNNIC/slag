using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F_UnityEngine_Material  {
    //コンストラクタ
    public static Material __NEW__(Shader shader)
    {
        return new Material(shader);
    }

    //メソッド
    public void SetTexture(Material m, string n, Texture tex)
    {
        m.SetTexture(n,tex);
    }
}
