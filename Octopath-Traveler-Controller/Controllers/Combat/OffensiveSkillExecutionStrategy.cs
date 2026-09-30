using Octopath_Traveler_View;

namespace Octopath_Traveler;

public abstract class OffensiveSkillExecutionStrategy
{
    private const string SingleTarget =
        "Single";

    private const int NoMinimumRemainingHitPoints =
        0;

    private const double StandardFinalDamageMultiplier =
        1.0;

    private readonly View view;
    private readonly BeastTargetSelectionMenu targetSelectionMenu;
    private readonly BoostPointSelectionMenu boostPointSelectionMenu;

    protected BeastDamageResolver DamageResolver { get; }

    protected WeaponSelectionMenu WeaponSelectionMenu { get; }

    protected OffensiveSkillExecutionStrategy(
        View view)
    {
        this.view =
            view;

        targetSelectionMenu =
            new BeastTargetSelectionMenu(
                view);

        boostPointSelectionMenu =
            new BoostPointSelectionMenu(
                view);

        DamageResolver =
            new BeastDamageResolver();

        WeaponSelectionMenu =
            new WeaponSelectionMenu(
                view);
    }

    public ActionExecutionResult Execute(
        ActiveSkillExecutionContext context)
    {
        IReadOnlyList<string>? attackTypes =
            SelectAttackTypes(
                context);

        if (WasAttackTypeSelectionCancelled(
            attackTypes))
        {
            return ActionExecutionResult.Cancelled;
        }

        Beast[]? targets =
            SelectTargets(
                context);

        if (WasTargetSelectionCancelled(
            targets))
        {
            return ActionExecutionResult.Cancelled;
        }

        boostPointSelectionMenu
            .ReadBoostPoints();

        context.User.SpendSkillPoints(
            context.Skill.SP);

        ExecuteSkill(
            context,
            targets!,
            attackTypes!);

        return ActionExecutionResult.Completed;
    }

    protected virtual IReadOnlyList<string>?
        SelectAttackTypes(
            ActiveSkillExecutionContext context)
    {
        return [context.Skill.Type];
    }

    protected virtual double
        GetFinalDamageMultiplier(
            Traveler traveler)
    {
        return StandardFinalDamageMultiplier;
    }

    protected virtual int
        GetMinimumRemainingHitPoints()
    {
        return NoMinimumRemainingHitPoints;
    }

    private bool WasAttackTypeSelectionCancelled(
        IReadOnlyList<string>? attackTypes)
    {
        return attackTypes is null;
    }

    private bool WasTargetSelectionCancelled(
        IReadOnlyList<Beast>? targets)
    {
        return targets is null;
    }

    private Beast[]? SelectTargets(
        ActiveSkillExecutionContext context)
    {
        if (HasSingleTarget(
            context.Skill))
        {
            return SelectSingleTarget(
                context);
        }

        return GetLivingBeasts(
            context.Beasts);
    }

    private Beast[]? SelectSingleTarget(
        ActiveSkillExecutionContext context)
    {
        Beast? selectedTarget =
            targetSelectionMenu.SelectTarget(
                context.User,
                context.Beasts);

        if (WasSingleTargetSelectionCancelled(
            selectedTarget))
        {
            return null;
        }

        return [selectedTarget!];
    }

    private bool WasSingleTargetSelectionCancelled(
        Beast? selectedTarget)
    {
        return selectedTarget is null;
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

    private bool HasSingleTarget(
        ActiveSkillCatalogEntry skill)
    {
        return skill.Target ==
               SingleTarget;
    }

    private void ExecuteSkill(
        ActiveSkillExecutionContext context,
        IReadOnlyList<Beast> targets,
        IReadOnlyList<string> attackTypes)
    {
        WriteSkillUse(
            context);

        foreach (Beast target in targets)
        {
            ApplyHits(
                context,
                target,
                attackTypes);
        }

        WriteRemainingHitPoints(
            targets);
    }

    private void ApplyHits(
        ActiveSkillExecutionContext context,
        Beast target,
        IReadOnlyList<string> attackTypes)
    {
        foreach (string attackType in attackTypes)
        {
            ApplyHit(
                context,
                target,
                attackType);
        }
    }

    private void ApplyHit(
        ActiveSkillExecutionContext context,
        Beast target,
        string attackType)
    {
        DamageRequest damageRequest =
            CreateDamageRequest(
                context,
                target,
                attackType);

        BeastHitRequest hitRequest =
            CreateHitRequest(
                target,
                attackType,
                damageRequest);

        BeastDamageResult result =
            DamageResolver.Resolve(
                hitRequest);

        WriteDamage(
            target,
            attackType,
            result);

        WriteBreakingPoint(
            target,
            result);
    }

    private BeastHitRequest CreateHitRequest(
        Beast target,
        string attackType,
        DamageRequest damageRequest)
    {
        return new BeastHitRequest
        {
            DamageRequest =
                damageRequest,

            Target =
                target,

            AttackType =
                attackType,

            MinimumRemainingHitPoints =
                GetMinimumRemainingHitPoints()
        };
    }

    private DamageRequest CreateDamageRequest(
        ActiveSkillExecutionContext context,
        Beast target,
        string attackType)
    {
        return new DamageRequest
        {
            OffensiveStat =
                GetOffensiveStat(
                    context.User,
                    attackType),

            DefensiveStat =
                GetDefensiveStat(
                    target,
                    attackType),

            Modifier =
                context.Skill.Modifier,

            FinalMultiplier =
                GetFinalDamageMultiplier(
                    context.User)
        };
    }

    private int GetOffensiveStat(
        Traveler traveler,
        string attackType)
    {
        if (DamageTypes.IsPhysical(
            attackType))
        {
            return traveler.PhysicalAttack;
        }

        return traveler.ElementalAttack;
    }

    private int GetDefensiveStat(
        Beast target,
        string attackType)
    {
        if (DamageTypes.IsPhysical(
            attackType))
        {
            return target.PhysicalDefense;
        }

        return target.ElementalDefense;
    }

    private void WriteSkillUse(
        ActiveSkillExecutionContext context)
    {
        view.WriteLine(
            CombatText.SeparatorLine);

        view.WriteLine(
            string.Format(
                CombatText.SkillUseFormat,
                context.User.Name,
                context.Skill.Name));
    }

    private void WriteDamage(
        Beast target,
        string attackType,
        BeastDamageResult result)
    {
        string weaknessSuffix =
            GetWeaknessSuffix(
                result);

        view.WriteLine(
            string.Format(
                CombatText.TypedDamageFormat,
                target.Name,
                result.Damage,
                attackType,
                weaknessSuffix));
    }

    private string GetWeaknessSuffix(
        BeastDamageResult result)
    {
        if (HasWeakness(
            result))
        {
            return CombatText.WeaknessSuffix;
        }

        return string.Empty;
    }

    private bool HasWeakness(
        BeastDamageResult result)
    {
        return result.IsWeakness;
    }

    private void WriteBreakingPoint(
        Beast target,
        BeastDamageResult result)
    {
        if (DidNotEnterBreakingPoint(
            result))
        {
            return;
        }

        view.WriteLine(
            string.Format(
                CombatText.BreakingPointFormat,
                target.Name));
    }

    private bool DidNotEnterBreakingPoint(
        BeastDamageResult result)
    {
        return !result
            .EnteredBreakingPoint;
    }

    private void WriteRemainingHitPoints(
        IReadOnlyList<Beast> targets)
    {
        foreach (Beast target in targets)
        {
            view.WriteLine(
                string.Format(
                    CombatText.RemainingHitPointsFormat,
                    target.Name,
                    target.CurrentHP));
        }
    }
}

public class StandardOffensiveSkillStrategy
    : OffensiveSkillExecutionStrategy
{
    public StandardOffensiveSkillStrategy(
        View view)
        : base(view)
    {
    }
}

public class LastStandSkillStrategy
    : OffensiveSkillExecutionStrategy
{
    private const int PercentageScale =
        100;

    private const double
        DamageIncreasePerMissingPercent =
            0.03;

    private const double
        StandardDamageMultiplier =
            1.0;

    public LastStandSkillStrategy(
        View view)
        : base(view)
    {
    }

    protected override double
        GetFinalDamageMultiplier(
            Traveler traveler)
    {
        int missingHitPointPercentage =
            GetMissingHitPointPercentage(
                traveler);

        double additionalMultiplier =
            missingHitPointPercentage *
            DamageIncreasePerMissingPercent;

        return StandardDamageMultiplier +
               additionalMultiplier;
    }

    private int GetMissingHitPointPercentage(
        Traveler traveler)
    {
        int missingHitPoints =
            traveler.MaxHP -
            traveler.CurrentHP;

        double missingPercentage =
            (double)missingHitPoints *
            PercentageScale /
            traveler.MaxHP;

        return Convert.ToInt32(
            Math.Floor(
                missingPercentage));
    }
}

public class MercyStrikeSkillStrategy
    : OffensiveSkillExecutionStrategy
{
    private const int MinimumRemainingHitPoints =
        1;

    public MercyStrikeSkillStrategy(
        View view)
        : base(view)
    {
    }

    protected override int
        GetMinimumRemainingHitPoints()
    {
        return MinimumRemainingHitPoints;
    }
}

public class ShootingStarsSkillStrategy
    : OffensiveSkillExecutionStrategy
{
    private static readonly string[] AttackTypes =
    [
        DamageTypes.Wind,
        DamageTypes.Light,
        DamageTypes.Dark
    ];

    public ShootingStarsSkillStrategy(
        View view)
        : base(view)
    {
    }

    protected override IReadOnlyList<string>?
        SelectAttackTypes(
            ActiveSkillExecutionContext context)
    {
        return AttackTypes;
    }
}

public class NightmareChimeraSkillStrategy
    : OffensiveSkillExecutionStrategy
{
    private static readonly IReadOnlyList<string>
        AllWeapons =
            DamageTypes.Physical;

    public NightmareChimeraSkillStrategy(
        View view)
        : base(view)
    {
    }

    protected override IReadOnlyList<string>?
        SelectAttackTypes(
            ActiveSkillExecutionContext context)
    {
        string? selectedWeapon =
            WeaponSelectionMenu.SelectWeapon(
                AllWeapons);

        if (WasWeaponSelectionCancelled(
            selectedWeapon))
        {
            return null;
        }

        return [selectedWeapon!];
    }

    private bool WasWeaponSelectionCancelled(
        string? selectedWeapon)
    {
        return selectedWeapon is null;
    }
}