using Draw2DShapesLite;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DrawCeiling : MonoBehaviour
{
    // Rysuj Polygon Area w części 2D na żywo
    // - przyciąganie do punktów z niższej kondygnacji

    // W 3D rysuje się bryła 3D o zadanej wysokości


    public Material material;
    DomaManager domaManager;

    GameObject area3D;
    GameObject area2D;

    List<Vector3> polygonPoints;

    FloorDoma activeFloorDoma;

    GameObject ceilingObject;

    // Start is called before the first frame update
    void Start()
    {
        domaManager = DomaManager.Instance;
        activeFloorDoma = domaManager.ActiveDomaFloor;

        area3D = GameObject.Find("3DArea");
        area2D = GameObject.Find("2DArea");

        polygonPoints = new List<Vector3>();
    }

    // Update is called once per frame
    void Update()
    {
        if (domaManager.ResetDataFunctionNow)
        {
            polygonPoints.Clear();
            //polygonPoints = null;
            //polygonPoints = new List<Vector3>();
            domaManager.ResetDataFunctionNow = false;
        }

        // Sledzenie punktów z kondygnacji niższej
        var nearestPoint = NearestPoint2D(0.5f);

        if (Input.GetMouseButtonDown(0))
        {
            print("polygonPoints.Count: " + polygonPoints.Count);

            if (nearestPoint != null)
            {
                polygonPoints.Add((Vector3)nearestPoint);
            }
            else
            {
                polygonPoints.Add(domaManager.mousePosition2D);
            }
        }
        //else if (Input.GetMouseButton(0))
        //{
        //    ceilingObject = DrawPolygon();
        //}
        else if (Input.GetMouseButtonUp(0))
        {
            if (ceilingObject) DestroyImmediate(ceilingObject);
            ceilingObject = DrawPolygon();

            var c2D = new Ceiling2D(polygonPoints, ceilingObject);
            var c3D = new Ceiling3D();
            domaManager.CeilingDomas[domaManager.ActiveDomaFloor.Number].Ceilings.Add(new Ceiling(c2D, c3D));
        }
        else if (Input.GetMouseButtonDown(1))
        {

            // TODO - poprawić rysowanie kolejnych płyt stropowych

            DrawPolygon3D(polygonPoints);
        }
    }

    private GameObject DrawPolygon()
    {
        GameObject ceiling = new GameObject();
        ceiling.name = "Ceiling_" + domaManager.ActiveDomaFloor.Number + (domaManager.ActiveDomaFloor.Number + 1);

        var vertices3D = polygonPoints.ToArray();
        var vertices2D = vertices3D.Select(v => new Vector2(v.x, v.y)).ToArray();

        var triangulator = new Triangulator(vertices2D);
        var indices = triangulator.Triangulate();

        var colors = Enumerable.Range(0, vertices3D.Length)
            .Select(i => Random.ColorHSV())
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
        meshRenderer.material = material;

        var filter = ceiling.AddComponent<MeshFilter>();
        filter.mesh = mesh;

        return ceiling;
    }


    private GameObject DrawPolygon3D(List<Vector3> polygonPoints3D)
    {
        GameObject polyExtruderGO = new GameObject();

        PolyExtruder polyExtruder = polyExtruderGO.AddComponent<PolyExtruder>();
        polyExtruderGO.name = "TEST EXTRUDER";

        var vertices2D = polygonPoints3D.Select(v => new Vector2(v.x, v.y)).ToArray();
        polyExtruder.createPrism(polyExtruderGO.name, 0.2f, vertices2D, Color.grey, true);

        print("activeFloorDoma.LevelBottom " + domaManager.ActiveDomaFloor.LevelBottom);
        print("activeFloorDoma.Height " + domaManager.ActiveDomaFloor.Height);

        polyExtruderGO.transform.Translate(new Vector3(2000, domaManager.ActiveDomaFloor.LevelBottom + domaManager.ActiveDomaFloor.Height, 0), Space.World);

        polyExtruder.prismColor = Color.blue;

        // Combine meshes
        MeshFilter[] allMesheFilters = polyExtruderGO.GetComponentsInChildren<MeshFilter>();

        var mesh = CombineMeshes(allMesheFilters.ToList());

        GameObject newObj = new GameObject();
        newObj.name = "STROP";
        newObj.transform.position = polyExtruderGO.transform.position;
        newObj.transform.rotation = polyExtruderGO.transform.rotation;
        newObj.transform.localScale = polyExtruderGO.transform.localScale;

        var newObjMeshFilter = newObj.AddComponent<MeshFilter>();
        var newObjMeshRenderer = newObj.AddComponent<MeshRenderer>();
        newObjMeshFilter.mesh = mesh;
        newObjMeshRenderer.material = material;

        DestroyImmediate(polyExtruderGO);

        return newObj;
    }

    public Matrix4x4 GetTRSMatrix(Transform transform)
    {
        return Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale);
    }

    private Mesh CombineMeshes(List<MeshFilter> allMesheFilters)
    {
        Mesh[] meshes = allMesheFilters.Select(x => x.mesh).ToArray();

        // Odwracamy trójkąty w dolnej siatce aby była widoczna od dołu --DOMA -- Niepotrzebne ale wcześniej było konieczne
        //meshes[0].triangles = meshes[0].triangles.Reverse().ToArray();

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

    private Vector3 GetPosition3dView(Vector3 position2dView, float Y)
    {
        Vector3 position3dView = new Vector3(position2dView.x, Y, position2dView.y);
        return position3dView;
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