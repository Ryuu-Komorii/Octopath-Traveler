namespace Octopath_Traveler;

public class ActiveSkillExecutionContext
{
    public Traveler User { get; init; } = null!;

    public IReadOnlyList<Traveler> Travelers { get; init; } =
        Array.Empty<Traveler>();

    public IReadOnlyList<Beast> Beasts { get; init; } =
        Array.Empty<Beast>();

    public ActiveSkillCatalogEntry Skill { get; init; } = null!;
}