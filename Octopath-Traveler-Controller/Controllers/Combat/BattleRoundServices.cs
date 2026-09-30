namespace Octopath_Traveler;

public class BattleRoundServices
{
    public TurnOrderBuilder TurnOrderBuilder { get; init; } =
        null!;

    public CombatTurnExecutor TurnExecutor { get; init; } =
        null!;

    public BattleOutcomeEvaluator OutcomeEvaluator { get; init; } =
        null!;

    public BattleWriter BattleWriter { get; init; } =
        null!;
}