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

    private DomaContainer domaContainer;
    private bool activeMiniCameraFor3DView = true;

    public Vector3 mousePosition2D;

    public HoleInWall2D HoleInWall2D { get; set; }

    public GameObject gameObjectCircleTip;
    private SpriteRenderer newImage;

    public bool ResetDataFunctionNow { get; set; } = false;

    public int TypeRoof { get; set; } = 1;

    public bool IsPointerOverUIButton { get; set; } = false;

    /// <summary>
    /// Test dokumentacji Doma Piętra
    /// </summary>
    public List<FloorDoma> DomaFloors { get; set; }
    public FloorDoma ActiveDomaFloor { get; set; }

    public List<CeilingDoma> CeilingDomas { get; set; }

    public List<RoofDoma> DomaRoofs { get; set; }

    public SelectedObjectDoma SelectedObject { get; set; } = null;

    public Vector3 RectUIMousePositionForSelection { get; set; }

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

        Enable2DView();

        gameObjectCircleTip = new GameObject("CircleTip");
        newImage = gameObjectCircleTip.AddComponent<SpriteRenderer>();
        newImage.sprite = ImageToolTipPoint;
        gameObjectCircleTip.SetActive(false);

        float floor1H = 2.2f;
        float floor2H = 3f;
        float floor3H = 2f;

        float ceilingH = 0.2f;

        FloorDoma floorDoma0 = new FloorDoma("Piwnica", 0, floor1H, -floor1H + 0.2f);
        CeilingDoma ceilingDoma01 = new CeilingDoma("Strop 1", 0, ceilingH, 0.2f);
        FloorDoma floorDoma1 = new FloorDoma("Parter", 1, floor2H, 0.2f + ceilingH);
        CeilingDoma ceilingDoma12 = new CeilingDoma("Strop 2", 1, ceilingH, 0.2f + ceilingH + floor2H);
        FloorDoma floorDoma2 = new FloorDoma("Piętro I", 2, floor3H, 0.2f + ceilingH + floor2H + ceilingH);
        CeilingDoma ceilingDoma23 = new CeilingDoma("Strop 3", 3, ceilingH, 0.2f + ceilingH + floor2H + ceilingH + floor3H);
        DomaFloors = new List<FloorDoma>() { floorDoma0, floorDoma1, floorDoma2 };
        ActiveDomaFloor = DomaFloors.FirstOrDefault();

        CeilingDomas = new List<CeilingDoma>() { ceilingDoma01, ceilingDoma12, ceilingDoma23 };

        DomaRoofs = new List<RoofDoma>();
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
        ActiveDomaFloor = DomaFloors[id];
    }

    // Włączenie widoczności dla wybranych Tagów
    public void Show2DObjectsOnTags(string[] floorTags)
    {
        var allWallObjects2D = DomaFloors.SelectMany(x => x.Walls.Select(w => w.Wall2D.Wal2DInstance)).ToList();

        foreach (var wallObj in allWallObjects2D)
        {
            if (floorTags.Contains(wallObj.tag))
            {
                wallObj.GetComponent<PolygonCollider2D>().enabled = true;

                wallObj.GetComponent<Renderer>().enabled = true;

                foreach (var child in wallObj.GetComponentsInChildren<Renderer>())
                {
                    child.enabled = true;
                }
            }
            else
            {
                wallObj.GetComponent<PolygonCollider2D>().enabled = false;

                wallObj.GetComponent<Renderer>().enabled = false;

                foreach (var child in wallObj.GetComponentsInChildren<Renderer>())
                {
                    child.enabled = false;
                }
            }
        }
    }

    // Pobierz wszystkie punkty na rysunku 2D dla wybranych Tagów
    public List<Vector3> GetAllPoints2DWallsOnTags(string[] floorTags)
    {
        var vectors = new HashSet<Vector3>();
        var allWallObjects2D = DomaFloors.SelectMany(x => x.Walls).ToList();

        foreach (var wall in allWallObjects2D)
        {
            vectors.Add(wall.Wall2D.StartPoint);
            vectors.Add(wall.Wall2D.EndPoint);
        }

        return vectors.ToList();
    }

    // Pobierz wszystkie punkty na rysunku 2D dla wybranych Tagów
    public WallDoma GetWallDomaByGameObject(GameObject gameObject)
    {
        return DomaFloors.SelectMany(x => x.Walls).Where(w => GameObject.ReferenceEquals(w.Wall2D.Wal2DInstance, gameObject)).FirstOrDefault();
    }

    public WallDoma GetWallDomaByGameObject3D(GameObject gameObject)
    {
        return DomaFloors.SelectMany(x => x.Walls).Where(w => GameObject.ReferenceEquals(w.Wall3D.Wall3DInstance, gameObject)).FirstOrDefault();
    }

    public CeilingDoma GetCeilingDomaByGameObject3D(GameObject gameObject)
    {
        foreach (var item in CeilingDomas)
        {
            if(item.Ceilings.Any(x => GameObject.ReferenceEquals(x.Ceiling3D.Ceiling3DInstance, gameObject)))
            {
                return item;
            }
        }

        return null;
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

        //area2D.gameObject.SetActive(true);
        //area3D.gameObject.SetActive(false);
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

        //area2D.gameObject.SetActive(false);
        //area3D.gameObject.SetActive(true);
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
    public Wall2D Wall2D { get; set; }
    public Wall3D Wall3D { get; set; }
}
