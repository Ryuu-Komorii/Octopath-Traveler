namespace Octopath_Traveler;

public class Traveler : CombatUnit
{
    private const int InitialBoostPoints = 1;
    private const int BoostPointsGainedPerRound = 1;
    private const int MaximumBoostPoints = 5;
    private const int MinimumHitPoints = 0;

    public string Name { get; }
    public int MaxHP { get; }
    public int CurrentHP { get; private set; }
    public int MaxSP { get; }
    public int CurrentSP { get; private set; }
    public int PhysicalAttack { get; }
    public int PhysicalDefense { get; }
    public int ElementalAttack { get; }
    public int ElementalDefense { get; }
    public int Speed { get; }
    public int BoostPoints { get; private set; }
    public IReadOnlyList<string> Weapons { get; }
    public IReadOnlyList<string> ActiveSkillNames { get; }

    public Traveler(
        TravelerCatalogEntry catalogEntry,
        TravelerTeamMember teamMember)
    {
        Name = catalogEntry.Name;
        MaxHP = catalogEntry.Stats.HP;
        CurrentHP = MaxHP;
        MaxSP = catalogEntry.Stats.SP;
        CurrentSP = MaxSP;
        PhysicalAttack = catalogEntry.Stats.PhysAtk;
        PhysicalDefense = catalogEntry.Stats.PhysDef;
        ElementalAttack = catalogEntry.Stats.ElemAtk;
        ElementalDefense = catalogEntry.Stats.ElemDef;
        Speed = catalogEntry.Stats.Speed;
        BoostPoints = InitialBoostPoints;
        Weapons = catalogEntry.Weapons;
        ActiveSkillNames = teamMember.ActiveSkillNames;
    }

    public bool IsAlive()
    {
        return CurrentHP > MinimumHitPoints;
    }

    public void ReceiveDamage(int damage)
    {
        CurrentHP = Math.Max(
            MinimumHitPoints,
            CurrentHP - damage);
    }

    public void GainBoostPoint()
    {
        BoostPoints = Math.Min(
            MaximumBoostPoints,
            BoostPoints + BoostPointsGainedPerRound);
    }
}