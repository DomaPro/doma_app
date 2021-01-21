using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Główny Manager aplikacji
/// Zawiera wszystkie informacje o...
/// </summary>
public class DomaManager : MonoBehaviour
{
    private static DomaManager _instance;
    public static DomaManager Instance { get { return _instance; } }

    // ************************************************************************************************************

    [Header("Containers")]
    public RawImage box2DContainer;
    public RawImage box3DContainer;
    public RawImage miniBox3DContainer;

    [Header("Cameras")]
    public Camera cameraFor2DView;
    public GameObject cameraFor3DView;
    public Camera miniCameraFor3DView;

    [Header("Buttons")]
    public Image switchMini3DView;

    [Header("Areas")]
    public GameObject area2D;
    public GameObject area3D;

    [Header("Others")]
    public Sprite ImageToolTipPoint;

    [Header("Current Status Doma")]
    public CurrentStatusDoma currentStatusDoma;

    [Header("Lights")]
    public Light lightFor2DView;
    public Light lightFor3DView;
    public Light lightForMini3DView;

    public TMPro.TMP_InputField fileNameTextBox;
    public TMPro.TMP_Text savedFilesTextBox;

    private DomaContainer domaContainer;
    private bool activeMiniCameraFor3DView = true;

    public Vector3 mousePosition2D;

    public HoleInWall2D HoleInWall2D { get; set; }

    public GameObject gameObjectCircleTip;
    private SpriteRenderer newImage;

    public bool ResetDataFunctionNow { get; set; } = false;

    public int TypeRoof { get; set; } = 1;

    public bool IsPointerOverUIButton { get; set; } = false;

    public SelectedObjectDoma SelectedObject { get; set; } = null;

    public Vector3 RectUIMousePositionForSelection { get; set; }

    List<GameObject> referencesToObjectsForRemove;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void RemoveAllInstances(List<GameObject> referencesToObjectsForRemove)
    {
        // Usunięcie starych instancji
        if (referencesToObjectsForRemove != null && referencesToObjectsForRemove.Count > 0)
        {
            foreach (var obj in referencesToObjectsForRemove)
            {
                DestroyImmediate(obj);
            }
        }
    }

    public List<GameObject> GetReferencesToInstances()
    {
        List<GameObject> listObjectsToRemove = new List<GameObject>();

        if (currentStatusDoma.appSystem == null) return null;

        if (currentStatusDoma.appSystem.Walls.Count > 0)
        {
            foreach (var wall in currentStatusDoma.appSystem.Walls)
            {
                listObjectsToRemove.Add(wall.Instance2D);
                listObjectsToRemove.Add(wall.Instance3D);
            }
        }

        if (currentStatusDoma.appSystem.Ceilings.Count > 0)
        {
            foreach (var ceiling in currentStatusDoma.appSystem.Ceilings)
            {
                listObjectsToRemove.Add(ceiling.Instance2D);
                listObjectsToRemove.Add(ceiling.Instance3D);
            }
        }

        if (currentStatusDoma.appSystem.Roofs.Count > 0)
        {
            foreach (var roof in currentStatusDoma.appSystem.Roofs)
            {
                //listObjectsToRemove.Add(roof.Instance2D);
                listObjectsToRemove.Add(roof.Instance3D);
            }
        }

        return listObjectsToRemove;
    }

    public void LoadData(string nameFile)
    {
        SerializeSystem saveSystem = SaveSystem.LoadObject(nameFile);

        referencesToObjectsForRemove = GetReferencesToInstances();
        currentStatusDoma.appSystem = null;

        if (saveSystem == null)
        {
            var startSystem = new AppSystem();

            currentStatusDoma.appSystem = startSystem;

            SetActiveFloorById(0);
            currentStatusDoma.ShowWalls2dOnFloors(new Guid[] { currentStatusDoma.activeFloor.Id });
            return;
        }
        else
        {
            currentStatusDoma.appSystem = new AppSystem(saveSystem);
            
            foreach (var wall in currentStatusDoma.appSystem.Walls)
            {
                wall.Instance2D = wall.DrawWall2D();
                wall.Instance3D = wall.DrawWall3D();
            }

            foreach (var ceiling in currentStatusDoma.appSystem.Ceilings)
            {
                ceiling.Instance2D = ceiling.DrawCeiling2D();
                ceiling.Instance3D = ceiling.DrawCeiling3D();
            }

            foreach (var roof in currentStatusDoma.appSystem.Roofs)
            {
                //roof.Instance2D = roof.DrawRoofType1();
                roof.Instance3D = roof.DrawRoofType1();
            }

            SetActiveFloorById(0);
            currentStatusDoma.ShowWalls2dOnFloors(new Guid[] { currentStatusDoma.appSystem.Floors[0].Id });
        }

        RemoveAllInstances(referencesToObjectsForRemove);
    }

    public void ShowSavedFiles()
    {
        string val = "";
        var list = SaveSystem.GetNameAllFiles();

        foreach (var item in list)
        {
            val += item;
            val += "\n";
        }

        savedFilesTextBox.text = val;
    }

    /// <summary>
    /// Test dokumentacji
    /// </summary>
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        domaContainer = DomaContainer.Instance;

        Application.targetFrameRate = domaContainer.TargetFrameRate;

        // Turn off v-sync
        //QualitySettings.vSyncCount = 2;

        LoadData("domaApp");

        Enable2DView();

        ShowSavedFiles();

        gameObjectCircleTip = new GameObject("CircleTip");
        newImage = gameObjectCircleTip.AddComponent<SpriteRenderer>();
        newImage.sprite = ImageToolTipPoint;
        gameObjectCircleTip.SetActive(false);
        currentStatusDoma.activeFloor = currentStatusDoma.appSystem.Floors.FirstOrDefault();
    }

    /// <summary>
    /// Przypisuje aktywną kondygnację na podstawie numeru ID
    /// </summary>
    /// <param name="id">Numer indeksu kondygnacji</param>
    /// <returns>
    /// Brak
    /// </returns>
    public void SetActiveFloorById(int id)
    {
        currentStatusDoma.activeFloor = currentStatusDoma.appSystem.Floors[id];
    }

    public void Enable2DView()
    {
        box2DContainer.gameObject.SetActive(true);
        box3DContainer.gameObject.SetActive(false);

        cameraFor2DView.gameObject.SetActive(true);
        cameraFor3DView.gameObject.SetActive(false);

        miniBox3DContainer.gameObject.SetActive(activeMiniCameraFor3DView);
        miniCameraFor3DView.gameObject.SetActive(activeMiniCameraFor3DView);
        switchMini3DView.gameObject.SetActive(!activeMiniCameraFor3DView);

        lightForMini3DView.gameObject.SetActive(true);
    }

    public void Enable3DView()
    {
        box2DContainer.gameObject.SetActive(false);
        box3DContainer.gameObject.SetActive(true);

        cameraFor2DView.gameObject.SetActive(false);
        cameraFor3DView.gameObject.SetActive(true);

        miniBox3DContainer.gameObject.SetActive(false);
        miniCameraFor3DView.gameObject.SetActive(false);
        switchMini3DView.gameObject.SetActive(false);

        lightForMini3DView.gameObject.SetActive(false);
    }

    public void SwitchMini3DView()
    {
        activeMiniCameraFor3DView = !activeMiniCameraFor3DView;

        if (miniBox3DContainer.gameObject.activeSelf)
        {
            miniBox3DContainer.gameObject.SetActive(false);
            miniCameraFor3DView.gameObject.SetActive(false);
            switchMini3DView.gameObject.SetActive(true);
        }
        else
        {
            miniBox3DContainer.gameObject.SetActive(true);
            miniCameraFor3DView.gameObject.SetActive(true);
            switchMini3DView.gameObject.SetActive(false);
        }
    }

    public ObjectPositionValue GetObjectPositionValue(GameObject parentObject)
    {
        var allObjectsIn3DArea = new List<Transform>();

        allObjectsIn3DArea = parentObject.GetComponentsInChildren<Transform>().ToList();
        var allVertex = new List<Vector3>();


        foreach (var item in allObjectsIn3DArea)
        {
            if (item.name == parentObject.name) continue;
            if (item.name == "CameraForMini3DView") continue;
            if (item.name == "Camera3DView") continue;

            Vector3[] vertices = item.GetComponent<MeshFilter>().mesh.vertices;

            foreach (var vert in vertices)
            {
                allVertex.Add(item.gameObject.transform.TransformPoint(vert) - parentObject.transform.position);
            }
        }

        float maxX = allVertex.Max(v => v.x);
        float minX = allVertex.Min(v => v.x);
        float maxZ = allVertex.Max(v => v.z);
        float minZ = allVertex.Min(v => v.z);
        float maxY = allVertex.Max(v => v.y);
        float minY = allVertex.Min(v => v.y);

        var halfX = (maxX + minX) / 2;
        var halfZ = (maxZ + minZ) / 2;

        Vector3 centralVector = new Vector3(halfX, maxY, halfZ);

        List<float> distances = new List<float>();

        foreach (var item in allVertex)
        {
            distances.Add(Vector3.Distance(new Vector3(item.x, 0, item.z), new Vector3(centralVector.x, 0, centralVector.z)));
        }

        float maxDistance = distances.Max();

        return new ObjectPositionValue(centralVector, maxDistance, maxY);

    }
}

public class HoleInWall2D
{
    public GameObject objectWall2D { get; set; }
    public Vector3 mousePosition { get; set; }
    public DomaElement domaElement { get; set; }

    public DWall Wall { get; set; }
}
