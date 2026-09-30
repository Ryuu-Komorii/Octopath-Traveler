using Octopath_Traveler_View;

namespace Octopath_Traveler;

public class BeastSkillResultWriter
{
    private readonly View view;

    public BeastSkillResultWriter(
        View view)
    {
        this.view = view;
    }

    public void WriteSkillUse(
        Beast beast,
        BeastSkillCatalogEntry skill)
    {
        view.WriteLine(
            CombatText.SeparatorLine);

        view.WriteLine(
            string.Format(
                CombatText.SkillUseFormat,
                beast.Name,
                skill.Name));
    }

    public void WriteDefendIfNeeded(
        Traveler target)
    {
        if (IsNotDefending(target))
        {
            return;
        }

        view.WriteLine(
            string.Format(
                CombatText.DefendFormat,
                target.Name));
    }

    public void WriteDamage(
        Traveler target,
        int damage,
        BeastAttackKind attackKind)
    {
        string damageFormat =
            GetDamageFormat(
                attackKind);

        view.WriteLine(
            string.Format(
                damageFormat,
                target.Name,
                damage));
    }

    public void WriteDamage(
        Traveler target,
        int damage)
    {
        view.WriteLine(
            string.Format(
                CombatText.DamageFormat,
                target.Name,
                damage));
    }

    public void WriteRemainingHitPoints(
        IReadOnlyList<Traveler> targets)
    {
        foreach (Traveler target in targets)
        {
            view.WriteLine(
                string.Format(
                    CombatText.RemainingHitPointsFormat,
                    target.Name,
                    target.CurrentHP));
        }
    }

    private bool IsNotDefending(
        Traveler target)
    {
        return !target.IsDefending();
    }

    private string GetDamageFormat(
        BeastAttackKind attackKind)
    {
        return attackKind switch
        {
            BeastAttackKind.Physical =>
                CombatText.PhysicalDamageFormat,

            BeastAttackKind.Elemental =>
                CombatText.ElementalDamageFormat,

            _ =>
                throw new ArgumentOutOfRangeException(
                    nameof(attackKind))
        };
    }
}