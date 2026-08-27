namespace Octopath_Traveler;

public class DamageCalculator
{
    private const int MinimumDamage = 0;

    public int CalculatePhysicalDamage(
        int physicalAttack,
        int physicalDefense,
        double modifier)
    {
        double rawDamage =
            physicalAttack * modifier - physicalDefense;

        double nonNegativeDamage =
            Math.Max(MinimumDamage, rawDamage);

        return Convert.ToInt32(
            Math.Floor(nonNegativeDamage));
    }
}