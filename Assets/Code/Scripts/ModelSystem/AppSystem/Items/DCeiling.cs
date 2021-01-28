using System;
using Draw2DShapesLite;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DCeiling : MonoBehaviour
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Tag { get; set; }
    public float Height { get; set; }
    public float LevelBottom { get; set; }
    public Material Material { get; set; }

    public Vector3[] Points { get; set; } // 2D
    public int[] Triangles3D { get; set; } // 3D
    public Vector3[] Vertices3D { get; set; }  // 3D

    public GameObject Instance2D { get; set; }
    public GameObject Instance3D { get; set; }

    public DCeiling(float height, float levelBottom, Material material, Vector3[] points)
    {
        Id = Guid.NewGuid();
        Name = "Ceiling_" + Id;
        Tag = "Ceiling";
        Height = height;
        LevelBottom = levelBottom;
        Material = material;
        Points = points;
    }

    public DCeiling(SCeiling sCeiling)
    {
        Id = sCeiling.Id;
        Name = sCeiling.Name;
        Tag = sCeiling.Tag;
        Height = sCeiling.Height;
        LevelBottom = sCeiling.LevelBottom;
        Material = Resources.Load("Materials/" + sCeiling.Material, typeof(Material)) as Material;

        List<Vector3> tempPoints = new List<Vector3>();
        foreach (var v in sCeiling.Points)
        {
            tempPoints.Add(new Vector3(v[0], v[1], v[2]));
        }
        Points = tempPoints.ToArray();

        Triangles3D = sCeiling.Triangles3D;

        List<Vector3> tempVertices = new List<Vector3>();
        foreach (var v in sCeiling.Vertices3D)
        {
            tempVertices.Add(new Vector3(v[0], v[1], v[2]));
        }
        Vertices3D = tempVertices.ToArray();
    }

    public GameObject DrawCeiling2D()
    {
        GameObject ceiling = new GameObject();
        ceiling.name = Name;

        var vertices3D = Points.ToArray();
        var vertices2D = vertices3D.Select(v => new Vector2(v.x, v.y)).ToArray();

        var triangulator = new Triangulator(vertices2D);
        var indices = triangulator.Triangulate();

        var colors = Enumerable.Range(0, vertices3D.Length)
            .Select(i => UnityEngine.Random.ColorHSV())
            .ToArray();

        var mesh = new Mesh
        {
            vertices = vertices3D,
            triangles = indices,
            colors = colors
        };

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        var meshRenderer = ceiling.AddComponent<MeshRenderer>();
        meshRenderer.material = Material;

        var filter = ceiling.AddComponent<MeshFilter>();
        filter.mesh = mesh;

        var refInstance2D = Instance2D;
        if (refInstance2D != null) DestroyImmediate(refInstance2D);

        ceiling.layer = LayerMask.NameToLayer("2DArea");
        ceiling.transform.parent = GameObject.Find("2DArea").transform;

        Instance2D = ceiling;

        return ceiling;
    }

    public GameObject DrawCeiling3D()
    {
        GameObject polyExtruderGO = new GameObject();

        PolyExtruder polyExtruder = polyExtruderGO.AddComponent<PolyExtruder>();
        polyExtruderGO.name = "TEST EXTRUDER";

        var vertices2D = Points.Select(v => new Vector2(v.x, v.y)).ToArray();
        polyExtruder.createPrism(polyExtruderGO.name, 0.2f, vertices2D, Color.grey, true);

        polyExtruderGO.transform.Translate(new Vector3(2000, LevelBottom, 0), Space.World);

        polyExtruder.prismColor = Color.blue;

        // Combine meshes
        MeshFilter[] allMesheFilters = polyExtruderGO.GetComponentsInChildren<MeshFilter>();

        var mesh = CombineMeshes(allMesheFilters.ToList());

        if (Vertices3D != null && Vertices3D.Length > 0)
        {
            mesh.uv = UvCalculator.CalculateUVs(Vertices3D, 1);
        }
        mesh.RecalculateNormals();

        GameObject newObj = new GameObject();
        newObj.name = Name;

        newObj.transform.position = polyExtruderGO.transform.position;
        newObj.transform.rotation = polyExtruderGO.transform.rotation;
        newObj.transform.localScale = polyExtruderGO.transform.localScale;

        var newObjMeshFilter = newObj.AddComponent<MeshFilter>();
        var newObjMeshRenderer = newObj.AddComponent<MeshRenderer>();
        newObjMeshFilter.mesh = mesh;
        newObjMeshRenderer.material = Material;

        newObj.AddComponent<MeshCollider>();
        newObj.tag = "Ceiling";

        DestroyImmediate(polyExtruderGO);

        var refInstance3D = Instance3D;
        if (refInstance3D != null) DestroyImmediate(refInstance3D);

        newObj.layer = LayerMask.NameToLayer("3DArea");
        newObj.transform.parent = GameObject.Find("3DArea").transform;

        Instance3D = newObj;

        return newObj;
    }

    public Matrix4x4 GetTRSMatrix(Transform transform)
    {
        return Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale);
    }

    private Mesh CombineMeshes(List<MeshFilter> allMesheFilters)
    {
        Mesh[] meshes = allMesheFilters.Select(x => x.mesh).ToArray();

        var combine = new CombineInstance[meshes.Length];
        for (int i = 0; i < meshes.Length; i++)
        {
            combine[i].mesh = meshes[i];
            combine[i].transform = GetTRSMatrix(allMesheFilters[i].transform);
        }

        var mesh = new Mesh();
        mesh.CombineMeshes(combine);
        return mesh;
    }
}
