using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class slagunity_root : MonoBehaviour {

    public slagtool.slag m_slag {get; private set; }
    public void SetSlag(slagtool.slag slag) { m_slag =slag; }
    //public static slagunity_root V;
    //public static slagtool.slag         SLAG;

	//void Start () {
	//	V = this;
	//}
}
