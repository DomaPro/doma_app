using Net3dBool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DomaElement
{
    public GameObject DomaObject2D { get; set; }
    public GameObject DomaObject3D { get; set; }
    public Solid DomaObject3DSolid { get; set; }
    public List<Vector3> Points2D { get; set; }
    public List<Vector3> Points3D { get; set; }

    public DomaElement(GameObject domaObject2D, GameObject domaObject3D, Solid domaObject3DSolid, List<Vector3> points2D, List<Vector3> points3D)
    {
        DomaObject2D = domaObject2D;
        DomaObject3D = domaObject3D;
        DomaObject3DSolid = domaObject3DSolid;
        Points2D = points2D;
        Points3D = points3D;
    }
}
