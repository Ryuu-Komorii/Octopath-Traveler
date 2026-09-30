namespace Octopath_Traveler;

public class DamageRequest
{
    public int OffensiveStat { get; init; }
    public int DefensiveStat { get; init; }
    public double Modifier { get; init; }
    public double FinalMultiplier { get; init; } = 1.0;
}