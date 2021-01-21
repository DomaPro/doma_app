using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    [Header("Current Status Doma")]
    public CurrentStatusDoma currentStatusDoma;

    public Camera portalCamera2d; // Camera for 2D View
    public Camera portalCamera3d; // Camera for 3D View

    public Material materialSelectedObject;

    private List<string> tagsToChange;

    DomaManager domaManager;

    void Start()
    {
        domaManager = DomaManager.Instance;
        tagsToChange = new List<string>() { "Wall", "Ceiling", "Roof" };
    }

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
            if (domaManager.SelectedObject != null)
            {
                if (domaManager.SelectedObject.DomaObjectInstance.tag == "Wall")
                {
                    // Gdy œciana
                    var wall = currentStatusDoma.GetWallByGameObject3D(domaManager.SelectedObject.DomaObjectInstance);

                    var wall2D = wall.Instance2D;
                    var wall3D = wall.Instance3D;


                    currentStatusDoma.appSystem.Walls.RemoveAll(x => x.Id == wall.Id);

                    DestroyImmediate(wall2D);
                    DestroyImmediate(wall3D);

                    domaManager.SelectedObject = null;
                }
                else if (domaManager.SelectedObject.DomaObjectInstance.tag == "Ceiling")
                {
                    // Gdy strop
                    var ceiling = currentStatusDoma.GetCeilingByGameObject3D(domaManager.SelectedObject.DomaObjectInstance);

                    var c2D = ceiling.Instance2D;
                    var c3D = ceiling.Instance3D;

                    currentStatusDoma.appSystem.Ceilings.RemoveAll(x => x.Id == ceiling.Id);

                    DestroyImmediate(c2D);
                    DestroyImmediate(c3D);

                    domaManager.SelectedObject = null;
                }
                else if (domaManager.SelectedObject.DomaObjectInstance.tag == "Roof")
                {
                    // Gdy dach
                    var roof = currentStatusDoma.GetRoofByGameObject3D(domaManager.SelectedObject.DomaObjectInstance);

                    //var r2D = roof.Instance2D;
                    var r3D = roof.Instance3D;

                    currentStatusDoma.appSystem.Roofs.RemoveAll(x => x.Id == roof.Id);

                    //DestroyImmediate(c2D);
                    DestroyImmediate(r3D);

                    domaManager.SelectedObject = null;
                }
            }
        }
    }
}
