using UnityEngine;
using System.Collections.Generic;

public class Level : MonoBehaviour {

    public static Level instance;

    public enum MoveDir : int { up = 0, right = 1, down = 2, left = 3};
    List<Triangle> levelTriangles;

    public struct TileState
    {
        public Triangle first;
        public Triangle second;

        public bool canEnterFrom(MoveDir dir, Triangle.Alignment align)
        {
            if (first == null)
                return true;
            else if (second == null)
            {
                //check if the Triangles could fit together.
                if ((int)first.direction != ((int)align + 2) % 4)
                    return false;
                //check if the triangle is moving in the correct direction to slot-in
                switch(align)
                {
                    case Triangle.Alignment.SW:
                        if (dir == MoveDir.up || dir == MoveDir.right)
                            return true;
                        break;
                    case Triangle.Alignment.SE:
                        if (dir == MoveDir.up || dir == MoveDir.left)
                            return true;
                        break;
                    case Triangle.Alignment.NE:
                        if (dir == MoveDir.down || dir == MoveDir.left)
                            return true;
                        break;
                    case Triangle.Alignment.NW:
                        if (dir == MoveDir.down || dir == MoveDir.right)
                            return true;
                        break;
                }
            }
            return false;
        }

        /// <summary>
        /// true if the second triangle in this tile is blocking exit.
        /// </summary>
        /// <param name="dir"> the direction that the triangle wants to move</param>
        /// <param name="align"> the alignment of the triangle that is moving </param>
        /// <returns></returns>
        public bool canExitTo(MoveDir dir, Triangle.Alignment align)
        {
            if (first == null)
                return true;
            switch(align)
            {
                case Triangle.Alignment.SW:
                    if (dir == MoveDir.down || dir == MoveDir.left)
                        return true;
                    break;
                case Triangle.Alignment.SE:
                    if (dir == MoveDir.down || dir == MoveDir.right)
                        return true;
                    break;
                case Triangle.Alignment.NE:
                    if (dir == MoveDir.up || dir == MoveDir.right)
                        return true;
                    break;
                case Triangle.Alignment.NW:
                    if (dir == MoveDir.up || dir == MoveDir.left)
                        return true;
                    break;
            }
            return false;
        }
    }

	// Use this for initialization
	void Start () {
        instance = this;
        levelTriangles = new List<Triangle>();
	    foreach(Triangle t in GetComponentsInChildren<Triangle>())
        {
            t.regenerate = true;
            levelTriangles.Add(t);
            t.parent = gameObject;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public bool canMove(Triangle tri, MoveDir movingTo)
    {
        TileState enteringTile = new TileState();
        switch (movingTo)
        {
            case MoveDir.up:
                enteringTile = getTrianglesAt(tri.x, tri.y + 1);
                break;
            case MoveDir.down:
                enteringTile = getTrianglesAt(tri.x, tri.y - 1);
                break;
            case MoveDir.left:
                enteringTile = getTrianglesAt(tri.x - 1, tri.y);
                break;
            case MoveDir.right:
                enteringTile = getTrianglesAt(tri.x + 1, tri.y);
                break;
        }
        return (getTrianglesAt(tri.x, tri.y).canExitTo(movingTo, tri.direction) &&
            enteringTile.canEnterFrom(movingTo, tri.direction));
    }

    MoveDir oppositeDir(MoveDir dir)
    {
        return (MoveDir)(((int)dir + 2) % 4);
    }

    public TileState getTrianglesAt(int x, int y)
    {
        TileState tile = new TileState();
        foreach(Triangle t in levelTriangles)
        {
            if (t.x == x && t.y == y)
                if (tile.first == null)
                    tile.first = t;
                else
                    tile.second = t;
        }
        return tile;
    }
}
