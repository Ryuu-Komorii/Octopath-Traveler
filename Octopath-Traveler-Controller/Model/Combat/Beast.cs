namespace Octopath_Traveler;

public class Beast : CombatUnit
{
    private const int MinimumHitPoints = 0;
    private const int MinimumShields = 0;
    private const int MinimumDamageToLoseShield = 1;
    private const int BreakingDurationInRounds = 2;
    private const int CurrentRoundDuration = 1;

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

    public CombatUnitTypePriority TypePriority =>
        CombatUnitTypePriority.Beast;

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

        if (IsDead())
        {
            return;
        }

        RecoverIfNeeded(
            roundNumber);

        ClearExpiredRecoveryPriority(
            roundNumber);
    }

    public void ApplyTurnPenalty(
        int durationInRounds)
    {
        if (HasActiveTurnPenalty())
        {
            ExtendTurnPenalty(
                durationInRounds);

            return;
        }

        StartTurnPenalty(
            durationInRounds);
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
            return TurnPriority
                .BreakingRecovery;
        }

        if (HasTurnPenalty(
            roundNumber))
        {
            return TurnPriority
                .SkillPenalty;
        }

        return TurnPriority
            .Normal;
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
        if (CannotLoseShield(
            damage))
        {
            return false;
        }

        LoseShield();

        if (HasRemainingShields())
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

    private bool IsDead()
    {
        return !IsAlive();
    }

    private void RecoverIfNeeded(
        int roundNumber)
    {
        if (IsNotScheduledToRecover(
            roundNumber))
        {
            return;
        }

        RecoverFromBreakingPoint(
            roundNumber);
    }

    private bool IsNotScheduledToRecover(
        int roundNumber)
    {
        return !IsScheduledToRecover(
            roundNumber);
    }

    private bool CannotLoseShield(
        int damage)
    {
        return !CanLoseShield(
            damage);
    }

    private bool CanLoseShield(
        int damage)
    {
        return HasDamageToLoseShield(
                   damage) &&
               !IsBreakingPoint() &&
               HasRemainingShields();
    }

    private bool HasDamageToLoseShield(
        int damage)
    {
        return damage >=
               MinimumDamageToLoseShield;
    }

    private void LoseShield()
    {
        Shields--;
    }

    private bool HasRemainingShields()
    {
        return Shields >
               MinimumShields;
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
        if (HasNoBreakingStartRound())
        {
            return false;
        }

        int firstBreakingRound =
            breakingStartedRound!.Value;

        int lastBreakingRound =
            GetLastBreakingRound(
                firstBreakingRound);

        return IsRoundInsideBreakingPeriod(
            roundNumber,
            firstBreakingRound,
            lastBreakingRound);
    }

    private bool HasNoBreakingStartRound()
    {
        return !breakingStartedRound
            .HasValue;
    }

    private int GetLastBreakingRound(
        int firstBreakingRound)
    {
        return firstBreakingRound +
               BreakingDurationInRounds -
               CurrentRoundDuration;
    }

    private bool IsRoundInsideBreakingPeriod(
        int roundNumber,
        int firstBreakingRound,
        int lastBreakingRound)
    {
        return roundNumber >=
                   firstBreakingRound &&
               roundNumber <=
                   lastBreakingRound;
    }

    private bool IsScheduledToRecover(
        int roundNumber)
    {
        if (HasNoBreakingStartRound())
        {
            return false;
        }

        int recoveryRound =
            GetRecoveryRound();

        return roundNumber ==
               recoveryRound;
    }

    private int GetRecoveryRound()
    {
        return breakingStartedRound!.Value +
               BreakingDurationInRounds;
    }

    private bool IsRecoveryRound(
        int roundNumber)
    {
        return HasRecoveryPriority(
                   roundNumber) ||
               IsScheduledToRecover(
                   roundNumber);
    }

    private bool HasRecoveryPriority(
        int roundNumber)
    {
        return recoveryPriorityRound ==
               roundNumber;
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
        if (HasCurrentRecoveryPriority(
            roundNumber))
        {
            return;
        }

        recoveryPriorityRound =
            null;
    }

    private bool HasCurrentRecoveryPriority(
        int roundNumber)
    {
        return recoveryPriorityRound ==
               roundNumber;
    }

    private bool HasActiveTurnPenalty()
    {
        return turnPenaltyThroughRound
                   .HasValue &&
               turnPenaltyThroughRound.Value >=
                   currentRound;
    }

    private void ExtendTurnPenalty(
        int durationInRounds)
    {
        turnPenaltyThroughRound +=
            durationInRounds;
    }

    private void StartTurnPenalty(
        int durationInRounds)
    {
        turnPenaltyThroughRound =
            currentRound +
            durationInRounds -
            CurrentRoundDuration;
    }

    private bool HasTurnPenalty(
        int roundNumber)
    {
        return turnPenaltyThroughRound
                   .HasValue &&
               roundNumber <=
                   turnPenaltyThroughRound.Value;
    }
}