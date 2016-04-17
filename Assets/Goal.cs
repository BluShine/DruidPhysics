using UnityEngine;
using System.Collections.Generic;

public class Goal : MonoBehaviour {
    
    public static Goal instance;
    public List<Triangle> triangles;

	// Use this for initialization
	void Start () {
        instance = this;
        triangles = new List<Triangle>();
        foreach(Triangle t in transform.GetComponentsInChildren<Triangle>())
        {
            triangles.Add(t);
        }
	}
	
	// Update is called once per frame
	void Update () {
	    
	}
}
