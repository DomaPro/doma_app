using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SCeiling
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Tag { get; set; }
    public float Height { get; set; }
    public float LevelBottom { get; set; }
    public string Material { get; set; }

    public List<float[]> Points { get; set; } // 2D
    public int[] Triangles3D { get; set; } // 3D
    public List<float[]> Vertices3D { get; set; }  // 3D

    public SCeiling(DCeiling dCeiling)
    {
        Id = dCeiling.Id;
        Name = dCeiling.Name;
        Tag = dCeiling.Tag;
        Height = dCeiling.Height;
        LevelBottom = dCeiling.LevelBottom;
        Material = dCeiling.Material.name;

        Points = new List<float[]>();
        foreach (var v in dCeiling.Points)
        {
            Points.Add(new float[3] { v.x, v.y, v.z });
        }

        Triangles3D = dCeiling.Instance3D.GetComponent<MeshFilter>().mesh.triangles;

        Vertices3D = new List<float[]>();
        foreach (var v in dCeiling.Instance3D.GetComponent<MeshFilter>().mesh.vertices)
        {
            Vertices3D.Add(new float[3] { v.x, v.y, v.z });
        }

    }
}
