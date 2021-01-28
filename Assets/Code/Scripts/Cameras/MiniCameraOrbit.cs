using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MiniCameraOrbit : MonoBehaviour
{
    public GameObject target;
    public float speedRotationHorizontal = 30f;
    public float distanceCameraToObjects = 200f;

    public float zoomSpeed = 100f;
    public float moveSpeed = 2f;

    private DomaManager domaManager;
    private Vector3 lastMousePosition;
    private Vector3 _lastCentralVector;
    private Camera cam;

    void Start()
    {
        transform.parent = target.transform;
        cam = gameObject.GetComponent<Camera>();
        domaManager = DomaManager.Instance;

        var objectPositionValue = domaManager.GetObjectPositionValue(target);

        try
        {
            transform.localPosition = new Vector3(objectPositionValue.CentralVector.x + objectPositionValue.MaxDistanceFromCentral + distanceCameraToObjects, objectPositionValue.MaxPositionY / 1.5f, objectPositionValue.CentralVector.z);
            _lastCentralVector = objectPositionValue.CentralVector;
        }
        catch
        {

        }

        transform.localEulerAngles = new Vector3(0, -90, 0);
    }

    private void Update()
    {

        // Save last mouse position
        if (Input.GetMouseButtonDown(2))
        {
            lastMousePosition = Input.mousePosition;
        }

        // Zoom Camera (SCROLL MOUSE)
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            float mouseScroll = Input.GetAxis("Mouse ScrollWheel");
            cam.fieldOfView -= mouseScroll * zoomSpeed;
        }

        // Moving Vertical Camera (PRESSED SCROLL MOUSE)
        if (Input.GetMouseButton(2))
        {
            Vector3 diffMousePosition = Input.mousePosition - lastMousePosition;
            Vector3 moveVector = new Vector3(0f, diffMousePosition.y, 0f);

            transform.position += transform.TransformDirection(moveVector) * Time.deltaTime;
        }
    }

    void LateUpdate()
    {
        var objectPositionValue = domaManager.GetObjectPositionValue(target);

        if (objectPositionValue == null) return;

        if (!V3Equal(objectPositionValue.CentralVector, _lastCentralVector))
        {
            transform.localPosition = new Vector3(objectPositionValue.CentralVector.x + objectPositionValue.MaxDistanceFromCentral + distanceCameraToObjects, objectPositionValue.MaxPositionY / 1.5f, objectPositionValue.CentralVector.z);
            transform.localEulerAngles = new Vector3(0, -90, 0);
            _lastCentralVector = objectPositionValue.CentralVector;
        }

        transform.RotateAround(objectPositionValue.CentralVector + target.transform.position, Vector3.down, speedRotationHorizontal * Time.deltaTime);
    }

    public bool V3Equal(Vector3 a, Vector3 b)
    {
        return Vector3.SqrMagnitude(a - b) < 0.0001;
    }
}
