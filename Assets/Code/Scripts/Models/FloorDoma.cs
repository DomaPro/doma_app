using Net3dBool;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FloorDoma
{
    public string Name { get; set; } // Floor 0
    public string Tag { get; set; } // Floor_0
    public int Number { get; set; } // 0
    public float Height { get; set; } = 3f; // 3m (Storey height)
    public float LevelBottom { get; set; } = 0; // 0 Down height level
    public List<WallDoma> Walls { get; set; } // Walls objects

    public FloorDoma(string name, int number, float height, float levelBottom)
    {
        Name = name;
        Number = number;
        Height = height;
        LevelBottom = levelBottom;

        Tag = "Floor_" + number;

        Walls = new List<WallDoma>();
    }

    // Pobierz wszystkie punkty na rysunku 2D dla ścian na całej kondygnacji
    public List<GameObject> GetAllObjects2DWalls()
    {
        return Walls.Select(x => x.Wall2D.Wal2DInstance).ToList();
    }

    // Pobierz wszystkie punkty na rysunku 2D dla ścian na całej kondygnacji
    public List<Vector3> GetAllPoints2DWalls()
    {
        var vectors = new HashSet<Vector3>();

        foreach (var wall in Walls)
        {
            vectors.Add(wall.Wall2D.StartPoint);
            vectors.Add(wall.Wall2D.EndPoint);
        }

        return vectors.ToList();
    }

    public float GetLevelBottom()
    {
        return LevelBottom;

    }
    public float GetLevelTop()
    {
        return LevelBottom + Height;
    }

    public float GetLevelMiddle()
    {
        return LevelBottom + Height / 2;
    }
}

public class WallDoma
{
    public Wall2D Wall2D { get; set; }
    public Wall3D Wall3D { get; set; }
    public List<HoleDoma> Holes { get; set; }
    public Material Material { get; set; }

    public WallDoma(Wall2D wall2D, Wall3D wall3D)
    {
        Wall2D = wall2D;
        Wall3D = wall3D;

        Holes = new List<HoleDoma>();
    }
}

public class Wall2D
{
    // <summary> 
    public Vector3 StartPoint { get; set; }
    public Vector3 EndPoint { get; set; }
    public GameObject Wal2DInstance { get; set; }

    public Wall2D(Vector3 startPoint, Vector3 endPoint, GameObject wal2DInstance)
    {
        StartPoint = startPoint;
        EndPoint = endPoint;
        Wal2DInstance = wal2DInstance;
    }
}

public class Wall3D
{
    public Vector3 StartPoint { get; set; }
    public Vector3 EndPoint { get; set; }
    public GameObject Wall3DInstance { get; set; }
    public Solid Wall3DSolidInstance { get; set; }

    public Wall3D(Vector3 startPoint, Vector3 endPoint, GameObject wall3DInstance, Solid wall3DSolidInstance)
    {
        StartPoint = startPoint;
        EndPoint = endPoint;
        Wall3DInstance = wall3DInstance;
        Wall3DSolidInstance = wall3DSolidInstance;
    }
}


public class HoleDoma
{
    public GameObject Wall;
}
