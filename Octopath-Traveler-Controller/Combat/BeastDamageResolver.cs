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
            new BeastHitRequest
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

        return Resolve(
            hitRequest);
    }

    public BeastDamageResult Resolve(
        BeastHitRequest hitRequest)
    {
        Beast target =
            hitRequest.Target;

        bool isWeakness =
            target.HasWeakness(
                hitRequest.AttackType);

        bool wasBreaking =
            target.IsBreakingPoint();

        double combatMultiplier =
            GetCombatMultiplier(
                isWeakness,
                wasBreaking);

        DamageRequest finalDamageRequest =
            CreateFinalDamageRequest(
                hitRequest.DamageRequest,
                combatMultiplier);

        int calculatedDamage =
            damageCalculator.Calculate(
                finalDamageRequest);

        int finalDamage =
            GetFinalDamage(
                target,
                calculatedDamage,
                hitRequest.MinimumRemainingHitPoints);

        bool enteredBreakingPoint =
            RegisterWeaknessHit(
                target,
                isWeakness,
                finalDamage);

        target.ReceiveDamage(
            finalDamage);

        return new BeastDamageResult(
            finalDamage,
            isWeakness,
            enteredBreakingPoint);
    }

    private DamageRequest CreateFinalDamageRequest(
        DamageRequest originalRequest,
        double combatMultiplier)
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
                combatMultiplier
        };
    }

    private double GetCombatMultiplier(
        bool isWeakness,
        bool isBreaking)
    {
        if (isWeakness &&
            isBreaking)
        {
            return WeaknessAndBreakingMultiplier;
        }

        if (isWeakness)
        {
            return WeaknessMultiplier;
        }

        if (isBreaking)
        {
            return BreakingMultiplier;
        }

        return NormalMultiplier;
    }

    private int GetFinalDamage(
        Beast target,
        int calculatedDamage,
        int minimumRemainingHitPoints)
    {
        if (!HasMinimumRemainingHitPoints(
            minimumRemainingHitPoints))
        {
            return calculatedDamage;
        }

        return LimitDamageToMinimumHitPoints(
            target,
            calculatedDamage,
            minimumRemainingHitPoints);
    }

    private bool HasMinimumRemainingHitPoints(
        int minimumRemainingHitPoints)
    {
        return minimumRemainingHitPoints >
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
        bool isWeakness,
        int damage)
    {
        if (!isWeakness)
        {
            return false;
        }

        return target.RegisterWeaknessHit(
            damage);
    }
}