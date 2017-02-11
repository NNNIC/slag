using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F_UnityEngine_MeshFilter  {

    public Mesh __GET__mesh(object o)           { return ((MeshFilter)o).mesh;    }
    public void __SET__mesh(object o, object v) { ((MeshFilter)o).mesh = (Mesh)v; }


}
