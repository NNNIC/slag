using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playslagreset : MonoBehaviour {

	// Use this for initialization
    IEnumerator Start()
    {
        yield return null;
        SceneManager.LoadScene("playslag");
    }

}
