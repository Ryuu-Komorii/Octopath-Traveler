using Octopath_Traveler_View;

namespace Octopath_Traveler;

public class ActiveSkillSelectionMenu
{
    private const string SeparatorLine = "----------------------------------------";
    private const string SelectionHeaderFormat =
        "Seleccione una habilidad para {0}";
    private const string CancelOptionText = "Cancelar";
    private const int FirstOptionNumber = 1;

    private readonly View view;

    public ActiveSkillSelectionMenu(View view)
    {
        this.view = view;
    }

    public string? SelectSkill(Traveler traveler)
    {
        WriteMenu(traveler);
        int selectedOption = ReadSelectedOption();

        return GetSelectedSkill(
            traveler.ActiveSkillNames,
            selectedOption);
    }

    private void WriteMenu(Traveler traveler)
    {
        view.WriteLine(SeparatorLine);
        view.WriteLine(GetSelectionHeader(traveler));

        WriteSkillOptions(traveler.ActiveSkillNames);
        WriteCancelOption(traveler.ActiveSkillNames.Count);
    }

    private string GetSelectionHeader(Traveler traveler)
    {
        return string.Format(
            SelectionHeaderFormat,
            traveler.Name);
    }

    private void WriteSkillOptions(
        IReadOnlyList<string> skillNames)
    {
        int optionNumber = FirstOptionNumber;

        foreach (string skillName in skillNames)
        {
            view.WriteLine($"{optionNumber}: {skillName}");
            optionNumber++;
        }
    }

    private void WriteCancelOption(int skillCount)
    {
        int cancelOption =
            GetCancelOptionNumber(skillCount);

        view.WriteLine(
            $"{cancelOption}: {CancelOptionText}");
    }

    private int ReadSelectedOption()
    {
        return int.Parse(view.ReadLine());
    }

    private string? GetSelectedSkill(
        IReadOnlyList<string> skillNames,
        int selectedOption)
    {
        if (IsCancelOption(skillNames, selectedOption))
        {
            return null;
        }

        int skillIndex =
            selectedOption - FirstOptionNumber;

        return skillNames[skillIndex];
    }

    private bool IsCancelOption(
        IReadOnlyList<string> skillNames,
        int selectedOption)
    {
        return selectedOption ==
               GetCancelOptionNumber(skillNames.Count);
    }

    private int GetCancelOptionNumber(int skillCount)
    {
        return skillCount + FirstOptionNumber;
    }
}