using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour {

    public List<GameObject> animals;
    public int currentAnimal = 0;
    GameObject animalObj;
    List<Triangle> playerTriangles;
    Triangle eye;
    bool tapped = false;
    float moveCooldown = 0;
    static float MOVESPEED = .3f;

	// Use this for initialization
	void Start () {
        playerTriangles = new List<Triangle>();
        animalObj = Instantiate(animals[0]);
        foreach(Triangle tri in animalObj.GetComponentsInChildren<Triangle>())
        {
            playerTriangles.Add(tri);
            if(tri.name == "eye")
            {
                eye = tri;
            }
            tri.regenerate = true;
            tri.moved = true;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (moveCooldown == 0)
            movement();
        moveCooldown = Mathf.Max(0, moveCooldown - Time.deltaTime);
	}

    private void movement()
    {
        if (Input.GetAxis("Vertical") > 0)
        {
            moveCooldown = MOVESPEED;
            foreach(Triangle tri in playerTriangles)
            {
                tri.move(tri.x, tri.y + 1);
            }
        }
        else if (Input.GetAxis("Vertical") < 0)
        {
            moveCooldown = MOVESPEED;
            foreach (Triangle tri in playerTriangles)
            {
                tri.move(tri.x, tri.y - 1);
            }
        }
        else if (Input.GetAxis("Horizontal") > 0)
        {
            moveCooldown = MOVESPEED;
            foreach (Triangle tri in playerTriangles)
            {
                tri.move(tri.x + 1, tri.y);
            }
        }
        else if (Input.GetAxis("Horizontal") < 0)
        {
            moveCooldown = MOVESPEED;
            foreach (Triangle tri in playerTriangles)
            {
                tri.move(tri.x - 1, tri.y);
            }
        }
    }
}
