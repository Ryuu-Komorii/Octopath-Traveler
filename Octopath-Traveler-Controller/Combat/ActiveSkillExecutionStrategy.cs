using Octopath_Traveler_View;

namespace Octopath_Traveler;

public interface ActiveSkillExecutionStrategy
{
    ActionExecutionResult Execute(
        ActiveSkillExecutionContext context);
}

public class OffensiveActiveSkillExecutionStrategy
    : ActiveSkillExecutionStrategy
{
    private readonly OffensiveSkillExecutor
        offensiveSkillExecutor;

    public OffensiveActiveSkillExecutionStrategy(
        View view)
    {
        offensiveSkillExecutor =
            new OffensiveSkillExecutor(
                view);
    }

    public ActionExecutionResult Execute(
        ActiveSkillExecutionContext context)
    {
        return offensiveSkillExecutor.Execute(
            context.User,
            context.Beasts,
            context.Skill);
    }
}

public class SpearheadSkillExecutionStrategy
    : ActiveSkillExecutionStrategy
{
    private readonly OffensiveSkillExecutor
        offensiveSkillExecutor;

    public SpearheadSkillExecutionStrategy(
        View view)
    {
        offensiveSkillExecutor =
            new OffensiveSkillExecutor(
                view);
    }

    public ActionExecutionResult Execute(
        ActiveSkillExecutionContext context)
    {
        ActionExecutionResult result =
            offensiveSkillExecutor.Execute(
                context.User,
                context.Beasts,
                context.Skill);

        if (result ==
            ActionExecutionResult.Completed)
        {
            context.User
                .GrantSkillPriorityForNextRound();
        }

        return result;
    }
}

public class HealingSkillExecutionStrategy
    : ActiveSkillExecutionStrategy
{
    private const string AllyTarget =
        "Ally";

    private readonly TravelerTargetSelectionMenu
        targetSelectionMenu;

    private readonly BoostPointSelectionMenu
        boostPointSelectionMenu;

    private readonly HealingCalculator
        healingCalculator;

    private readonly ActiveSkillResultWriter
        resultWriter;

    public HealingSkillExecutionStrategy(
        View view)
    {
        targetSelectionMenu =
            new TravelerTargetSelectionMenu(
                view);

        boostPointSelectionMenu =
            new BoostPointSelectionMenu(
                view);

        healingCalculator =
            new HealingCalculator();

        resultWriter =
            new ActiveSkillResultWriter(
                view);
    }

    public ActionExecutionResult Execute(
        ActiveSkillExecutionContext context)
    {
        IReadOnlyList<Traveler>? targets =
            SelectTargets(
                context);

        if (targets is null)
        {
            return ActionExecutionResult.Cancelled;
        }

        boostPointSelectionMenu
            .ReadBoostPoints();

        context.User.SpendSkillPoints(
            context.Skill.SP);

        int healingAmount =
            healingCalculator.Calculate(
                context.User.ElementalDefense,
                context.Skill.Modifier);

        ApplyHealing(
            targets,
            healingAmount);

        WriteResult(
            context,
            targets,
            healingAmount);

        return ActionExecutionResult.Completed;
    }

    private IReadOnlyList<Traveler>? SelectTargets(
        ActiveSkillExecutionContext context)
    {
        if (context.Skill.Target ==
            AllyTarget)
        {
            return SelectSingleLivingTarget(
                context);
        }

        return GetLivingPartyTargets(
            context);
    }

    private IReadOnlyList<Traveler>?
        SelectSingleLivingTarget(
            ActiveSkillExecutionContext context)
    {
        Traveler? target =
            targetSelectionMenu.SelectLivingTarget(
                context.User,
                context.Travelers);

        if (target is null)
        {
            return null;
        }

        return [target];
    }

    private IReadOnlyList<Traveler>
        GetLivingPartyTargets(
            ActiveSkillExecutionContext context)
    {
        IEnumerable<Traveler> otherTravelers =
            context.Travelers
                .Where(IsAlive)
                .Where(traveler =>
                    traveler != context.User);

        IEnumerable<Traveler> user =
            context.Travelers
                .Where(traveler =>
                    traveler == context.User)
                .Where(IsAlive);

        return otherTravelers
            .Concat(user)
            .ToArray();
    }

    private bool IsAlive(
        Traveler traveler)
    {
        return traveler.IsAlive();
    }

    private void ApplyHealing(
        IReadOnlyList<Traveler> targets,
        int healingAmount)
    {
        foreach (Traveler target in targets)
        {
            target.RecoverHitPoints(
                healingAmount);
        }
    }

    private void WriteResult(
        ActiveSkillExecutionContext context,
        IReadOnlyList<Traveler> targets,
        int healingAmount)
    {
        resultWriter.WriteSkillUse(
            context.User,
            context.Skill);

        foreach (Traveler target in targets)
        {
            resultWriter.WriteHealing(
                target,
                healingAmount);
        }

        foreach (Traveler target in targets)
        {
            resultWriter.WriteRemainingHitPoints(
                target);
        }
    }
}

public class ReviveSkillExecutionStrategy
    : ActiveSkillExecutionStrategy
{
    private readonly BoostPointSelectionMenu
        boostPointSelectionMenu;

    private readonly ActiveSkillResultWriter
        resultWriter;

    public ReviveSkillExecutionStrategy(
        View view)
    {
        boostPointSelectionMenu =
            new BoostPointSelectionMenu(
                view);

        resultWriter =
            new ActiveSkillResultWriter(
                view);
    }

    public ActionExecutionResult Execute(
        ActiveSkillExecutionContext context)
    {
        Traveler[] fallenTravelers =
            context.Travelers
                .Where(IsFallen)
                .ToArray();

        boostPointSelectionMenu
            .ReadBoostPoints();

        context.User.SpendSkillPoints(
            context.Skill.SP);

        resultWriter.WriteSkillUse(
            context.User,
            context.Skill);

        ReviveTargets(
            fallenTravelers);

        WriteRemainingHitPoints(
            fallenTravelers);

        return ActionExecutionResult.Completed;
    }

    private bool IsFallen(
        Traveler traveler)
    {
        return !traveler.IsAlive();
    }

    private void ReviveTargets(
        IReadOnlyList<Traveler> targets)
    {
        foreach (Traveler target in targets)
        {
            target.Revive();

            resultWriter.WriteRevive(
                target);
        }
    }

    private void WriteRemainingHitPoints(
        IReadOnlyList<Traveler> targets)
    {
        foreach (Traveler target in targets)
        {
            resultWriter.WriteRemainingHitPoints(
                target);
        }
    }
}

public class VivifySkillExecutionStrategy
    : ActiveSkillExecutionStrategy
{
    private readonly TravelerTargetSelectionMenu
        targetSelectionMenu;

    private readonly BoostPointSelectionMenu
        boostPointSelectionMenu;

    private readonly HealingCalculator
        healingCalculator;

    private readonly ActiveSkillResultWriter
        resultWriter;

    public VivifySkillExecutionStrategy(
        View view)
    {
        targetSelectionMenu =
            new TravelerTargetSelectionMenu(
                view);

        boostPointSelectionMenu =
            new BoostPointSelectionMenu(
                view);

        healingCalculator =
            new HealingCalculator();

        resultWriter =
            new ActiveSkillResultWriter(
                view);
    }

    public ActionExecutionResult Execute(
        ActiveSkillExecutionContext context)
    {
        Traveler? target =
            targetSelectionMenu.SelectFallenTarget(
                context.User,
                context.Travelers);

        if (target is null)
        {
            return ActionExecutionResult.Cancelled;
        }

        boostPointSelectionMenu
            .ReadBoostPoints();

        context.User.SpendSkillPoints(
            context.Skill.SP);

        int healingAmount =
            healingCalculator.Calculate(
                context.User.ElementalDefense,
                context.Skill.Modifier);

        ApplyVivify(
            target,
            healingAmount);

        WriteResult(
            context,
            target,
            healingAmount);

        return ActionExecutionResult.Completed;
    }

    private void ApplyVivify(
        Traveler target,
        int healingAmount)
    {
        target.Revive();

        target.RecoverHitPoints(
            healingAmount);
    }

    private void WriteResult(
        ActiveSkillExecutionContext context,
        Traveler target,
        int healingAmount)
    {
        resultWriter.WriteSkillUse(
            context.User,
            context.Skill);

        resultWriter.WriteRevive(
            target);

        resultWriter.WriteHealing(
            target,
            healingAmount);

        resultWriter.WriteRemainingHitPoints(
            target);
    }
}

public class LegholdTrapSkillExecutionStrategy
    : ActiveSkillExecutionStrategy
{
    private const int EffectDuration = 2;

    private readonly BeastTargetSelectionMenu
        targetSelectionMenu;

    private readonly BoostPointSelectionMenu
        boostPointSelectionMenu;

    private readonly ActiveSkillResultWriter
        resultWriter;

    public LegholdTrapSkillExecutionStrategy(
        View view)
    {
        targetSelectionMenu =
            new BeastTargetSelectionMenu(
                view);

        boostPointSelectionMenu =
            new BoostPointSelectionMenu(
                view);

        resultWriter =
            new ActiveSkillResultWriter(
                view);
    }

    public ActionExecutionResult Execute(
        ActiveSkillExecutionContext context)
    {
        Beast? target =
            targetSelectionMenu.SelectTarget(
                context.User,
                context.Beasts);

        if (target is null)
        {
            return ActionExecutionResult.Cancelled;
        }

        boostPointSelectionMenu
            .ReadBoostPoints();

        context.User.SpendSkillPoints(
            context.Skill.SP);

        target.ApplyTurnPenalty(
            EffectDuration);

        resultWriter.WriteSkillUse(
            context.User,
            context.Skill);

        resultWriter.WriteTurnPenalty(
            target,
            EffectDuration);

        return ActionExecutionResult.Completed;
    }
}