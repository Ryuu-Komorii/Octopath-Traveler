namespace Octopath_Traveler;

public class TravelerTurnContext
{
    public Traveler User { get; init; } = null!;

    public IReadOnlyList<Traveler> Travelers { get; init; } =
        Array.Empty<Traveler>();

    public IReadOnlyList<Beast> Beasts { get; init; } =
        Array.Empty<Beast>();
}