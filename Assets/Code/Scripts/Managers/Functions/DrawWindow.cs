using System;
using UnityEngine;

public class DrawWindow : MonoBehaviour
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
                float width = 4.5f;
                float height = 1.5f;
                float space = 0.3f;

                float w;
                bool bW = float.TryParse(domaManager.widthInput.text, out w);
                if (bW) width = w;

                float h;
                bool bH = float.TryParse(domaManager.heightInput.text, out h);
                if (bH) height = h;

                float s;
                bool bS = float.TryParse(domaManager.spaceInput.text, out s);
                if (bS) space = s;

                print("w: " + width);
                print("h: " + height);
                print("s: " + space);


                DWall dWall = domaManager.HoleInWall2D.Wall;
                var hole = dWall.DrawHole3D(new Vector3(position.Value.x, domaManager.currentStatusDoma.activeFloor.LevelBottom + domaManager.currentStatusDoma.activeFloor.Height - space - (height / 2), position.Value.y), width, height);
                dWall.DrawWallWithHole(hole);
            }
        }
    }
}
