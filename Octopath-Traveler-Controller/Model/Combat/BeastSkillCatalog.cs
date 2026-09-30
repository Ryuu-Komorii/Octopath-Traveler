namespace Octopath_Traveler;

public class BeastSkillCatalog
{
    private readonly IReadOnlyList<BeastSkillCatalogEntry> skills;

    public BeastSkillCatalog(
        IReadOnlyList<BeastSkillCatalogEntry> skills)
    {
        this.skills = skills;
    }

    public BeastSkillCatalogEntry Find(
        string skillName)
    {
        return skills.First(
            skill => HasName(
                skill,
                skillName));
    }

    private bool HasName(
        BeastSkillCatalogEntry skill,
        string skillName)
    {
        return skill.Name ==
               skillName;
    }
}