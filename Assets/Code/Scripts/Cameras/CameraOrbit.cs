using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public GameObject target;
    public float speedRotationHorizontal = 20f;
    public float speedRotationVertical = 20f;
    public float speedZoom= 200f;

    private Vector3 _lastMousePosition;

    private float minX = 999999f;
    private float maxX = -999999f;
    private float minZ = 999999f;
    private float maxZ = -999999f;

    private DomaManager domaManager;

    void Start()
    {
        transform.parent = target.transform;

        domaManager = DomaManager.Instance;

        Vector3 centralVector = domaManager.GetObjectPositionValue(target).CentralVector;
        //transform.localPosition = new Vector3(centralVector.x, 700f, centralVector.z - 300f);
        //GetComponent<Camera>().orthographicSize = 800f;
    }

    void LateUpdate()
    {
        Vector3 centralVector = domaManager.GetObjectPositionValue(target).CentralVector;

        // Rotation camera
        if (Input.GetMouseButtonDown(0) && Input.GetButton("LeftShift"))
        {
            _lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0) && Input.GetButton("LeftShift"))
        {
            float currentXAbs = Mathf.Abs(Input.mousePosition.x);
            float currentYAbs = Mathf.Abs(Input.mousePosition.y);

            float lastXAbs = Mathf.Abs(_lastMousePosition.x);
            float lastYAbs = Mathf.Abs(_lastMousePosition.y);

            float differentHorizontal = Mathf.Max(currentXAbs, lastXAbs) - Mathf.Min(currentXAbs, lastXAbs);
            float differentVertical = Mathf.Max(currentYAbs, lastYAbs) - Mathf.Min(currentYAbs, lastYAbs);

            if(differentHorizontal > differentVertical)
            {
                if (Input.mousePosition.x >= _lastMousePosition.x)
                {
                    transform.RotateAround(centralVector + target.transform.position, Vector3.up, speedRotationHorizontal * Time.deltaTime * Input.mousePosition.x / 100);
                }
                else
                {
                    transform.RotateAround(centralVector + target.transform.position, Vector3.down, speedRotationHorizontal * Time.deltaTime * Input.mousePosition.x / 100);
                }
            }
            else
            {
                if (Input.mousePosition.y >= _lastMousePosition.y)
                {
                    if (transform.rotation.x < 0) transform.eulerAngles = new Vector3(0, transform.rotation.y, transform.rotation.z);
                    transform.Rotate(Vector3.right * speedRotationVertical * Time.deltaTime);
                }
                else
                {
                    if (transform.rotation.x < 0) transform.eulerAngles = new Vector3(0, transform.rotation.y, transform.rotation.z);
                    transform.Rotate(Vector3.left * speedRotationVertical * Time.deltaTime);
                }
            }
        }

        // Zoom In/Out
        if (Input.GetAxis("Mouse ScrollWheel") != 0 && Input.GetButton("LeftShift"))
        {
            float mouseScroll = Input.GetAxis("Mouse ScrollWheel");
            GetComponent<Camera>().orthographicSize -= mouseScroll * speedZoom;
        }

        // Change Height Camera
        if (Input.GetMouseButtonDown(2) && Input.GetButton("LeftShift"))
        {
            _lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(2) && Input.GetButton("LeftShift"))
        {
            float currentXAbs = Mathf.Abs(Input.mousePosition.x);
            float currentYAbs = Mathf.Abs(Input.mousePosition.y);

            float lastXAbs = Mathf.Abs(_lastMousePosition.x);
            float lastYAbs = Mathf.Abs(_lastMousePosition.y);

            float differentHorizontal = Mathf.Max(currentXAbs, lastXAbs) - Mathf.Min(currentXAbs, lastXAbs);
            float differentVertical = Mathf.Max(currentYAbs, lastYAbs) - Mathf.Min(currentYAbs, lastYAbs);

            if (Input.mousePosition.y >= _lastMousePosition.y)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + differentVertical * Time.deltaTime, transform.position.z);
            }
            else
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - differentVertical * Time.deltaTime, transform.position.z);
            }
        }

    }
}
