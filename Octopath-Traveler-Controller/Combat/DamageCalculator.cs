namespace Octopath_Traveler;

public class DamageCalculator
{
    private const int MinimumDamage = 0;
    private const double StandardFinalMultiplier = 1.0;

    public int CalculatePhysicalDamage(
        int physicalAttack,
        int physicalDefense,
        double modifier)
    {
        DamageRequest request =
            CreateStandardRequest(
                physicalAttack,
                physicalDefense,
                modifier);

        return Calculate(request);
    }

    public int CalculateElementalDamage(
        int elementalAttack,
        int elementalDefense,
        double modifier)
    {
        DamageRequest request =
            CreateStandardRequest(
                elementalAttack,
                elementalDefense,
                modifier);

        return Calculate(request);
    }

    public int Calculate(
        DamageRequest request)
    {
        double baseDamage =
            CalculateBaseDamage(request);

        double finalDamage =
            baseDamage *
            request.FinalMultiplier;

        return Convert.ToInt32(
            Math.Floor(finalDamage));
    }

    private DamageRequest CreateStandardRequest(
        int offensiveStat,
        int defensiveStat,
        double modifier)
    {
        return new DamageRequest
        {
            OffensiveStat = offensiveStat,
            DefensiveStat = defensiveStat,
            Modifier = modifier,
            FinalMultiplier = StandardFinalMultiplier
        };
    }

    private double CalculateBaseDamage(
        DamageRequest request)
    {
        double rawDamage =
            request.OffensiveStat *
            request.Modifier -
            request.DefensiveStat;

        return Math.Max(
            MinimumDamage,
            rawDamage);
    }
}