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
            new OffensiveSkillExecutor(view);
    }

    public ActionExecutionResult Execute(
        ActiveSkillExecutionContext context)
    {
        return offensiveSkillExecutor.Execute(
            context);
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
            new OffensiveSkillExecutor(view);
    }

    public ActionExecutionResult Execute(
        ActiveSkillExecutionContext context)
    {
        ActionExecutionResult result =
            offensiveSkillExecutor.Execute(
                context);

        GrantPriorityIfCompleted(
            context,
            result);

        return result;
    }

    private void GrantPriorityIfCompleted(
        ActiveSkillExecutionContext context,
        ActionExecutionResult result)
    {
        if (WasCompleted(result))
        {
            context.User
                .GrantSkillPriorityForNextRound();
        }
    }

    private bool WasCompleted(
        ActionExecutionResult result)
    {
        return result ==
               ActionExecutionResult.Completed;
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
            SelectTargets(context);

        if (WasTargetSelectionCancelled(
            targets))
        {
            return ActionExecutionResult.Cancelled;
        }

        boostPointSelectionMenu.ReadBoostPoints();

        SpendSkillPoints(context);

        int healingAmount =
            CalculateHealing(context);

        ApplyHealing(
            targets!,
            healingAmount);

        WriteResult(
            context,
            targets!,
            healingAmount);

        return ActionExecutionResult.Completed;
    }

    private bool WasTargetSelectionCancelled(
        IReadOnlyList<Traveler>? targets)
    {
        return targets is null;
    }

    private void SpendSkillPoints(
        ActiveSkillExecutionContext context)
    {
        context.User.SpendSkillPoints(
            context.Skill.SP);
    }

    private int CalculateHealing(
        ActiveSkillExecutionContext context)
    {
        return healingCalculator.Calculate(
            context.User.ElementalDefense,
            context.Skill.Modifier);
    }

    private IReadOnlyList<Traveler>? SelectTargets(
        ActiveSkillExecutionContext context)
    {
        if (TargetsSingleAlly(
            context.Skill))
        {
            return SelectSingleLivingTarget(
                context);
        }

        return GetLivingPartyTargets(
            context);
    }

    private bool TargetsSingleAlly(
        ActiveSkillCatalogEntry skill)
    {
        return skill.Target ==
               AllyTarget;
    }

    private IReadOnlyList<Traveler>?
        SelectSingleLivingTarget(
            ActiveSkillExecutionContext context)
    {
        Traveler? target =
            targetSelectionMenu.SelectLivingTarget(
                context.User,
                context.Travelers);

        if (WasSingleTargetSelectionCancelled(
            target))
        {
            return null;
        }

        return [target!];
    }

    private bool WasSingleTargetSelectionCancelled(
        Traveler? target)
    {
        return target is null;
    }

    private IReadOnlyList<Traveler>
        GetLivingPartyTargets(
            ActiveSkillExecutionContext context)
    {
        IEnumerable<Traveler> otherTravelers =
            context.Travelers
                .Where(IsAlive)
                .Where(traveler =>
                    IsNotUser(
                        traveler,
                        context.User));

        IEnumerable<Traveler> user =
            context.Travelers
                .Where(traveler =>
                    IsUser(
                        traveler,
                        context.User))
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

    private bool IsNotUser(
        Traveler traveler,
        Traveler user)
    {
        return traveler != user;
    }

    private bool IsUser(
        Traveler traveler,
        Traveler user)
    {
        return traveler == user;
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

        WriteHealingResults(
            targets,
            healingAmount);

        WriteRemainingHitPoints(
            targets);
    }

    private void WriteHealingResults(
        IReadOnlyList<Traveler> targets,
        int healingAmount)
    {
        foreach (Traveler target in targets)
        {
            resultWriter.WriteHealing(
                target,
                healingAmount);
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
            GetFallenTravelers(context);

        boostPointSelectionMenu.ReadBoostPoints();

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

    private Traveler[] GetFallenTravelers(
        ActiveSkillExecutionContext context)
    {
        return context.Travelers
            .Where(IsFallen)
            .ToArray();
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
            SelectTarget(context);

        if (WasTargetSelectionCancelled(
            target))
        {
            return ActionExecutionResult.Cancelled;
        }

        boostPointSelectionMenu.ReadBoostPoints();

        context.User.SpendSkillPoints(
            context.Skill.SP);

        int healingAmount =
            CalculateHealing(context);

        ApplyVivify(
            target!,
            healingAmount);

        WriteResult(
            context,
            target!,
            healingAmount);

        return ActionExecutionResult.Completed;
    }

    private Traveler? SelectTarget(
        ActiveSkillExecutionContext context)
    {
        return targetSelectionMenu.SelectFallenTarget(
            context.User,
            context.Travelers);
    }

    private bool WasTargetSelectionCancelled(
        Traveler? target)
    {
        return target is null;
    }

    private int CalculateHealing(
        ActiveSkillExecutionContext context)
    {
        return healingCalculator.Calculate(
            context.User.ElementalDefense,
            context.Skill.Modifier);
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
            SelectTarget(context);

        if (WasTargetSelectionCancelled(
            target))
        {
            return ActionExecutionResult.Cancelled;
        }

        boostPointSelectionMenu.ReadBoostPoints();

        context.User.SpendSkillPoints(
            context.Skill.SP);

        ApplyTurnPenalty(
            target!);

        WriteResult(
            context,
            target!);

        return ActionExecutionResult.Completed;
    }

    private Beast? SelectTarget(
        ActiveSkillExecutionContext context)
    {
        return targetSelectionMenu.SelectTarget(
            context.User,
            context.Beasts);
    }

    private bool WasTargetSelectionCancelled(
        Beast? target)
    {
        return target is null;
    }

    private void ApplyTurnPenalty(
        Beast target)
    {
        target.ApplyTurnPenalty(
            EffectDuration);
    }

    private void WriteResult(
        ActiveSkillExecutionContext context,
        Beast target)
    {
        resultWriter.WriteSkillUse(
            context.User,
            context.Skill);

        resultWriter.WriteTurnPenalty(
            target,
            EffectDuration);
    }
}