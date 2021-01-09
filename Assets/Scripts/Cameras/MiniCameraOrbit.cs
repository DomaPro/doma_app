using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MiniCameraOrbit : MonoBehaviour
{
    public GameObject target;
    public float speedRotationHorizontal = 30f;
    public float distanceCameraToObjects = 200f;

    private DomaManager domaManager;
    private Vector3 _lastCentralVector;

    void Start()
    {
        transform.parent = target.transform;

        domaManager = DomaManager.Instance;

        //AddCubeToArea(target, new Vector3(0, 0, 0), new Vector3(1000, 500, 20), new Vector3(0, 0, 0));
        //AddCubeToArea(target, new Vector3(0, 0, 0), new Vector3(1000, 500, 20), new Vector3(0, 90, 0));
        //AddCubeToArea(target, new Vector3(100, 0, 200), new Vector3(1000, 500, 20), new Vector3(0, 45, 0));

        var objectPositionValue = domaManager.GetObjectPositionValue(target);

        transform.localPosition = new Vector3(objectPositionValue.CentralVector.x + objectPositionValue.MaxDistanceFromCentral + distanceCameraToObjects, objectPositionValue.MaxPositionY / 1.5f, objectPositionValue.CentralVector.z);
        _lastCentralVector = objectPositionValue.CentralVector;

        transform.localEulerAngles = new Vector3(0, -90, 0);
    }

    void LateUpdate()
    {
        var objectPositionValue = domaManager.GetObjectPositionValue(target);

        if (!V3Equal(objectPositionValue.CentralVector, _lastCentralVector))
        {
            transform.localPosition = new Vector3(objectPositionValue.CentralVector.x + objectPositionValue.MaxDistanceFromCentral + distanceCameraToObjects, objectPositionValue.MaxPositionY / 1.5f, objectPositionValue.CentralVector.z);
            transform.localEulerAngles = new Vector3(0, -90, 0);
            _lastCentralVector = objectPositionValue.CentralVector;
        }

        transform.RotateAround(objectPositionValue.CentralVector + target.transform.position, Vector3.down, speedRotationHorizontal * Time.deltaTime);
    }

    GameObject AddCubeToArea(GameObject parent, Vector3 localPosition, Vector3 localScale, Vector3 rotation)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.parent = parent.transform;

        cube.transform.localPosition = localPosition;
        cube.transform.localScale = localScale;
        cube.transform.localRotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);

        return cube;
    }

    public bool V3Equal(Vector3 a, Vector3 b)
    {
        return Vector3.SqrMagnitude(a - b) < 0.0001;
    }
}
