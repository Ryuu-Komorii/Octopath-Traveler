using Octopath_Traveler_View;

namespace Octopath_Traveler;

public class ActiveSkillResultWriter
{
    private const string SeparatorLine =
        "----------------------------------------";

    private const string SkillUseFormat =
        "{0} usa {1}";

    private const string HealingFormat =
        "{0} recupera {1} de vida";

    private const string ReviveFormat =
        "{0} revive";

    private const string RemainingHitPointsFormat =
        "{0} termina con HP:{1}";

    private const string TurnPenaltyFormat =
        "{0} tendrá menor prioridad de turno durante {1} rondas";

    private readonly View view;

    public ActiveSkillResultWriter(
        View view)
    {
        this.view = view;
    }

    public void WriteSkillUse(
        Traveler user,
        ActiveSkillCatalogEntry skill)
    {
        view.WriteLine(
            SeparatorLine);

        view.WriteLine(
            string.Format(
                SkillUseFormat,
                user.Name,
                skill.Name));
    }

    public void WriteHealing(
        Traveler target,
        int amount)
    {
        view.WriteLine(
            string.Format(
                HealingFormat,
                target.Name,
                amount));
    }

    public void WriteRevive(
        Traveler target)
    {
        view.WriteLine(
            string.Format(
                ReviveFormat,
                target.Name));
    }

    public void WriteRemainingHitPoints(
        Traveler target)
    {
        view.WriteLine(
            string.Format(
                RemainingHitPointsFormat,
                target.Name,
                target.CurrentHP));
    }

    public void WriteTurnPenalty(
        Beast target,
        int duration)
    {
        view.WriteLine(
            string.Format(
                TurnPenaltyFormat,
                target.Name,
                duration));
    }
}