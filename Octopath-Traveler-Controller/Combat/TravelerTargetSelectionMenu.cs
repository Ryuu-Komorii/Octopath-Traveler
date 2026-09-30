using Octopath_Traveler_View;

namespace Octopath_Traveler;

public class TravelerTargetSelectionMenu
{
    private const string SeparatorLine =
        "----------------------------------------";

    private const string SelectionHeaderFormat =
        "Seleccione un objetivo para {0}";

    private const string CancelOptionText =
        "Cancelar";

    private const int FirstOptionNumber = 1;

    private readonly View view;

    public TravelerTargetSelectionMenu(
        View view)
    {
        this.view = view;
    }

    public Traveler? SelectLivingTarget(
        Traveler user,
        IReadOnlyList<Traveler> travelers)
    {
        Traveler[] availableTargets =
            travelers
                .Where(IsAlive)
                .ToArray();

        return SelectTarget(
            user,
            availableTargets);
    }

    public Traveler? SelectFallenTarget(
        Traveler user,
        IReadOnlyList<Traveler> travelers)
    {
        Traveler[] availableTargets =
            travelers
                .Where(IsFallen)
                .ToArray();

        return SelectTarget(
            user,
            availableTargets);
    }

    private Traveler? SelectTarget(
        Traveler user,
        IReadOnlyList<Traveler> availableTargets)
    {
        WriteMenu(
            user,
            availableTargets);

        int selectedOption =
            ReadSelectedOption();

        return GetSelectedTarget(
            availableTargets,
            selectedOption);
    }

    private bool IsAlive(
        Traveler traveler)
    {
        return traveler.IsAlive();
    }

    private bool IsFallen(
        Traveler traveler)
    {
        return !traveler.IsAlive();
    }

    private void WriteMenu(
        Traveler user,
        IReadOnlyList<Traveler> travelers)
    {
        view.WriteLine(
            SeparatorLine);

        view.WriteLine(
            string.Format(
                SelectionHeaderFormat,
                user.Name));

        WriteTravelerOptions(
            travelers);

        WriteCancelOption(
            travelers.Count);
    }

    private void WriteTravelerOptions(
        IReadOnlyList<Traveler> travelers)
    {
        int optionNumber =
            FirstOptionNumber;

        foreach (Traveler traveler in travelers)
        {
            WriteTravelerOption(
                traveler,
                optionNumber);

            optionNumber++;
        }
    }

    private void WriteTravelerOption(
        Traveler traveler,
        int optionNumber)
    {
        view.WriteLine(
            $"{optionNumber}: {FormatTraveler(traveler)}");
    }

    private string FormatTraveler(
        Traveler traveler)
    {
        return
            $"{traveler.Name} - " +
            $"HP:{traveler.CurrentHP}/{traveler.MaxHP} " +
            $"SP:{traveler.CurrentSP}/{traveler.MaxSP} " +
            $"BP:{traveler.BoostPoints}";
    }

    private void WriteCancelOption(
        int travelerCount)
    {
        int cancelOption =
            GetCancelOptionNumber(
                travelerCount);

        view.WriteLine(
            $"{cancelOption}: {CancelOptionText}");
    }

    private int ReadSelectedOption()
    {
        return int.Parse(
            view.ReadLine());
    }

    private Traveler? GetSelectedTarget(
        IReadOnlyList<Traveler> travelers,
        int selectedOption)
    {
        if (IsCancelOption(
            travelers,
            selectedOption))
        {
            return null;
        }

        int targetIndex =
            selectedOption -
            FirstOptionNumber;

        return travelers[
            targetIndex];
    }

    private bool IsCancelOption(
        IReadOnlyList<Traveler> travelers,
        int selectedOption)
    {
        return selectedOption ==
               GetCancelOptionNumber(
                   travelers.Count);
    }

    private int GetCancelOptionNumber(
        int travelerCount)
    {
        return travelerCount +
               FirstOptionNumber;
    }
}