namespace Octopath_Traveler;

public class BasicAttackContext
{
    public Traveler Attacker { get; init; } = null!;

    public Beast Target { get; init; } = null!;

    public string Weapon { get; init; } =
        string.Empty;
}