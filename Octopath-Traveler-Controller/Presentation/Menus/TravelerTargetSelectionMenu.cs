using Octopath_Traveler_View;

namespace Octopath_Traveler;

public class TravelerTargetSelectionMenu
    : SelectionMenu<Traveler>
{
    public TravelerTargetSelectionMenu(
        View view)
        : base(view)
    {
    }

    public Traveler? SelectLivingTarget(
        Traveler user,
        IReadOnlyList<Traveler> travelers)
    {
        Traveler[] livingTravelers =
            travelers
                .Where(IsAlive)
                .ToArray();

        return SelectTraveler(
            user,
            livingTravelers);
    }

    public Traveler? SelectFallenTarget(
        Traveler user,
        IReadOnlyList<Traveler> travelers)
    {
        Traveler[] fallenTravelers =
            travelers
                .Where(IsFallen)
                .ToArray();

        return SelectTraveler(
            user,
            fallenTravelers);
    }

    protected override string FormatOption(
        Traveler traveler)
    {
        return string.Format(
            CombatText.TravelerStatusFormat,
            traveler.Name,
            traveler.CurrentHP,
            traveler.MaxHP,
            traveler.CurrentSP,
            traveler.MaxSP,
            traveler.BoostPoints);
    }

    private Traveler? SelectTraveler(
        Traveler user,
        IReadOnlyList<Traveler> travelers)
    {
        string header =
            GetSelectionHeader(
                user);

        return SelectOption(
            header,
            travelers);
    }

    private string GetSelectionHeader(
        Traveler traveler)
    {
        return string.Format(
            CombatText.TargetSelectionHeaderFormat,
            traveler.Name);
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
}