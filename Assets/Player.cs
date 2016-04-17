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
            tri.parent = gameObject;
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
            //check if each triangle can move
            bool stopped = false;
            foreach (Triangle tri in playerTriangles)
            {
                if (!Level.instance.canMove(tri, Level.MoveDir.up))
                {
                    tri.shakeTime = MOVESPEED;
                    stopped = true; ;
                }
            }
            if (stopped)
                return;
            //move
            foreach (Triangle tri in playerTriangles)
            {
                tri.move(tri.x, tri.y + 1);
            }
        }
        else if (Input.GetAxis("Vertical") < 0)
        {
            moveCooldown = MOVESPEED;
            //check if each triangle can move
            bool stopped = false;
            foreach (Triangle tri in playerTriangles)
            {
                if (!Level.instance.canMove(tri, Level.MoveDir.down))
                {
                    tri.shakeTime = MOVESPEED;
                    stopped = true; ;
                }
            }
            if (stopped)
                return;
            //move
            foreach (Triangle tri in playerTriangles)
            {
                tri.move(tri.x, tri.y - 1);
            }
        }
        else if (Input.GetAxis("Horizontal") > 0)
        {
            moveCooldown = MOVESPEED;
            //check if each triangle can move
            bool stopped = false;
            foreach (Triangle tri in playerTriangles)
            {
                if (!Level.instance.canMove(tri, Level.MoveDir.right))
                {
                    tri.shakeTime = MOVESPEED;
                    stopped = true; ;
                }
            }
            if (stopped)
                return;
            //move
            foreach (Triangle tri in playerTriangles)
            {
                tri.move(tri.x + 1, tri.y);
            }
        }
        else if (Input.GetAxis("Horizontal") < 0)
        {
            moveCooldown = MOVESPEED;
            //check if each triangle can move
            bool stopped = false;
            foreach (Triangle tri in playerTriangles)
            {
                if (!Level.instance.canMove(tri, Level.MoveDir.left))
                {
                    tri.shakeTime = MOVESPEED;
                    stopped = true; ;
                }
            }
            if (stopped)
                return;
            //move
            foreach (Triangle tri in playerTriangles)
            {
                tri.move(tri.x - 1, tri.y);
            }
        }
    }
}
