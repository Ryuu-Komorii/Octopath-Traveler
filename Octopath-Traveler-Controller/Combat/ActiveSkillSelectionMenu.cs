using Octopath_Traveler_View;

namespace Octopath_Traveler;

public class ActiveSkillSelectionMenu
{
    private const string SeparatorLine =
        "----------------------------------------";

    private const string SelectionHeaderFormat =
        "Seleccione una habilidad para {0}";

    private const string CancelOptionText =
        "Cancelar";

    private const int FirstOptionNumber = 1;

    private readonly View view;
    private readonly ActiveSkillCatalog activeSkillCatalog;

    public ActiveSkillSelectionMenu(
        View view,
        ActiveSkillCatalog activeSkillCatalog)
    {
        this.view = view;
        this.activeSkillCatalog =
            activeSkillCatalog;
    }

    public string? SelectSkill(
        Traveler traveler)
    {
        string[] availableSkillNames =
            GetAvailableSkillNames(
                traveler);

        WriteMenu(
            traveler,
            availableSkillNames);

        int selectedOption =
            ReadSelectedOption();

        return GetSelectedSkill(
            availableSkillNames,
            selectedOption);
    }

    private string[] GetAvailableSkillNames(
        Traveler traveler)
    {
        return traveler.ActiveSkillNames
            .Where(skillName =>
                CanUseSkill(
                    traveler,
                    skillName))
            .ToArray();
    }

    private bool CanUseSkill(
        Traveler traveler,
        string skillName)
    {
        ActiveSkillCatalogEntry skill =
            activeSkillCatalog.Find(
                skillName);

        return traveler.HasEnoughSkillPoints(
            skill.SP);
    }

    private void WriteMenu(
        Traveler traveler,
        IReadOnlyList<string> skillNames)
    {
        view.WriteLine(
            SeparatorLine);

        view.WriteLine(
            string.Format(
                SelectionHeaderFormat,
                traveler.Name));

        WriteSkillOptions(
            skillNames);

        WriteCancelOption(
            skillNames.Count);
    }

    private void WriteSkillOptions(
        IReadOnlyList<string> skillNames)
    {
        int optionNumber =
            FirstOptionNumber;

        foreach (string skillName in skillNames)
        {
            view.WriteLine(
                $"{optionNumber}: {skillName}");

            optionNumber++;
        }
    }

    private void WriteCancelOption(
        int skillCount)
    {
        int cancelOption =
            GetCancelOptionNumber(
                skillCount);

        view.WriteLine(
            $"{cancelOption}: {CancelOptionText}");
    }

    private int ReadSelectedOption()
    {
        return int.Parse(
            view.ReadLine());
    }

    private string? GetSelectedSkill(
        IReadOnlyList<string> skillNames,
        int selectedOption)
    {
        if (IsCancelOption(
            skillNames,
            selectedOption))
        {
            return null;
        }

        int skillIndex =
            selectedOption -
            FirstOptionNumber;

        return skillNames[
            skillIndex];
    }

    private bool IsCancelOption(
        IReadOnlyList<string> skillNames,
        int selectedOption)
    {
        return selectedOption ==
               GetCancelOptionNumber(
                   skillNames.Count);
    }

    private int GetCancelOptionNumber(
        int skillCount)
    {
        return skillCount +
               FirstOptionNumber;
    }
}