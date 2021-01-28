using System;
using UnityEngine;

public class DrawDoor : MonoBehaviour
{
    DomaManager domaManager;
    GameObject cube;

    void Start()
    {
        domaManager = DomaManager.Instance;

        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = "HoleCube";
        cube.GetComponent<Renderer>().material.color = Color.red;
        cube.transform.localScale = new Vector3(1f, 0.2f, 0.2f);
    }


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
                var hole = dWall.DrawHole3D(new Vector3(position.Value.x, domaManager.currentStatusDoma.activeFloor.LevelBottom + 1.1f, position.Value.y), 1f, 2.2f);
                dWall.DrawWallWithHole(hole);
            }
        }
    }
}