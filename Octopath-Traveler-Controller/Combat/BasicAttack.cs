using Octopath_Traveler_View;

namespace Octopath_Traveler;

public enum ActionExecutionResult
{
    Completed,
    Cancelled
}

public class BasicAttack
{
    private const double BasicAttackModifier = 1.3;
    private const int MinimumBoostPointsToSelect = 1;

    private const string SeparatorLine =
        "----------------------------------------";

    private const string AttackHeaderFormat =
        "{0} ataca";

    private const string DamageMessageFormat =
        "{0} recibe {1} de daño de tipo {2}{3}";

    private const string WeaknessSuffix =
        " con debilidad";

    private const string BreakingPointMessageFormat =
        "{0} entra en Breaking Point";

    private const string RemainingHitPointsFormat =
        "{0} termina con HP:{1}";

    private readonly View view;

    private readonly WeaponSelectionMenu
        weaponSelectionMenu;

    private readonly BeastTargetSelectionMenu
        targetSelectionMenu;

    private readonly BoostPointSelectionMenu
        boostPointSelectionMenu;

    private readonly BeastDamageResolver
        damageResolver;

    public BasicAttack(View view)
    {
        this.view = view;

        weaponSelectionMenu =
            new WeaponSelectionMenu(view);

        targetSelectionMenu =
            new BeastTargetSelectionMenu(view);

        boostPointSelectionMenu =
            new BoostPointSelectionMenu(view);

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

        BeastDamageResult damageResult =
            ResolveDamage(
                traveler,
                selectedTarget!,
                selectedWeapon!);

        WriteAttackSummary(
            traveler,
            selectedTarget!,
            selectedWeapon!,
            damageResult);

        return ActionExecutionResult.Completed;
    }

    private BeastDamageResult ResolveDamage(
        Traveler traveler,
        Beast target,
        string weapon)
    {
        DamageRequest damageRequest =
            new DamageRequest
            {
                OffensiveStat =
                    traveler.PhysicalAttack,

                DefensiveStat =
                    target.PhysicalDefense,

                Modifier =
                    BasicAttackModifier
            };

        return damageResolver.Resolve(
            damageRequest,
            target,
            weapon);
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
        Traveler traveler,
        Beast target,
        string weapon,
        BeastDamageResult damageResult)
    {
        view.WriteLine(
            SeparatorLine);

        WriteAttackHeader(
            traveler);

        WriteDamageMessage(
            target,
            weapon,
            damageResult);

        WriteBreakingPointMessage(
            target,
            damageResult);

        WriteRemainingHitPoints(
            target);
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
        Beast target,
        string weapon,
        BeastDamageResult damageResult)
    {
        string weaknessSuffix =
            GetWeaknessSuffix(
                damageResult);

        view.WriteLine(
            string.Format(
                DamageMessageFormat,
                target.Name,
                damageResult.Damage,
                weapon,
                weaknessSuffix));
    }

    private string GetWeaknessSuffix(
        BeastDamageResult damageResult)
    {
        if (damageResult.IsWeakness)
        {
            return WeaknessSuffix;
        }

        return string.Empty;
    }

    private void WriteBreakingPointMessage(
        Beast target,
        BeastDamageResult damageResult)
    {
        if (!damageResult.EnteredBreakingPoint)
        {
            return;
        }

        view.WriteLine(
            string.Format(
                BreakingPointMessageFormat,
                target.Name));
    }

    private void WriteRemainingHitPoints(
        Beast target)
    {
        view.WriteLine(
            string.Format(
                RemainingHitPointsFormat,
                target.Name,
                target.CurrentHP));
    }
}