using Octopath_Traveler_View;

namespace Octopath_Traveler;

public enum BattleOutcome
{
    InProgress,
    PlayerWon,
    EnemyWon
}

public class Battle
{
    private const string SeparatorLine =
        "----------------------------------------";
    private const string RoundStartMessageFormat =
        "INICIA RONDA {0}";
    private const string PlayerVictoryMessage =
        "Gana equipo del jugador";
    private const string EnemyVictoryMessage =
        "Gana equipo del enemigo";
    private const int InitialRoundNumber = 1;
    private const int FirstPendingTurnIndex = 0;

    private readonly View view;
    private readonly BattleStateWriter stateWriter;
    private readonly TurnOrderBuilder turnOrderBuilder;
    private readonly TurnOrderWriter turnOrderWriter;
    private readonly TravelerTurn travelerTurn;
    private readonly BeastTurn beastTurn;

    public Battle(View view)
    {
        this.view = view;
        stateWriter = new BattleStateWriter(view);
        turnOrderBuilder = new TurnOrderBuilder();
        turnOrderWriter = new TurnOrderWriter(view);
        travelerTurn = new TravelerTurn(view);
        beastTurn = new BeastTurn(view);
    }

    public void Play(CombatRoster combatRoster)
    {
        int roundNumber = InitialRoundNumber;
        BattleOutcome outcome = BattleOutcome.InProgress;

        while (IsBattleInProgress(outcome))
        {
            outcome = PlayRound(
                combatRoster,
                roundNumber);

            roundNumber++;
        }

        WriteWinner(outcome);
    }

    private BattleOutcome PlayRound(
        CombatRoster combatRoster,
        int roundNumber)
    {
        WriteRoundHeader(roundNumber);

        List<CombatUnit> pendingTurns =
            turnOrderBuilder
                .Build(combatRoster)
                .ToList();

        BattleOutcome outcome =
            PlayPendingTurns(
                combatRoster,
                pendingTurns);

        GainRoundBoostPoints(
            combatRoster,
            outcome);

        return outcome;
    }

    private void GainRoundBoostPoints(
        CombatRoster combatRoster,
        BattleOutcome outcome)
    {
        if (HasBattleEnded(outcome))
        {
            return;
        }

        IEnumerable<Traveler> livingTravelers =
            combatRoster.Travelers.Where(IsAlive);

        foreach (Traveler traveler in livingTravelers)
        {
            traveler.GainBoostPoint();
        }
    }

    private BattleOutcome PlayPendingTurns(
        CombatRoster combatRoster,
        List<CombatUnit> pendingTurns)
    {
        while (HasPendingTurns(pendingTurns))
        {
            BattleOutcome outcome =
                PrepareNextTurn(
                    combatRoster,
                    pendingTurns);

            if (HasBattleEnded(outcome))
            {
                return outcome;
            }

            if (HasNoPendingTurns(pendingTurns))
            {
                return BattleOutcome.InProgress;
            }

            outcome = ExecuteFirstPendingTurn(
                combatRoster,
                pendingTurns);

            if (HasBattleEnded(outcome))
            {
                return outcome;
            }
        }

        return BattleOutcome.InProgress;
    }

    private BattleOutcome PrepareNextTurn(
        CombatRoster combatRoster,
        List<CombatUnit> pendingTurns)
    {
        RemoveDeadUnits(pendingTurns);

        BattleOutcome outcome =
            GetBattleOutcome(combatRoster);

        if (HasBattleEnded(outcome))
        {
            return outcome;
        }

        if (HasNoPendingTurns(pendingTurns))
        {
            return BattleOutcome.InProgress;
        }

        WriteTurnContext(
            combatRoster,
            pendingTurns);

        return BattleOutcome.InProgress;
    }

    private BattleOutcome ExecuteFirstPendingTurn(
        CombatRoster combatRoster,
        List<CombatUnit> pendingTurns)
    {
        CombatUnit currentUnit =
            pendingTurns[FirstPendingTurnIndex];

        BattleOutcome outcome =
            ExecuteTurn(
                currentUnit,
                combatRoster);

        pendingTurns.RemoveAt(
            FirstPendingTurnIndex);

        return outcome;
    }

    private BattleOutcome ExecuteTurn(
        CombatUnit combatUnit,
        CombatRoster combatRoster)
    {
        return combatUnit switch
        {
            Traveler traveler =>
                ExecuteTravelerTurn(
                    traveler,
                    combatRoster),

            Beast beast =>
                ExecuteBeastTurn(
                    beast,
                    combatRoster),

            _ => throw new ArgumentOutOfRangeException(
                nameof(combatUnit))
        };
    }

    private BattleOutcome ExecuteTravelerTurn(
        Traveler traveler,
        CombatRoster combatRoster)
    {
        TravelerTurnOutcome turnOutcome =
            travelerTurn.Execute(
                traveler,
                combatRoster.Beasts);

        if (DidTravelersRunAway(turnOutcome))
        {
            return BattleOutcome.EnemyWon;
        }

        return GetBattleOutcome(combatRoster);
    }

    private BattleOutcome ExecuteBeastTurn(
        Beast beast,
        CombatRoster combatRoster)
    {
        beastTurn.Execute(
            beast,
            combatRoster.Travelers);

        return GetBattleOutcome(combatRoster);
    }

    private void WriteTurnContext(
        CombatRoster combatRoster,
        IReadOnlyList<CombatUnit> pendingTurns)
    {
        view.WriteLine(SeparatorLine);

        stateWriter.Write(combatRoster);

        IReadOnlyList<CombatUnit> nextRoundOrder =
            turnOrderBuilder.Build(combatRoster);

        view.WriteLine(SeparatorLine);

        turnOrderWriter.Write(
            pendingTurns,
            nextRoundOrder);
    }

    private void WriteRoundHeader(int roundNumber)
    {
        view.WriteLine(SeparatorLine);

        view.WriteLine(
            string.Format(
                RoundStartMessageFormat,
                roundNumber));
    }

    private BattleOutcome GetBattleOutcome(
        CombatRoster combatRoster)
    {
        if (HasNoLivingTravelers(combatRoster))
        {
            return BattleOutcome.EnemyWon;
        }

        if (HasNoLivingBeasts(combatRoster))
        {
            return BattleOutcome.PlayerWon;
        }

        return BattleOutcome.InProgress;
    }

    private bool HasNoLivingTravelers(
        CombatRoster combatRoster)
    {
        return !combatRoster.Travelers.Any(
            IsAlive);
    }

    private bool HasNoLivingBeasts(
        CombatRoster combatRoster)
    {
        return !combatRoster.Beasts.Any(
            IsAlive);
    }

    private bool IsAlive(Traveler traveler)
    {
        return traveler.IsAlive();
    }

    private bool IsAlive(Beast beast)
    {
        return beast.IsAlive();
    }

    private void RemoveDeadUnits(
        List<CombatUnit> pendingTurns)
    {
        pendingTurns.RemoveAll(IsDead);
    }

    private bool IsDead(CombatUnit combatUnit)
    {
        return !combatUnit.IsAlive();
    }

    private bool HasPendingTurns(
        IReadOnlyCollection<CombatUnit> pendingTurns)
    {
        return pendingTurns.Count > 0;
    }

    private bool HasNoPendingTurns(
        IReadOnlyCollection<CombatUnit> pendingTurns)
    {
        return !HasPendingTurns(pendingTurns);
    }

    private bool IsBattleInProgress(
        BattleOutcome outcome)
    {
        return outcome == BattleOutcome.InProgress;
    }

    private bool HasBattleEnded(
        BattleOutcome outcome)
    {
        return !IsBattleInProgress(outcome);
    }

    private bool DidTravelersRunAway(
        TravelerTurnOutcome outcome)
    {
        return outcome == TravelerTurnOutcome.RanAway;
    }

    private void WriteWinner(BattleOutcome outcome)
    {
        view.WriteLine(SeparatorLine);

        if (DidPlayerWin(outcome))
        {
            view.WriteLine(PlayerVictoryMessage);
            return;
        }

        view.WriteLine(EnemyVictoryMessage);
    }

    private bool DidPlayerWin(BattleOutcome outcome)
    {
        return outcome == BattleOutcome.PlayerWon;
    }
}