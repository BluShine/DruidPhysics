using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour {
    
    public static Goal instance;
    public List<Triangle> triangles;
    static float LEVELENDANIM = 3f;
    float endingTimer = 0f;
    float implosionTime = 0;

    public bool won = false;

    public string nextScene;

	// Use this for initialization
	void Start () {
        instance = this;
        triangles = new List<Triangle>();
        foreach(Triangle t in transform.GetComponentsInChildren<Triangle>())
        {
            triangles.Add(t);
        }
	}

    public void win()
    {
        won = true;
        endingTimer = LEVELENDANIM;
    }
	
	// Update is called once per frame
	void Update () {
	    if(won)
        {
            endingTimer -= Time.deltaTime;
            if(endingTimer <= 0)
            {
                SceneManager.LoadScene(nextScene);
            }
            implosionTime += Time.deltaTime;
            if(implosionTime > .05f)
            {
                implosionTime -= .05f;
                Triangle tri = Level.instance.levelTriangles[Mathf.FloorToInt(Random.value * (Level.instance.levelTriangles.Count - 1))];
                tri.move(tri.x + Mathf.FloorToInt(Random.value * 6 - 3), tri.y + Mathf.FloorToInt(Random.value * 6 - 3));
            }
        }
	}
}
