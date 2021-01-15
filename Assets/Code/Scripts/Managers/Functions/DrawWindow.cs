using Net3dBool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class DrawWindow : MonoBehaviour
{
    public Material material;

    DomaManager domaManager;
    GameObject area3D;
    GameObject area2D;

    GameObject cube;

    FloorDoma activeFloorDoma;

    bool timedOut;
    CancellationToken ct;

    // Start is called before the first frame update
    void Start()
    {
        domaManager = DomaManager.Instance;
        activeFloorDoma = domaManager.ActiveDomaFloor;

        area3D = GameObject.Find("3DArea");
        area2D = GameObject.Find("2DArea");

        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = "HoleCube";
        cube.GetComponent<Renderer>().material.color = Color.red;
        cube.transform.localScale = new Vector3(1f, 0.2f, 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        if (domaManager.HoleInWall2D != null)
        {
            if (!cube.activeSelf) cube.SetActive(true);

            cube.transform.position = domaManager.HoleInWall2D.mousePosition;

            Vector3 startPoint = domaManager.HoleInWall2D.Wall2D.StartPoint;
            Vector3 endPoint = domaManager.HoleInWall2D.Wall2D.EndPoint;
            Vector3 v = endPoint - startPoint;
            Quaternion rotation = Quaternion.LookRotation(v, Vector3.forward);
            rotation *= Quaternion.Euler(0, 90, 0);
            cube.transform.rotation = rotation;
        }
        else
        {
            cube.SetActive(false);
        }

        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(PerformTimedAction(MyEndlessTask));

            Vector3? position = domaManager.HoleInWall2D?.mousePosition;

            if (position != null)
            {
                Vector3 startPoint = domaManager.HoleInWall2D.Wall2D.StartPoint;
                Vector3 endPoint = domaManager.HoleInWall2D.Wall2D.EndPoint;
                Vector3 v = endPoint - startPoint;

                // Rotation for 2D
                Quaternion rotation = Quaternion.LookRotation(v, Vector3.forward);
                rotation *= Quaternion.Euler(0, 90, 0);

                var obj2D = DrawHole2D(position.Value, rotation, 2f, 0.2f, 0.2f);
                obj2D.transform.parent = domaManager.HoleInWall2D.Wall2D.Wal2DInstance.transform;


                // Rotation for 3D
                startPoint = domaManager.HoleInWall2D.Wall2D.StartPoint;
                endPoint = domaManager.HoleInWall2D.Wall2D.EndPoint;
                v = endPoint - startPoint;
                rotation = Quaternion.LookRotation(v, Vector3.up);
                rotation *= Quaternion.Euler(0, 90, 0);

                var cubeHoleTuple = DrawHole3D(new Vector3(position.Value.x, 4f, position.Value.y), rotation, 2f, 1.5f);
                var cubeHoleObject = cubeHoleTuple.Item1;
                var cubeHoleSolid = cubeHoleTuple.Item2;

                GameObject objectWall = domaManager.HoleInWall2D.Wall3D.Wall3DInstance;
                var objectWallSolid = SolidFromGameObject(objectWall);

                GameObject newGameObject = new GameObject();
                newGameObject.name = "OOOOOOOO";
                newGameObject.transform.Translate(new Vector3(2000, 0, 0), Space.World);
                MeshFilter mf = newGameObject.AddComponent<MeshFilter>();
                Mesh tmesh = new Mesh();

                var modeller = new Net3dBool.BooleanModeller(objectWallSolid, cubeHoleSolid);

                Solid tmp = null;
                tmp = modeller.getDifference();

                tmesh.vertices = GetVertices(tmp);
                tmesh.triangles = tmp.getIndices();
                tmesh.colors = GetColorsMesh(tmp);
                tmesh.RecalculateNormals();
                mf.mesh = tmesh;

                mf.mesh.RecalculateTangents(0);
                mf.mesh.RecalculateNormals();
                mf.mesh.RecalculateBounds();
                mf.mesh.OptimizeReorderVertexBuffer();
                mf.mesh.OptimizeIndexBuffers();
                mf.mesh.Optimize();

                MeshRenderer mr = newGameObject.AddComponent<MeshRenderer>();

                DestroyImmediate(domaManager.HoleInWall2D.Wall3D.Wall3DInstance);
                DestroyImmediate(cubeHoleObject);

                domaManager.HoleInWall2D.Wall3D.Wall3DInstance = newGameObject;
                domaManager.HoleInWall2D.Wall3D.Wall3DSolidInstance = tmp;

                newGameObject.GetComponent<MeshRenderer>().material = material;

                newGameObject.AddComponent<MeshCollider>();
                newGameObject.tag = "Wall";

                newGameObject.transform.parent = area3D.transform;
            }
        }
    }

    private void MyEndlessTask()
    {
        while (true)
        {
            ct.ThrowIfCancellationRequested();
            print("Running");
        }
    }

    private IEnumerator PerformTimedAction(Action action, int timeout = 5)
    {
        CancellationTokenSource cts = new CancellationTokenSource();
        ct = cts.Token;

        Coroutine timeoutCoroutine = StartCoroutine(TimeoutChecker(timeout));
        var t = Task.Run(action, ct);
        yield return new WaitWhile(() => t.Status != TaskStatus.RanToCompletion && !timedOut);

        if (timedOut)
        {
            cts.Cancel();
            print("Task timed out");
        }
        else
        {
            StopCoroutine(timeoutCoroutine);
            print("Task successfully completed");
        }
    }

    private IEnumerator TimeoutChecker(float timeout)
    {
        timedOut = false;
        while (timeout > 0)
        {
            timeout -= Time.deltaTime;
            yield return null;
        }
        timedOut = true;
    }

    Solid SolidFromGameObject(GameObject gameObject)
    {
        Mesh objectWallMesh = gameObject.GetComponent<MeshFilter>().mesh;
        int[] triangles = objectWallMesh.triangles;
        Point3d[] vertices = Vector3ToPoint3d(objectWallMesh.vertices.ToList()).ToArray();
        int length = objectWallMesh.vertexCount;

        return new Net3dBool.Solid(vertices, triangles, getColorArray(length, Color.black));
    }

    List<Point3d> Vector3ToPoint3d(List<Vector3> vectors)
    {
        var points3d = new List<Point3d>();

        foreach (var item in vectors)
        {
            points3d.Add(new Point3d(item.x, item.y, item.z));
        }

        return points3d;
    }


    GameObject DrawHole2D(Vector3 position, Quaternion rotation, float width, float height, float wWall)
    {
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = "HoleWindowCube";
        cube.GetComponent<Renderer>().material.color = Color.black;
        cube.transform.localScale = new Vector3(width, height, wWall);
        cube.transform.position = position;
        cube.transform.rotation = rotation;

        cube.tag = domaManager.ActiveDomaFloor.Tag;

        return cube;
    }

    Tuple<GameObject, Solid> DrawHole3D(Vector3 pos, Quaternion rotation, float width, float height)
    {
        activeFloorDoma = domaManager.ActiveDomaFloor;

        pos = new Vector3(pos.x, activeFloorDoma.GetLevelTop() -(height / 2) - 0.3f, pos.z);

        GameObject gameObject = new GameObject();
        gameObject.name = "Hole";

        float w = width;
        float h = height;
        float g = 1;

        Vector3[] vertices = {
            new Vector3 (pos.x - w/2, pos.y - h /2, pos.z + g / 2),
            new Vector3 (pos.x - w/2, pos.y - h /2, pos.z - g / 2),
            new Vector3 (pos.x - w/2, pos.y + h /2, pos.z - g / 2),
            new Vector3 (pos.x - w/2, pos.y + h /2, pos.z + g / 2),
            new Vector3 (pos.x + w/2, pos.y + h /2, pos.z + g / 2),
            new Vector3 (pos.x + w/2, pos.y + h /2, pos.z - g / 2),
            new Vector3 (pos.x + w/2, pos.y - h /2, pos.z - g / 2),
            new Vector3 (pos.x + w/2, pos.y - h /2, pos.z + g / 2),
        };

        Point3d[] points3d = vertices.Select(v => new Point3d(v.x, v.y, v.z)).ToArray();

        int[] triangles = {
            0, 2, 1, //face front
			0, 3, 2,
            2, 3, 4, //face top
			2, 4, 5,
            1, 2, 5, //face right
			1, 5, 6,
            0, 7, 4, //face left
			0, 4, 3,
            5, 4, 7, //face back
			5, 7, 6,
            0, 6, 7, //face bottom
			0, 1, 6
        };

        var obj = new Net3dBool.Solid(points3d, triangles, getColorArray(8, Color.black));

        MeshFilter mf = gameObject.AddComponent<MeshFilter>();
        Mesh tmesh = new Mesh();

        tmesh.vertices = GetVertices(obj);
        tmesh.triangles = obj.getIndices();
        tmesh.colors = GetColorsMesh(obj);
        tmesh.RecalculateNormals();
        mf.mesh = tmesh;
        MeshRenderer mr = gameObject.AddComponent<MeshRenderer>();

        gameObject.transform.parent = area3D.transform;
        gameObject.tag = domaManager.ActiveDomaFloor.Tag;

        gameObject.AddComponent<MeshCollider>();

        return new Tuple<GameObject, Solid>(gameObject, obj);
    }

    Vector3[] GetVertices(Solid mesh)
    {
        int mlen = mesh.getVertices().Length;
        Vector3[] vertices = new Vector3[mlen];
        for (int i = 0; i < mlen; i++)
        {
            Net3dBool.Point3d p = mesh.getVertices()[i];
            vertices[i] = new Vector3((float)p.x, (float)p.y, (float)p.z);
        }

        return vertices;
    }

    Color[] GetColorsMesh(Solid mesh)
    {
        int clen = mesh.getColors().Length;
        Color[] clrs = new Color[clen];
        for (int j = 0; j < clen; j++)
        {
            Net3dBool.Color3f c = mesh.getColors()[j];
            clrs[j] = new Color((float)c.r, (float)c.g, (float)c.b);
        }
        return clrs;
    }

    public Net3dBool.Color3f[] getColorArray(int length, Color c)
    {
        var ar = new Net3dBool.Color3f[length];
        for (var i = 0; i < length; i++)
            ar[i] = new Net3dBool.Color3f(c.r, c.g, c.b);
        return ar;
    }
}
