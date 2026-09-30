using Octopath_Traveler_View;

namespace Octopath_Traveler;

public abstract class BeastSkillExecutionStrategy
{
    public abstract void Execute(
        Beast beast,
        IReadOnlyList<Traveler> travelers,
        BeastSkillCatalogEntry skill);
}

public class DamageBeastSkillExecutionStrategy
    : BeastSkillExecutionStrategy
{
    private const string SeparatorLine =
        "----------------------------------------";

    private const string SkillUseMessageFormat =
        "{0} usa {1}";

    private const string DefendMessageFormat =
        "{0} se defiende";

    private const string PhysicalDamageMessageFormat =
        "{0} recibe {1} de daño físico";

    private const string ElementalDamageMessageFormat =
        "{0} recibe {1} de daño elemental";

    private const string RemainingHitPointsFormat =
        "{0} termina con HP:{1}";

    private const double NormalDamageMultiplier =
        1.0;

    private const double DefendDamageMultiplier =
        0.5;

    private readonly View view;
    private readonly BeastAttackKind attackKind;
    private readonly TravelerTargetSelector targetSelector;
    private readonly DamageCalculator damageCalculator;

    public DamageBeastSkillExecutionStrategy(
        View view,
        BeastAttackKind attackKind,
        TravelerTargetSelector targetSelector)
    {
        this.view = view;
        this.attackKind = attackKind;
        this.targetSelector = targetSelector;

        damageCalculator =
            new DamageCalculator();
    }

    public override void Execute(
        Beast beast,
        IReadOnlyList<Traveler> travelers,
        BeastSkillCatalogEntry skill)
    {
        IReadOnlyList<Traveler> targets =
            targetSelector.Select(
                travelers);

        WriteSkillUse(
            beast,
            skill);

        ApplySkillToTargets(
            beast,
            targets,
            skill);

        WriteRemainingHitPoints(
            targets);
    }

    private void ApplySkillToTargets(
        Beast beast,
        IReadOnlyList<Traveler> targets,
        BeastSkillCatalogEntry skill)
    {
        foreach (Traveler target in targets)
        {
            ApplyHits(
                beast,
                target,
                skill);
        }
    }

    private void ApplyHits(
        Beast beast,
        Traveler target,
        BeastSkillCatalogEntry skill)
    {
        for (int hit = 0;
             hit < skill.Hits;
             hit++)
        {
            ApplyHit(
                beast,
                target,
                skill);
        }
    }

    private void ApplyHit(
        Beast beast,
        Traveler target,
        BeastSkillCatalogEntry skill)
    {
        int damage =
            CalculateDamage(
                beast,
                target,
                skill);

        WriteDefendMessage(
            target);

        target.ReceiveDamage(
            damage);

        WriteDamageMessage(
            target,
            damage);
    }

    private int CalculateDamage(
        Beast beast,
        Traveler target,
        BeastSkillCatalogEntry skill)
    {
        DamageRequest damageRequest =
            CreateDamageRequest(
                beast,
                target,
                skill);

        return damageCalculator.Calculate(
            damageRequest);
    }

    private DamageRequest CreateDamageRequest(
        Beast beast,
        Traveler target,
        BeastSkillCatalogEntry skill)
    {
        if (attackKind ==
            BeastAttackKind.Physical)
        {
            return CreatePhysicalDamageRequest(
                beast,
                target,
                skill);
        }

        return CreateElementalDamageRequest(
            beast,
            target,
            skill);
    }

    private DamageRequest CreatePhysicalDamageRequest(
        Beast beast,
        Traveler target,
        BeastSkillCatalogEntry skill)
    {
        return new DamageRequest
        {
            OffensiveStat =
                beast.PhysicalAttack,

            DefensiveStat =
                target.PhysicalDefense,

            Modifier =
                skill.Modifier,

            FinalMultiplier =
                GetDefendMultiplier(
                    target)
        };
    }

    private DamageRequest CreateElementalDamageRequest(
        Beast beast,
        Traveler target,
        BeastSkillCatalogEntry skill)
    {
        return new DamageRequest
        {
            OffensiveStat =
                beast.ElementalAttack,

            DefensiveStat =
                target.ElementalDefense,

            Modifier =
                skill.Modifier,

            FinalMultiplier =
                GetDefendMultiplier(
                    target)
        };
    }

    private double GetDefendMultiplier(
        Traveler target)
    {
        if (target.IsDefending())
        {
            return DefendDamageMultiplier;
        }

        return NormalDamageMultiplier;
    }

    private void WriteSkillUse(
        Beast beast,
        BeastSkillCatalogEntry skill)
    {
        view.WriteLine(
            SeparatorLine);

        view.WriteLine(
            string.Format(
                SkillUseMessageFormat,
                beast.Name,
                skill.Name));
    }

    private void WriteDefendMessage(
        Traveler target)
    {
        if (!target.IsDefending())
        {
            return;
        }

        view.WriteLine(
            string.Format(
                DefendMessageFormat,
                target.Name));
    }

    private void WriteDamageMessage(
        Traveler target,
        int damage)
    {
        string messageFormat =
            GetDamageMessageFormat();

        view.WriteLine(
            string.Format(
                messageFormat,
                target.Name,
                damage));
    }

    private string GetDamageMessageFormat()
    {
        if (attackKind ==
            BeastAttackKind.Physical)
        {
            return PhysicalDamageMessageFormat;
        }

        return ElementalDamageMessageFormat;
    }

    private void WriteRemainingHitPoints(
        IReadOnlyList<Traveler> targets)
    {
        foreach (Traveler target in targets)
        {
            view.WriteLine(
                string.Format(
                    RemainingHitPointsFormat,
                    target.Name,
                    target.CurrentHP));
        }
    }
}

public class VortalClawExecutionStrategy
    : BeastSkillExecutionStrategy
{
    private const string SeparatorLine =
        "----------------------------------------";

    private const string SkillUseMessageFormat =
        "{0} usa {1}";

    private const string DamageMessageFormat =
        "{0} recibe {1} de daño";

    private const string RemainingHitPointsFormat =
        "{0} termina con HP:{1}";

    private const int HalfDivisor = 2;

    private readonly View view;

    public VortalClawExecutionStrategy(
        View view)
    {
        this.view = view;
    }

    public override void Execute(
        Beast beast,
        IReadOnlyList<Traveler> travelers,
        BeastSkillCatalogEntry skill)
    {
        Traveler[] targets =
            GetLivingTravelers(
                travelers);

        WriteSkillUse(
            beast,
            skill);

        ApplyDamage(
            targets);

        WriteRemainingHitPoints(
            targets);
    }

    private Traveler[] GetLivingTravelers(
        IReadOnlyList<Traveler> travelers)
    {
        return travelers
            .Where(IsAlive)
            .ToArray();
    }

    private bool IsAlive(
        Traveler traveler)
    {
        return traveler.IsAlive();
    }

    private void ApplyDamage(
        IReadOnlyList<Traveler> targets)
    {
        foreach (Traveler target in targets)
        {
            int damage =
                CalculateDamage(
                    target);

            target.ReceiveDamage(
                damage);

            WriteDamage(
                target,
                damage);
        }
    }

    private int CalculateDamage(
        Traveler target)
    {
        int finalHitPoints =
            target.CurrentHP /
            HalfDivisor;

        return target.CurrentHP -
               finalHitPoints;
    }

    private void WriteSkillUse(
        Beast beast,
        BeastSkillCatalogEntry skill)
    {
        view.WriteLine(
            SeparatorLine);

        view.WriteLine(
            string.Format(
                SkillUseMessageFormat,
                beast.Name,
                skill.Name));
    }

    private void WriteDamage(
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
        IReadOnlyList<Traveler> targets)
    {
        foreach (Traveler target in targets)
        {
            view.WriteLine(
                string.Format(
                    RemainingHitPointsFormat,
                    target.Name,
                    target.CurrentHP));
        }
    }
}