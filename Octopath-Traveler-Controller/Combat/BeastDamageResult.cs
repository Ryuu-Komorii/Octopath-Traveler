namespace Octopath_Traveler;

public class BeastDamageResult
{
    public int Damage { get; }
    public bool IsWeakness { get; }
    public bool EnteredBreakingPoint { get; }

    public BeastDamageResult(
        int damage,
        bool isWeakness,
        bool enteredBreakingPoint)
    {
        Damage = damage;
        IsWeakness = isWeakness;
        EnteredBreakingPoint =
            enteredBreakingPoint;
    }
}