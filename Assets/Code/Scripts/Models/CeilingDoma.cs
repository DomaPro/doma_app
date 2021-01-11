using Net3dBool;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CeilingDoma
{
    public string Name { get; set; } // Floor 0
    public string Tag { get; set; } // Floor_0
    public int Number { get; set; } // 0
    public float Height { get; set; } = 0.2f; // 20cm (Storey height)
    public float LevelBottom { get; set; } = 0; // 0 Down height level
    public List<Ceiling> Ceilings { get; set; } // Ceilings objects

    public CeilingDoma(string name, int number, float height, float levelBottom)
    {
        Name = name;
        Number = number;
        Height = height;
        LevelBottom = levelBottom;

        Tag = "Ceiling_" + number + (number + 1);

        Ceilings = new List<Ceiling>();
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

public class Ceiling
{
    public Ceiling2D Ceiling2D { get; set; }
    public Ceiling3D Ceiling3D { get; set; }
    //public List<HoleDoma> Holes { get; set; } // Otwory w stropie
    public Material Material { get; set; }

    public Ceiling(Ceiling2D ceiling2D, Ceiling3D ceiling3D)
    {
        Ceiling2D = ceiling2D;
        Ceiling3D = ceiling3D;
    }
}

public class Ceiling2D
{
    public List<Vector3> Corners { get; set; }

    public GameObject Ceiling2DInstance { get; set; }

    public Ceiling2D(List<Vector3> corners, GameObject ceiling2DInstance)
    {
        Corners = corners;
        Ceiling2DInstance = ceiling2DInstance;
    }
}

public class Ceiling3D
{
    public List<Vector3> Corners { get; set; }
    public GameObject Ceiling3DInstance { get; set; }
    public Solid Ceiling3DSolidInstance { get; set; }

    public Ceiling3D(List<Vector3> corners, GameObject ceiling3DInstance, Solid ceiling3DSolidInstance)
    {
        Corners = corners;
        Ceiling3DInstance = ceiling3DInstance;
        Ceiling3DSolidInstance = ceiling3DSolidInstance;
    }

    public Ceiling3D()
    {
    }
}