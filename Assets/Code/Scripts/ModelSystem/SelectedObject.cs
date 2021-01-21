using System;
using UnityEngine;

public class SelectedObject
{
    public GameObject ObjectInstance { get; set; }
    public Material OriginalMaterial { get; set; }

    public SelectedObject(GameObject objectInstance)
    {
        ObjectInstance = objectInstance;
        OriginalMaterial = objectInstance.GetComponent<Renderer>().material;
    }
}
