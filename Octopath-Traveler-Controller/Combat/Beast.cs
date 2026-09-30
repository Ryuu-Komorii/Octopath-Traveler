namespace Octopath_Traveler;

public class Beast : CombatUnit
{
    private const int MinimumHitPoints = 0;
    private const int MinimumShields = 0;
    private const int BreakingDurationInRounds = 2;

    private int currentRound;
    private int? breakingStartedRound;
    private int? recoveryPriorityRound;
    private int? turnPenaltyThroughRound;

    public string Name { get; }

    public int MaxHP { get; }
    public int CurrentHP { get; private set; }

    public int PhysicalAttack { get; }
    public int PhysicalDefense { get; }

    public int ElementalAttack { get; }
    public int ElementalDefense { get; }

    public int Speed { get; }

    public string SkillName { get; }

    public int MaxShields { get; }
    public int Shields { get; private set; }

    public IReadOnlyList<string> Weaknesses { get; }

    public Beast(
        BeastCatalogEntry catalogEntry)
    {
        Name =
            catalogEntry.Name;

        MaxHP =
            catalogEntry.Stats.HP;

        CurrentHP =
            MaxHP;

        PhysicalAttack =
            catalogEntry.Stats.PhysAtk;

        PhysicalDefense =
            catalogEntry.Stats.PhysDef;

        ElementalAttack =
            catalogEntry.Stats.ElemAtk;

        ElementalDefense =
            catalogEntry.Stats.ElemDef;

        Speed =
            catalogEntry.Stats.Speed;

        SkillName =
            catalogEntry.Skill;

        MaxShields =
            catalogEntry.Shields;

        Shields =
            MaxShields;

        Weaknesses =
            catalogEntry.Weaknesses;
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

        if (!IsAlive())
        {
            return;
        }

        if (IsScheduledToRecover(
            roundNumber))
        {
            RecoverFromBreakingPoint(
                roundNumber);

            return;
        }

        ClearExpiredRecoveryPriority(
            roundNumber);
    }

    public void ApplyTurnPenalty(
        int durationInRounds)
    {
        if (HasActiveTurnPenalty())
        {
            turnPenaltyThroughRound +=
                durationInRounds;

            return;
        }

        turnPenaltyThroughRound =
            currentRound +
            durationInRounds - 1;
    }

    public bool CanActInRound(
        int roundNumber)
    {
        return IsAlive() &&
               !IsBreakingInRound(
                   roundNumber);
    }

    public TurnPriority GetTurnPriority(
        int roundNumber)
    {
        if (IsRecoveryRound(
            roundNumber))
        {
            return TurnPriority.BreakingRecovery;
        }

        if (HasTurnPenalty(
            roundNumber))
        {
            return TurnPriority.SkillPenalty;
        }

        return TurnPriority.Normal;
    }

    public bool IsBreakingPoint()
    {
        return IsBreakingInRound(
            currentRound);
    }

    public bool HasWeakness(
        string attackType)
    {
        return Weaknesses.Contains(
            attackType);
    }

    public bool RegisterWeaknessHit(
        int damage)
    {
        if (!CanLoseShield(
            damage))
        {
            return false;
        }

        Shields--;

        if (Shields >
            MinimumShields)
        {
            return false;
        }

        EnterBreakingPoint();

        return true;
    }

    public void ReceiveDamage(
        int damage)
    {
        CurrentHP =
            Math.Max(
                MinimumHitPoints,
                CurrentHP - damage);
    }

    private bool CanLoseShield(
        int damage)
    {
        return damage > 0 &&
               !IsBreakingPoint() &&
               Shields > MinimumShields;
    }

    private void EnterBreakingPoint()
    {
        Shields =
            MinimumShields;

        breakingStartedRound =
            currentRound;
    }

    private bool IsBreakingInRound(
        int roundNumber)
    {
        if (!breakingStartedRound.HasValue)
        {
            return false;
        }

        int firstBreakingRound =
            breakingStartedRound.Value;

        int lastBreakingRound =
            firstBreakingRound +
            BreakingDurationInRounds - 1;

        return roundNumber >= firstBreakingRound &&
               roundNumber <= lastBreakingRound;
    }

    private bool IsScheduledToRecover(
        int roundNumber)
    {
        if (!breakingStartedRound.HasValue)
        {
            return false;
        }

        int recoveryRound =
            breakingStartedRound.Value +
            BreakingDurationInRounds;

        return roundNumber ==
               recoveryRound;
    }

    private bool IsRecoveryRound(
        int roundNumber)
    {
        return recoveryPriorityRound ==
                   roundNumber ||
               IsScheduledToRecover(
                   roundNumber);
    }

    private void RecoverFromBreakingPoint(
        int roundNumber)
    {
        Shields =
            MaxShields;

        recoveryPriorityRound =
            roundNumber;

        breakingStartedRound =
            null;
    }

    private void ClearExpiredRecoveryPriority(
        int roundNumber)
    {
        if (recoveryPriorityRound !=
            roundNumber)
        {
            recoveryPriorityRound =
                null;
        }
    }

    private bool HasActiveTurnPenalty()
    {
        return turnPenaltyThroughRound.HasValue &&
               turnPenaltyThroughRound.Value >=
               currentRound;
    }

    private bool HasTurnPenalty(
        int roundNumber)
    {
        return turnPenaltyThroughRound.HasValue &&
               roundNumber <=
               turnPenaltyThroughRound.Value;
    }
}