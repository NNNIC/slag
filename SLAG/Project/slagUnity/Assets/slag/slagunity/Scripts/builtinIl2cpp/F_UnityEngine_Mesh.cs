using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F_UnityEngine_Mesh  {

    //コンストラクタ
    public static Mesh __NEW__()
    {
        return new Mesh();
    }    

    //メンバ
    public Vector3[] __GET__vertices(object o)           { return ((Mesh)o).vertices;         }
    public void      __SET__vertices(object o, object v) { ((Mesh)o).vertices = (Vector3[])v; }

    public int[]     __GET__triangles(object o)           { return ((Mesh)o).triangles;       }
    public void      __SET__triangles(object o, object v) { ((Mesh)o).triangles = (int[])v;   }

    public Vector2[] __GET__uv(object o)                  { return ((Mesh)o).uv;              }
    public void      __SET__uv(object o, object v)        { ((Mesh)o).uv = (Vector2[])v;      }

    public Vector3[] __GET__normals(object o)             { return ((Mesh)o).normals;         }
    public void      __SET__normals(object o, object v)   { ((Mesh)o).normals = (Vector3[])v; }

}
