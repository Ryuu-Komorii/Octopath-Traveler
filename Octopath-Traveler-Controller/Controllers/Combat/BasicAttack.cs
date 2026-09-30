using Octopath_Traveler_View;

namespace Octopath_Traveler;

public enum ActionExecutionResult
{
    Completed,
    Cancelled
}

public class BasicAttack
{
    private const double BasicAttackModifier =
        1.3;

    private const int MinimumBoostPointsToSelect =
        1;

    private const string AttackHeaderFormat =
        "{0} ataca";

    private readonly View view;
    private readonly WeaponSelectionMenu weaponSelectionMenu;
    private readonly BeastTargetSelectionMenu targetSelectionMenu;
    private readonly BoostPointSelectionMenu boostPointSelectionMenu;
    private readonly BeastDamageResolver damageResolver;

    public BasicAttack(
        View view)
    {
        this.view =
            view;

        weaponSelectionMenu =
            new WeaponSelectionMenu(
                view);

        targetSelectionMenu =
            new BeastTargetSelectionMenu(
                view);

        boostPointSelectionMenu =
            new BoostPointSelectionMenu(
                view);

        damageResolver =
            new BeastDamageResolver();
    }

    public ActionExecutionResult Execute(
        Traveler traveler,
        IReadOnlyList<Beast> beasts)
    {
        string? selectedWeapon =
            weaponSelectionMenu.SelectWeapon(
                traveler);

        if (WasWeaponSelectionCancelled(
            selectedWeapon))
        {
            return ActionExecutionResult.Cancelled;
        }

        Beast? selectedTarget =
            targetSelectionMenu.SelectTarget(
                traveler,
                beasts);

        if (WasTargetSelectionCancelled(
            selectedTarget))
        {
            return ActionExecutionResult.Cancelled;
        }

        ReadBoostPointsIfAvailable(
            traveler);

        BasicAttackContext context =
            CreateContext(
                traveler,
                selectedTarget!,
                selectedWeapon!);

        BeastDamageResult damageResult =
            ResolveDamage(
                context);

        WriteAttackSummary(
            context,
            damageResult);

        return ActionExecutionResult.Completed;
    }

    private BasicAttackContext CreateContext(
        Traveler attacker,
        Beast target,
        string weapon)
    {
        return new BasicAttackContext
        {
            Attacker =
                attacker,

            Target =
                target,

            Weapon =
                weapon
        };
    }

    private BeastDamageResult ResolveDamage(
        BasicAttackContext context)
    {
        DamageRequest damageRequest =
            CreateDamageRequest(
                context);

        return damageResolver.Resolve(
            damageRequest,
            context.Target,
            context.Weapon);
    }

    private DamageRequest CreateDamageRequest(
        BasicAttackContext context)
    {
        return new DamageRequest
        {
            OffensiveStat =
                context.Attacker.PhysicalAttack,

            DefensiveStat =
                context.Target.PhysicalDefense,

            Modifier =
                BasicAttackModifier
        };
    }

    private bool WasWeaponSelectionCancelled(
        string? selectedWeapon)
    {
        return selectedWeapon is null;
    }

    private bool WasTargetSelectionCancelled(
        Beast? selectedTarget)
    {
        return selectedTarget is null;
    }

    private void ReadBoostPointsIfAvailable(
        Traveler traveler)
    {
        if (HasBoostPointsToSelect(
            traveler))
        {
            boostPointSelectionMenu
                .ReadBoostPoints();
        }
    }

    private bool HasBoostPointsToSelect(
        Traveler traveler)
    {
        return traveler.BoostPoints >=
               MinimumBoostPointsToSelect;
    }

    private void WriteAttackSummary(
        BasicAttackContext context,
        BeastDamageResult damageResult)
    {
        view.WriteLine(
            CombatText.SeparatorLine);

        WriteAttackHeader(
            context.Attacker);

        WriteDamageMessage(
            context,
            damageResult);

        WriteBreakingPointMessage(
            context.Target,
            damageResult);

        WriteRemainingHitPoints(
            context.Target);
    }

    private void WriteAttackHeader(
        Traveler attacker)
    {
        view.WriteLine(
            string.Format(
                AttackHeaderFormat,
                attacker.Name));
    }

    private void WriteDamageMessage(
        BasicAttackContext context,
        BeastDamageResult damageResult)
    {
        string weaknessSuffix =
            GetWeaknessSuffix(
                damageResult);

        view.WriteLine(
            string.Format(
                CombatText.TypedDamageFormat,
                context.Target.Name,
                damageResult.Damage,
                context.Weapon,
                weaknessSuffix));
    }

    private string GetWeaknessSuffix(
        BeastDamageResult damageResult)
    {
        if (HasWeakness(
            damageResult))
        {
            return CombatText.WeaknessSuffix;
        }

        return string.Empty;
    }

    private bool HasWeakness(
        BeastDamageResult damageResult)
    {
        return damageResult.IsWeakness;
    }

    private void WriteBreakingPointMessage(
        Beast target,
        BeastDamageResult damageResult)
    {
        if (DidNotEnterBreakingPoint(
            damageResult))
        {
            return;
        }

        view.WriteLine(
            string.Format(
                CombatText.BreakingPointFormat,
                target.Name));
    }

    private bool DidNotEnterBreakingPoint(
        BeastDamageResult damageResult)
    {
        return !damageResult
            .EnteredBreakingPoint;
    }

    private void WriteRemainingHitPoints(
        Beast target)
    {
        view.WriteLine(
            string.Format(
                CombatText.RemainingHitPointsFormat,
                target.Name,
                target.CurrentHP));
    }
}