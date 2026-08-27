using Octopath_Traveler_View;

namespace Octopath_Traveler;

public class BeastTargetSelectionMenu
{
    private const string SeparatorLine = "----------------------------------------";
    private const string SelectionHeaderFormat =
        "Seleccione un objetivo para {0}";
    private const string CancelOptionText = "Cancelar";
    private const int FirstOptionNumber = 1;

    private readonly View view;

    public BeastTargetSelectionMenu(View view)
    {
        this.view = view;
    }

    public Beast? SelectTarget(
        Traveler traveler,
        IReadOnlyList<Beast> beasts)
    {
        Beast[] livingBeasts = GetLivingBeasts(beasts);

        WriteMenu(traveler, livingBeasts);
        int selectedOption = ReadSelectedOption();

        return GetSelectedTarget(
            livingBeasts,
            selectedOption);
    }

    private Beast[] GetLivingBeasts(
        IReadOnlyList<Beast> beasts)
    {
        return beasts
            .Where(IsAlive)
            .ToArray();
    }

    private bool IsAlive(Beast beast)
    {
        return beast.IsAlive();
    }

    private void WriteMenu(
        Traveler traveler,
        IReadOnlyList<Beast> livingBeasts)
    {
        view.WriteLine(SeparatorLine);
        view.WriteLine(GetSelectionHeader(traveler));

        WriteBeastOptions(livingBeasts);
        WriteCancelOption(livingBeasts.Count);
    }

    private string GetSelectionHeader(Traveler traveler)
    {
        return string.Format(
            SelectionHeaderFormat,
            traveler.Name);
    }

    private void WriteBeastOptions(
        IReadOnlyList<Beast> livingBeasts)
    {
        int optionNumber = FirstOptionNumber;

        foreach (Beast beast in livingBeasts)
        {
            WriteBeastOption(beast, optionNumber);
            optionNumber++;
        }
    }

    private void WriteBeastOption(
        Beast beast,
        int optionNumber)
    {
        view.WriteLine(
            $"{optionNumber}: {FormatBeast(beast)}");
    }

    private string FormatBeast(Beast beast)
    {
        return $"{beast.Name} - HP:{beast.CurrentHP}/{beast.MaxHP} " +
               $"Shields:{beast.Shields}";
    }

    private void WriteCancelOption(int beastCount)
    {
        int cancelOption =
            GetCancelOptionNumber(beastCount);

        view.WriteLine(
            $"{cancelOption}: {CancelOptionText}");
    }

    private int ReadSelectedOption()
    {
        return int.Parse(view.ReadLine());
    }

    private Beast? GetSelectedTarget(
        IReadOnlyList<Beast> livingBeasts,
        int selectedOption)
    {
        if (IsCancelOption(
                livingBeasts,
                selectedOption))
        {
            return null;
        }

        int targetIndex =
            selectedOption - FirstOptionNumber;

        return livingBeasts[targetIndex];
    }

    private bool IsCancelOption(
        IReadOnlyList<Beast> livingBeasts,
        int selectedOption)
    {
        return selectedOption ==
               GetCancelOptionNumber(livingBeasts.Count);
    }

    private int GetCancelOptionNumber(int beastCount)
    {
        return beastCount + FirstOptionNumber;
    }
}