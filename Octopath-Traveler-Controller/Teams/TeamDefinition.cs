namespace Octopath_Traveler;

public class TeamDefinition
{
    public IReadOnlyList<TravelerTeamMember> Travelers { get; }
    public IReadOnlyList<string> BeastNames { get; }

    public TeamDefinition(
        IReadOnlyList<TravelerTeamMember> travelers,
        IReadOnlyList<string> beastNames)
    {
        Travelers = travelers;
        BeastNames = beastNames;
    }
}