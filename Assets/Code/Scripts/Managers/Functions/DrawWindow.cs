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

    DFloor activeFloor;

    // Start is called before the first frame update
    void Start()
    {
        domaManager = DomaManager.Instance;
        activeFloor = domaManager.currentStatusDoma.activeFloor;

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

            Vector3 startPoint = domaManager.HoleInWall2D.Wall.StartPoint;
            Vector3 endPoint = domaManager.HoleInWall2D.Wall.EndPoint;
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
            Vector3? position = domaManager.HoleInWall2D?.mousePosition;

            if (position != null)
            {
                DWall dWall = domaManager.HoleInWall2D.Wall;
                var hole = dWall.DrawHole3D(new Vector3(position.Value.x, domaManager.currentStatusDoma.activeFloor.LevelBottom + domaManager.currentStatusDoma.activeFloor.Height - 0.3f - 0.75f, position.Value.y), 1.5f, 1.5f);
                dWall.DrawWallWithHole(hole);


                //Vector3 startPoint = domaManager.HoleInWall2D.Wall2D.StartPoint;
                //Vector3 endPoint = domaManager.HoleInWall2D.Wall2D.EndPoint;
                //Vector3 v = endPoint - startPoint;

                //// Rotation for 2D
                //Quaternion rotation = Quaternion.LookRotation(v, Vector3.forward);
                //rotation *= Quaternion.Euler(0, 90, 0);

                //var obj2D = DrawHole2D(position.Value, rotation, 2f, 0.2f, 0.2f);
                //obj2D.transform.parent = domaManager.HoleInWall2D.Wall2D.Wal2DInstance.transform;


                //// Rotation for 3D
                //startPoint = domaManager.HoleInWall2D.Wall2D.StartPoint;
                //endPoint = domaManager.HoleInWall2D.Wall2D.EndPoint;
                //v = endPoint - startPoint;
                //rotation = Quaternion.LookRotation(v, Vector3.up);
                //rotation *= Quaternion.Euler(0, 90, 0);

                //var cubeHoleTuple = DrawHole3D(new Vector3(position.Value.x, 4f, position.Value.y), rotation, 2f, 1.5f);
                //var cubeHoleObject = cubeHoleTuple.Item1;
                //var cubeHoleSolid = cubeHoleTuple.Item2;

                //GameObject objectWall = domaManager.HoleInWall2D.Wall3D.Wall3DInstance;
                //var objectWallSolid = SolidFromGameObject(objectWall);

                //GameObject newGameObject = new GameObject();
                //newGameObject.name = "OOOOOOOO";
                //newGameObject.transform.Translate(new Vector3(2000, 0, 0), Space.World);
                //MeshFilter mf = newGameObject.AddComponent<MeshFilter>();
                //Mesh tmesh = new Mesh();

                //var modeller = new Net3dBool.BooleanModeller(objectWallSolid, cubeHoleSolid);

                //Solid tmp = null;
                //tmp = modeller.getDifference();

                //tmesh.vertices = GetVertices(tmp);
                //tmesh.triangles = tmp.getIndices();
                //tmesh.colors = GetColorsMesh(tmp);
                //tmesh.RecalculateNormals();
                //mf.mesh = tmesh;

                //mf.mesh.RecalculateTangents(0);
                //mf.mesh.RecalculateNormals();
                //mf.mesh.RecalculateBounds();
                //mf.mesh.OptimizeReorderVertexBuffer();
                //mf.mesh.OptimizeIndexBuffers();
                //mf.mesh.Optimize();

                //MeshRenderer mr = newGameObject.AddComponent<MeshRenderer>();

                //DestroyImmediate(domaManager.HoleInWall2D.Wall3D.Wall3DInstance);
                //DestroyImmediate(cubeHoleObject);

                //domaManager.HoleInWall2D.Wall3D.Wall3DInstance = newGameObject;
                //domaManager.HoleInWall2D.Wall3D.Wall3DSolidInstance = tmp;

                //newGameObject.GetComponent<MeshRenderer>().material = material;

                //newGameObject.AddComponent<MeshCollider>();
                //newGameObject.tag = "Wall";

                //newGameObject.transform.parent = area3D.transform;
            }
        }
    }

   // GameObject DrawHole2D(Vector3 position, Quaternion rotation, float width, float height, float wWall)
   // {
   //     var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
   //     cube.name = "HoleWindowCube";
   //     cube.GetComponent<Renderer>().material.color = Color.black;
   //     cube.transform.localScale = new Vector3(width, height, wWall);
   //     cube.transform.position = position;
   //     cube.transform.rotation = rotation;

   //     cube.tag = domaManager.ActiveDomaFloor.Tag;

   //     return cube;
   // }
}
