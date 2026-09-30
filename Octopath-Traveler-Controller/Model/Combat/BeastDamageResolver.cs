namespace Octopath_Traveler;

public class BeastDamageResolver
{
    private const double NormalMultiplier = 1.0;
    private const double WeaknessMultiplier = 1.5;
    private const double BreakingMultiplier = 1.5;
    private const double WeaknessAndBreakingMultiplier = 2.0;

    private const int MinimumDamage = 0;
    private const int NoMinimumRemainingHitPoints = 0;

    private readonly DamageCalculator damageCalculator;

    public BeastDamageResolver()
    {
        damageCalculator =
            new DamageCalculator();
    }

    public BeastDamageResult Resolve(
        DamageRequest damageRequest,
        Beast target,
        string attackType)
    {
        BeastHitRequest hitRequest =
            CreateHitRequest(
                damageRequest,
                target,
                attackType);

        return Resolve(
            hitRequest);
    }

    public BeastDamageResult Resolve(
        BeastHitRequest hitRequest)
    {
        BeastDamageCondition damageCondition =
            GetDamageCondition(
                hitRequest.Target,
                hitRequest.AttackType);

        DamageRequest finalDamageRequest =
            CreateFinalDamageRequest(
                hitRequest.DamageRequest,
                damageCondition);

        int calculatedDamage =
            damageCalculator.Calculate(
                finalDamageRequest);

        int finalDamage =
            GetFinalDamage(
                hitRequest.Target,
                calculatedDamage,
                hitRequest.MinimumRemainingHitPoints);

        bool enteredBreakingPoint =
            RegisterWeaknessHit(
                hitRequest.Target,
                damageCondition,
                finalDamage);

        hitRequest.Target.ReceiveDamage(
            finalDamage);

        return new BeastDamageResult
        {
            Damage =
                finalDamage,

            IsWeakness =
                HasWeakness(
                    damageCondition),

            EnteredBreakingPoint =
                enteredBreakingPoint
        };
    }

    private BeastHitRequest CreateHitRequest(
        DamageRequest damageRequest,
        Beast target,
        string attackType)
    {
        return new BeastHitRequest
        {
            DamageRequest =
                damageRequest,

            Target =
                target,

            AttackType =
                attackType,

            MinimumRemainingHitPoints =
                NoMinimumRemainingHitPoints
        };
    }

    private BeastDamageCondition GetDamageCondition(
        Beast target,
        string attackType)
    {
        if (HasWeaknessAndBreaking(
            target,
            attackType))
        {
            return BeastDamageCondition
                .WeaknessAndBreaking;
        }

        if (target.HasWeakness(
            attackType))
        {
            return BeastDamageCondition
                .Weakness;
        }

        if (target.IsBreakingPoint())
        {
            return BeastDamageCondition
                .Breaking;
        }

        return BeastDamageCondition
            .Normal;
    }

    private bool HasWeaknessAndBreaking(
        Beast target,
        string attackType)
    {
        return target.HasWeakness(
                   attackType) &&
               target.IsBreakingPoint();
    }

    private DamageRequest CreateFinalDamageRequest(
        DamageRequest originalRequest,
        BeastDamageCondition damageCondition)
    {
        return new DamageRequest
        {
            OffensiveStat =
                originalRequest.OffensiveStat,

            DefensiveStat =
                originalRequest.DefensiveStat,

            Modifier =
                originalRequest.Modifier,

            FinalMultiplier =
                originalRequest.FinalMultiplier *
                GetCombatMultiplier(
                    damageCondition)
        };
    }

    private double GetCombatMultiplier(
        BeastDamageCondition damageCondition)
    {
        return damageCondition switch
        {
            BeastDamageCondition.Weakness =>
                WeaknessMultiplier,

            BeastDamageCondition.Breaking =>
                BreakingMultiplier,

            BeastDamageCondition.WeaknessAndBreaking =>
                WeaknessAndBreakingMultiplier,

            _ =>
                NormalMultiplier
        };
    }

    private int GetFinalDamage(
        Beast target,
        int calculatedDamage,
        int minimumRemainingHitPoints)
    {
        if (HasNoMinimumHitPoints(
            minimumRemainingHitPoints))
        {
            return calculatedDamage;
        }

        return LimitDamageToMinimumHitPoints(
            target,
            calculatedDamage,
            minimumRemainingHitPoints);
    }

    private bool HasNoMinimumHitPoints(
        int minimumRemainingHitPoints)
    {
        return minimumRemainingHitPoints ==
               NoMinimumRemainingHitPoints;
    }

    private int LimitDamageToMinimumHitPoints(
        Beast target,
        int calculatedDamage,
        int minimumRemainingHitPoints)
    {
        int maximumAllowedDamage =
            target.CurrentHP -
            minimumRemainingHitPoints;

        maximumAllowedDamage =
            Math.Max(
                MinimumDamage,
                maximumAllowedDamage);

        return Math.Min(
            calculatedDamage,
            maximumAllowedDamage);
    }

    private bool RegisterWeaknessHit(
        Beast target,
        BeastDamageCondition damageCondition,
        int damage)
    {
        if (HasNoWeakness(
            damageCondition))
        {
            return false;
        }

        return target.RegisterWeaknessHit(
            damage);
    }

    private bool HasNoWeakness(
        BeastDamageCondition damageCondition)
    {
        return damageCondition is
            BeastDamageCondition.Normal or
            BeastDamageCondition.Breaking;
    }

    private bool HasWeakness(
        BeastDamageCondition damageCondition)
    {
        return damageCondition is
            BeastDamageCondition.Weakness or
            BeastDamageCondition.WeaknessAndBreaking;
    }
}