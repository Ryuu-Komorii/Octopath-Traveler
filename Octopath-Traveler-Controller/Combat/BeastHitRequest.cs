namespace Octopath_Traveler;

public class BeastHitRequest
{
    public DamageRequest DamageRequest { get; init; } = new();

    public Beast Target { get; init; } = null!;

    public string AttackType { get; init; } =
        string.Empty;

    public int MinimumRemainingHitPoints { get; init; }
}