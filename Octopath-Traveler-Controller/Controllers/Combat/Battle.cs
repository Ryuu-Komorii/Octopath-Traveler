using Octopath_Traveler_View;

namespace Octopath_Traveler;

public class Battle
{
    private const int InitialRoundNumber = 1;
    private const int NextRoundOffset = 1;

    private readonly BattleRound battleRound;

    private readonly BattleOutcomeEvaluator
        outcomeEvaluator;

    private readonly BattleWriter
        battleWriter;

    public Battle(
        View view,
        ActiveSkillCatalog activeSkillCatalog,
        BeastSkillCatalog beastSkillCatalog)
    {
        outcomeEvaluator =
            new BattleOutcomeEvaluator();

        battleWriter =
            new BattleWriter(view);

        TurnOrderBuilder turnOrderBuilder =
            new TurnOrderBuilder();

        TravelerTurn travelerTurn =
            new TravelerTurn(
                view,
                activeSkillCatalog);

        BeastTurn beastTurn =
            new BeastTurn(
                view,
                beastSkillCatalog);

        CombatTurnExecutor turnExecutor =
            new CombatTurnExecutor(
                travelerTurn,
                beastTurn,
                outcomeEvaluator);

        BattleRoundServices services =
            new BattleRoundServices
            {
                TurnOrderBuilder =
                    turnOrderBuilder,

                TurnExecutor =
                    turnExecutor,

                OutcomeEvaluator =
                    outcomeEvaluator,

                BattleWriter =
                    battleWriter
            };

        battleRound =
            new BattleRound(
                services);
    }

    public void Play(
        CombatRoster roster)
    {
        int roundNumber =
            InitialRoundNumber;

        BattleOutcome outcome =
            BattleOutcome.InProgress;

        while (outcomeEvaluator
               .IsBattleInProgress(outcome))
        {
            outcome =
                battleRound.Play(
                    roster,
                    roundNumber);

            roundNumber =
                GetNextRound(
                    roundNumber);
        }

        battleWriter.WriteWinner(
            outcome);
    }

    private int GetNextRound(
        int currentRound)
    {
        return currentRound +
               NextRoundOffset;
    }
}