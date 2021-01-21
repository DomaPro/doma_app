using Draw2DShapesLite;
using Net3dBool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DWall : MonoBehaviour
{
    public Guid Id { get; set; }
    public Guid FloorId { get; set; }
    public string Name { get; set; }
    public string Tag { get; set; }
    public Vector3 StartPoint { get; set; }
    public Vector3 EndPoint { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
    public float BottomLevel { get; set; }

    public Material Material { get; set; }

    public GameObject Instance2D { get; set; }
    public GameObject Instance3D { get; set; }

    public int[] Triangles3D { get; set; }
    public Vector3[] Vertices3D { get; set; }

    public DWall()
    {
        Id = Guid.NewGuid();
    }

    public DWall(Guid floorId, Vector3 startPoint, Vector3 endPoint, float width, float height, float bottomLevel, Material material, GameObject instance2D = null, GameObject instance3D = null)
    {
        Id = Guid.NewGuid();
        FloorId = floorId;
        Name = "Wall_" + Id;
        Tag = "Wall";
        StartPoint = startPoint;
        EndPoint = endPoint;
        Width = width;
        Height = height;
        BottomLevel = bottomLevel;
        Material = material;
        Instance2D = instance2D;
        Instance3D = instance3D;

        Instance2D.name = Name;
        Instance3D.name = Name;
    }

    public DWall(SWall sWall)
    {
        Id = sWall.Id;
        FloorId = sWall.FloorId;
        Name = sWall.Name;
        Tag = sWall.Tag;
        StartPoint = new Vector3(sWall.StartPoint[0], sWall.StartPoint[1], sWall.StartPoint[2]);
        EndPoint = new Vector3(sWall.EndPoint[0], sWall.EndPoint[1], sWall.EndPoint[2]);
        Width = sWall.Width;
        Height = sWall.Height;
        BottomLevel = sWall.BottomLevel;
        Material = Resources.Load("Materials/" + sWall.Material, typeof(Material)) as Material;
        Instance2D = null;
        Instance3D = null;
        Triangles3D = sWall.Triangles3D;

        List<Vector3> temp = new List<Vector3>();
        foreach (var v in sWall.Vertices3D)
        {
            temp.Add(new Vector3(v[0], v[1], v[2]));
        }
        Vertices3D = temp.ToArray();
    }

    public GameObject DrawWall2D()
    {
        var startP = new Vector2(StartPoint.x, StartPoint.y);
        var endP = new Vector2(EndPoint.x, EndPoint.y);

        List<Vector2> vertices2D = new List<Vector2>();
        List<Vector3> vertices3D = new List<Vector3>();

        GameObject polygon = new GameObject();
        polygon.name = Name;
        polygon.tag = Tag;

        Vector2 vNormalized = (endP - startP).normalized;
        Vector2 vPerpendicular = new Vector2(vNormalized.y, -vNormalized.x).normalized;

        var P1 = startP + vPerpendicular * Width / 2;
        var P2 = startP - vPerpendicular * Width / 2;
        var P3 = endP - vPerpendicular * Width / 2;
        var P4 = endP + vPerpendicular * Width / 2;

        vertices3D.Add(new Vector3(P1.x, P1.y, 0));
        vertices3D.Add(new Vector3(P2.x, P2.y, 0));
        vertices3D.Add(new Vector3(P3.x, P3.y, 0));
        vertices3D.Add(new Vector3(P4.x, P4.y, 0));

        vertices2D.Add(P1);
        vertices2D.Add(P2);
        vertices2D.Add(P3);
        vertices2D.Add(P4);

        var triangulator = new Triangulator(vertices2D.ToArray());
        var indices = triangulator.Triangulate();

        var mesh = new Mesh
        {
            vertices = vertices3D.ToArray(),
            triangles = indices
        };

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        var mr = polygon.AddComponent<MeshRenderer>();
        mr.material = Material;

        var filter = polygon.AddComponent<MeshFilter>();
        filter.mesh = mesh;

        polygon.transform.parent = GameObject.Find("2DArea").transform;

        var polygonCollider = polygon.AddComponent<PolygonCollider2D>();
        polygonCollider.points = vertices2D.ToArray();

        polygon.AddComponent<WallScript>();

        polygon.layer = LayerMask.NameToLayer("2DArea");

        return polygon;
    }

    public GameObject DrawWall3D()
    {
        GameObject wallobject = new GameObject();
        wallobject.name = Name;
        wallobject.tag = Tag;

        MeshFilter mf = wallobject.AddComponent<MeshFilter>();
        Mesh tmesh = new Mesh();
        tmesh.vertices = Vertices3D;
        tmesh.triangles = Triangles3D;
        tmesh.RecalculateNormals();
        mf.mesh = tmesh;

        var mr = wallobject.AddComponent<MeshRenderer>();
        mr.material = Material;

        wallobject.transform.Translate(new Vector3(2000, 0, 0), Space.World);

        wallobject.transform.parent = GameObject.Find("3DArea").transform;

        // Doda Collider gdy œciana ma d³ugoœæ min. 20 cm
        if (Vector3.Distance(StartPoint, EndPoint) > 0.2f)
        {
            wallobject.AddComponent<MeshCollider>();
        }

        wallobject.layer = LayerMask.NameToLayer("3DArea");

        return wallobject;
    }

    public GameObject DrawWallWithHole(GameObject holeObject)
    {
        GameObject wall = new GameObject();
        wall.name = Name;

        var solidWall = SolidFromGameObject(Instance3D);
        var solidHole = SolidFromGameObject(holeObject);

        // Referencja do starej œciany (aby mo¿na by³o usun¹c obiekt)
        var oldWall = Instance3D;

        //DestroyImmediate(Instance3D);
        //DestroyImmediate(holeObject);

        var modeller = new Net3dBool.BooleanModeller(solidWall, solidHole);

        Solid tmp = modeller.getDifference();

        MeshFilter mf = wall.AddComponent<MeshFilter>();
        Mesh tmesh = new Mesh();
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

        var mr = wall.AddComponent<MeshRenderer>();
        mr.material = Material;

        wall.AddComponent<MeshCollider>();
        wall.tag = "Wall";

        wall.transform.parent = GameObject.Find("3DArea").transform;

        wall.transform.Translate(new Vector3(2000, 0, 0), Space.World);

        Instance3D = wall;

        DestroyImmediate(oldWall);
        DestroyImmediate(holeObject);

        wall.layer = LayerMask.NameToLayer("3DArea");

        return wall;
    }

    public GameObject GableWall3D(float kneeHeight)
    {
        GameObject gableWall = new GameObject();
        gableWall.name = Name;

        var sp = GetPosition3dView(StartPoint, 0f); // StartPoint
        var ep = GetPosition3dView(EndPoint, 0f); // EndPoint

        // Referencja do starej œciany (aby mo¿na by³o usun¹c obiekt)
        var oldWall = Instance3D;

        List<Vector3> points = new List<Vector3>();

        Vector3 centralVector = (sp + ep) / 2;

        // Front
        points.Add(new Vector3(sp.x, BottomLevel, sp.z));
        points.Add(new Vector3(sp.x, BottomLevel + kneeHeight, sp.z));
        points.Add(new Vector3(centralVector.x, BottomLevel + Height, centralVector.z));
        points.Add(new Vector3(ep.x, BottomLevel + kneeHeight, ep.z));
        points.Add(new Vector3(ep.x, BottomLevel, ep.z));

        // Back
        var l = ep - sp;
        var s = Vector3.Normalize(Vector3.Cross(l, Vector3.up));

        points[0] = points[0] - s * Width / 2;
        points[1] = points[1] - s * Width / 2;
        points[2] = points[2] - s * Width / 2;
        points[3] = points[3] - s * Width / 2;
        points[4] = points[4] - s * Width / 2;

        points.Add(points[0] + s * Width);
        points.Add(points[1] + s * Width);
        points.Add(points[2] + s * Width);
        points.Add(points[3] + s * Width);
        points.Add(points[4] + s * Width);

        List<int> triangles = new List<int>();

        // Front triangles
        triangles.AddRange(new int[] { 0, 1, 4 });
        triangles.AddRange(new int[] { 4, 1, 3 });
        triangles.AddRange(new int[] { 1, 2, 3 });

        // Back triangles
        triangles.AddRange(new int[] { 9, 6, 5 });
        triangles.AddRange(new int[] { 6, 9, 8 });
        triangles.AddRange(new int[] { 7, 6, 8 });

        // Bottom triangles
        triangles.AddRange(new int[] { 0, 9, 5 });
        triangles.AddRange(new int[] { 0, 4, 9 });

        // LeftTop
        triangles.AddRange(new int[] { 6, 7, 1 });
        triangles.AddRange(new int[] { 1, 7, 2 });

        // LeftBottom
        triangles.AddRange(new int[] { 5, 6, 0 });
        triangles.AddRange(new int[] { 0, 6, 1 });

        // RightBottom
        triangles.AddRange(new int[] { 4, 8, 9 });
        triangles.AddRange(new int[] { 4, 3, 8 });

        // RightTop
        triangles.AddRange(new int[] { 3, 2, 8 });
        triangles.AddRange(new int[] { 8, 2, 7 });


        var mesh = new Mesh
        {
            vertices = points.ToArray(),
            triangles = triangles.ToArray()
        };

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        var meshRenderer = gableWall.AddComponent<MeshRenderer>();
        var filter = gableWall.AddComponent<MeshFilter>();
        meshRenderer.material = Material;
        filter.mesh = mesh;

        gableWall.AddComponent<MeshCollider>();
        gableWall.tag = "Wall";

        gableWall.transform.Translate(new Vector3(2000, 0, 0), Space.World);

        gableWall.transform.parent = GameObject.Find("3DArea").transform;
        gableWall.layer = LayerMask.NameToLayer("3DArea");

        Instance3D = gableWall;

        DestroyImmediate(oldWall);

        return gableWall;
    }

    public GameObject DrawHole3D(Vector3 position, float width, float height)
    {
        Vector3 v = EndPoint - StartPoint;
        Quaternion rotation = Quaternion.LookRotation(v, Vector3.up);
        rotation *= Quaternion.Euler(0, 90, 0);

        var holeObject = DrawCube3D(position, rotation, width, height, 1f);
        holeObject.transform.parent = Instance3D.transform;

        return holeObject;
    }

    GameObject DrawCube3D(Vector3 position, Quaternion rotation, float width, float height, float wWall)
    {
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

        // Przesuniêcie wierzcho³ków Cube
        var vertices = cube.GetComponent<MeshFilter>().mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3((vertices[i].x * width) + position.x, (vertices[i].y * height) + position.y, vertices[i].z + position.z);
        }
        cube.GetComponent<MeshFilter>().mesh.vertices = vertices;

        cube.name = "HoleCube";
        cube.GetComponent<Renderer>().material.color = Color.black;
        cube.transform.localScale = new Vector3(width, height, wWall);
        cube.transform.position = position;
        cube.transform.rotation = rotation;

        return cube;
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

    public Net3dBool.Color3f[] getColorArray(int length, Color c)
    {
        var ar = new Net3dBool.Color3f[length];
        for (var i = 0; i < length; i++)
            ar[i] = new Net3dBool.Color3f(c.r, c.g, c.b);
        return ar;
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

    private Vector3 GetPosition3dView(Vector3 position2dView, float Y)
    {
        Vector3 position3dView = new Vector3(position2dView.x, Y, position2dView.y);
        return position3dView;
    }
}
