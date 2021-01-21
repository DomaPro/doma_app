using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SWall
{
    public Guid Id { get; set; }
    public Guid FloorId { get; set; }
    public string Name { get; set; }
    public string Tag { get; set; }
    public float[] StartPoint { get; set; }
    public float[] EndPoint { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
    public float BottomLevel { get; set; }

    public string Material { get; set; }

    public int[] Triangles3D { get; set; }
    public List<float[]> Vertices3D { get; set; }


    public SWall(DWall dWall)
    {
        Id = dWall.Id;
        FloorId = dWall.FloorId;
        Name = dWall.Name;
        Tag = dWall.Tag;
        StartPoint = new float[3] { dWall.StartPoint.x, dWall.StartPoint.y, dWall.StartPoint.z };
        EndPoint = new float[3] { dWall.EndPoint.x, dWall.EndPoint.y, dWall.EndPoint.z };
        Width = dWall.Width;
        Height = dWall.Height;
        BottomLevel = dWall.BottomLevel;
        Material = dWall.Material.name;
        Triangles3D = dWall.Instance3D.GetComponent<MeshFilter>().mesh.triangles;

        Vertices3D = new List<float[]>();
        foreach (var v in dWall.Instance3D.GetComponent<MeshFilter>().mesh.vertices)
        {
            Vertices3D.Add(new float[3] { v.x, v.y, v.z });
        }
    }
}
