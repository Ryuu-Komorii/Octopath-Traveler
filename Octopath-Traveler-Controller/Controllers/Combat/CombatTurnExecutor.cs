namespace Octopath_Traveler;

public class CombatTurnExecutor
{
    private readonly TravelerTurn travelerTurn;
    private readonly BeastTurn beastTurn;

    private readonly BattleOutcomeEvaluator
        outcomeEvaluator;

    public CombatTurnExecutor(
        TravelerTurn travelerTurn,
        BeastTurn beastTurn,
        BattleOutcomeEvaluator outcomeEvaluator)
    {
        this.travelerTurn =
            travelerTurn;

        this.beastTurn =
            beastTurn;

        this.outcomeEvaluator =
            outcomeEvaluator;
    }

    public BattleOutcome Execute(
        CombatUnit combatUnit,
        CombatRoster roster)
    {
        return combatUnit switch
        {
            Traveler traveler =>
                ExecuteTravelerTurn(
                    traveler,
                    roster),

            Beast beast =>
                ExecuteBeastTurn(
                    beast,
                    roster),

            _ =>
                throw new ArgumentOutOfRangeException(
                    nameof(combatUnit))
        };
    }

    private BattleOutcome ExecuteTravelerTurn(
        Traveler traveler,
        CombatRoster roster)
    {
        TravelerTurnOutcome turnOutcome =
            travelerTurn.Execute(
                traveler,
                roster.Travelers,
                roster.Beasts);

        if (DidTravelersRunAway(
            turnOutcome))
        {
            return BattleOutcome.EnemyWon;
        }

        return outcomeEvaluator.Evaluate(
            roster);
    }

    private BattleOutcome ExecuteBeastTurn(
        Beast beast,
        CombatRoster roster)
    {
        beastTurn.Execute(
            beast,
            roster.Travelers);

        return outcomeEvaluator.Evaluate(
            roster);
    }

    private bool DidTravelersRunAway(
        TravelerTurnOutcome outcome)
    {
        return outcome ==
               TravelerTurnOutcome.RanAway;
    }
}