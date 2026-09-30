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

    private readonly TravelerActionMenu
        actionMenu;

    private readonly ActiveSkillSelectionMenu
        skillSelectionMenu;

    private readonly BasicAttack
        basicAttack;

    private readonly ActiveSkillCatalog
        activeSkillCatalog;

    private readonly ActiveSkillExecutionStrategyFactory
        skillStrategyFactory;

    public TravelerTurn(
        View view,
        ActiveSkillCatalog activeSkillCatalog)
    {
        this.view =
            view;

        this.activeSkillCatalog =
            activeSkillCatalog;

        actionMenu =
            new TravelerActionMenu(
                view);

        skillSelectionMenu =
            new ActiveSkillSelectionMenu(
                view,
                activeSkillCatalog);

        basicAttack =
            new BasicAttack(
                view);

        skillStrategyFactory =
            new ActiveSkillExecutionStrategyFactory(
                view);
    }

    public TravelerTurnOutcome Execute(
        Traveler traveler,
        IReadOnlyList<Traveler> travelers,
        IReadOnlyList<Beast> beasts)
    {
        TravelerTurnOutcome outcome;

        do
        {
            outcome =
                ExecuteSelectedAction(
                    traveler,
                    travelers,
                    beasts);
        }
        while (ShouldContinueTurn(
            outcome));

        return outcome;
    }

    private TravelerTurnOutcome ExecuteSelectedAction(
        Traveler traveler,
        IReadOnlyList<Traveler> travelers,
        IReadOnlyList<Beast> beasts)
    {
        TravelerAction selectedAction =
            actionMenu.ReadAction(
                traveler);

        return ResolveAction(
            selectedAction,
            traveler,
            travelers,
            beasts);
    }

    private TravelerTurnOutcome ResolveAction(
        TravelerAction selectedAction,
        Traveler traveler,
        IReadOnlyList<Traveler> travelers,
        IReadOnlyList<Beast> beasts)
    {
        return selectedAction switch
        {
            TravelerAction.BasicAttack =>
                ExecuteBasicAttack(
                    traveler,
                    beasts),

            TravelerAction.UseSkill =>
                ExecuteSkill(
                    traveler,
                    travelers,
                    beasts),

            TravelerAction.Defend =>
                ExecuteDefend(
                    traveler),

            TravelerAction.RunAway =>
                ExecuteRunAway(),

            _ =>
                throw new ArgumentOutOfRangeException(
                    nameof(selectedAction))
        };
    }

    private TravelerTurnOutcome ExecuteBasicAttack(
        Traveler traveler,
        IReadOnlyList<Beast> beasts)
    {
        ActionExecutionResult result =
            basicAttack.Execute(
                traveler,
                beasts);

        return ConvertActionResult(
            result);
    }

    private TravelerTurnOutcome ExecuteSkill(
        Traveler traveler,
        IReadOnlyList<Traveler> travelers,
        IReadOnlyList<Beast> beasts)
    {
        string? selectedSkillName =
            skillSelectionMenu.SelectSkill(
                traveler);

        if (selectedSkillName is null)
        {
            return TravelerTurnOutcome.Continue;
        }

        ActiveSkillCatalogEntry skill =
            activeSkillCatalog.Find(
                selectedSkillName);

        ActiveSkillExecutionContext context =
            new ActiveSkillExecutionContext(
                traveler,
                travelers,
                beasts,
                skill);

        ActiveSkillExecutionStrategy strategy =
            skillStrategyFactory.Create(
                skill);

        ActionExecutionResult result =
            strategy.Execute(
                context);

        return ConvertActionResult(
            result);
    }

    private TravelerTurnOutcome ExecuteDefend(
        Traveler traveler)
    {
        traveler.Defend();

        return TravelerTurnOutcome.Completed;
    }

    private TravelerTurnOutcome ConvertActionResult(
        ActionExecutionResult result)
    {
        return result switch
        {
            ActionExecutionResult.Completed =>
                TravelerTurnOutcome.Completed,

            ActionExecutionResult.Cancelled =>
                TravelerTurnOutcome.Continue,

            _ =>
                throw new ArgumentOutOfRangeException(
                    nameof(result))
        };
    }

    private TravelerTurnOutcome ExecuteRunAway()
    {
        view.WriteLine(
            SeparatorLine);

        view.WriteLine(
            RunAwayMessage);

        return TravelerTurnOutcome.RanAway;
    }

    private bool ShouldContinueTurn(
        TravelerTurnOutcome outcome)
    {
        return outcome ==
               TravelerTurnOutcome.Continue;
    }
}