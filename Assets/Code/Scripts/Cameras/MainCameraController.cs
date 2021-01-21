using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainCameraController : MonoBehaviour
{
    public Camera portalCamera2d; // Camera for 2D View
    public Camera portalCamera3d; // Camera for 2D View
    public RawImage texture2DView; // Rect on UI with 2D View

    public Material materialSelectedObject;

    GameObject mouseCube;

    DomaManager domaManager;

    private List<string> tagsToChange;

    private Vector3 lastPoint;

    void Start()
    {
        domaManager = DomaManager.Instance;

        mouseCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        mouseCube.name = "MouseCube";
        mouseCube.GetComponent<Renderer>().material.color = Color.blue;
        mouseCube.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        tagsToChange = new List<string>() { "Wall", "Ceiling", "Roof" };
    }

    void Update()
    {
        var mp = Input.mousePosition;

        int marginLeft = Mathf.RoundToInt(((Screen.width - texture2DView.rectTransform.rect.width) / 2) + texture2DView.transform.localPosition.x);
        int marginBottom = Mathf.RoundToInt(((Screen.height - texture2DView.rectTransform.rect.height) / 2) + texture2DView.transform.localPosition.y);

        Rect rect2DUI = new Rect(marginLeft, marginBottom, texture2DView.rectTransform.rect.width, texture2DView.rectTransform.rect.height);

        if (rect2DUI.Contains(mp))
        {
            // Mouse Position in areaTexture2D
            var rectUIMousePosition = new Vector3(mp.x - rect2DUI.xMin, mp.y - rect2DUI.yMin, mp.z);

            // Position corners camera rect
            //var topRight = secondCamera.ViewportToWorldPoint(new Vector3(1, 1, secondCamera.nearClipPlane));
            //var topLeft = secondCamera.ViewportToWorldPoint(new Vector3(0, 1, secondCamera.nearClipPlane));
            //var bottomleft = secondCamera.ViewportToWorldPoint(new Vector3(0, 0, secondCamera.nearClipPlane));
            //var bottomRight = secondCamera.ViewportToWorldPoint(new Vector3(1, 0, secondCamera.nearClipPlane));

            domaManager.RectUIMousePositionForSelection = rectUIMousePosition;

            // Mouse Position in camera area
            var positionInCameraRect = portalCamera2d.ViewportToWorldPoint(new Vector3(rectUIMousePosition.x / portalCamera2d.pixelWidth, rectUIMousePosition.y / portalCamera2d.pixelHeight, portalCamera2d.nearClipPlane));

            var realePositionInCameraRect2D = new Vector3(positionInCameraRect.x, positionInCameraRect.y, 0);

            // Save in DomaManager
            domaManager.mousePosition2D = realePositionInCameraRect2D;

            // Cube follow Mouse Position
            mouseCube.transform.position = realePositionInCameraRect2D;

        }
    }
}
