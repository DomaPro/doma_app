using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DRoof: MonoBehaviour
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Tag { get; set; }

    public float Height { get; set; }
    public float Thickness { get; set; }
    public float LevelBottom { get; set; }

    public Material Material { get; set; }

    public Vector3[] Points { get; set; } // 2D

    public int[] Triangles3D { get; set; }
    public Vector3[] Vertices3D { get; set; }

    public GameObject Instance2D { get; set; }
    public GameObject Instance3D { get; set; }

    public DRoof(float height, float thickness, float levelBottom, Vector3[] points, Material material)
    {
        Id = Guid.NewGuid();
        Name = Name = "Roof_" + Id;
        Tag = "Roof";
        Thickness = thickness;
        Height = height;
        LevelBottom = levelBottom;
        Material = material;
        Points = points;
    }

    public DRoof(SRoof sRoof)
    {
        Id = sRoof.Id;
        Name = sRoof.Name;
        Tag = sRoof.Tag;
        Material = Resources.Load("Materials/" + sRoof.Material, typeof(Material)) as Material;
        Thickness = sRoof.Thickness;
        Height = sRoof.Height;
        LevelBottom = sRoof.LevelBottom;

        List<Vector3> tempPoints = new List<Vector3>();
        foreach (var v in sRoof.Points)
        {
            tempPoints.Add(new Vector3(v[0], v[1], v[2]));
        }
        Points = tempPoints.ToArray();

        Triangles3D = sRoof.Triangles3D;

        List<Vector3> temp = new List<Vector3>();
        foreach (var v in sRoof.Vertices3D)
        {
            temp.Add(new Vector3(v[0], v[1], v[2]));
        }
        Vertices3D = temp.ToArray();
    }

    public GameObject DrawRoof3D()
    {
        GameObject roofObject = new GameObject();
        roofObject.name = Name;
        roofObject.tag = Tag;

        MeshFilter mf = roofObject.AddComponent<MeshFilter>();
        Mesh tmesh = new Mesh();
        tmesh.vertices = Vertices3D;
        tmesh.triangles = Triangles3D;
        if (Vertices3D != null && Vertices3D.Length > 0)
        {
            tmesh.uv = UvCalculator.CalculateUVs(Vertices3D, 1);
        }
        tmesh.RecalculateNormals();
        mf.mesh = tmesh;

        var mr = roofObject.AddComponent<MeshRenderer>();
        mr.material = Material;

        roofObject.transform.Translate(new Vector3(2000, 0, 0), Space.World);

        roofObject.transform.parent = GameObject.Find("3DArea").transform;

        roofObject.AddComponent<MeshCollider>();

        roofObject.layer = LayerMask.NameToLayer("3DArea");

        return roofObject;
    }

    public GameObject DrawRoofType1_v2()
    {
        // Zaczynamy rysowaæ od Lewy Górny Naro¿nik
        GameObject roof = new GameObject();
        roof.name = Name;

        var oldRoof = Instance3D;

        var vertices2D = Points.Select(v => new Vector2(v.x, v.y)).ToArray();

        List<Vector3> points = new List<Vector3>();

        //var y1 = 0f; // wysokoœæ
        //var l1 = 0.1f; // okap z boku
        //var l2 = 0.1f; // okap na szczycie

        //Height = 2.8f;

        var y1 = 0.5f; // wysokoœæ
        var l1 = 0.7f; // okap z boku
        var l2 = 0.7f; // okap na szczycie

        // P0'
        var v03n = (vertices2D[0] - vertices2D[3]).normalized;
        var v01n = (vertices2D[0] - vertices2D[1]).normalized;
        var pl01 = v03n * l1;
        var pl02 = v01n * l2;
        var P0prim = vertices2D[0] + pl01 + pl02;

        // P6'
        var v12n = (vertices2D[1] - vertices2D[2]).normalized;
        var v10n = (vertices2D[1] - vertices2D[0]).normalized;
        var pl61 = v12n * l1;
        var pl62 = v10n * l2;
        var P6prim = vertices2D[1] + pl61 + pl62;

        // P10'
        var v23n = (vertices2D[2] - vertices2D[3]).normalized;
        var v21n = (vertices2D[2] - vertices2D[1]).normalized;
        var pl101 = v21n * l1;
        var pl102 = v23n * l2;
        var P10prim = vertices2D[2] + pl101 + pl102;

        // P4'
        var v30n = (vertices2D[3] - vertices2D[0]).normalized;
        var v32n = (vertices2D[3] - vertices2D[2]).normalized;
        var pl41 = v30n * l1;
        var pl42 = v32n * l2;
        var P4prim = vertices2D[3] + pl41 + pl42;

        Vector2 cV12 = (P6prim + P10prim) / 2;
        Vector2 cV30 = (P4prim + P0prim) / 2;


        points.Add(new Vector3(P0prim.x, LevelBottom - y1, P0prim.y)); // 0
        points.Add(new Vector3(P0prim.x, LevelBottom + Thickness - y1, P0prim.y)); // 1
        points.Add(new Vector3(cV30.x, LevelBottom + Height - Thickness, cV30.y)); // 2
        points.Add(new Vector3(cV30.x, LevelBottom + Height, cV30.y)); // 3
        points.Add(new Vector3(P4prim.x, LevelBottom - y1, P4prim.y)); // 4
        points.Add(new Vector3(P4prim.x, LevelBottom + Thickness - y1, P4prim.y)); // 5
        points.Add(new Vector3(P6prim.x, LevelBottom - y1, P6prim.y)); // 6
        points.Add(new Vector3(P6prim.x, LevelBottom + Thickness - y1, P6prim.y)); // 7
        points.Add(new Vector3(cV12.x, LevelBottom + Height - Thickness, cV12.y)); // 8
        points.Add(new Vector3(cV12.x, LevelBottom + Height, cV12.y)); // 9
        points.Add(new Vector3(P10prim.x, LevelBottom - y1, P10prim.y)); // 10
        points.Add(new Vector3(P10prim.x, LevelBottom + Thickness - y1, P10prim.y)); // 11

        List<int> triangles = new List<int>();

        // Front
        triangles.AddRange(new int[] { 0, 1, 3 });
        triangles.AddRange(new int[] { 0, 3, 2 });
        triangles.AddRange(new int[] { 2, 3, 4 });
        triangles.AddRange(new int[] { 4, 3, 5 });

        // Back
        triangles.AddRange(new int[] { 10, 11, 9 });
        triangles.AddRange(new int[] { 10, 9, 8 });
        triangles.AddRange(new int[] { 9, 7, 6 });
        triangles.AddRange(new int[] { 9, 6, 8 });

        // Rest
        triangles.AddRange(new int[] { 4, 5, 11 });
        triangles.AddRange(new int[] { 4, 11, 10 });
        triangles.AddRange(new int[] { 5, 9, 11 });
        triangles.AddRange(new int[] { 5, 3, 9 });

        triangles.AddRange(new int[] { 6, 7, 1 });
        triangles.AddRange(new int[] { 6, 1, 0 });
        triangles.AddRange(new int[] { 7, 3, 1 });
        triangles.AddRange(new int[] { 7, 9, 3 });

        // Inside
        triangles.AddRange(new int[] { 0, 2, 6 });
        triangles.AddRange(new int[] { 2, 8, 6 });
        triangles.AddRange(new int[] { 8, 2, 4 });
        triangles.AddRange(new int[] { 8, 4, 10 });

        var mesh = new Mesh
        {
            vertices = points.ToArray(),
            triangles = triangles.ToArray()
        };

        mesh.uv = UvCalculator.CalculateUVs(mesh.vertices, 1);

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        var meshRenderer = roof.AddComponent<MeshRenderer>();
        var filter = roof.AddComponent<MeshFilter>();
        meshRenderer.material = Material;
        filter.mesh = mesh;

        roof.AddComponent<MeshCollider>();
        roof.tag = "Roof";
        roof.transform.parent = GameObject.Find("3DArea").transform;
        roof.layer = LayerMask.NameToLayer("3DArea");

        roof.transform.Translate(new Vector3(2000, 0, 0), Space.World);

        Instance3D = roof;

        if (oldRoof != null) DestroyImmediate(oldRoof);

        return roof;
    }

    public GameObject DrawRoofType1()
    {
        // Zaczynamy rysowaæ od Lewy Górny Naro¿nik
        GameObject roof = new GameObject();
        roof.name = Name;

        var oldRoof = Instance3D;

        var vertices2D = Points.Select(v => new Vector2(v.x, v.y)).ToArray();

        List<Vector3> points = new List<Vector3>();

        Vector2 cV12 = (vertices2D[1] + vertices2D[2]) / 2;
        Vector2 cV30 = (vertices2D[3] + vertices2D[0]) / 2;

        points.Add(new Vector3(vertices2D[0].x, LevelBottom, vertices2D[0].y)); // 0
        points.Add(new Vector3(vertices2D[0].x, LevelBottom + Thickness, vertices2D[0].y)); // 1
        points.Add(new Vector3(cV30.x, LevelBottom + Height - Thickness, cV30.y)); // 2
        points.Add(new Vector3(cV30.x, LevelBottom + Height, cV30.y)); // 3
        points.Add(new Vector3(vertices2D[3].x, LevelBottom, vertices2D[3].y)); // 4
        points.Add(new Vector3(vertices2D[3].x, LevelBottom + Thickness, vertices2D[3].y)); // 5
        points.Add(new Vector3(vertices2D[1].x, LevelBottom, vertices2D[1].y)); // 6
        points.Add(new Vector3(vertices2D[1].x, LevelBottom + Thickness, vertices2D[1].y)); // 7
        points.Add(new Vector3(cV12.x, LevelBottom + Height - Thickness, cV12.y)); // 8
        points.Add(new Vector3(cV12.x, LevelBottom + Height, cV12.y)); // 9
        points.Add(new Vector3(vertices2D[2].x, LevelBottom, vertices2D[2].y)); // 10
        points.Add(new Vector3(vertices2D[2].x, LevelBottom + Thickness, vertices2D[2].y)); // 11

        List<int> triangles = new List<int>();

        // Front
        triangles.AddRange(new int[] { 0, 1, 3 });
        triangles.AddRange(new int[] { 0, 3, 2 });
        triangles.AddRange(new int[] { 2, 3, 4 });
        triangles.AddRange(new int[] { 4, 3, 5 });

        // Back
        triangles.AddRange(new int[] { 10, 11, 9 });
        triangles.AddRange(new int[] { 10, 9, 8 });
        triangles.AddRange(new int[] { 9, 7, 6 });
        triangles.AddRange(new int[] { 9, 6, 8 });

        // Rest
        triangles.AddRange(new int[] { 4, 5, 11 });
        triangles.AddRange(new int[] { 4, 11, 10 });
        triangles.AddRange(new int[] { 5, 9, 11 });
        triangles.AddRange(new int[] { 5, 3, 9 });

        triangles.AddRange(new int[] { 6, 7, 1 });
        triangles.AddRange(new int[] { 6, 1, 0 });
        triangles.AddRange(new int[] { 7, 3, 1 });
        triangles.AddRange(new int[] { 7, 9, 3 });

        var mesh = new Mesh
        {
            vertices = points.ToArray(),
            triangles = triangles.ToArray()
        };

        mesh.uv = UvCalculator.CalculateUVs(mesh.vertices, 1);

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        var meshRenderer = roof.AddComponent<MeshRenderer>();
        var filter = roof.AddComponent<MeshFilter>();
        meshRenderer.material = Material;
        filter.mesh = mesh;

        roof.AddComponent<MeshCollider>();
        roof.tag = "Roof";
        roof.transform.parent = GameObject.Find("3DArea").transform;
        roof.layer = LayerMask.NameToLayer("3DArea");

        roof.transform.Translate(new Vector3(2000, 0, 0), Space.World);

        Instance3D = roof;

        if (oldRoof != null) DestroyImmediate(oldRoof);

        return roof;
    }

    public GameObject DrawRoofType2()
    {
        // Zaczynamy rysowaæ od Lewy Górny Naro¿nik
        GameObject roof = new GameObject();
        roof.name = Name;

        var oldRoof = Instance3D;

        var vertices2D = Points.Select(v => new Vector2(v.x, v.y)).ToArray();

        List<Vector3> points = new List<Vector3>();

        Vector2 cV12 = (vertices2D[1] + vertices2D[2]) / 2;
        Vector2 cV30 = (vertices2D[3] + vertices2D[0]) / 2;

        var p1 = new Vector3(cV30.x, LevelBottom + Height, cV30.y);
        var p4 = new Vector3(cV12.x, LevelBottom + Height, cV12.y);

        points.Add(new Vector3(vertices2D[0].x, LevelBottom, vertices2D[0].y)); // 0
        points.Add(GetPointDistanceFromPosition(p1, p4, 2f)); // 1
        points.Add(new Vector3(vertices2D[3].x, LevelBottom, vertices2D[3].y)); // 2
        points.Add(new Vector3(vertices2D[1].x, LevelBottom, vertices2D[1].y)); // 3
        points.Add(GetPointDistanceFromPosition(p4, p1, 2f)); // 4
        points.Add(new Vector3(vertices2D[2].x, LevelBottom, vertices2D[2].y)); // 5

        List<int> triangles = new List<int>();

        // Front
        triangles.AddRange(new int[] { 0, 1, 2 });

        // Back
        triangles.AddRange(new int[] { 5, 4, 3 });

        // Rest
        triangles.AddRange(new int[] { 2, 1, 4 });
        triangles.AddRange(new int[] { 2, 4, 5 });

        triangles.AddRange(new int[] { 3, 4, 0 });
        triangles.AddRange(new int[] { 4, 1, 0 });

        var mesh = new Mesh
        {
            vertices = points.ToArray(),
            triangles = triangles.ToArray()
        };

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        var meshRenderer = roof.AddComponent<MeshRenderer>();
        var filter = roof.AddComponent<MeshFilter>();
        meshRenderer.material = Material;
        filter.mesh = mesh;

        roof.AddComponent<MeshCollider>();
        roof.tag = "Roof";
        roof.transform.parent = GameObject.Find("3DArea").transform;
        roof.layer = LayerMask.NameToLayer("3DArea");

        roof.transform.Translate(new Vector3(2000, 0, 0), Space.World);

        Instance3D = roof;

        if (oldRoof != null) DestroyImmediate(oldRoof);

        return roof;
    }

    Vector3 GetPointDistanceFromPosition(Vector3 A, Vector3 B, float distance)
    {
        Vector3 P = distance * Vector3.Normalize(B - A);

        return A + P;
    }
}