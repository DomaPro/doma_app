using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public Camera portalCamera2d; // Camera for 2D View
    public Camera portalCamera3d; // Camera for 3D View

    public Material materialSelectedObject;

    private List<string> tagsToChange;

    DomaManager domaManager;

    // Start is called before the first frame update
    void Start()
    {
        domaManager = DomaManager.Instance;
        tagsToChange = new List<string>() { "Wall", "Ceiling", "Roof" };
    }

    // Update is called once per frame
    void Update()
    {
        var rectUIMousePosition = domaManager.RectUIMousePositionForSelection;

        // Dla rysunku 2D
        //if (Input.GetMouseButtonDown(0))
        //{
        //    RaycastHit hit;
        //    Ray ray = portalCamera2d.ViewportPointToRay(new Vector3(rectUIMousePosition.x / portalCamera2d.pixelWidth, rectUIMousePosition.y / portalCamera2d.pixelHeight, portalCamera2d.nearClipPlane));

        //    if (Physics.Raycast(ray, out hit))
        //    {
        //        print("2D: " + hit.collider.gameObject);
        //    }
        //}

        // Dla rysunku 3D
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = portalCamera3d.ViewportPointToRay(new Vector3(rectUIMousePosition.x / portalCamera3d.pixelWidth, rectUIMousePosition.y / portalCamera3d.pixelHeight, portalCamera3d.nearClipPlane));

            if (Physics.Raycast(ray, out hit))
            {
                print("3D : " + hit.collider.gameObject);

                if (domaManager.SelectedObject == null)
                {
                    // Dodanie obecnego
                    domaManager.SelectedObject = new SelectedObjectDoma(hit.collider.gameObject);
                    if (tagsToChange.Contains(hit.collider.gameObject.tag))
                    {
                        hit.collider.gameObject.GetComponent<Renderer>().material = materialSelectedObject;
                    }
                }
                else
                {
                    if (GameObject.ReferenceEquals(domaManager.SelectedObject.DomaObjectInstance, hit.collider.gameObject))
                    {
                        // Reset obecnego
                        hit.collider.gameObject.GetComponent<Renderer>().material = domaManager.SelectedObject.OriginalMaterial;
                        domaManager.SelectedObject = null;
                    }
                    else
                    {
                        // Reset poprzedniego
                        if(domaManager.SelectedObject != null && domaManager.SelectedObject.DomaObjectInstance != null)
                        {
                            domaManager.SelectedObject.DomaObjectInstance.GetComponent<Renderer>().material = domaManager.SelectedObject.OriginalMaterial;
                        }
                        
                        // Dodanie nowego
                        domaManager.SelectedObject = new SelectedObjectDoma(hit.collider.gameObject);
                        if (tagsToChange.Contains(hit.collider.gameObject.tag))
                        {
                            hit.collider.gameObject.GetComponent<Renderer>().material = materialSelectedObject;
                        }
                    }
                }
            }
        }

        if (Input.GetKeyDown("delete"))
        {
            print("DELETE");

            if (domaManager.SelectedObject != null)
            {
                if(domaManager.SelectedObject.DomaObjectInstance.tag == "Wall")
                {
                    // Gdy œciana
                    var wallDoma = domaManager.GetWallDomaByGameObject3D(domaManager.SelectedObject.DomaObjectInstance);

                    foreach (var item in domaManager.DomaFloors)
                    {
                        foreach (var wall in item.Walls)
                        {
                            if(GameObject.ReferenceEquals(wall.Wall3D.Wall3DInstance, domaManager.SelectedObject.DomaObjectInstance)){
                                item.Walls.Remove(wallDoma);
                                break;
                            }
                        }
                    }

                    DestroyImmediate(wallDoma.Wall2D.Wal2DInstance);
                    DestroyImmediate(wallDoma.Wall3D.Wall3DInstance);

                    domaManager.SelectedObject = null;
                }
                else if (domaManager.SelectedObject.DomaObjectInstance.tag == "Ceiling")
                {
                    // Gdy strop
                    var ceilingDoma = domaManager.GetCeilingDomaByGameObject3D(domaManager.SelectedObject.DomaObjectInstance);

                    foreach (var item in ceilingDoma.Ceilings)
                    {
                        DestroyImmediate(item.Ceiling2D.Ceiling2DInstance);
                        DestroyImmediate(item.Ceiling3D.Ceiling3DInstance);
                    }

                    domaManager.SelectedObject = null;
                }
            }
        }
    }
}
