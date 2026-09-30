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
    private const double NormalDamageMultiplier =
        1.0;

    private const double DefendDamageMultiplier =
        0.5;

    private const int FirstHitIndex =
        0;

    private readonly BeastSkillResultWriter
        resultWriter;

    private readonly BeastAttackKind
        attackKind;

    private readonly TravelerTargetSelector
        targetSelector;

    private readonly DamageCalculator
        damageCalculator;

    public DamageBeastSkillExecutionStrategy(
        BeastSkillResultWriter resultWriter,
        BeastAttackKind attackKind,
        TravelerTargetSelector targetSelector)
    {
        this.resultWriter =
            resultWriter;

        this.attackKind =
            attackKind;

        this.targetSelector =
            targetSelector;

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

        resultWriter.WriteSkillUse(
            beast,
            skill);

        ApplySkillToTargets(
            beast,
            targets,
            skill);

        resultWriter.WriteRemainingHitPoints(
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
        for (int hit = FirstHitIndex;
             HasRemainingHits(
                 hit,
                 skill);
             hit++)
        {
            ApplyHit(
                beast,
                target,
                skill);
        }
    }

    private bool HasRemainingHits(
        int currentHit,
        BeastSkillCatalogEntry skill)
    {
        return currentHit <
               skill.Hits;
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

        resultWriter.WriteDefendIfNeeded(
            target);

        target.ReceiveDamage(
            damage);

        resultWriter.WriteDamage(
            target,
            damage,
            attackKind);
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
        return new DamageRequest
        {
            OffensiveStat =
                GetOffensiveStat(
                    beast),

            DefensiveStat =
                GetDefensiveStat(
                    target),

            Modifier =
                skill.Modifier,

            FinalMultiplier =
                GetDefendMultiplier(
                    target)
        };
    }

    private int GetOffensiveStat(
        Beast beast)
    {
        return attackKind switch
        {
            BeastAttackKind.Physical =>
                beast.PhysicalAttack,

            BeastAttackKind.Elemental =>
                beast.ElementalAttack,

            _ =>
                throw new ArgumentOutOfRangeException(
                    nameof(attackKind))
        };
    }

    private int GetDefensiveStat(
        Traveler target)
    {
        return attackKind switch
        {
            BeastAttackKind.Physical =>
                target.PhysicalDefense,

            BeastAttackKind.Elemental =>
                target.ElementalDefense,

            _ =>
                throw new ArgumentOutOfRangeException(
                    nameof(attackKind))
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
}

public class VortalClawExecutionStrategy
    : BeastSkillExecutionStrategy
{
    private const int HalfDivisor =
        2;

    private readonly BeastSkillResultWriter
        resultWriter;

    public VortalClawExecutionStrategy(
        BeastSkillResultWriter resultWriter)
    {
        this.resultWriter =
            resultWriter;
    }

    public override void Execute(
        Beast beast,
        IReadOnlyList<Traveler> travelers,
        BeastSkillCatalogEntry skill)
    {
        Traveler[] targets =
            GetLivingTravelers(
                travelers);

        resultWriter.WriteSkillUse(
            beast,
            skill);

        ApplyDamage(
            targets);

        resultWriter.WriteRemainingHitPoints(
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
            ApplyDamage(
                target);
        }
    }

    private void ApplyDamage(
        Traveler target)
    {
        int damage =
            CalculateDamage(
                target);

        target.ReceiveDamage(
            damage);

        resultWriter.WriteDamage(
            target,
            damage);
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
}