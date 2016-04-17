using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class Level : MonoBehaviour {

    public static Level instance;

    public GameObject trianglePrefab;

    public Gradient brushColors;

    public enum MoveDir : int { up = 0, right = 1, down = 2, left = 3};
    List<Triangle> levelTriangles;

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
                (int)tri.direction != (int)(t.direction + 2 % 4))
            {
                DestroyImmediate(t.gameObject);
            }
        }
    }

    public void paintTrianglesAt(int x, int y)
    {
        foreach (Triangle t in GetComponentsInChildren<Triangle>())
        {
            if (t.x == x && t.y == y)
            {
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
                DestroyImmediate(t.gameObject);
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
        l.overWriteTrianglesAt(tri);
    }
}
