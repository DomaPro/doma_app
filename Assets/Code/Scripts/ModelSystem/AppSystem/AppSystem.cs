using System.Collections.Generic;
using UnityEngine;

public class AppSystem
{
    public List<DFloor> Floors { get; set; }
    public List<DWall> Walls { get; set; }
    public List<DCeiling> Ceilings { get; set; }
    public List<DRoof> Roofs { get; set; }

    public AppSystem()
    {
        Floors = new List<DFloor>();
        Walls = new List<DWall>();
        Ceilings = new List<DCeiling>();
        Roofs = new List<DRoof>();

        float floor1H = 2.2f;
        float floor2H = 3f;
        float floor3H = 2.5f;
        float ceilingH = 0.2f;

        Floors.Add(new DFloor("Piwnica", "Floor_0", floor1H, -floor1H + 0.2f));
        Floors.Add(new DFloor("Parter", "Floor_1", floor2H, 0.2f + ceilingH));
        Floors.Add(new DFloor("Piêtro I", "Floor_2", floor3H, 0.2f + ceilingH + floor2H + ceilingH));
    }

    public AppSystem(SerializeSystem serializeSystem)
    {
        Floors = new List<DFloor>();
        Walls = new List<DWall>();
        Ceilings = new List<DCeiling>();
        Roofs = new List<DRoof>();

        foreach (var sFloor in serializeSystem.Floors)
        {
            Floors.Add(new DFloor(sFloor));
        }

        foreach (var sWall in serializeSystem.Walls)
        {
            Walls.Add(new DWall(sWall));
        }

        foreach (var sCeiling in serializeSystem.Ceilings)
        {
            Ceilings.Add(new DCeiling(sCeiling));
        }

        foreach (var sRoof in serializeSystem.Roofs)
        {
            Roofs.Add(new DRoof(sRoof));
        }
    }
}
