using System;

[System.Serializable]
public class SFloor
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Tag { get; set; }
    public float Height { get; set; }
    public float LevelBottom { get; set; }

    public SFloor(DFloor dFloor)
    {
        Id = dFloor.Id;
        Name = dFloor.Name;
        Tag = dFloor.Tag;
        Height = dFloor.Height;
        LevelBottom = dFloor.LevelBottom;
    }
}
