using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Triangle : MonoBehaviour {

    [Range(0, 3)]
    public int direction = 0;
    public Color color = Color.gray;

    public bool regenerate = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (regenerate)
        {
            BuildTriangle();
            regenerate = false;
        }
	}

    void BuildTriangle()
    {
        MeshFilter mFilter = GetComponent<MeshFilter>();
        Mesh mesh = mFilter.sharedMesh;

        mesh.vertices = new Vector3[3] {
            new Vector3(-.5f, -.5f),
            new Vector3(-.5f, .5f),
            new Vector3(.5f, -.5f) };
        mesh.triangles = new int[3] { 0, 1, 2 };
        mesh.uv = new Vector2[3] {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1) };
        mesh.colors = new Color[] { color, color, color };
    }
}
