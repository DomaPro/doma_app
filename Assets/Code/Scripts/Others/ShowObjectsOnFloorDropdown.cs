using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ShowObjectsOnFloorDropdown : MonoBehaviour
{
    [Header("Current Status Doma")]
    public CurrentStatusDoma currentStatusDoma;

    TMP_Dropdown dropDown;
    DomaManager domaManager;


    void Start()
    {
        domaManager = DomaManager.Instance;
        dropDown = GetComponent<TMP_Dropdown>();

        dropDown.ClearOptions();
        dropDown.AddOptions(new List<string> { "Wszystko" });
        dropDown.AddOptions(currentStatusDoma.appSystem.Floors.Select(x => x.Name).ToList());

        dropDown.onValueChanged.AddListener(delegate
        {
            dropDownValueChangedHandler(dropDown);
        });
    }

    void Update()
    {
        
    }

    void Destroy()
    {
        dropDown.onValueChanged.RemoveAllListeners();
    }

    public void SetDropdownIndex(int index)
    {
        dropDown.value = index;
    }

    private void dropDownValueChangedHandler(TMP_Dropdown target)
    {
        ShowObjectsOnFloors(target.value);

        //domaManager.SetActiveFloorById(target.value);
        //currentStatusDoma.ShowWalls2dOnFloors(new Guid[] { currentStatusDoma.activeFloor.Id });
    }

    public void ShowObjectsOnFloors(int index)
    {
        Guid guidFloor;

        if(index == 0)
        {
            EnableAllWalls(true);
            EnableCeilings(true);
            EnableRoofs(true);
            return;
        }
        else if (index > 0)
        {
            guidFloor = currentStatusDoma.appSystem.Floors[index - 1].Id;
        }

        EnableWallsById(guidFloor);
        EnableCeilings(false);
        EnableRoofs(false);
    }

    public void EnableWallsById(Guid gloorId)
    {
        foreach (var wall in currentStatusDoma.appSystem.Walls)
        {
            var cont = false;
            if (wall.FloorId == gloorId) cont = true;
            else cont = false;

            wall.Instance2D.SetActive(cont);
            wall.Instance2D.GetComponent<PolygonCollider2D>().enabled = cont;
            wall.Instance2D.GetComponent<Renderer>().enabled = cont;
            foreach (var child in wall.Instance2D.GetComponentsInChildren<Renderer>())
            {
                child.enabled = cont;
            }

            wall.Instance3D.SetActive(cont);
        }
    }

    public void EnableAllWalls(bool value)
    {
        foreach (var wall in currentStatusDoma.appSystem.Walls)
        {
            wall.Instance2D.SetActive(value);
            wall.Instance2D.GetComponent<PolygonCollider2D>().enabled = value;
            wall.Instance2D.GetComponent<Renderer>().enabled = value;
            foreach (var child in wall.Instance2D.GetComponentsInChildren<Renderer>())
            {
                child.enabled = value;
            }

            wall.Instance3D.SetActive(value);
        }
    }

    public void EnableCeilings(bool value)
    {
        foreach (var ceiling in currentStatusDoma.appSystem.Ceilings)
        {
            ceiling.Instance2D.SetActive(value);
            //ceiling.Instance2D.GetComponent<PolygonCollider2D>().enabled = value;
            ceiling.Instance2D.GetComponent<Renderer>().enabled = value;
            foreach (var child in ceiling.Instance2D.GetComponentsInChildren<Renderer>())
            {
                child.enabled = value;
            }

            ceiling.Instance3D.SetActive(value);
        }
    }

    public void EnableRoofs(bool value)
    {
        foreach (var roof in currentStatusDoma.appSystem.Roofs)
        {
            roof.Instance3D.SetActive(value);
        }
    }
}
