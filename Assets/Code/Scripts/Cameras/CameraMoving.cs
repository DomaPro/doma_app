using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoving : MonoBehaviour
{
    public float speedMoving = 5f;
    public float speedZoom = 5f;

    private Vector3 _lastMousePosition;


    void LateUpdate()
    {
        // Moving camera XY
        if (Input.GetMouseButtonDown(0) && Input.GetButton("LeftShift"))
        {
            _lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0) && Input.GetButton("LeftShift"))
        {
            var mousePosition = Input.mousePosition;

            transform.position = new Vector3(transform.position.x + (_lastMousePosition.x - mousePosition.x) * Time.deltaTime * speedMoving, transform.position.y, transform.position.z + (_lastMousePosition.y - mousePosition.y) * Time.deltaTime * speedMoving);
        }

        // Zoom In/Out
        if (Input.GetAxis("Mouse ScrollWheel") != 0 && Input.GetButton("LeftShift"))
        {
            float mouseScroll = Input.GetAxis("Mouse ScrollWheel");
            GetComponent<Camera>().orthographicSize -= mouseScroll * speedZoom;
        }
    }
}
