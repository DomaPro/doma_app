using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class SerializeSystem
{
    public List<SFloor> Floors { get; set; }
    public List<SWall> Walls { get; set; }
    public List<SCeiling> Ceilings { get; set; }
    public List<SRoof> Roofs { get; set; }

    public SerializeSystem(AppSystem appSystem)
    {
        Floors = new List<SFloor>();
        Walls = new List<SWall>();
        Ceilings = new List<SCeiling>();
        Roofs = new List<SRoof>();

        foreach (var dFloor in appSystem.Floors)
        {
            Floors.Add(new SFloor(dFloor));
        }

        foreach (var dWall in appSystem.Walls)
        {
            Walls.Add(new SWall(dWall));
        }

        foreach (var dCeiling in appSystem.Ceilings)
        {
            Ceilings.Add(new SCeiling(dCeiling));
        }

        foreach (var dRoof in appSystem.Roofs)
        {
           Roofs.Add(new SRoof(dRoof));
        }
    }
}
