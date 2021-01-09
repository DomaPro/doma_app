using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Parent object for elements 3D")]
    public GameObject target;

    [Header("Camera Positioning")]
    public Vector2 cameraOffset = new Vector2(10f, 14f);
    public float lookAtOffset = 2f;

    [Header("Move Controls")]
    public float moveSpeed = 200f;
    public float rotateSpeed = 45f;

    [Header("Zoom Controls")]
    public float startingZoom = 2000f;
    public float zoomSpeed = 100f;
    public float nearZoomLimit = 1500f;
    public float farZoomLimit = 2500f;

    Camera cam;
    Vector3 lastMousePosition;
    Vector3 centralVector;

    DomaManager domaManager;

    private void Awake()
    {
        cam = GetComponentInChildren<Camera>();
        cam.transform.localPosition = new Vector3(0f, Mathf.Abs(cameraOffset.y), -Mathf.Abs(cameraOffset.x));
        cam.transform.LookAt(transform.position + Vector3.up * lookAtOffset);
        cam.orthographicSize = startingZoom;
    }

    private void Start()
    {
        domaManager = DomaManager.Instance;
    }

    void Update()
    {
        if((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2)) && Input.GetButton("LeftShift"))
        {
            centralVector = domaManager.GetObjectPositionValue(target).CentralVector;
            lastMousePosition = Input.mousePosition;
        }

        // Moving Camera (LEFT SHIFT + PRESSED SCROLL MOUSE)
        if (Input.GetMouseButton(2) && Input.GetButton("LeftShift"))
        {
            Vector3 diffMousePosition = Input.mousePosition - lastMousePosition;
            Vector3 moveVector = new Vector3(diffMousePosition.x * moveSpeed, 0f, diffMousePosition.y * moveSpeed);
            transform.position += transform.TransformDirection(-moveVector) * Time.deltaTime;
        }

        // Rotate Horizontal Camera (LEFT SHIFT + LEFT MOUSE)
        if (Input.GetMouseButton(0) && Input.GetButton("LeftShift"))
        {
            Vector3 diffMousePosition = Input.mousePosition - lastMousePosition;
            transform.RotateAround(centralVector + target.transform.position, new Vector3(0, diffMousePosition.x, 0), rotateSpeed * Time.deltaTime * Input.mousePosition.x / 100);
        }

        // Rotate Vertical Camera (LEFT SHIFT + RIGHT MOUSE)
        if (Input.GetMouseButton(1) && Input.GetButton("LeftShift"))
        {
            Vector3 diffMousePosition = Input.mousePosition - lastMousePosition;
            transform.Rotate(new Vector3(diffMousePosition.y, 0f, 0f) * rotateSpeed * Time.deltaTime / 100);
        }

        // Zoom Camera (LEFT SHIFT + SCROLL MOUSE)
        if (Input.GetAxis("Mouse ScrollWheel") != 0 && Input.GetButton("LeftShift"))
        {
            float mouseScroll = Input.GetAxis("Mouse ScrollWheel");
            cam.orthographicSize -= mouseScroll * zoomSpeed;
        }
    }
}
