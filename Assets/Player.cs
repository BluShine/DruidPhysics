using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour {

    public static Player instance;
    public List<GameObject> animals;
    public int currentAnimal = 0;
    public int unlockedAnimals = 1;
    GameObject animalObj;
    List<Triangle> playerTriangles;
    public Triangle eye;
    bool tapped = false;
    float moveCooldown = 0;
    public static float MOVESPEED = .3f;
    float shiftTimer = 0;
    bool shiftStarting = false;
    bool shifting = false;

	// Use this for initialization
	void Start () {
        instance = this;
        playerTriangles = new List<Triangle>();
        animalObj = Instantiate(animals[currentAnimal]);
        int xOffset = Mathf.RoundToInt(transform.position.x);
        int yOffset = Mathf.RoundToInt(transform.position.y);
        foreach (Triangle tri in animalObj.GetComponentsInChildren<Triangle>())
        {
            playerTriangles.Add(tri);
            if(tri.name == "eye")
            {
                eye = tri;
            }
            tri.x += xOffset;
            tri.y += yOffset;
            tri.regenerate = true;
            tri.moved = true;
            tri.parent = gameObject;
           
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (moveCooldown == 0 && !shifting && !Goal.instance.won)
            movement();
        moveCooldown = Mathf.Max(0, moveCooldown - Time.deltaTime);

        if(shifting)
        {
            shiftTimer = Mathf.Max(0, shiftTimer - Time.deltaTime);
            if (shiftTimer == 0)
            {
                if (shiftStarting)
                {
                    
                    shiftStarting = false;
                    int x = eye.x;
                    int y = eye.y;
                    Vector3 pos = eye.transform.position;
                    //destroy old animal
                    GameObject.Destroy(eye.transform.parent.gameObject);
                    foreach (Triangle tri in playerTriangles)
                    {
                        GameObject.Destroy(tri.gameObject);
                    }

                    GameObject nAnimal = (GameObject)Instantiate(animals[currentAnimal], pos, Quaternion.Euler(0, 0, 0));
                    playerTriangles = new List<Triangle>();
                    foreach (Triangle tri in nAnimal.transform.GetComponentsInChildren<Triangle>())
                    {
                        playerTriangles.Add(tri);
                        if (tri.name == "eye")
                            eye = tri;
                        tri.move(tri.x + x, tri.y + y);
                    }
                    shiftTimer = MOVESPEED;
                    Level.instance.checkGoal();
                }
                else
                {
                    //check for a bad shift
                    bool badShift = false;
                    foreach(Triangle tri in playerTriangles)
                    {
                        if (!Level.instance.openTriangle(tri.x, tri.y, tri.direction))
                            badShift = true;
                    }
                    if(badShift)
                    {
                        shapeshift();
                    }
                    else
                        shifting = false;
                }
            }
        }
	}

    public TileState getTile(int xPos, int yPos)
    {
        TileState tile = new TileState();
        foreach (Triangle t in playerTriangles)
        {
            if (t.x == xPos && t.y == yPos)
                if (tile.first == null)
                    tile.first = t;
                else
                    tile.second = t;
        }
        return tile;
    }

    private void shapeshift()
    {
        shifting = true;
        shiftTimer = MOVESPEED;
        shiftStarting = true;
        currentAnimal = (currentAnimal + 1) % unlockedAnimals;
        foreach(Triangle tri in playerTriangles)
        {
            tri.move(eye.x, eye.y);
        }
    }

    private void movement()
    {
        bool move = false;
        Level.MoveDir dir = Level.MoveDir.up;
        if (Input.GetAxis("Vertical") > 0)
        {
            move = true;
            dir = Level.MoveDir.up;
        }
        else if (Input.GetAxis("Vertical") < 0)
        {
            move = true;
            dir = Level.MoveDir.down;
        }
        else if (Input.GetAxis("Horizontal") > 0)
        {
            move = true;
            dir = Level.MoveDir.right;
        }
        else if (Input.GetAxis("Horizontal") < 0)
        {
            move = true;
            dir = Level.MoveDir.left;
        } else if (Input.GetButton("Fire3"))
        {
            shapeshift();
            return;
        }

        if (move)
        {
            moveCooldown = MOVESPEED;
            Level.instance.blocksToPush = new List<Block>();
            //check if each triangle can move
            bool stopped = false;
            foreach (Triangle tri in playerTriangles)
            {
                if (!Level.instance.canMove(tri, dir))
                {
                    tri.shakeTime = MOVESPEED;
                    stopped = true;
                }
            }
            if (stopped)
                return;
            //move our triangles
            foreach (Triangle tri in playerTriangles)
            {
                switch (dir) {
                    case Level.MoveDir.up:
                        tri.move(tri.x, tri.y + 1);
                        break;
                    case Level.MoveDir.down:
                        tri.move(tri.x, tri.y - 1);
                        break;
                    case Level.MoveDir.right:
                        tri.move(tri.x + 1, tri.y);
                        break;
                    case Level.MoveDir.left:
                        tri.move(tri.x - 1, tri.y);
                        break;
                }
            }

            //move the blocks that we pushed
            foreach(Block b in Level.instance.blocksToPush)
            {
                b.push(dir);
            }
            
            foreach(Block b in Level.instance.blocks)
            {
                b.alreadyPushed = false;
            }

            Level.instance.checkGoal();
        }
    }
}
