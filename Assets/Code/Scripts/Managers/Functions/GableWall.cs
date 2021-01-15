using Draw2DShapesLite;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GableWall : MonoBehaviour
{
    public Material material;

    DomaManager domaManager;

    GameObject area3D;
    GameObject area2D;

    FloorDoma activeFloorDoma;

    public float KneeWall = 0.5f;

    void Start()
    {
        Debug.Log("GabeWall STARTED");

        domaManager = DomaManager.Instance;
        activeFloorDoma = domaManager.ActiveDomaFloor;

        area3D = GameObject.Find("3DArea");
        area2D = GameObject.Find("2DArea");
    }

    void Update()
    {
        // Dzia³anie dla widoku 2D

        // Pobranie œciany poprzez klikniêcie
        if (Input.GetMouseButtonDown(0))
        {
            var mp = domaManager.mousePosition2D; // pozycja myszki w strefie 2D

            // Pobraæ wszystkie œciany na aktywnej kondygnacji

            //List<WallDoma> wallsOnTheFloor = domaManager.ActiveDomaFloor.Walls;
            //List<Wall2D> walls2D = wallsOnTheFloor.Select(x => x.Wall2D).ToList();

            // Myszka znajduje siê nad œcian¹
            if(domaManager.HoleInWall2D != null)
            {
                print("domaManager.HoleInWall2D: " + domaManager.HoleInWall2D.objectWall2D.name);

                Wall3D wall3D = domaManager.HoleInWall2D.Wall3D;

                var obj = DrawGableWall3D(wall3D);

                DestroyImmediate(wall3D.Wall3DInstance);
                wall3D.Wall3DInstance = obj;
            }

        }

        // Dzia³anie dla widoku 3D
    }

    private GameObject DrawGableWall3D(Wall3D wall3D)
    {
        GameObject gableWall = new GameObject();
        gableWall.name = "Gable Wall";

        List<Vector3> points = new List<Vector3>();

        Vector3 centralVector = (wall3D.StartPoint + wall3D.EndPoint) / 2;

        float width = 0.2f;

        // Front
        points.Add(new Vector3(wall3D.StartPoint.x, domaManager.ActiveDomaFloor.LevelBottom, wall3D.StartPoint.z));
        points.Add(new Vector3(wall3D.StartPoint.x, domaManager.ActiveDomaFloor.LevelBottom + KneeWall, wall3D.StartPoint.z));
        points.Add(new Vector3(centralVector.x, domaManager.ActiveDomaFloor.LevelBottom + domaManager.ActiveDomaFloor.Height, centralVector.z));
        points.Add(new Vector3(wall3D.EndPoint.x, domaManager.ActiveDomaFloor.LevelBottom + KneeWall, wall3D.EndPoint.z));
        points.Add(new Vector3(wall3D.EndPoint.x, domaManager.ActiveDomaFloor.LevelBottom, wall3D.EndPoint.z));

        // Back
        var l = wall3D.EndPoint - wall3D.StartPoint;
        var s = Vector3.Normalize(Vector3.Cross(l, Vector3.up));

        points[0] = points[0] - s * width / 2;
        points[1] = points[1] - s * width / 2;
        points[2] = points[2] - s * width / 2;
        points[3] = points[3] - s * width / 2;
        points[4] = points[4] - s * width / 2;

        points.Add(points[0] + s * width);
        points.Add(points[1] + s * width);
        points.Add(points[2] + s * width);
        points.Add(points[3] + s * width);
        points.Add(points[4] + s * width);

        List<int> triangles = new List<int>();

        print("LEN POINTS: " + points.Count);

        // Front triangles
        triangles.AddRange(new int[] { 0, 1, 4 });
        triangles.AddRange(new int[] { 4, 1, 3 });
        triangles.AddRange(new int[] { 1, 2, 3 });

        // Back triangles
        triangles.AddRange(new int[] { 9, 6, 5 });
        triangles.AddRange(new int[] { 6, 9, 8 });
        triangles.AddRange(new int[] { 7, 6, 8 });

        // Bottom triangles
        triangles.AddRange(new int[] { 0, 9, 5});
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
        meshRenderer.material = material;
        filter.mesh = mesh;

        gableWall.AddComponent<MeshCollider>();
        gableWall.tag = "Ceiling";

        gableWall.tag = "Wall";

        gableWall.transform.Translate(new Vector3(2000, 0, 0), Space.World);

        return gableWall;
    }

    public Vector3 LerpByDistance(Vector3 A, Vector3 B, float x)
    {
        Vector3 P = x * Vector3.Normalize(B - A) + A;
        return P;
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

        // Odwracamy trójk¹ty w dolnej siatce aby by³a widoczna od do³u --DOMA -- Niepotrzebne ale wczeœniej by³o konieczne
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
}
