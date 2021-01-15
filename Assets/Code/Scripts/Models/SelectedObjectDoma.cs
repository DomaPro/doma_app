using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedObjectDoma
{
    public GameObject DomaObjectInstance { get; set; }
    public Material OriginalMaterial { get; set; }

    public SelectedObjectDoma(GameObject domaObjectInstance)
    {
        DomaObjectInstance = domaObjectInstance;
        OriginalMaterial = domaObjectInstance.GetComponent<Renderer>().material;
    }
}
