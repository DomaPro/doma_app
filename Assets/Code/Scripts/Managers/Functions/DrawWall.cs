using Draw2DShapesLite;
using Net3dBool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DrawWall : MonoBehaviour
{
    [Header("Current Status Doma")]
    public CurrentStatusDoma currentStatusDoma;

    [Header("Material")]
    public Material material;

    DomaManager domaManager;

    Vector3 startPoint2dView;
    Vector3 endPoint2dView;

    Vector3 startPoint3dView;
    Vector3 endPoint3dView;

    GameObject area3D;
    GameObject area2D;

    GameObject tempLineRenderer;

    DFloor activeFloor;

    private Vector3 lastPoint;

    void Start()
    {
        domaManager = DomaManager.Instance;
        activeFloor = currentStatusDoma.activeFloor;

        area3D = GameObject.Find("3DArea");
        area2D = GameObject.Find("2DArea");

        tempLineRenderer = new GameObject();
        tempLineRenderer.name = "TempLineRender";
        tempLineRenderer.AddComponent<LineRenderer>();
        tempLineRenderer.GetComponent<LineRenderer>().startWidth = 0.2f;
        tempLineRenderer.GetComponent<LineRenderer>().endWidth = 0.2f;
        tempLineRenderer.transform.parent = area2D.transform;

        lastPoint = Vector3.zero;
    }

    void Update()
    {
        var nearestPoint = NearestPoint2D(0.5f);

        // Poszukiwanie punktu do zakończenia rysowania ściany
        var point = FindPointOn(domaManager.mousePosition2D, lastPoint, nearestPoint, KeyCode.LeftControl);

        if (Input.GetMouseButtonDown(0))
        {
            if (nearestPoint != null)
            {
                lastPoint = point;
                startPoint2dView = point;
                startPoint3dView = GetPosition3dView(point, 2.5f);
            }
            else
            {
                lastPoint = point;
                startPoint2dView = point;
                startPoint3dView = GetPosition3dView(point, 2.5f);
            }
        }
        else if (Input.GetMouseButton(0))
        {
            domaManager.lengthWallText.text = System.Math.Round(Vector3.Distance(lastPoint, point), 2).ToString();

            endPoint2dView = point;
            endPoint3dView = GetPosition3dView(point, 2.5f);

            tempLineRenderer.GetComponent<LineRenderer>().SetPosition(0, startPoint2dView);
            tempLineRenderer.GetComponent<LineRenderer>().SetPosition(1, endPoint2dView);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (nearestPoint != null)
            {
                lastPoint = point;
                endPoint2dView = point;
                endPoint3dView = GetPosition3dView(point, 2.5f);
            }
            else
            {
                lastPoint = point;
                endPoint2dView = point;
                endPoint3dView = GetPosition3dView(point, 2.5f);
            }

            // ********************************************************************************
            float widthWall = 0.2f;
            // 2D
            var obj2D = DrawMesh2D(startPoint2dView, endPoint2dView, widthWall, "Mesh2DPolygon");

            // 3D
            var obj3D = DrawMesh3D(startPoint3dView, endPoint3dView, widthWall, "Mesh3DProBuilder");


            // NEW
            DWall dWall = new DWall(currentStatusDoma.activeFloor.Id, startPoint2dView, endPoint2dView, 0.2f, currentStatusDoma.activeFloor.Height, currentStatusDoma.activeFloor.LevelBottom, material, obj2D, obj3D.Item1);
            currentStatusDoma.appSystem.Walls.Add(dWall);
        }
    }

    private Vector3 FindPointOn(Vector3 currentPosition, Vector3 lastPosition, Vector3? existingPosition, KeyCode keyCode)
    {
        Vector3 result = currentPosition;

        var distX = Mathf.Abs(currentPosition.x - lastPosition.x);
        var distY = Mathf.Abs(currentPosition.y - lastPosition.y);

        if (Input.GetKey(keyCode))
        {
            if (distX >= distY)
            {
                if (Input.GetKey(KeyCode.A))
                {
                    if (existingPosition != null)
                    {
                        result = new Vector3(lastPosition.x, existingPosition.Value.y, 0);
                    }
                    else
                    {
                        result = new Vector3(lastPosition.x, currentPosition.y, 0);
                    }
                }
                else
                {
                    if (existingPosition != null)
                    {
                        result = new Vector3(existingPosition.Value.x, lastPosition.y, 0);
                    }
                    else
                    {
                        result = new Vector3(currentPosition.x, lastPosition.y, 0);
                    }
                }
            }
            else
            {
                if (Input.GetKey(KeyCode.A))
                {
                    if (existingPosition != null)
                    {
                        result = new Vector3(existingPosition.Value.x, lastPosition.y, 0);
                    }
                    else
                    {
                        result = new Vector3(currentPosition.x, lastPosition.y, 0);
                    }
                }
                else
                {
                    if (existingPosition != null)
                    {
                        result = new Vector3(lastPosition.x, existingPosition.Value.y, 0);
                    }
                    else
                    {
                        result = new Vector3(lastPosition.x, currentPosition.y, 0);
                    }
                }
            }
        }
        else
        {
            if (existingPosition != null)
            {
                result = existingPosition.Value;
            }
            else
            {
                result = currentPosition;
            }
        }

        return result;
    }


    public GameObject DrawMesh2D(Vector3 startPoint, Vector3 endPoint, float width, string name)
    {
        var startP = new Vector2(startPoint.x, startPoint.y);
        var endP = new Vector2(endPoint.x, endPoint.y);

        List<Vector2> vertices2D = new List<Vector2>();
        List<Vector3> vertices3D = new List<Vector3>();

        GameObject polygon = new GameObject();
        polygon.name = name;

        Vector2 vNormalized = (endP - startP).normalized;
        Vector2 vPerpendicular = new Vector2(vNormalized.y, -vNormalized.x).normalized;

        var P1 = startP + vPerpendicular * width / 2;
        var P2 = startP - vPerpendicular * width / 2;
        var P3 = endP - vPerpendicular * width / 2;
        var P4 = endP + vPerpendicular * width / 2;

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

        var meshRenderer = polygon.AddComponent<MeshRenderer>();

        var filter = polygon.AddComponent<MeshFilter>();
        filter.mesh = mesh;

        polygon.transform.parent = area2D.transform;

        var polygonCollider = polygon.AddComponent<PolygonCollider2D>();
        polygonCollider.points = vertices2D.ToArray();

        polygon.AddComponent<WallScript>();

        polygon.tag = currentStatusDoma.activeFloor.Tag;

        polygon.layer = LayerMask.NameToLayer("2DArea");

        return polygon;
    }

    public Tuple<GameObject, Solid> DrawMesh3D(Vector3 startPoint, Vector3 endPoint, float width, string name)
    {
        activeFloor = currentStatusDoma.activeFloor;
        startPoint = new Vector3(startPoint.x, activeFloor.LevelBottom, startPoint.z);
        endPoint = new Vector3(endPoint.x, activeFloor.LevelBottom, endPoint.z);
        float h = activeFloor.Height;

        GameObject gameObject = new GameObject();
        gameObject.name = name;

        var between = endPoint - startPoint;
        var distance = Vector3.Distance(startPoint, endPoint);

        Vector3 vNormalized = endPoint - startPoint;
        Vector3 vPerpendicular = Vector3.Normalize(Vector3.Cross(vNormalized, Vector3.up));

        var P1 = startPoint + vPerpendicular * width / 2;
        var P2 = startPoint - vPerpendicular * width / 2;
        var P3 = endPoint - vPerpendicular * width / 2;
        var P4 = endPoint + vPerpendicular * width / 2;

        Vector3[] vertices = {
            P1,
            P2,
            new Vector3 (P2.x, P2.y + h, P2.z),
            new Vector3 (P1.x, P1.y + h, P1.z),
            new Vector3 (P4.x, P4.y + h, P4.z),
            new Vector3 (P3.x, P3.y + h, P3.z),
            P3,
            P4,
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

        gameObject.transform.Translate(new Vector3(2000, 0, 0), Space.World);

        gameObject.GetComponent<MeshRenderer>().material = material;

        gameObject.transform.parent = area3D.transform;

        // Doda Collider gdy ściana ma długość min. 20 cm
        if (Vector3.Distance(startPoint, endPoint) > 0.2f)
        {
            gameObject.AddComponent<MeshCollider>();
        }

        gameObject.tag = "Wall";
        gameObject.layer = LayerMask.NameToLayer("3DArea");

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

    private Vector3? NearestPoint2D(float distance)
    {
        List<Vector3> listPoints;
        var activeFloor = currentStatusDoma.activeFloor;
        if (currentStatusDoma.GetFloorIndex(activeFloor.Id) > 0)
        {
            listPoints = currentStatusDoma.GetPointsForWallsOnFloor(new Guid[] { activeFloor.Id, currentStatusDoma.GetFloorBelow(activeFloor.Id).Id });
        }
        else
        {
            listPoints = currentStatusDoma.GetPointsForWallsOnFloor(new Guid[] { activeFloor.Id });
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

    private Vector3 GetPosition3dView(Vector3 position2dView, float Y)
    {
        Vector3 position3dView = new Vector3(position2dView.x, Y, position2dView.y);
        return position3dView;
    }
}