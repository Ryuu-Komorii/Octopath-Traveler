namespace Octopath_Traveler;

public class Beast : CombatUnit
{
    private const int MinimumHitPoints = 0;

    public string Name { get; }
    public int MaxHP { get; }
    public int CurrentHP { get; private set; }
    public int PhysicalAttack { get; }
    public int PhysicalDefense { get; }
    public int ElementalAttack { get; }
    public int ElementalDefense { get; }
    public int Speed { get; }
    public string SkillName { get; }
    public int Shields { get; }
    public IReadOnlyList<string> Weaknesses { get; }

    public Beast(BeastCatalogEntry catalogEntry)
    {
        Name = catalogEntry.Name;
        MaxHP = catalogEntry.Stats.HP;
        CurrentHP = MaxHP;
        PhysicalAttack = catalogEntry.Stats.PhysAtk;
        PhysicalDefense = catalogEntry.Stats.PhysDef;
        ElementalAttack = catalogEntry.Stats.ElemAtk;
        ElementalDefense = catalogEntry.Stats.ElemDef;
        Speed = catalogEntry.Stats.Speed;
        SkillName = catalogEntry.Skill;
        Shields = catalogEntry.Shields;
        Weaknesses = catalogEntry.Weaknesses;
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
}