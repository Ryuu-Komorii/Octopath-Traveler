using Octopath_Traveler_View;

namespace Octopath_Traveler;

public class WeaponSelectionMenu
    : SelectionMenu<string>
{
    public WeaponSelectionMenu(View view)
        : base(view)
    {
    }

    public string? SelectWeapon(
        Traveler traveler)
    {
        return SelectWeapon(
            traveler.Weapons);
    }

    public string? SelectWeapon(
        IReadOnlyList<string> weapons)
    {
        return SelectOption(
            CombatText.WeaponSelectionHeader,
            weapons);
    }

    protected override string FormatOption(
        string weapon)
    {
        return weapon;
    }
}