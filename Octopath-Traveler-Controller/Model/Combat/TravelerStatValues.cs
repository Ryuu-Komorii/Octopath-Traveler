namespace Octopath_Traveler;

public class TravelerStatValues
{
    public int MaxHitPoints { get; set; }
    public int MaxSkillPoints { get; set; }
    public int PhysicalAttack { get; set; }
    public int PhysicalDefense { get; set; }
    public int ElementalAttack { get; set; }
    public int ElementalDefense { get; set; }
    public int Speed { get; set; }

    public TravelerStatValues(
        TravelerStats catalogStats)
    {
        MaxHitPoints =
            catalogStats.HP;

        MaxSkillPoints =
            catalogStats.SP;

        PhysicalAttack =
            catalogStats.PhysAtk;

        PhysicalDefense =
            catalogStats.PhysDef;

        ElementalAttack =
            catalogStats.ElemAtk;

        ElementalDefense =
            catalogStats.ElemDef;

        Speed =
            catalogStats.Speed;
    }
}