using Octopath_Traveler_View;

namespace Octopath_Traveler;

public class TravelerActionMenu
{
    private const string SeparatorLine = "----------------------------------------";
    private const string TurnHeaderFormat = "Turno de {0}";
    private const string BasicAttackOptionText = "1: Ataque básico";
    private const string UseSkillOptionText = "2: Usar habilidad";
    private const string DefendOptionText = "3: Defender";
    private const string RunAwayOptionText = "4: Huir";

    private const int BasicAttackOption = 1;
    private const int UseSkillOption = 2;
    private const int DefendOption = 3;
    private const int RunAwayOption = 4;

    private readonly View view;

    public TravelerActionMenu(View view)
    {
        this.view = view;
    }

    public TravelerAction ReadAction(Traveler traveler)
    {
        WriteMenu(traveler);
        int selectedOption = ReadSelectedOption();

        return ConvertToAction(selectedOption);
    }

    private void WriteMenu(Traveler traveler)
    {
        view.WriteLine(SeparatorLine);
        view.WriteLine(GetTurnHeader(traveler));
        view.WriteLine(BasicAttackOptionText);
        view.WriteLine(UseSkillOptionText);
        view.WriteLine(DefendOptionText);
        view.WriteLine(RunAwayOptionText);
    }

    private string GetTurnHeader(Traveler traveler)
    {
        return string.Format(
            TurnHeaderFormat,
            traveler.Name);
    }

    private int ReadSelectedOption()
    {
        return int.Parse(view.ReadLine());
    }

    private TravelerAction ConvertToAction(int selectedOption)
    {
        return selectedOption switch
        {
            BasicAttackOption => TravelerAction.BasicAttack,
            UseSkillOption => TravelerAction.UseSkill,
            DefendOption => TravelerAction.Defend,
            RunAwayOption => TravelerAction.RunAway,
            _ => throw new ArgumentOutOfRangeException(
                nameof(selectedOption))
        };
    }
}