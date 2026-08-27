using Octopath_Traveler_View;

namespace Octopath_Traveler;

public enum TravelerTurnOutcome
{
    Continue,
    Completed,
    RanAway
}

public class TravelerTurn
{
    private const string SeparatorLine =
        "----------------------------------------";

    private const string RunAwayMessage =
        "El equipo de viajeros ha huido!";

    private readonly View view;
    private readonly TravelerActionMenu actionMenu;
    private readonly ActiveSkillSelectionMenu skillSelectionMenu;
    private readonly BasicAttack basicAttack;

    public TravelerTurn(View view)
    {
        this.view = view;
        actionMenu = new TravelerActionMenu(view);
        skillSelectionMenu =
            new ActiveSkillSelectionMenu(view);
        basicAttack = new BasicAttack(view);
    }

    public TravelerTurnOutcome Execute(
        Traveler traveler,
        IReadOnlyList<Beast> beasts)
    {
        TravelerTurnOutcome outcome;

        do
        {
            outcome = ExecuteSelectedAction(
                traveler,
                beasts);
        }
        while (ShouldContinueTurn(outcome));

        return outcome;
    }

    private TravelerTurnOutcome ExecuteSelectedAction(
        Traveler traveler,
        IReadOnlyList<Beast> beasts)
    {
        TravelerAction selectedAction =
            actionMenu.ReadAction(traveler);

        return ResolveAction(
            selectedAction,
            traveler,
            beasts);
    }

    private TravelerTurnOutcome ResolveAction(
        TravelerAction selectedAction,
        Traveler traveler,
        IReadOnlyList<Beast> beasts)
    {
        return selectedAction switch
        {
            TravelerAction.BasicAttack =>
                ExecuteBasicAttack(traveler, beasts),

            TravelerAction.UseSkill =>
                ExecuteSkillSelection(traveler),

            TravelerAction.Defend =>
                TravelerTurnOutcome.Completed,

            TravelerAction.RunAway =>
                ExecuteRunAway(),

            _ => throw new ArgumentOutOfRangeException(
                nameof(selectedAction))
        };
    }

    private TravelerTurnOutcome ExecuteBasicAttack(
        Traveler traveler,
        IReadOnlyList<Beast> beasts)
    {
        ActionExecutionResult result =
            basicAttack.Execute(traveler, beasts);

        return ConvertAttackResult(result);
    }

    private TravelerTurnOutcome ConvertAttackResult(
        ActionExecutionResult result)
    {
        return result switch
        {
            ActionExecutionResult.Completed =>
                TravelerTurnOutcome.Completed,

            ActionExecutionResult.Cancelled =>
                TravelerTurnOutcome.Continue,

            _ => throw new ArgumentOutOfRangeException(
                nameof(result))
        };
    }

    private TravelerTurnOutcome ExecuteSkillSelection(
        Traveler traveler)
    {
        skillSelectionMenu.SelectSkill(traveler);

        return TravelerTurnOutcome.Continue;
    }

    private TravelerTurnOutcome ExecuteRunAway()
    {
        view.WriteLine(SeparatorLine);
        view.WriteLine(RunAwayMessage);

        return TravelerTurnOutcome.RanAway;
    }

    private bool ShouldContinueTurn(
        TravelerTurnOutcome outcome)
    {
        return outcome ==
               TravelerTurnOutcome.Continue;
    }
}