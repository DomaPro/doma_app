using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SRoof
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Tag { get; set; }
    public float Height { get; set; }
    public float Thickness { get; set; }
    public float LevelBottom { get; set; }

    public String Material { get; set; }

    public List<float[]> Points { get; set; } // 2D
    public int[] Triangles3D { get; set; }
    public List<float[]> Vertices3D { get; set; }


    public SRoof(DRoof dRoof)
    {
        Id = dRoof.Id;
        Name = dRoof.Name;
        Tag = dRoof.Tag;
        Thickness = dRoof.Thickness;
        Height = dRoof.Height;
        LevelBottom = dRoof.LevelBottom;
        Material = dRoof.Material.name;

        Points = new List<float[]>();
        foreach (var v in dRoof.Points)
        {
            Points.Add(new float[3] { v.x, v.y, v.z });
        }

        Triangles3D = dRoof.Instance3D.GetComponent<MeshFilter>().mesh.triangles;

        Vertices3D = new List<float[]>();
        foreach (var v in dRoof.Instance3D.GetComponent<MeshFilter>().mesh.vertices)
        {
            Vertices3D.Add(new float[3] { v.x, v.y, v.z });
        }
    }
}
