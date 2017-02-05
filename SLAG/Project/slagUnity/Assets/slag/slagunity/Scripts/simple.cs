using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class simple : MonoBehaviour {

    public string m_folder = @"N:\Project\test";

    public string m_file   = "test01.js";


    void Start()
    {
        Exec();
    }

    [ContextMenu("Execute")]
    public void Exec()
    {
        var su = slagunity.Create(gameObject);
        su.LoadFile(Path.Combine(m_folder,m_file));
        Debug.Log("Checksum:" + su.GetMD5());
        su.Run();

    }
}