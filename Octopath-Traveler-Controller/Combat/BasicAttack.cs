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
        "{0} recibe {1} de daño de tipo {2}";

    private const string RemainingHitPointsFormat =
        "{0} termina con HP:{1}";

    private readonly View view;
    private readonly WeaponSelectionMenu weaponSelectionMenu;
    private readonly BeastTargetSelectionMenu targetSelectionMenu;
    private readonly BoostPointSelectionMenu boostPointSelectionMenu;
    private readonly DamageCalculator damageCalculator;

    public BasicAttack(View view)
    {
        this.view = view;

        weaponSelectionMenu =
            new WeaponSelectionMenu(view);

        targetSelectionMenu =
            new BeastTargetSelectionMenu(view);

        boostPointSelectionMenu =
            new BoostPointSelectionMenu(view);

        damageCalculator =
            new DamageCalculator();
    }

    public ActionExecutionResult Execute(
        Traveler traveler,
        IReadOnlyList<Beast> beasts)
    {
        string? selectedWeapon =
            weaponSelectionMenu.SelectWeapon(traveler);

        if (WasWeaponSelectionCancelled(selectedWeapon))
        {
            return ActionExecutionResult.Cancelled;
        }

        Beast? selectedTarget =
            targetSelectionMenu.SelectTarget(
                traveler,
                beasts);

        if (WasTargetSelectionCancelled(selectedTarget))
        {
            return ActionExecutionResult.Cancelled;
        }

        string weapon = selectedWeapon!;
        Beast target = selectedTarget!;

        ReadBoostPointsIfAvailable(traveler);

        int damage = CalculateDamage(
            traveler,
            target);

        target.ReceiveDamage(damage);

        WriteAttackSeparator();
        WriteAttackHeader(traveler);

        WriteDamageMessage(
            target,
            damage,
            weapon);

        WriteRemainingHitPoints(target);

        return ActionExecutionResult.Completed;
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
        if (HasBoostPointsToSelect(traveler))
        {
            boostPointSelectionMenu.ReadBoostPoints();
        }
    }

    private bool HasBoostPointsToSelect(
        Traveler traveler)
    {
        return traveler.BoostPoints >=
               MinimumBoostPointsToSelect;
    }

    private int CalculateDamage(
        Traveler traveler,
        Beast target)
    {
        return damageCalculator.CalculatePhysicalDamage(
            traveler.PhysicalAttack,
            target.PhysicalDefense,
            BasicAttackModifier);
    }

    private void WriteAttackSeparator()
    {
        view.WriteLine(SeparatorLine);
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
        int damage,
        string weapon)
    {
        view.WriteLine(
            string.Format(
                DamageMessageFormat,
                target.Name,
                damage,
                weapon));
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