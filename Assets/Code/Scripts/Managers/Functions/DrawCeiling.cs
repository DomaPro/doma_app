using Draw2DShapesLite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DrawCeiling : MonoBehaviour
{
    [Header("Current Status Doma")]
    public CurrentStatusDoma currentStatusDoma;

    public Material material;
    DomaManager domaManager;

    GameObject area3D;
    GameObject area2D;

    List<Vector3> polygonPoints;

    DFloor activeFloor;

    GameObject ceilingTempObject; // tymczasowy obiekt stropu 2D, podgląd na czas rysowania

    // Start is called before the first frame update
    void Start()
    {
        domaManager = DomaManager.Instance;
        activeFloor = domaManager.currentStatusDoma.activeFloor;

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
            domaManager.ResetDataFunctionNow = false;
        }

        // Sledzenie punktów z kondygnacji niższej
        var nearestPoint = NearestPoint2D(0.5f);

        if (Input.GetMouseButtonDown(0))
        {
            if (nearestPoint != null)
            {
                polygonPoints.Add((Vector3)nearestPoint);
            }
            else
            {
                polygonPoints.Add(domaManager.mousePosition2D);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            DCeiling dCeiling = new DCeiling(0.2f, domaManager.currentStatusDoma.activeFloor.LevelBottom + domaManager.currentStatusDoma.activeFloor.Height, material, polygonPoints.ToArray());

            var refCeilingTempObject = ceilingTempObject;
            if (refCeilingTempObject != null) DestroyImmediate(refCeilingTempObject);

            ceilingTempObject = dCeiling.DrawCeiling2D();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            DCeiling dCeiling = new DCeiling(0.2f, domaManager.currentStatusDoma.activeFloor.LevelBottom + domaManager.currentStatusDoma.activeFloor.Height, material, polygonPoints.ToArray());
            dCeiling.DrawCeiling2D();
            dCeiling.DrawCeiling3D();

            //var ceiling2D = dCeiling.DrawCeiling2D();
            //var ceiling3D = dCeiling.DrawCeiling3D();

            //dCeiling.Instance2D = ceiling2D;
            //dCeiling.Instance3D = ceiling3D;

            domaManager.currentStatusDoma.appSystem.Ceilings.Add(dCeiling);

            DestroyImmediate(ceilingTempObject);
            ceilingTempObject = null;
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