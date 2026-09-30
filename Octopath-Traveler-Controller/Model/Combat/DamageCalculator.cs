namespace Octopath_Traveler;

public class DamageCalculator
{
    private const int MinimumDamage = 0;

    public int Calculate(
        DamageRequest request)
    {
        double baseDamage =
            CalculateBaseDamage(
                request);

        double finalDamage =
            ApplyFinalMultiplier(
                baseDamage,
                request);

        return RoundDown(
            finalDamage);
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

    private double ApplyFinalMultiplier(
        double baseDamage,
        DamageRequest request)
    {
        return baseDamage *
               request.FinalMultiplier;
    }

    private int RoundDown(
        double damage)
    {
        return Convert.ToInt32(
            Math.Floor(damage));
    }
}