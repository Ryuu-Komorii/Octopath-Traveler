using Octopath_Traveler_View;

namespace Octopath_Traveler;

public class WeaponSelectionMenu
{
    private const string SeparatorLine = "----------------------------------------";
    private const string SelectionHeader = "Seleccione un arma";
    private const string CancelOptionText = "Cancelar";
    private const int FirstOptionNumber = 1;

    private readonly View view;

    public WeaponSelectionMenu(View view)
    {
        this.view = view;
    }

    public string? SelectWeapon(Traveler traveler)
    {
        WriteMenu(traveler.Weapons);
        int selectedOption = ReadSelectedOption();

        return GetSelectedWeapon(
            traveler.Weapons,
            selectedOption);
    }

    private void WriteMenu(IReadOnlyList<string> weapons)
    {
        view.WriteLine(SeparatorLine);
        view.WriteLine(SelectionHeader);
        WriteWeaponOptions(weapons);
        WriteCancelOption(weapons.Count);
    }

    private void WriteWeaponOptions(
        IReadOnlyList<string> weapons)
    {
        int optionNumber = FirstOptionNumber;

        foreach (string weapon in weapons)
        {
            view.WriteLine($"{optionNumber}: {weapon}");
            optionNumber++;
        }
    }

    private void WriteCancelOption(int weaponCount)
    {
        int cancelOption = GetCancelOptionNumber(weaponCount);

        view.WriteLine(
            $"{cancelOption}: {CancelOptionText}");
    }

    private int ReadSelectedOption()
    {
        return int.Parse(view.ReadLine());
    }

    private string? GetSelectedWeapon(
        IReadOnlyList<string> weapons,
        int selectedOption)
    {
        if (IsCancelOption(weapons, selectedOption))
        {
            return null;
        }

        int weaponIndex =
            selectedOption - FirstOptionNumber;

        return weapons[weaponIndex];
    }

    private bool IsCancelOption(
        IReadOnlyList<string> weapons,
        int selectedOption)
    {
        return selectedOption ==
               GetCancelOptionNumber(weapons.Count);
    }

    private int GetCancelOptionNumber(int weaponCount)
    {
        return weaponCount + FirstOptionNumber;
    }
}