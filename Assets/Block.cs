using UnityEngine;
using System.Collections.Generic;

public class Block : MonoBehaviour {

    public List<Triangle> triangles;

    public bool alreadyPushed = false;

	// Use this for initialization
	void Start () {
        triangles = new List<Triangle>();
	    foreach(Triangle t in transform.GetComponentsInChildren<Triangle>())
        {
            triangles.Add(t);
            t.regenerate = true;
            t.parent = gameObject;
        }
	}
	
	// Update is called once per frame
	void Update () {
        alreadyPushed = false;
	}

    public TileState getTrianglesAt(int x, int y)
    {
        TileState tile = new TileState();
        foreach (Triangle t in triangles)
        {
            if (t.x == x && t.y == y)
                if (tile.first == null)
                    tile.first = t;
                else
                    tile.second = t;
        }
        return tile;
    }

    public bool tryPushing(Level.MoveDir dir)
    {
        //check if each triangle can move
        bool stopped = false;
        foreach (Triangle tri in triangles)
        {
            if (!Level.instance.canMove(tri, Level.MoveDir.right))
            {
                tri.shakeTime = Player.MOVESPEED;
                stopped = true;
            }
        }
        if (stopped)
            return false;
        //move
        foreach (Triangle tri in triangles)
        {
            tri.move(tri.x, tri.y);
        }
        return true;
    }

    public void push(Level.MoveDir dir)
    {
        foreach (Triangle tri in triangles)
        {
            switch (dir)
            {
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
    }
}
