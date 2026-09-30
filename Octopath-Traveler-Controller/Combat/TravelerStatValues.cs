namespace Octopath_Traveler;

public class TravelerStatValues
{
    public int MaxHitPoints { get; private set; }
    public int MaxSkillPoints { get; private set; }
    public int PhysicalAttack { get; private set; }
    public int PhysicalDefense { get; }
    public int ElementalAttack { get; private set; }
    public int ElementalDefense { get; }
    public int Speed { get; private set; }

    public TravelerStatValues(TravelerStats catalogStats)
    {
        MaxHitPoints = catalogStats.HP;
        MaxSkillPoints = catalogStats.SP;
        PhysicalAttack = catalogStats.PhysAtk;
        PhysicalDefense = catalogStats.PhysDef;
        ElementalAttack = catalogStats.ElemAtk;
        ElementalDefense = catalogStats.ElemDef;
        Speed = catalogStats.Speed;
    }

    public void IncreaseMaxHitPoints(int amount)
    {
        MaxHitPoints += amount;
    }

    public void IncreaseMaxSkillPoints(int amount)
    {
        MaxSkillPoints += amount;
    }

    public void IncreasePhysicalAttack(int amount)
    {
        PhysicalAttack += amount;
    }

    public void IncreaseElementalAttack(int amount)
    {
        ElementalAttack += amount;
    }

    public void IncreaseSpeed(int amount)
    {
        Speed += amount;
    }
}