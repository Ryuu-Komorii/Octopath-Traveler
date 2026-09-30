using Octopath_Traveler_View;

namespace Octopath_Traveler;

public class BoostPointSelectionMenu
{
    private const string SelectionPrompt =
        "Seleccione cuantos BP utilizar";

    private readonly View view;

    public BoostPointSelectionMenu(View view)
    {
        this.view = view;
    }

    public int ReadBoostPoints()
    {
        WritePrompt();

        return int.Parse(
            view.ReadLine());
    }

    private void WritePrompt()
    {
        view.WriteLine(
            CombatText.SeparatorLine);

        view.WriteLine(
            SelectionPrompt);
    }
}