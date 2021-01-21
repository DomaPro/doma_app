using System;
using UnityEngine;

public class DFloor
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Tag { get; set; }
    public float Height { get; set; }
    public float LevelBottom { get; set; }

    public DFloor(string name, string tag, float height, float levelBottom)
    {
        Id = Guid.NewGuid();
        Name = name;
        Tag = tag;
        Height = height;
        LevelBottom = levelBottom;
    }

    public DFloor(SFloor sFloor)
    {
        Id = sFloor.Id;
        Name = sFloor.Name;
        Tag = sFloor.Tag;
        Height = sFloor.Height;
        LevelBottom = sFloor.LevelBottom;
    }
}
