using Octopath_Traveler_View;

namespace Octopath_Traveler;

public class ActiveSkillSelectionMenu
    : SelectionMenu<string>
{
    private readonly ActiveSkillCatalog
        activeSkillCatalog;

    public ActiveSkillSelectionMenu(
        View view,
        ActiveSkillCatalog activeSkillCatalog)
        : base(view)
    {
        this.activeSkillCatalog =
            activeSkillCatalog;
    }

    public string? SelectSkill(
        Traveler traveler)
    {
        string[] availableSkills =
            GetAvailableSkillNames(traveler);

        string header =
            GetSelectionHeader(traveler);

        return SelectOption(
            header,
            availableSkills);
    }

    protected override string FormatOption(
        string skillName)
    {
        return skillName;
    }

    private string[] GetAvailableSkillNames(
        Traveler traveler)
    {
        return traveler.ActiveSkillNames
            .Where(skillName =>
                CanUseSkill(traveler, skillName))
            .ToArray();
    }

    private bool CanUseSkill(
        Traveler traveler,
        string skillName)
    {
        ActiveSkillCatalogEntry skill =
            activeSkillCatalog.Find(skillName);

        return traveler.HasEnoughSkillPoints(
            skill.SP);
    }

    private string GetSelectionHeader(
        Traveler traveler)
    {
        return string.Format(
            CombatText.ActiveSkillSelectionHeaderFormat,
            traveler.Name);
    }
}