namespace Octopath_Traveler;

public class DamageRequest
{
    private const double DefaultFinalMultiplier =
        1.0;

    public int OffensiveStat { get; init; }

    public int DefensiveStat { get; init; }

    public double Modifier { get; init; }

    public double FinalMultiplier { get; init; } =
        DefaultFinalMultiplier;
}