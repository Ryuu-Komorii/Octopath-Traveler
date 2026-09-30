namespace Octopath_Traveler;

public class TravelerCatalogEntry
{
    public string Name { get; set; } = string.Empty;
    public TravelerStats Stats { get; set; } = new();
    public string[] Weapons { get; set; } = [];
}

public class TravelerStats
{
    public int HP { get; set; }
    public int SP { get; set; }
    public int PhysAtk { get; set; }
    public int PhysDef { get; set; }
    public int ElemAtk { get; set; }
    public int ElemDef { get; set; }
    public int Speed { get; set; }
}