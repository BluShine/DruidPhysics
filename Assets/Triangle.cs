using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Triangle : MonoBehaviour {

    public enum Alignment : int { SW = 0, SE = 1, NE = 2, NW =3 };
    public Alignment direction = 0;
    public Color color = Color.gray;
    public int x = 0;
    public int y = 0;
    static float MOVESPEED = .2f;
    static float MOVEDELAY = .1f;
    float moveTime = 0;
    public float shakeTime = 0;
    Vector3 prevPos = Vector3.zero;
    float prevRotation = 0;
    float targetRotation = 0;
    Vector2 lastShake = Vector2.zero;
    Vector2 nextShake = Vector2.zero;
    float shakeyShake = 0;
    static float SHAKEYNESSSPEED = .05f;
    private bool pulsing = false;
    static float PULSESPEED = .5f;
    float pulseTime = 0;

    public bool regenerate = false;
    public bool moved = false;
    public float z = 0;

    public GameObject parent;

    public bool Pulsing
    {
        get
        {
            return pulsing;
        }

        set
        {
            pulsing = value;
            if (!pulsing)
                transform.localScale = Vector3.one;
        }
    }

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
            if(moveTime == 0)
                moveTime = MOVESPEED;
            prevPos = transform.position;
            prevRotation = transform.rotation.eulerAngles.z;
            moved = !moved;
            switch(direction)
            {
                case Alignment.SW:
                    targetRotation = 0;
                    break;
                case Alignment.SE:
                    targetRotation = 90f;
                    break;
                case Alignment.NE:
                    targetRotation = 180f;
                    break;
                case Alignment.NW:
                    targetRotation = 270f;
                    break;
            }
        }
        if(moveTime > 0)
        {
            moveTime = Mathf.Max(0, moveTime - Time.deltaTime);
            if (Application.isEditor && !Application.isPlaying)
                moveTime = 0;
            transform.position = Vector3.Lerp(new Vector3(x, y, z), prevPos, moveTime / MOVESPEED);
            transform.rotation = Quaternion.Lerp(Quaternion.Euler(0, 0, targetRotation), 
                Quaternion.Euler(0, 0, prevRotation), moveTime / MOVESPEED);
        }
        if(shakeTime > 0)
        {
            shakeTime = Mathf.Max(0, shakeTime - Time.deltaTime);
            shakeyShake -= Time.deltaTime;
            if (shakeyShake < 0)
            {
                shakeyShake += SHAKEYNESSSPEED;
                lastShake = nextShake;
                if (shakeTime < SHAKEYNESSSPEED * 2)
                {
                    nextShake = Vector2.zero;
                }
                else
                {
                    nextShake = new Vector2(Random.value * .4f - .2f, Random.value * .4f - .2f);
                }
            }
            Vector2 shakeVector = Vector2.Lerp(nextShake, lastShake, shakeyShake / SHAKEYNESSSPEED);
            transform.position = new Vector3(x + shakeVector.x, y + shakeVector.y, z);
        }
        if (pulsing)
        {
            pulseTime += Time.deltaTime;
            if (pulseTime > PULSESPEED * 2)
                pulseTime -= PULSESPEED * 2;
            transform.localScale = Vector3.Lerp(Vector3.one, new Vector3(.8f, .8f, .8f), 
                Mathf.Abs(pulseTime - PULSESPEED)/PULSESPEED);
        }
	}

    public void move(int xTarget, int yTarget)
    {
        x = xTarget;
        y = yTarget;
        moved = true;
        moveTime = MOVESPEED + Random.value * MOVEDELAY;
        Pulsing = false;
    }

    void BuildTriangle()
    {
        MeshFilter mFilter = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();

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

        mFilter.mesh = mesh;
    }
}
