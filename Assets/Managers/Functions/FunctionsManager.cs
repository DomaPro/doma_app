using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FunctionsManager : MonoBehaviour
{
    [Header("Wall")]
    public GameObject drawWall2D;
    public Button activateButtonDrawWall2D;

    [Header("Hole")]
    public GameObject addHole2D;
    public Button activateButtonAddHole2D;

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

    GameObject activeFunction;
    DomaManager domaManager;

    private void Start()
    {
        domaManager = DomaManager.Instance;
    }


    public void DrawWall2D()
    {
        FloorDoma activeFloorDoma = domaManager.ActiveDomaFloor;
        if (activeFloorDoma.Number > 0)
        {
            string activeFloorTag = activeFloorDoma.Tag;
            string lowerActiveFloorTag = domaManager.DomaFloors[activeFloorDoma.Number - 1].Tag;

            domaManager.Show2DObjectsOnTags(new string[] { lowerActiveFloorTag, activeFloorTag });
        }

        if (activeFunction == drawWall2D)
        {
            ChangeColorButton(activateButtonDrawWall2D, Color.white);

            activeFunction = null;
            drawWall2D.SetActive(false);

            Debug.Log("DrawWall2D is Deactivated");
        }
        else
        {
            ChangeColorButton(activateButtonDrawWall2D, Color.magenta);

            activeFunction = drawWall2D;
            drawWall2D.SetActive(true);

            Debug.Log("DrawWall2D is Activated");
        }
    }

    public void GableWall()
    {
        FloorDoma activeFloorDoma = domaManager.ActiveDomaFloor;
        if (activeFloorDoma.Number > 0)
        {
            string activeFloorTag = activeFloorDoma.Tag;
            string lowerActiveFloorTag = domaManager.DomaFloors[activeFloorDoma.Number - 1].Tag;

            domaManager.Show2DObjectsOnTags(new string[] { lowerActiveFloorTag, activeFloorTag });
        }

        if (activeFunction == gableWall)
        {
            ChangeColorButton(activateButtonGableWall, Color.white);

            activeFunction = null;
            gableWall.SetActive(false);

            Debug.Log("GableWall is Deactivated");
        }
        else
        {
            ChangeColorButton(activateButtonGableWall, Color.magenta);

            activeFunction = gableWall;
            gableWall.SetActive(true);

            Debug.Log("GableWall is Activated");
        }
    }

    public void AddHole2D()
    {
        if (activeFunction == addHole2D)
        {
            ChangeColorButton(activateButtonAddHole2D, Color.white);

            activeFunction = null;
            addHole2D.SetActive(false);

            Debug.Log("DrawWall2D is Deactivated");
        }
        else
        {
            ChangeColorButton(activateButtonAddHole2D, Color.magenta);

            activeFunction = addHole2D;
            addHole2D.SetActive(true);

            Debug.Log("DrawWall2D is Activated");
        }
    }

    public void DrawDoor()
    {
        if (activeFunction == drawDoor)
        {
            ChangeColorButton(activateButtonDrawDoor, Color.white);

            activeFunction = null;
            drawDoor.SetActive(false);

            Debug.Log("DrawDoor is Deactivated");
        }
        else
        {
            ChangeColorButton(activateButtonDrawDoor, Color.magenta);

            activeFunction = drawDoor;
            drawDoor.SetActive(true);

            FloorDoma activeFloorDoma = domaManager.ActiveDomaFloor;
            string activeFloorTag = activeFloorDoma.Tag;

            domaManager.Show2DObjectsOnTags(new string[] { activeFloorTag });

            Debug.Log("DrawDoor is Activated");
        }
    }

    public void DrawWindow()
    {
        if (activeFunction == drawWindow)
        {
            ChangeColorButton(activateButtonDrawWindow, Color.white);

            activeFunction = null;
            drawWindow.SetActive(false);

            Debug.Log("DrawWindow is Deactivated");
        }
        else
        {
            ChangeColorButton(activateButtonDrawWindow, Color.magenta);

            activeFunction = drawWindow;
            drawWindow.SetActive(true);

            Debug.Log("DrawWindow is Activated");
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
            ChangeColorButton(activateButtonDrawCeiling, Color.white);

            activeFunction = null;
            drawCeiling.SetActive(false);

            Debug.Log("DrawCeiling is Deactivated");
        }
        else
        {
            ChangeColorButton(activateButtonDrawCeiling, Color.magenta);

            activeFunction = drawCeiling;
            drawCeiling.SetActive(true);

            Debug.Log("DrawCeiling is Activated");
        }
    }

    //public void RefreshWallCorners()
    //{
    //    Debug.Log("RefreshWallCorners");

    //    // Wszystkie obiekty 2D wraz z punktem początkowym i końcowym
    //    List<RefreshWallCornersModel> dict = new List<RefreshWallCornersModel>();
    //    foreach (var item in domaManager.domaElements)
    //    {
    //        foreach (var v in item.Points2D)
    //        {
    //            dict.Add(new RefreshWallCornersModel { ObjectWall = item.DomaObject2D, Point = v });
    //        }
    //    }

    //    foreach (var item in dict)
    //    {
    //        Debug.Log("ELEMENT: " + item.ObjectWall.GetInstanceID() + " | " + item.Point.ToString("F10"));
    //    }

    //    // Pętla przez wszystkie obiekty na rysunku 2D (otagować żeby bralo tylko ściany)
    //    foreach (var item in dict)
    //    {
    //        // Znajdź obiekt dla item
    //        var firstObject = domaManager.domaElements.Where(x => x.DomaObject2D == item.ObjectWall).FirstOrDefault();

    //        // Znajdź inny obiekt, który posiada taki sam punkt
    //        var secondObjectDict = dict.Where(x => !GameObject.ReferenceEquals(x.ObjectWall, item.ObjectWall) && x.Point == item.Point).FirstOrDefault();
    //        DomaElement secondObject = null;
    //        if (secondObjectDict != null)
    //        {
    //            secondObject = domaManager.domaElements.Where(x => x.DomaObject2D == secondObjectDict.ObjectWall).FirstOrDefault();
    //        }

    //        if(firstObject != null && secondObject != null)
    //        {
    //            // Tu mamy dostęp do 2 obiektów, które mają wspólny punkt
    //            Debug.Log("ZestawDanych: " + firstObject.DomaObject2D.GetInstanceID() + " | " + String.Join("; ", firstObject.Points2D.ToArray()) + " ||| " + secondObject.DomaObject2D.GetInstanceID() + " | " + String.Join("; ", secondObject.Points2D.ToArray()));
    //        }
    //    }
    //}


    public void ChangeColorButton(Button button, Color color)
    {
        var image = button.GetComponent<Image>();
        image.color = color;

        //ColorBlock colors = button.colors;
        //colors.normalColor = color;
        //colors.highlightedColor = Color.gray;
        //button.colors = colors;
    }
}

public class RefreshWallCornersModel
{
    public GameObject ObjectWall { get; set; }
    public Vector3 Point { get; set; }
}