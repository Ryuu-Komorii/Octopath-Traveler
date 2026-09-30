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
    private const string RunAwayMessage =
        "El equipo de viajeros ha huido!";

    private readonly View view;
    private readonly TravelerActionMenu actionMenu;
    private readonly ActiveSkillSelectionMenu skillSelectionMenu;
    private readonly BasicAttack basicAttack;
    private readonly ActiveSkillCatalog activeSkillCatalog;

    private readonly ActiveSkillExecutionStrategyFactory
        skillStrategyFactory;

    public TravelerTurn(
        View view,
        ActiveSkillCatalog activeSkillCatalog)
    {
        this.view = view;

        this.activeSkillCatalog =
            activeSkillCatalog;

        actionMenu =
            new TravelerActionMenu(view);

        skillSelectionMenu =
            new ActiveSkillSelectionMenu(
                view,
                activeSkillCatalog);

        basicAttack =
            new BasicAttack(view);

        skillStrategyFactory =
            new ActiveSkillExecutionStrategyFactory(
                view);
    }

    public TravelerTurnOutcome Execute(
        Traveler traveler,
        IReadOnlyList<Traveler> travelers,
        IReadOnlyList<Beast> beasts)
    {
        TravelerTurnContext context =
            CreateTurnContext(
                traveler,
                travelers,
                beasts);

        TravelerTurnOutcome outcome;

        do
        {
            outcome =
                ExecuteSelectedAction(
                    context);
        }
        while (ShouldContinueTurn(outcome));

        return outcome;
    }

    private TravelerTurnContext CreateTurnContext(
        Traveler traveler,
        IReadOnlyList<Traveler> travelers,
        IReadOnlyList<Beast> beasts)
    {
        return new TravelerTurnContext
        {
            User = traveler,
            Travelers = travelers,
            Beasts = beasts
        };
    }

    private TravelerTurnOutcome ExecuteSelectedAction(
        TravelerTurnContext context)
    {
        TravelerAction selectedAction =
            actionMenu.ReadAction(
                context.User);

        return ResolveAction(
            selectedAction,
            context);
    }

    private TravelerTurnOutcome ResolveAction(
        TravelerAction selectedAction,
        TravelerTurnContext context)
    {
        return selectedAction switch
        {
            TravelerAction.BasicAttack =>
                ExecuteBasicAttack(context),

            TravelerAction.UseSkill =>
                ExecuteSkill(context),

            TravelerAction.Defend =>
                ExecuteDefend(
                    context.User),

            TravelerAction.RunAway =>
                ExecuteRunAway(),

            _ =>
                throw new ArgumentOutOfRangeException(
                    nameof(selectedAction))
        };
    }

    private TravelerTurnOutcome ExecuteBasicAttack(
        TravelerTurnContext context)
    {
        ActionExecutionResult result =
            basicAttack.Execute(
                context.User,
                context.Beasts);

        return ConvertActionResult(
            result);
    }

    private TravelerTurnOutcome ExecuteSkill(
        TravelerTurnContext turnContext)
    {
        string? selectedSkillName =
            skillSelectionMenu.SelectSkill(
                turnContext.User);

        if (WasSkillSelectionCancelled(
            selectedSkillName))
        {
            return TravelerTurnOutcome.Continue;
        }

        ActiveSkillCatalogEntry skill =
            activeSkillCatalog.Find(
                selectedSkillName!);

        ActiveSkillExecutionContext skillContext =
            CreateSkillContext(
                turnContext,
                skill);

        ActiveSkillExecutionStrategy strategy =
            skillStrategyFactory.Create(
                skill);

        ActionExecutionResult result =
            strategy.Execute(
                skillContext);

        return ConvertActionResult(
            result);
    }

    private bool WasSkillSelectionCancelled(
        string? selectedSkillName)
    {
        return selectedSkillName is null;
    }

    private ActiveSkillExecutionContext CreateSkillContext(
        TravelerTurnContext turnContext,
        ActiveSkillCatalogEntry skill)
    {
        return new ActiveSkillExecutionContext
        {
            User =
                turnContext.User,

            Travelers =
                turnContext.Travelers,

            Beasts =
                turnContext.Beasts,

            Skill =
                skill
        };
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
            CombatText.SeparatorLine);

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