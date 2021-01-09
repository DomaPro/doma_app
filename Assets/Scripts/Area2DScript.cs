using Draw2DShapesLite;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Area2DScript : MonoBehaviour
{
    public Vector3 worldPosition;
    public GameObject plane2D;

    public Camera mainCamera;
    public Camera image2DCamera;

    public RawImage box2DView;

    private Vector3 startMousePosition;
    private Vector3 currentMousePosition;

    GameObject temp;
    
    void Start()
    {
        
    }

    void Update()
    {
        //Vector3 pos = box2DView.transform.position;
        //Vector3 viewportPoint = mainCamera.WorldToViewportPoint(pos);

        //Vector3 test = image2DCamera.ScreenToWorldPoint(Input.mousePosition);

        //Debug.Log("viewportPoint : " + viewportPoint);
        //Debug.Log("test : " + test);


        //var mp = Input.mousePosition;
        //var vec = mainCamera.ScreenToWorldPoint(mp);

        //Debug.Log("MousePosition MainCamera : " + mainCamera.ScreenToWorldPoint(mp) + " Image2DCamera : " + image2DCamera.ScreenToWorldPoint(mp));


        //Plane plane = new Plane(new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(0, 0, 1));
        //float distance;

        //var mp = Input.mousePosition;
        //var mp2 = new Vector3(mp.x, mp.y, 100);
        //Ray ray = image2DCamera.ScreenPointToRay(mp2);
        //if (plane.Raycast(ray, out distance))
        //{
        //    worldPosition = ray.GetPoint(distance);
        //}

        //Debug.Log("mp : " + mp2);
        //Debug.Log("worldPosition : " + worldPosition);

        //Debug.DrawRay(mp, new Vector3(0, -1000, 0), Color.green);

        //if (Input.GetMouseButtonDown(0))
        //{
        //    //Plane plane = new Plane(new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(0, 0, 1));
        //    Plane plane = new Plane(Vector3.up, Vector3.zero);

        //    float distance;

        //    var mp = Input.mousePosition;
        //    Ray ray = camera.ScreenPointToRay(mp);
        //    if (plane.Raycast(ray, out distance))
        //    {
        //        Debug.Log("DISTANCE : " + distance);
        //        Debug.Log("MOUSE POSITION : " + mp);
        //        worldPosition = ray.GetPoint(distance);
        //    }

        //    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    cube.transform.position = worldPosition;

        //    Debug.Log("WorldPosition : " + worldPosition);
        //}



        //if (Input.GetMouseButtonDown(0))
        //{
        //    var mp = Input.mousePosition;
        //    startMousePosition = new Vector3(mp.x, 0, mp.y);
        //}
        //else if (Input.GetMouseButtonUp(0))
        //{
        //    var mp = Input.mousePosition;
        //    currentMousePosition = new Vector3(mp.x, 0, mp.y);

        //    DrawWall();
        //}
    }

    public void DrawWall()
    {
        GameObject wall = new GameObject();
        wall.name = "Wall";
        wall.transform.parent = gameObject.transform;

        wall.transform.localPosition = new Vector3(0, 0.1f, 0);

        MeshFilter meshFilter = wall.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = wall.AddComponent<MeshRenderer>();
        //Color FillColor = Color.red;

        List<Vector3> vertices = new List<Vector3>();

        var localStartMousePosition = startMousePosition + gameObject.transform.position;
        var localCurrentMousePosition = currentMousePosition + gameObject.transform.position;
        //Debug.Log("startMousePosition : " + startMousePosition + " | localStartMousePosition: " + localStartMousePosition);
        //Debug.Log("currentMousePosition : " + currentMousePosition + " | localCurrentMousePosition: " + localCurrentMousePosition);

        vertices.Add(new Vector3(localStartMousePosition.x, 0, localStartMousePosition.z - 0.2f));
        vertices.Add(new Vector3(localStartMousePosition.x, 0, localStartMousePosition.z + 0.2f));
        vertices.Add(new Vector3(localCurrentMousePosition.x, 0, localCurrentMousePosition.z - 0.2f));
        vertices.Add(new Vector3(localCurrentMousePosition.x, 0, localCurrentMousePosition.z + 0.2f));

        Vector2[] vertices2D = new Vector2[vertices.Count];
        for (int i = 0; i < vertices.Count; i++)
        {
            vertices2D[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

        var triangles = new Triangulator(vertices2D).Triangulate();

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles;

        meshFilter.mesh = mesh;
    }
}