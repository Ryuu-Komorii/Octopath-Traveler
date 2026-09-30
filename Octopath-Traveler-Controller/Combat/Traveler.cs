namespace Octopath_Traveler;

public class Traveler : CombatUnit
{
    private const int InitialBoostPoints = 1;
    private const int BoostPointsGainedPerRound = 1;
    private const int MaximumBoostPoints = 5;
    private const int MinimumHitPoints = 0;
    private const int RevivalHitPoints = 1;

    private int currentRound;
    private int? defendedRound;
    private int? skillPriorityRound;

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
    public IReadOnlyList<string> PassiveSkillNames { get; }

    public Traveler(
        TravelerCatalogEntry catalogEntry,
        TravelerTeamMember teamMember,
        TravelerStatValues statValues)
    {
        Name = catalogEntry.Name;

        MaxHP = statValues.MaxHitPoints;
        CurrentHP = MaxHP;

        MaxSP = statValues.MaxSkillPoints;
        CurrentSP = MaxSP;

        PhysicalAttack = statValues.PhysicalAttack;
        PhysicalDefense = statValues.PhysicalDefense;
        ElementalAttack = statValues.ElementalAttack;
        ElementalDefense = statValues.ElementalDefense;
        Speed = statValues.Speed;

        BoostPoints = InitialBoostPoints;

        Weapons = catalogEntry.Weapons;
        ActiveSkillNames = teamMember.ActiveSkillNames;
        PassiveSkillNames = teamMember.PassiveSkillNames;
    }

    public bool IsAlive()
    {
        return CurrentHP >
               MinimumHitPoints;
    }

    public void BeginRound(
        int roundNumber)
    {
        currentRound =
            roundNumber;
    }

    public void Defend()
    {
        defendedRound =
            currentRound;
    }

    public void GrantSkillPriorityForNextRound()
    {
        skillPriorityRound =
            currentRound + 1;
    }

    public bool IsDefending()
    {
        return defendedRound ==
               currentRound;
    }

    public bool CanActInRound(
        int roundNumber)
    {
        return IsAlive();
    }

    public TurnPriority GetTurnPriority(
        int roundNumber)
    {
        if (DefendedPreviousRound(
            roundNumber))
        {
            return TurnPriority.Defend;
        }

        if (HasSkillPriority(
            roundNumber))
        {
            return TurnPriority.SkillPriority;
        }

        return TurnPriority.Normal;
    }

    public bool HasEnoughSkillPoints(
        int skillPointCost)
    {
        return CurrentSP >=
               skillPointCost;
    }

    public void ReceiveDamage(
        int damage)
    {
        CurrentHP =
            Math.Max(
                MinimumHitPoints,
                CurrentHP - damage);
    }

    public void RecoverHitPoints(
        int amount)
    {
        CurrentHP =
            Math.Min(
                MaxHP,
                CurrentHP + amount);
    }

    public void Revive()
    {
        if (IsAlive())
        {
            return;
        }

        CurrentHP =
            RevivalHitPoints;
    }

    public void SpendSkillPoints(
        int skillPointCost)
    {
        CurrentSP -=
            skillPointCost;
    }

    public void GainBoostPoint()
    {
        BoostPoints =
            Math.Min(
                MaximumBoostPoints,
                BoostPoints +
                BoostPointsGainedPerRound);
    }

    private bool DefendedPreviousRound(
        int roundNumber)
    {
        return defendedRound ==
               roundNumber - 1;
    }

    private bool HasSkillPriority(
        int roundNumber)
    {
        return skillPriorityRound ==
               roundNumber;
    }
}