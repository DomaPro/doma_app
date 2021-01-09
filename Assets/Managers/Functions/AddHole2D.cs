using System.Collections;
using System.Collections.Generic;
//using UnityEditor.ProBuilder;
using UnityEngine;
using Net3dBool;
using System;
using System.Linq;

/// <summary>
/// Dodawanie otwórów do ścian 2D oraz 3D
/// </summary>
public class AddHole2D : MonoBehaviour
{
    /// <summary>
    /// Przypisanie materiału dla obiektu z otworem (np. okiennym)
    /// </summary>
    public Material material;

    DomaManager domaManager;
    GameObject area3D;
    GameObject area2D;

    GameObject cube;

    void Start()
    {
        Debug.Log("AddHole2D STARTED Function");

        domaManager = DomaManager.Instance;

        area3D = GameObject.Find("3DArea");
        area2D = GameObject.Find("2DArea");

        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = "HoleCube";
        cube.GetComponent<Renderer>().material.color = Color.red;
        cube.transform.localScale = new Vector3(1f, 0.2f, 0.2f);

        //CreateC1();
        //CreateC2();
    }

    //void CreateC1()
    //{
    //    c1 = CreateCube(100, 50, 10);
    //    c1.transform.position = new Vector3(1000, 0, 0);
    //}

    //void CreateC2()
    //{
    //    c2 = CreateCube(50, 25, 5);
    //    c2.transform.position = new Vector3(1080, 0, 20);
    //    c2.transform.rotation = Quaternion.Euler(0, 90, 0);
    //}

    void Update()
    {
        Debug.Log("AddHole2D UPDATED");

        if (domaManager.HoleInWall2D != null)
        {
            Debug.Log("HoleInWall2D != null");

            if (!cube.activeSelf) cube.SetActive(true);

            cube.transform.position = domaManager.HoleInWall2D.mousePosition;

            Vector3 startPoint = domaManager.HoleInWall2D.domaElement.Points2D[0];
            Vector3 endPoint = domaManager.HoleInWall2D.domaElement.Points2D[1];
            Vector3 v = endPoint - startPoint;
            Quaternion rotation = Quaternion.LookRotation(v, Vector3.forward);
            rotation *= Quaternion.Euler(0, 90, 0);
            cube.transform.rotation = rotation;
        }
        else
        {
            Debug.Log("HoleInWall2D == null");
            cube.SetActive(false);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse DOWN!");

            Vector3? position = domaManager.HoleInWall2D?.mousePosition;

            if (position != null)
            {
                Vector3 startPoint = domaManager.HoleInWall2D.domaElement.Points2D[0];
                Vector3 endPoint = domaManager.HoleInWall2D.domaElement.Points2D[1];
                Vector3 v = endPoint - startPoint;

                // Rotation for 2D
                Quaternion rotation = Quaternion.LookRotation(v, Vector3.forward);
                rotation *= Quaternion.Euler(0, 90, 0);

                DrawHole2D(position.Value, rotation);

                // Rotation for 3D
                startPoint = domaManager.HoleInWall2D.domaElement.Points3D[0];
                endPoint = domaManager.HoleInWall2D.domaElement.Points3D[1];
                v = endPoint - startPoint;
                rotation = Quaternion.LookRotation(v, Vector3.up);
                rotation *= Quaternion.Euler(0, 90, 0);

                var cubeHoleTuple = DrawHole3D(new Vector3(position.Value.x, 4f, position.Value.y), rotation);
                var cubeHoleObject = cubeHoleTuple.Item1;
                var cubeHoleSolid = cubeHoleTuple.Item2;

                GameObject objectWall = domaManager.HoleInWall2D.domaElement.DomaObject3D;
                Solid objectWallSolid = domaManager.HoleInWall2D.domaElement.DomaObject3DSolid;

                var modeller = new Net3dBool.BooleanModeller(objectWallSolid, cubeHoleSolid);
                var tmp = modeller.getDifference();

                GameObject newGameObject = new GameObject();
                newGameObject.name = "OOOOOOOO";

                newGameObject.transform.Translate(new Vector3(2000, 0, 0), Space.World);

                MeshFilter mf = newGameObject.AddComponent<MeshFilter>();
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

                MeshRenderer mr = newGameObject.AddComponent<MeshRenderer>();

                DestroyImmediate(domaManager.HoleInWall2D.domaElement.DomaObject3D);
                DestroyImmediate(cubeHoleObject);

                domaManager.HoleInWall2D.domaElement.DomaObject3D = newGameObject;
                domaManager.HoleInWall2D.domaElement.DomaObject3DSolid = tmp;

                newGameObject.GetComponent<MeshRenderer>().material = material;

                newGameObject.transform.parent = area3D.transform;
            }
        }

    }

    GameObject DrawHole2D(Vector3 position, Quaternion rotation)
    {
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = "HoleCube";
        cube.GetComponent<Renderer>().material.color = Color.black;
        cube.transform.localScale = new Vector3(1f, 0.2f, 0.2f);
        cube.transform.position = position;
        cube.transform.rotation = rotation;

        return cube;
    }

    Tuple<GameObject, Solid> DrawHole3D(Vector3 pos, Quaternion rotation)
    {
        GameObject gameObject = new GameObject();
        gameObject.name = "Hole";

        float w = 1;
        float h = 1;
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

        //gameObject.transform.Translate(new Vector3(2000, 0, 0), Space.World);

        gameObject.transform.parent = area3D.transform;

        
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