using Octopath_Traveler_View;

namespace Octopath_Traveler;

public class BeastTurn
{
    private const double AttackModifier = 1.3;
    private const string SeparatorLine =
        "----------------------------------------";
    private const string SkillUseMessageFormat =
        "{0} usa {1}";
    private const string DamageMessageFormat =
        "{0} recibe {1} de daño físico";
    private const string RemainingHitPointsFormat =
        "{0} termina con HP:{1}";

    private readonly View view;
    private readonly DamageCalculator damageCalculator;

    public BeastTurn(View view)
    {
        this.view = view;
        damageCalculator = new DamageCalculator();
    }

    public void Execute(
        Beast beast,
        IReadOnlyList<Traveler> travelers)
    {
        Traveler target = FindTarget(travelers);
        int damage = CalculateDamage(beast, target);

        target.ReceiveDamage(damage);

        WriteTurnSummary(beast, target, damage);
    }

    private Traveler FindTarget(
        IReadOnlyList<Traveler> travelers)
    {
        return travelers
            .Where(IsAlive)
            .OrderByDescending(GetCurrentHitPoints)
            .First();
    }

    private bool IsAlive(Traveler traveler)
    {
        return traveler.IsAlive();
    }

    private int GetCurrentHitPoints(Traveler traveler)
    {
        return traveler.CurrentHP;
    }

    private int CalculateDamage(
        Beast beast,
        Traveler target)
    {
        return damageCalculator.CalculatePhysicalDamage(
            beast.PhysicalAttack,
            target.PhysicalDefense,
            AttackModifier);
    }

    private void WriteTurnSummary(
        Beast beast,
        Traveler target,
        int damage)
    {
        view.WriteLine(SeparatorLine);
        WriteSkillUseMessage(beast);
        WriteDamageMessage(target, damage);
        WriteRemainingHitPoints(target);
    }

    private void WriteSkillUseMessage(Beast beast)
    {
        view.WriteLine(
            string.Format(
                SkillUseMessageFormat,
                beast.Name,
                beast.SkillName));
    }

    private void WriteDamageMessage(
        Traveler target,
        int damage)
    {
        view.WriteLine(
            string.Format(
                DamageMessageFormat,
                target.Name,
                damage));
    }

    private void WriteRemainingHitPoints(
        Traveler target)
    {
        view.WriteLine(
            string.Format(
                RemainingHitPointsFormat,
                target.Name,
                target.CurrentHP));
    }
}