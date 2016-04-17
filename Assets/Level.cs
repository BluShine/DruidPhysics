using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class Level : MonoBehaviour {

    public static Level instance;

    public GameObject trianglePrefab;

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
}

[CustomEditor(typeof(Level))]
public class LevelEditor : Editor
{
    Color brushColor = Color.gray;

    void OnEnable()
    {

    } 

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        {
            brushColor = EditorGUILayout.ColorField(brushColor);
            
        }
    }

    void OnSceneGUI()
    {
        Event current = Event.current;
        if(current.type == EventType.KeyDown && current.keyCode == KeyCode.Space)
        {
            GameObject prefab = (GameObject)serializedObject.FindProperty("trianglePrefab").objectReferenceValue;
            Vector3 pos = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
            Triangle tri = GameObject.Instantiate(prefab).GetComponent<Triangle>();
            tri.transform.parent = ((Level)serializedObject.targetObject).gameObject.transform;
            tri.x = Mathf.RoundToInt(pos.x);
            tri.y = Mathf.RoundToInt(pos.y);
            tri.color = brushColor;
            tri.regenerate = true;
            tri.moved = true;
            tri.name = "triangle";
            current.Use();
        }
    }
}
