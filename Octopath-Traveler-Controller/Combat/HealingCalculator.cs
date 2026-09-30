namespace Octopath_Traveler;

public class HealingCalculator
{
    public int Calculate(
        int elementalDefense,
        double modifier)
    {
        double healing =
            elementalDefense *
            modifier;

        return Convert.ToInt32(
            Math.Floor(healing));
    }
}