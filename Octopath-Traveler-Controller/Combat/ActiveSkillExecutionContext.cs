namespace Octopath_Traveler;

public class ActiveSkillExecutionContext
{
    public Traveler User { get; }
    public IReadOnlyList<Traveler> Travelers { get; }
    public IReadOnlyList<Beast> Beasts { get; }
    public ActiveSkillCatalogEntry Skill { get; }

    public ActiveSkillExecutionContext(
        Traveler user,
        IReadOnlyList<Traveler> travelers,
        IReadOnlyList<Beast> beasts,
        ActiveSkillCatalogEntry skill)
    {
        User = user;
        Travelers = travelers;
        Beasts = beasts;
        Skill = skill;
    }
}