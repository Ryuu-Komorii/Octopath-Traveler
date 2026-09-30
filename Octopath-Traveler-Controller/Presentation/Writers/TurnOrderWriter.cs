using Octopath_Traveler_View;

namespace Octopath_Traveler;

public class TurnOrderWriter
{
    private const string CurrentRoundHeader =
        "Turnos de la ronda";

    private const string NextRoundHeader =
        "Turnos de la siguiente ronda";

    private const string TurnFormat =
        "{0}.{1}";

    private const int FirstTurnNumber =
        1;

    private readonly View view;

    public TurnOrderWriter(
        View view)
    {
        this.view =
            view;
    }

    public void Write(
        IReadOnlyList<CombatUnit> currentRoundOrder,
        IReadOnlyList<CombatUnit> nextRoundOrder)
    {
        WriteCurrentRoundOrder(
            currentRoundOrder);

        WriteNextRoundOrder(
            nextRoundOrder);
    }

    private void WriteCurrentRoundOrder(
        IReadOnlyList<CombatUnit> turnOrder)
    {
        WriteTurnOrder(
            CurrentRoundHeader,
            turnOrder);
    }

    private void WriteNextRoundOrder(
        IReadOnlyList<CombatUnit> turnOrder)
    {
        view.WriteLine(
            CombatText.SeparatorLine);

        WriteTurnOrder(
            NextRoundHeader,
            turnOrder);
    }

    private void WriteTurnOrder(
        string header,
        IReadOnlyList<CombatUnit> turnOrder)
    {
        view.WriteLine(
            header);

        int turnNumber =
            FirstTurnNumber;

        foreach (CombatUnit unit in turnOrder)
        {
            WriteTurn(
                unit,
                turnNumber);

            turnNumber++;
        }
    }

    private void WriteTurn(
        CombatUnit unit,
        int turnNumber)
    {
        view.WriteLine(
            string.Format(
                TurnFormat,
                turnNumber,
                unit.Name));
    }
}