using Octopath_Traveler_View;

namespace Octopath_Traveler;

public class WeaponSelectionMenu
{
    private const string SeparatorLine =
        "----------------------------------------";

    private const string SelectionHeader =
        "Seleccione un arma";

    private const string CancelOptionText =
        "Cancelar";

    private const int FirstOptionNumber = 1;

    private readonly View view;

    public WeaponSelectionMenu(View view)
    {
        this.view = view;
    }

    public string? SelectWeapon(Traveler traveler)
    {
        return SelectWeapon(
            traveler.Weapons);
    }

    public string? SelectWeapon(
        IReadOnlyList<string> weapons)
    {
        WriteMenu(weapons);

        int selectedOption =
            ReadSelectedOption();

        return GetSelectedWeapon(
            weapons,
            selectedOption);
    }

    private void WriteMenu(
        IReadOnlyList<string> weapons)
    {
        view.WriteLine(
            SeparatorLine);

        view.WriteLine(
            SelectionHeader);

        WriteWeaponOptions(
            weapons);

        WriteCancelOption(
            weapons.Count);
    }

    private void WriteWeaponOptions(
        IReadOnlyList<string> weapons)
    {
        int optionNumber =
            FirstOptionNumber;

        foreach (string weapon in weapons)
        {
            WriteWeaponOption(
                optionNumber,
                weapon);

            optionNumber++;
        }
    }

    private void WriteWeaponOption(
        int optionNumber,
        string weapon)
    {
        view.WriteLine(
            $"{optionNumber}: {weapon}");
    }

    private void WriteCancelOption(
        int weaponCount)
    {
        int cancelOption =
            GetCancelOptionNumber(
                weaponCount);

        view.WriteLine(
            $"{cancelOption}: {CancelOptionText}");
    }

    private int ReadSelectedOption()
    {
        return int.Parse(
            view.ReadLine());
    }

    private string? GetSelectedWeapon(
        IReadOnlyList<string> weapons,
        int selectedOption)
    {
        if (IsCancelOption(
            weapons,
            selectedOption))
        {
            return null;
        }

        int weaponIndex =
            GetWeaponIndex(
                selectedOption);

        return weapons[
            weaponIndex];
    }

    private bool IsCancelOption(
        IReadOnlyList<string> weapons,
        int selectedOption)
    {
        return selectedOption ==
               GetCancelOptionNumber(
                   weapons.Count);
    }

    private int GetWeaponIndex(
        int selectedOption)
    {
        return selectedOption -
               FirstOptionNumber;
    }

    private int GetCancelOptionNumber(
        int weaponCount)
    {
        return weaponCount +
               FirstOptionNumber;
    }
}