using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class Level : MonoBehaviour {

    public static Level instance;

    public GameObject trianglePrefab;

    public Gradient brushColors;

    public enum MoveDir : int { up = 0, right = 1, down = 2, left = 3};
    public List<Triangle> levelTriangles;

    public List<Block> blocks;

    public List<Block> blocksToPush;

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

        foreach(Block b in FindObjectsOfType<Block>())
        {
            blocks.Add(b);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public static Vector2 dirToVector(MoveDir dir)
    {
        switch(dir)
        {
            case MoveDir.up:
                return new Vector2(0, 1);
            case MoveDir.down:
                return new Vector2(0, -1);
            case MoveDir.left:
                return new Vector2(-1, 0);
            case MoveDir.right:
                return new Vector2(1, 0);
        }
        return Vector2.zero;
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
        return getTrianglesAt(tri.x, tri.y).canExitTo(movingTo, tri.direction) &&
            enteringTile.canEnterFrom(movingTo, tri.direction) && pushBlocks(tri, movingTo);
    }

    public bool pushBlocks(Triangle tri, MoveDir moveDirect)
    {
        foreach(Block b in blocks)
        {
            //make sure that we don't try to collide with ourself or a block that we're already pushing.
            //and make sure that we don't push a block that we're already pushing.
            if (tri.parent != b.gameObject && !b.alreadyPushed) 
            {
                //pick the tri that we're going to collide with
                TileState enteringTile = new TileState();
                switch (moveDirect)
                {
                    case MoveDir.up:
                        enteringTile = b.getTrianglesAt(tri.x, tri.y + 1);
                        break;
                    case MoveDir.down:
                        enteringTile = b.getTrianglesAt(tri.x, tri.y - 1);
                        break;
                    case MoveDir.left:
                        enteringTile = b.getTrianglesAt(tri.x - 1, tri.y);
                        break;
                    case MoveDir.right:
                        enteringTile = b.getTrianglesAt(tri.x + 1, tri.y);
                        break;
                }
                //test collision with the block
                if (!(b.getTrianglesAt(tri.x, tri.y).canExitTo(moveDirect, tri.direction) &&
                    enteringTile.canEnterFrom(moveDirect, tri.direction)))
                {
                    //we collided, but let's see if we can push the block
                    b.alreadyPushed = true;
                    if (b.tryPushing(moveDirect))
                    {
                        blocksToPush.Add(b);
                    } else
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    public bool openTriangle(int x, int y, Triangle.Alignment align)
    {
        TileState ts = getTrianglesAt(x, y);
        TileState ts2 = getBlockTrianglesAt(x, y);
        if (ts.first != null && ((int)ts.first.direction + 2) % 4 != (int)align)
            return false;
        if (ts.second != null && ((int)ts.second.direction + 2) % 4 != (int)align)
            return false;
        if (ts2.first != null && ((int)ts2.first.direction + 2) % 4 != (int)align)
            return false;
        if (ts2.second != null && ((int)ts2.second.direction + 2) % 4 != (int)align)
            return false;

        return true;
    }

    public void checkGoal()
    {
        bool goalReached = true;
        foreach(Triangle t in Goal.instance.triangles)
        {
            TileState tile = getBlockAndPlayerTrianglesAt(t.x, t.y);
            if(tile.second != null)
            {
                tile.first.Pulsing = true;
                tile.second.Pulsing = true;
            } else if (tile.first != null && tile.first.direction == t.direction)
            {
                tile.first.Pulsing = true;
            } else
            {
                goalReached = false;
            }
        }
        if (goalReached)
            Goal.instance.win();
    }

    public TileState getBlockTrianglesAt(int x, int y)
    {
        List<Triangle> gotTris = new List<Triangle>();
        foreach (Block b in blocks)
        {
            TileState t = b.getTrianglesAt(x, y);
            if (t.first != null)
                gotTris.Add(t.first);
            if (t.second != null)
                gotTris.Add(t.second);
        }

        TileState rTile = new TileState();
        switch (gotTris.Count)
        {
            default:
                break;
            case 1:
                rTile.first = gotTris[0];
                break;
            case 2:
                rTile.first = gotTris[0];
                rTile.second = gotTris[1];
                break;
        }
        return rTile;
    }

    public TileState getBlockAndPlayerTrianglesAt(int x, int y)
    {
        List<Triangle> gotTris = new List<Triangle>();
        foreach(Block b in blocks)
        {
            TileState t = b.getTrianglesAt(x, y);
            if (t.first != null)
                gotTris.Add(t.first);
            if (t.second != null)
                gotTris.Add(t.second);
        }
        TileState tp = Player.instance.getTile(x, y);
        if (tp.first != null)
            gotTris.Add(tp.first);
        if (tp.second != null)
            gotTris.Add(tp.second);

        TileState rTile = new TileState();
        switch (gotTris.Count)
        {
            default:
                break;
            case 1:
                rTile.first = gotTris[0];
                break;
            case 2:
                rTile.first = gotTris[0];
                rTile.second = gotTris[1];
                break;
        }
        return rTile;
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

    public void overWriteTrianglesAt(Triangle tri)
    {
        foreach (Triangle t in GetComponentsInChildren<Triangle>())
        {
            if (t != tri && t.x == tri.x && t.y == tri.y &&
                (int)tri.direction % 2 != (int)t.direction % 2)
            {
                Undo.DestroyObjectImmediate(t.gameObject);
            }
        }
    }

    public void paintTrianglesAt(int x, int y)
    {
        foreach (Triangle t in GetComponentsInChildren<Triangle>())
        {
            if (t.x == x && t.y == y)
            {
                Undo.RecordObject(t, "changed triangle color");
                t.color = brushColors.Evaluate(Random.value);
                t.regenerate = true;
            }
        }
    }

    public void removeTrianglesAt(int x, int y)
    {
        foreach (Triangle t in GetComponentsInChildren<Triangle>())
        {
            if (t.x == x && t.y == y)
            {
                Undo.DestroyObjectImmediate(t.gameObject);
            }
        }
    }
}

[CustomEditor(typeof(Level))]
public class LevelEditor : Editor
{
    void OnEnable()
    {

    } 

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        {

        }
    }

    void OnSceneGUI()
    {
        Event current = Event.current;
        if(current.type == EventType.KeyDown)
        {
            Triangle.Alignment align = Triangle.Alignment.SW;
            switch(current.keyCode)
            {
                case KeyCode.A:
                    PlaceTriangle(Triangle.Alignment.SW);
                    break;
                case KeyCode.S:
                    PlaceTriangle(Triangle.Alignment.SE);
                    break;
                case KeyCode.W:
                    PlaceTriangle(Triangle.Alignment.NE);
                    break;
                case KeyCode.Q:
                    PlaceTriangle(Triangle.Alignment.NW);
                    break;
                case KeyCode.P:
                    PaintTriangle();
                    break;
                case KeyCode.X:
                    RemoveTriangle();
                    break;
                default:
                    return;
                    break;
            }

            
            current.Use();
        }
    }

    void PaintTriangle()
    {
        Vector3 pos = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
        Level l = (Level)serializedObject.targetObject;
        l.paintTrianglesAt(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
    }

    void RemoveTriangle()
    {
        Vector3 pos = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
        Level l = (Level)serializedObject.targetObject;
        l.removeTrianglesAt(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
    }

    void PlaceTriangle(Triangle.Alignment align)
    {
        GameObject prefab = (GameObject)serializedObject.FindProperty("trianglePrefab").objectReferenceValue;
        Vector3 pos = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
        Triangle tri = GameObject.Instantiate(prefab).GetComponent<Triangle>();
        Level l = (Level)serializedObject.targetObject;
        tri.transform.parent = l.gameObject.transform;
        tri.x = Mathf.RoundToInt(pos.x);
        tri.y = Mathf.RoundToInt(pos.y);
        tri.color = l.brushColors.Evaluate(Random.value);
        tri.direction = align;
        tri.regenerate = true;
        tri.moved = true;
        tri.name = "triangle";
        Undo.RegisterCreatedObjectUndo(tri.gameObject, "placed triangle");
        l.overWriteTrianglesAt(tri);
    }
}
