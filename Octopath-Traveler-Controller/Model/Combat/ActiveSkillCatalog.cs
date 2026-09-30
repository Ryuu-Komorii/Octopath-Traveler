namespace Octopath_Traveler;

public class ActiveSkillCatalog
{
    private readonly IReadOnlyList<ActiveSkillCatalogEntry> skills;

    public ActiveSkillCatalog(
        IReadOnlyList<ActiveSkillCatalogEntry> skills)
    {
        this.skills = skills;
    }

    public ActiveSkillCatalogEntry Find(string skillName)
    {
        return skills.First(skill =>
            HasName(skill, skillName));
    }

    private bool HasName(
        ActiveSkillCatalogEntry skill,
        string skillName)
    {
        return skill.Name == skillName;
    }
}