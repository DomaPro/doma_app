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

    DFloor activeFloor;

    public float KneeWall = 0f;

    void Start()
    {
        domaManager = DomaManager.Instance;
        activeFloor = domaManager.currentStatusDoma.activeFloor;

        area3D = GameObject.Find("3DArea");
        area2D = GameObject.Find("2DArea");
    }

    void Update()
    {
        // Pobranie œciany poprzez klikniêcie
        if (Input.GetMouseButtonDown(0))
        {
            // Myszka znajduje siê nad œcian¹
            if (domaManager.HoleInWall2D != null)
            {
                DWall wall = domaManager.HoleInWall2D.Wall;
                wall.GableWall3D(0f);
            }

        }

        // Dzia³anie dla widoku 3D (zmiana poprzez klikniêcie na œcianie 3D)
    }
}
