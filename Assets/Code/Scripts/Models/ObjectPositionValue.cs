using System.Collections;
using UnityEngine;

public class ObjectPositionValue
{
    public Vector3 CentralVector { get; set; }
    public float MaxDistanceFromCentral { get; set; }
    public float MaxPositionY { get; set; }

    public ObjectPositionValue(Vector3 centralVector, float maxDistanceFromCentral, float maxPositionY)
    {
        CentralVector = centralVector;
        MaxDistanceFromCentral = maxDistanceFromCentral;
        MaxPositionY = maxPositionY;
    }
}
