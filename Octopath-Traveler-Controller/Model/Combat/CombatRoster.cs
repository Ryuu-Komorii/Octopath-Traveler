namespace Octopath_Traveler;

public class CombatRoster
{
    public IReadOnlyList<Traveler> Travelers { get; }
    public IReadOnlyList<Beast> Beasts { get; }

    public CombatRoster(
        IReadOnlyList<Traveler> travelers,
        IReadOnlyList<Beast> beasts)
    {
        Travelers = travelers;
        Beasts = beasts;
    }
}