using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Triangle : MonoBehaviour {

    [Range(0, 3)]
    public int direction = 0;
    public Color color = Color.gray;
    public int x = 0;
    public int y = 0;
    static float MOVESPEED = .2f;
    float moveTime = 0;
    Vector3 prevPos = Vector3.zero;
    float prevRotation = 0;
    float targetRotation = 0;

    public bool regenerate = false;
    public bool moved = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        //rebuild triangle
        if (regenerate)
        {
            BuildTriangle();
            regenerate = false;
        }

        if(moved)
        {
            moveTime = MOVESPEED;
            prevPos = transform.position;
            prevRotation = transform.rotation.eulerAngles.z;
            moved = !moved;
            switch(direction)
            {
                case 0:
                    targetRotation = 0;
                    break;
                case 1:
                    targetRotation = 90f;
                    break;
                case 2:
                    targetRotation = 180f;
                    break;
                case 3:
                    targetRotation = 270f;
                    break;
            }
        }
        if(moveTime > 0)
        {
            moveTime = Mathf.Max(0, moveTime - Time.deltaTime);
            transform.position = Vector3.Lerp(new Vector3(x, y), prevPos, moveTime / MOVESPEED);
            transform.rotation = Quaternion.Lerp(Quaternion.Euler(0, 0, targetRotation), 
                Quaternion.Euler(0, 0, prevRotation), moveTime / MOVESPEED);
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
            new Vector2(0, 1),
            new Vector2(1, 0) };
        mesh.colors = new Color[] { color, color, color };
    }
}
