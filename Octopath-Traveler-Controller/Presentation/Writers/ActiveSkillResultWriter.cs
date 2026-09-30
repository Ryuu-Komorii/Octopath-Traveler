using Octopath_Traveler_View;

namespace Octopath_Traveler;

public class ActiveSkillResultWriter
{
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
            CombatText.SeparatorLine);

        view.WriteLine(
            string.Format(
                CombatText.SkillUseFormat,
                user.Name,
                skill.Name));
    }

    public void WriteHealing(
        Traveler target,
        int amount)
    {
        view.WriteLine(
            string.Format(
                CombatText.HealingFormat,
                target.Name,
                amount));
    }

    public void WriteRevive(
        Traveler target)
    {
        view.WriteLine(
            string.Format(
                CombatText.ReviveFormat,
                target.Name));
    }

    public void WriteRemainingHitPoints(
        Traveler target)
    {
        view.WriteLine(
            string.Format(
                CombatText.RemainingHitPointsFormat,
                target.Name,
                target.CurrentHP));
    }

    public void WriteTurnPenalty(
        Beast target,
        int duration)
    {
        view.WriteLine(
            string.Format(
                CombatText.TurnPenaltyFormat,
                target.Name,
                duration));
    }
}