namespace Octopath_Traveler;

public class BeastCatalogEntry
{
    public string Name { get; set; } = string.Empty;
    public BeastStats Stats { get; set; } = new();
    public string Skill { get; set; } = string.Empty;
    public int Shields { get; set; }
    public string[] Weaknesses { get; set; } = [];
}

public class BeastStats
{
    public int HP { get; set; }
    public int PhysAtk { get; set; }
    public int PhysDef { get; set; }
    public int ElemAtk { get; set; }
    public int ElemDef { get; set; }
    public int Speed { get; set; }
}