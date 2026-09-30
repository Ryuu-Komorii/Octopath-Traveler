namespace Octopath_Traveler;

public class BattleRound
{
    private const int FirstPendingTurnIndex = 0;
    private const int NextRoundOffset = 1;

    private readonly TurnOrderBuilder
        turnOrderBuilder;

    private readonly CombatTurnExecutor
        turnExecutor;

    private readonly BattleOutcomeEvaluator
        outcomeEvaluator;

    private readonly BattleWriter
        battleWriter;

    public BattleRound(
        BattleRoundServices services)
    {
        turnOrderBuilder =
            services.TurnOrderBuilder;

        turnExecutor =
            services.TurnExecutor;

        outcomeEvaluator =
            services.OutcomeEvaluator;

        battleWriter =
            services.BattleWriter;
    }

    public BattleOutcome Play(
        CombatRoster roster,
        int roundNumber)
    {
        PrepareUnits(
            roster,
            roundNumber);

        battleWriter.WriteRoundHeader(
            roundNumber);

        List<CombatUnit> pendingTurns =
            CreatePendingTurns(
                roster,
                roundNumber);

        BattleOutcome outcome =
            PlayPendingTurns(
                roster,
                pendingTurns,
                roundNumber);

        GainBoostPointsIfBattleContinues(
            roster,
            outcome);

        return outcome;
    }

    private void PrepareUnits(
        CombatRoster roster,
        int roundNumber)
    {
        IEnumerable<CombatUnit> units =
            GetAllUnits(
                roster);

        foreach (CombatUnit unit in units)
        {
            unit.BeginRound(
                roundNumber);
        }
    }

    private IEnumerable<CombatUnit> GetAllUnits(
        CombatRoster roster)
    {
        return roster.Travelers
            .Cast<CombatUnit>()
            .Concat(
                roster.Beasts);
    }

    private List<CombatUnit> CreatePendingTurns(
        CombatRoster roster,
        int roundNumber)
    {
        return turnOrderBuilder
            .Build(
                roster,
                roundNumber)
            .ToList();
    }

    private BattleOutcome PlayPendingTurns(
        CombatRoster roster,
        List<CombatUnit> pendingTurns,
        int roundNumber)
    {
        while (HasPendingTurns(
            pendingTurns))
        {
            BattleOutcome outcome =
                PlayNextTurn(
                    roster,
                    pendingTurns,
                    roundNumber);

            if (outcomeEvaluator.HasBattleEnded(
                outcome))
            {
                return outcome;
            }
        }

        return BattleOutcome.InProgress;
    }

    private BattleOutcome PlayNextTurn(
        CombatRoster roster,
        List<CombatUnit> pendingTurns,
        int roundNumber)
    {
        PrepareTurnQueue(
            roster,
            pendingTurns,
            roundNumber);

        BattleOutcome outcome =
            outcomeEvaluator.Evaluate(
                roster);

        if (outcomeEvaluator.HasBattleEnded(
            outcome))
        {
            return outcome;
        }

        if (HasNoPendingTurns(
            pendingTurns))
        {
            return BattleOutcome.InProgress;
        }

        WriteTurnContext(
            roster,
            pendingTurns,
            roundNumber);

        return ExecuteNextTurn(
            roster,
            pendingTurns);
    }

    private void PrepareTurnQueue(
        CombatRoster roster,
        List<CombatUnit> pendingTurns,
        int roundNumber)
    {
        RemoveUnavailableUnits(
            pendingTurns,
            roundNumber);

        ReorderPendingTurns(
            roster,
            pendingTurns,
            roundNumber);
    }

    private void RemoveUnavailableUnits(
        List<CombatUnit> pendingTurns,
        int roundNumber)
    {
        pendingTurns.RemoveAll(
            combatUnit =>
                CannotAct(
                    combatUnit,
                    roundNumber));
    }

    private bool CannotAct(
        CombatUnit combatUnit,
        int roundNumber)
    {
        return !combatUnit.CanActInRound(
            roundNumber);
    }

    private void ReorderPendingTurns(
        CombatRoster roster,
        List<CombatUnit> pendingTurns,
        int roundNumber)
    {
        IReadOnlyList<CombatUnit> orderedTurns =
            turnOrderBuilder.ReorderPending(
                roster,
                pendingTurns,
                roundNumber);

        pendingTurns.Clear();

        pendingTurns.AddRange(
            orderedTurns);
    }

    private void WriteTurnContext(
        CombatRoster roster,
        IReadOnlyList<CombatUnit> pendingTurns,
        int roundNumber)
    {
        IReadOnlyList<CombatUnit> nextRoundOrder =
            GetNextRoundOrder(
                roster,
                roundNumber);

        battleWriter.WriteTurnContext(
            roster,
            pendingTurns,
            nextRoundOrder);
    }

    private IReadOnlyList<CombatUnit>
        GetNextRoundOrder(
            CombatRoster roster,
            int roundNumber)
    {
        int nextRoundNumber =
            roundNumber +
            NextRoundOffset;

        return turnOrderBuilder.Build(
            roster,
            nextRoundNumber);
    }

    private BattleOutcome ExecuteNextTurn(
        CombatRoster roster,
        List<CombatUnit> pendingTurns)
    {
        CombatUnit currentUnit =
            GetFirstPendingTurn(
                pendingTurns);

        BattleOutcome outcome =
            turnExecutor.Execute(
                currentUnit,
                roster);

        RemoveFirstPendingTurn(
            pendingTurns);

        return outcome;
    }

    private CombatUnit GetFirstPendingTurn(
        IReadOnlyList<CombatUnit> pendingTurns)
    {
        return pendingTurns[
            FirstPendingTurnIndex];
    }

    private void RemoveFirstPendingTurn(
        List<CombatUnit> pendingTurns)
    {
        pendingTurns.RemoveAt(
            FirstPendingTurnIndex);
    }

    private void GainBoostPointsIfBattleContinues(
        CombatRoster roster,
        BattleOutcome outcome)
    {
        if (outcomeEvaluator.HasBattleEnded(
            outcome))
        {
            return;
        }

        GainBoostPoints(
            roster.Travelers);
    }

    private void GainBoostPoints(
        IReadOnlyList<Traveler> travelers)
    {
        foreach (Traveler traveler
                 in travelers.Where(IsAlive))
        {
            traveler.GainBoostPoint();
        }
    }

    private bool IsAlive(
        Traveler traveler)
    {
        return traveler.IsAlive();
    }

    private bool HasPendingTurns(
        IReadOnlyCollection<CombatUnit> pendingTurns)
    {
        return pendingTurns.Count >
               0;
    }

    private bool HasNoPendingTurns(
        IReadOnlyCollection<CombatUnit> pendingTurns)
    {
        return !HasPendingTurns(
            pendingTurns);
    }
}