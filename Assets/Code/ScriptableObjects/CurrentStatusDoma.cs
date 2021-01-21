using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class CurrentStatusDoma : ScriptableObject
{
    public AppSystem appSystem;

    public DFloor activeFloor;
    public SelectedObject selectedObject;



    // Œciana na podstawie instancji rysunku 2D
    public DWall GetWallByGameObject2D(GameObject gameObject)
    {
        return appSystem.Walls.Where(w => GameObject.ReferenceEquals(w.Instance2D, gameObject)).FirstOrDefault();
    }

    // Œciana na podstawie instancji rysunku 3D
    public DWall GetWallByGameObject3D(GameObject gameObject)
    {
        return appSystem.Walls.Where(w => GameObject.ReferenceEquals(w.Instance3D, gameObject)).FirstOrDefault();
    }

    // Strop na podstawie instancji rysunku 2D
    public DCeiling GetCeilingByGameObject2D(GameObject gameObject)
    {
        return appSystem.Ceilings.Where(w => GameObject.ReferenceEquals(w.Instance2D, gameObject)).FirstOrDefault();
    }

    // Strop na podstawie instancji rysunku 3D
    public DCeiling GetCeilingByGameObject3D(GameObject gameObject)
    {
        return appSystem.Ceilings.Where(w => GameObject.ReferenceEquals(w.Instance3D, gameObject)).FirstOrDefault();
    }

    // Dach na podstawie instancji rysunku 2D
    public DRoof GetRoofByGameObject2D(GameObject gameObject)
    {
        return appSystem.Roofs.Where(w => GameObject.ReferenceEquals(w.Instance2D, gameObject)).FirstOrDefault();
    }

    // Dach na podstawie instancji rysunku 3D
    public DRoof GetRoofByGameObject3D(GameObject gameObject)
    {
        return appSystem.Roofs.Where(w => GameObject.ReferenceEquals(w.Instance3D, gameObject)).FirstOrDefault();
    }

    // Pobierz wszystkie punkty na rysunku 2D dla wybranych Tagów
    public List<Vector3> GetPointsForWallsOnFloor(Guid[] floors)
    {
        var vectors = new HashSet<Vector3>();

        foreach (var guid in floors)
        {
            vectors.UnionWith(appSystem.Walls.Where(w => floors.Contains(w.FloorId)).Select(x => x.StartPoint).ToArray());
            vectors.UnionWith(appSystem.Walls.Where(w => floors.Contains(w.FloorId)).Select(x => x.EndPoint).ToArray());
        }

        return vectors.ToList();
    }

    // Pobierz piêtro poni¿ej wskazanego/aktywnego
    public DFloor GetFloorBelow(Guid floorId)
    {
        int index = appSystem.Floors.FindIndex(f => f.Id == floorId);

        return index > 0 ? appSystem.Floors[index - 1] : null;
    }

    // Pobierz piêtro poni¿ej wskazanego/aktywnego
    public int GetFloorIndex(Guid floorId)
    {
        return appSystem.Floors.FindIndex(f => f.Id == floorId);
    }

    public void ShowWalls2dOnFloors(Guid[] floorIds)
    {
        foreach (var wall in appSystem.Walls)
        {
            var cont = false;
            if (floorIds.Contains(wall.FloorId)) cont = true;
            else cont = false;

            wall.Instance2D.GetComponent<PolygonCollider2D>().enabled = cont;
            wall.Instance2D.GetComponent<Renderer>().enabled = cont;
            foreach (var child in wall.Instance2D.GetComponentsInChildren<Renderer>())
            {
                child.enabled = cont;
            }
        }
    }
}
