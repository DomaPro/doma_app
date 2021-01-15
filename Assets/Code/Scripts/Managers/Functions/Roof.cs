using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Roof : MonoBehaviour
{
    public Material material;
    DomaManager domaManager;

    GameObject area3D;
    GameObject area2D;

    FloorDoma activeFloorDoma;

    public float height = 3f;
    public float thickness = 0.3f;

    List<Vector3> polygonPoints;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Roof STARTED");
        domaManager = DomaManager.Instance;
        activeFloorDoma = domaManager.ActiveDomaFloor;

        area3D = GameObject.Find("3DArea");
        area2D = GameObject.Find("2DArea");

        polygonPoints = new List<Vector3>();
    }

    // Update is called once per frame
    void Update()
    {
        var nearestPoint = NearestPoint2D(0.5f);

        if (Input.GetMouseButtonDown(0))
        {
            if (domaManager.IsPointerOverUIButton)
            {
                return;
            }

            if (nearestPoint != null)
            {
                polygonPoints.Add((Vector3)nearestPoint);
            }
            else
            {
                polygonPoints.Add(domaManager.mousePosition2D);
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            if (polygonPoints.Count < 4) return;

            if (domaManager.TypeRoof == 1)
            {
                print("DACH TYPU 1");

                var roof = DrawRoofType1(polygonPoints);

                if (domaManager.DomaRoofs.Count > 0)
                {
                    DestroyImmediate(domaManager.DomaRoofs[0].RoofInstance);
                    domaManager.DomaRoofs[0].RoofInstance = roof;
                }
                else
                {
                    domaManager.DomaRoofs.Add(new RoofDoma() { RoofInstance = roof });
                }
            }
            else
            {
                print("DACH TYPU 2");

                var roof = DrawRoofType2(polygonPoints);

                if (domaManager.DomaRoofs.Count > 0)
                {
                    DestroyImmediate(domaManager.DomaRoofs[0].RoofInstance);
                    domaManager.DomaRoofs[0].RoofInstance = roof;
                }
                else
                {
                    domaManager.DomaRoofs.Add(new RoofDoma() { RoofInstance = roof });
                }
            }
        }
    }

    Vector3 GetPointDistanceFromPosition(Vector3 A, Vector3 B, float distance)
    {
        Vector3 P = distance * Vector3.Normalize(B - A);

        return A + P;
    }

    public GameObject DrawRoofType2(List<Vector3> polygonPoints3D)
    {
        // Zaczynamy rysowaæ od Lewy Górny Naro¿nik
        GameObject roof = new GameObject();
        roof.name = "Roof";

        var vertices2D = polygonPoints3D.Select(v => new Vector2(v.x, v.y)).ToArray();

        List<Vector3> points = new List<Vector3>();

        Vector2 cV12 = (vertices2D[1] + vertices2D[2]) / 2;
        Vector2 cV30 = (vertices2D[3] + vertices2D[0]) / 2;

        var p1 = new Vector3(cV30.x, domaManager.ActiveDomaFloor.GetLevelTop() + height, cV30.y);
        var p4 = new Vector3(cV12.x, domaManager.ActiveDomaFloor.GetLevelTop() + height, cV12.y);

        points.Add(new Vector3(vertices2D[0].x, domaManager.ActiveDomaFloor.GetLevelTop(), vertices2D[0].y)); // 0
        points.Add(GetPointDistanceFromPosition(p1, p4, 2f)); // 1
        points.Add(new Vector3(vertices2D[3].x, domaManager.ActiveDomaFloor.GetLevelTop(), vertices2D[3].y)); // 2
        points.Add(new Vector3(vertices2D[1].x, domaManager.ActiveDomaFloor.GetLevelTop(), vertices2D[1].y)); // 3
        points.Add(GetPointDistanceFromPosition(p4, p1, 2f)); // 4
        points.Add(new Vector3(vertices2D[2].x, domaManager.ActiveDomaFloor.GetLevelTop(), vertices2D[2].y)); // 5

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
        meshRenderer.material = material;
        filter.mesh = mesh;

        roof.AddComponent<MeshCollider>();
        roof.tag = "Roof";

        roof.transform.Translate(new Vector3(2000, 0, 0), Space.World);

        return roof;
    }

    public GameObject DrawRoofType1(List<Vector3> polygonPoints3D)
    {
        // Zaczynamy rysowaæ od Lewy Górny Naro¿nik
        GameObject roof = new GameObject();
        roof.name = "Roof";

        var vertices2D = polygonPoints3D.Select(v => new Vector2(v.x, v.y)).ToArray();

        List<Vector3> points = new List<Vector3>();

        Vector2 cV12 = (vertices2D[1] + vertices2D[2]) / 2;
        Vector2 cV30 = (vertices2D[3] + vertices2D[0]) / 2;

        points.Add(new Vector3(vertices2D[0].x, domaManager.ActiveDomaFloor.GetLevelTop(), vertices2D[0].y)); // 0
        points.Add(new Vector3(vertices2D[0].x, domaManager.ActiveDomaFloor.GetLevelTop() + thickness, vertices2D[0].y)); // 1
        points.Add(new Vector3(cV30.x, domaManager.ActiveDomaFloor.GetLevelTop() + height - thickness, cV30.y)); // 2
        points.Add(new Vector3(cV30.x, domaManager.ActiveDomaFloor.GetLevelTop() + height, cV30.y)); // 3
        points.Add(new Vector3(vertices2D[3].x, domaManager.ActiveDomaFloor.GetLevelTop(), vertices2D[3].y)); // 4
        points.Add(new Vector3(vertices2D[3].x, domaManager.ActiveDomaFloor.GetLevelTop() + thickness, vertices2D[3].y)); // 5
        points.Add(new Vector3(vertices2D[1].x, domaManager.ActiveDomaFloor.GetLevelTop(), vertices2D[1].y)); // 6
        points.Add(new Vector3(vertices2D[1].x, domaManager.ActiveDomaFloor.GetLevelTop() + thickness, vertices2D[1].y)); // 7
        points.Add(new Vector3(cV12.x, domaManager.ActiveDomaFloor.GetLevelTop() + height - thickness, cV12.y)); // 8
        points.Add(new Vector3(cV12.x, domaManager.ActiveDomaFloor.GetLevelTop() + height, cV12.y)); // 9
        points.Add(new Vector3(vertices2D[2].x, domaManager.ActiveDomaFloor.GetLevelTop(), vertices2D[2].y)); // 10
        points.Add(new Vector3(vertices2D[2].x, domaManager.ActiveDomaFloor.GetLevelTop() + thickness, vertices2D[2].y)); // 11

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

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        var meshRenderer = roof.AddComponent<MeshRenderer>();
        var filter = roof.AddComponent<MeshFilter>();
        meshRenderer.material = material;
        filter.mesh = mesh;

        roof.AddComponent<MeshCollider>();
        roof.tag = "Roof";

        roof.transform.Translate(new Vector3(2000, 0, 0), Space.World);

        return roof;
    }

    private Vector3? NearestPoint2D(float distance)
    {
        List<Vector3> listPoints;
        string nameFloor = "Floor_";
        var activeDomaFloor = domaManager.ActiveDomaFloor;
        if (activeDomaFloor.Number > 0)
        {
            listPoints = domaManager.GetAllPoints2DWallsOnTags(new string[] { nameFloor + (activeDomaFloor.Number - 1), nameFloor + activeDomaFloor.Number });
        }
        else
        {
            listPoints = domaManager.GetAllPoints2DWallsOnTags(new string[] { nameFloor + activeDomaFloor.Number });
        }

        foreach (var point in listPoints)
        {
            if (Vector3.Distance(domaManager.mousePosition2D, point) < distance)
            {
                domaManager.gameObjectCircleTip.transform.position = point;
                domaManager.gameObjectCircleTip.SetActive(true);

                return point;
            }
            else
            {
                domaManager.gameObjectCircleTip.SetActive(false);
            }
        }

        return null;
    }
}
