using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class titleScreen : MonoBehaviour {

    public string scene = "level1";

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Fire"))
        {
            SceneManager.LoadScene(scene);
        }
	}
}
