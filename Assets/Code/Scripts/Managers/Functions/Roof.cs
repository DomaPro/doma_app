using System;
using System.Collections.Generic;
using UnityEngine;

public class Roof : MonoBehaviour
{
    [Header("Current Status Doma")]
    public CurrentStatusDoma currentStatusDoma;

    public Material material;
    DomaManager domaManager;

    GameObject area3D;
    GameObject area2D;

    DFloor activeFloor;

    public float thickness = 0.3f;
    public float height = 2.5f;

    List<Vector3> polygonPoints;

    void Start()
    {
        domaManager = DomaManager.Instance;
        activeFloor = domaManager.currentStatusDoma.activeFloor;

        area3D = GameObject.Find("3DArea");
        area2D = GameObject.Find("2DArea");

        polygonPoints = new List<Vector3>();
    }

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

            DRoof dRoof = new DRoof(height, thickness, activeFloor.LevelBottom + activeFloor.Height, polygonPoints.ToArray(), material);

            if (domaManager.TypeRoof == 1)
            {
                dRoof.DrawRoofType1();
                currentStatusDoma.appSystem.Roofs.Add(dRoof);
            }
            else
            {
                dRoof.DrawRoofType2();
                currentStatusDoma.appSystem.Roofs.Add(dRoof);
            }

            polygonPoints.Clear();
        }
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
}
