using Octopath_Traveler_View;

namespace Octopath_Traveler;

public class BattleWriter
{
    private const string RoundStartMessageFormat =
        "INICIA RONDA {0}";

    private const string PlayerVictoryMessage =
        "Gana equipo del jugador";

    private const string EnemyVictoryMessage =
        "Gana equipo del enemigo";

    private readonly View view;

    private readonly BattleStateWriter
        stateWriter;

    private readonly TurnOrderWriter
        turnOrderWriter;

    public BattleWriter(View view)
    {
        this.view =
            view;

        stateWriter =
            new BattleStateWriter(
                view);

        turnOrderWriter =
            new TurnOrderWriter(
                view);
    }

    public void WriteRoundHeader(
        int roundNumber)
    {
        view.WriteLine(
            CombatText.SeparatorLine);

        view.WriteLine(
            string.Format(
                RoundStartMessageFormat,
                roundNumber));
    }

    public void WriteTurnContext(
        CombatRoster roster,
        IReadOnlyList<CombatUnit> pendingTurns,
        IReadOnlyList<CombatUnit> nextRoundOrder)
    {
        view.WriteLine(
            CombatText.SeparatorLine);

        stateWriter.Write(
            roster);

        view.WriteLine(
            CombatText.SeparatorLine);

        turnOrderWriter.Write(
            pendingTurns,
            nextRoundOrder);
    }

    public void WriteWinner(
        BattleOutcome outcome)
    {
        view.WriteLine(
            CombatText.SeparatorLine);

        if (DidPlayerWin(outcome))
        {
            view.WriteLine(
                PlayerVictoryMessage);

            return;
        }

        view.WriteLine(
            EnemyVictoryMessage);
    }

    private bool DidPlayerWin(
        BattleOutcome outcome)
    {
        return outcome ==
               BattleOutcome.PlayerWon;
    }
}