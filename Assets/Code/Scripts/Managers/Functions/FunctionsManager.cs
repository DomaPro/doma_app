using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FunctionsManager : MonoBehaviour
{
    [Header("Wall")]
    public GameObject drawWall;
    public Button activateButtonDrawWall;

    [Header("Hole")]
    public GameObject addHole;
    public Button activateButtonAddHole;

    [Header("Door")]
    public GameObject drawDoor;
    public Button activateButtonDrawDoor;

    [Header("Window")]
    public GameObject drawWindow;
    public Button activateButtonDrawWindow;

    [Header("Ceiling")]
    public GameObject drawCeiling;
    public Button activateButtonDrawCeiling;

    [Header("Gable Wall")]
    public GameObject gableWall;
    public Button activateButtonGableWall;

    [Header("Roof")]
    public GameObject roof;
    public Button activateButtonRoof;

    GameObject activeFunction;
    DomaManager domaManager;

    FloorDoma activeFloorDoma;

    List<FunctionModel> allFunctions;

    private void Start()
    {
        domaManager = DomaManager.Instance;
        allFunctions = new List<FunctionModel>();

        allFunctions.Add(new FunctionModel(drawWall, activateButtonDrawWall));
        allFunctions.Add(new FunctionModel(addHole, activateButtonAddHole));
        allFunctions.Add(new FunctionModel(drawDoor, activateButtonDrawDoor));
        allFunctions.Add(new FunctionModel(drawWindow, activateButtonDrawWindow));
        allFunctions.Add(new FunctionModel(drawCeiling, activateButtonDrawCeiling));
        allFunctions.Add(new FunctionModel(gableWall, activateButtonGableWall));
        allFunctions.Add(new FunctionModel(roof, activateButtonRoof));
    }

    public void DrawWall2D()
    {
        activeFloorDoma = domaManager.ActiveDomaFloor;
        if (activeFloorDoma.Number > 0)
        {
            string activeFloorTag = activeFloorDoma.Tag;
            string lowerActiveFloorTag = domaManager.DomaFloors[activeFloorDoma.Number - 1].Tag;

            domaManager.Show2DObjectsOnTags(new string[] { lowerActiveFloorTag, activeFloorTag });
        }

        if (activeFunction == drawWall)
        {
            DeactivateFunction(drawWall, activateButtonDrawWall);
        }
        else
        {
            ActivateFunction(drawWall, activateButtonDrawWall);
        }
    }

    public void GableWall()
    {
        activeFloorDoma = domaManager.ActiveDomaFloor;
        if (activeFloorDoma.Number > 0)
        {
            string activeFloorTag = activeFloorDoma.Tag;
            string lowerActiveFloorTag = domaManager.DomaFloors[activeFloorDoma.Number - 1].Tag;

            domaManager.Show2DObjectsOnTags(new string[] { lowerActiveFloorTag, activeFloorTag });
        }

        if (activeFunction == gableWall)
        {
            DeactivateFunction(gableWall, activateButtonGableWall);
        }
        else
        {
            ActivateFunction(gableWall, activateButtonGableWall);
        }
    }

    public void Roof()
    {
        activeFloorDoma = domaManager.ActiveDomaFloor;
        if (activeFloorDoma.Number > 0)
        {
            string activeFloorTag = activeFloorDoma.Tag;
            string lowerActiveFloorTag = domaManager.DomaFloors[activeFloorDoma.Number - 1].Tag;

            domaManager.Show2DObjectsOnTags(new string[] { lowerActiveFloorTag, activeFloorTag });
        }

        if (activeFunction == roof)
        {
            DeactivateFunction(roof, activateButtonRoof);
        }
        else
        {
            ActivateFunction(roof, activateButtonRoof);
        }
    }

    public void AddHole2D()
    {
        if (activeFunction == addHole)
        {
            DeactivateFunction(addHole, activateButtonAddHole);
        }
        else
        {
            ActivateFunction(addHole, activateButtonAddHole);
        }
    }

    public void DrawDoor()
    {
        if (activeFunction == drawDoor)
        {
            DeactivateFunction(drawDoor, activateButtonDrawDoor);
        }
        else
        {
            //FloorDoma activeFloorDoma = domaManager.ActiveDomaFloor;
            //string activeFloorTag = activeFloorDoma.Tag;

            //domaManager.Show2DObjectsOnTags(new string[] { activeFloorTag });

            ActivateFunction(drawDoor, activateButtonDrawDoor);
        }
    }

    public void DrawWindow()
    {
        if (activeFunction == drawWindow)
        {
            DeactivateFunction(drawWindow, activateButtonDrawWindow);
        }
        else
        {
            ActivateFunction(drawWindow, activateButtonDrawWindow);
        }
    }

    public void DrawCeiling()
    {
        domaManager.ResetDataFunctionNow = true;

        FloorDoma activeFloorDoma = domaManager.ActiveDomaFloor;
        if (activeFloorDoma.Number > 0)
        {
            string activeFloorTag = activeFloorDoma.Tag;
            string lowerActiveFloorTag = domaManager.DomaFloors[activeFloorDoma.Number - 1].Tag;

            domaManager.Show2DObjectsOnTags(new string[] { lowerActiveFloorTag, activeFloorTag });
        }

        if (activeFunction == drawCeiling)
        {
            DeactivateFunction(drawCeiling, activateButtonDrawCeiling);
        }
        else
        {
            ActivateFunction(drawCeiling, activateButtonDrawCeiling);
        }
    }

    public void SetTypeRoof(int typeRoof)
    {
        domaManager.TypeRoof = typeRoof;
    }

    public void QuitApp()
    {
        Application.Quit();
    }

    void ActivateFunction(GameObject function, Button button)
    {
        foreach (var item in allFunctions)
        {
            DeactivateFunction(item.GameObject, item.Button);
        }

        ChangeColorButton(button, Color.magenta);
        activeFunction = function;
        function.SetActive(true);
    }

    void DeactivateFunction(GameObject function, Button button)
    {
        ChangeColorButton(button, Color.white);
        activeFunction = null;
        function.SetActive(false);
    }

    public void ChangeColorButton(Button button, Color color)
    {
        var image = button.GetComponent<Image>();
        image.color = color;
    }
}

public class FunctionModel
{
    public GameObject GameObject { get; set; }
    public Button Button { get; set; }

    public FunctionModel(GameObject gameObject, Button button)
    {
        GameObject = gameObject;
        Button = button;
    }
}