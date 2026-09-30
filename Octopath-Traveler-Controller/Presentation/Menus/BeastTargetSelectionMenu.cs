using Octopath_Traveler_View;

namespace Octopath_Traveler;

public class BeastTargetSelectionMenu
    : SelectionMenu<Beast>
{
    public BeastTargetSelectionMenu(
        View view)
        : base(view)
    {
    }

    public Beast? SelectTarget(
        Traveler traveler,
        IReadOnlyList<Beast> beasts)
    {
        Beast[] livingBeasts =
            GetLivingBeasts(
                beasts);

        string header =
            GetSelectionHeader(
                traveler);

        return SelectOption(
            header,
            livingBeasts);
    }

    protected override string FormatOption(
        Beast beast)
    {
        return string.Format(
            CombatText.BeastStatusFormat,
            beast.Name,
            beast.CurrentHP,
            beast.MaxHP,
            beast.Shields);
    }

    private Beast[] GetLivingBeasts(
        IReadOnlyList<Beast> beasts)
    {
        return beasts
            .Where(IsAlive)
            .ToArray();
    }

    private bool IsAlive(
        Beast beast)
    {
        return beast.IsAlive();
    }

    private string GetSelectionHeader(
        Traveler traveler)
    {
        return string.Format(
            CombatText.TargetSelectionHeaderFormat,
            traveler.Name);
    }
}