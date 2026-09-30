using Octopath_Traveler_View;

namespace Octopath_Traveler;

public abstract class OffensiveSkillExecutionStrategy
{
    private const string SeparatorLine =
        "----------------------------------------";

    private const string SingleTarget =
        "Single";

    private const string SkillUseMessageFormat =
        "{0} usa {1}";

    private const string DamageMessageFormat =
        "{0} recibe {1} de daño de tipo {2}{3}";

    private const string WeaknessSuffix =
        " con debilidad";

    private const string BreakingPointMessageFormat =
        "{0} entra en Breaking Point";

    private const string RemainingHitPointsFormat =
        "{0} termina con HP:{1}";

    private const int NoMinimumRemainingHitPoints =
        0;

    private readonly View view;

    private readonly BeastTargetSelectionMenu
        targetSelectionMenu;

    private readonly BoostPointSelectionMenu
        boostPointSelectionMenu;

    protected readonly BeastDamageResolver
        DamageResolver;

    protected readonly WeaponSelectionMenu
        WeaponSelectionMenu;

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
        Traveler traveler,
        IReadOnlyList<Beast> beasts,
        ActiveSkillCatalogEntry skill)
    {
        IReadOnlyList<string>? attackTypes =
            SelectAttackTypes(
                traveler,
                skill);

        if (attackTypes is null)
        {
            return ActionExecutionResult.Cancelled;
        }

        Beast[]? targets =
            SelectTargets(
                traveler,
                beasts,
                skill);

        if (targets is null)
        {
            return ActionExecutionResult.Cancelled;
        }

        boostPointSelectionMenu
            .ReadBoostPoints();

        traveler.SpendSkillPoints(
            skill.SP);

        ExecuteSkill(
            traveler,
            targets,
            attackTypes,
            skill);

        return ActionExecutionResult.Completed;
    }

    protected virtual IReadOnlyList<string>?
        SelectAttackTypes(
            Traveler traveler,
            ActiveSkillCatalogEntry skill)
    {
        return [skill.Type];
    }

    protected virtual double
        GetFinalDamageMultiplier(
            Traveler traveler)
    {
        return 1.0;
    }

    protected virtual int
        GetMinimumRemainingHitPoints()
    {
        return NoMinimumRemainingHitPoints;
    }

    private Beast[]? SelectTargets(
        Traveler traveler,
        IReadOnlyList<Beast> beasts,
        ActiveSkillCatalogEntry skill)
    {
        if (HasSingleTarget(
            skill))
        {
            return SelectSingleTarget(
                traveler,
                beasts);
        }

        return GetLivingBeasts(
            beasts);
    }

    private Beast[]? SelectSingleTarget(
        Traveler traveler,
        IReadOnlyList<Beast> beasts)
    {
        Beast? selectedTarget =
            targetSelectionMenu.SelectTarget(
                traveler,
                beasts);

        if (selectedTarget is null)
        {
            return null;
        }

        return [selectedTarget];
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
        Traveler traveler,
        IReadOnlyList<Beast> targets,
        IReadOnlyList<string> attackTypes,
        ActiveSkillCatalogEntry skill)
    {
        WriteSkillUse(
            traveler,
            skill);

        foreach (Beast target in targets)
        {
            ApplyHits(
                traveler,
                target,
                attackTypes,
                skill);
        }

        WriteRemainingHitPoints(
            targets);
    }

    private void ApplyHits(
        Traveler traveler,
        Beast target,
        IReadOnlyList<string> attackTypes,
        ActiveSkillCatalogEntry skill)
    {
        foreach (string attackType
                 in attackTypes)
        {
            ApplyHit(
                traveler,
                target,
                attackType,
                skill);
        }
    }

    private void ApplyHit(
        Traveler traveler,
        Beast target,
        string attackType,
        ActiveSkillCatalogEntry skill)
    {
        DamageRequest damageRequest =
            CreateDamageRequest(
                traveler,
                target,
                attackType,
                skill);

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
        Traveler traveler,
        Beast target,
        string attackType,
        ActiveSkillCatalogEntry skill)
    {
        if (IsPhysicalType(
            attackType))
        {
            return CreatePhysicalDamageRequest(
                traveler,
                target,
                skill);
        }

        return CreateElementalDamageRequest(
            traveler,
            target,
            skill);
    }

    private DamageRequest CreatePhysicalDamageRequest(
        Traveler traveler,
        Beast target,
        ActiveSkillCatalogEntry skill)
    {
        return new DamageRequest
        {
            OffensiveStat =
                traveler.PhysicalAttack,

            DefensiveStat =
                target.PhysicalDefense,

            Modifier =
                skill.Modifier,

            FinalMultiplier =
                GetFinalDamageMultiplier(
                    traveler)
        };
    }

    private DamageRequest CreateElementalDamageRequest(
        Traveler traveler,
        Beast target,
        ActiveSkillCatalogEntry skill)
    {
        return new DamageRequest
        {
            OffensiveStat =
                traveler.ElementalAttack,

            DefensiveStat =
                target.ElementalDefense,

            Modifier =
                skill.Modifier,

            FinalMultiplier =
                GetFinalDamageMultiplier(
                    traveler)
        };
    }

    private bool IsPhysicalType(
        string attackType)
    {
        return attackType is
            "Sword" or
            "Spear" or
            "Dagger" or
            "Axe" or
            "Bow" or
            "Stave";
    }

    private void WriteSkillUse(
        Traveler traveler,
        ActiveSkillCatalogEntry skill)
    {
        view.WriteLine(
            SeparatorLine);

        view.WriteLine(
            string.Format(
                SkillUseMessageFormat,
                traveler.Name,
                skill.Name));
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
                DamageMessageFormat,
                target.Name,
                result.Damage,
                attackType,
                weaknessSuffix));
    }

    private string GetWeaknessSuffix(
        BeastDamageResult result)
    {
        if (result.IsWeakness)
        {
            return WeaknessSuffix;
        }

        return string.Empty;
    }

    private void WriteBreakingPoint(
        Beast target,
        BeastDamageResult result)
    {
        if (!result.EnteredBreakingPoint)
        {
            return;
        }

        view.WriteLine(
            string.Format(
                BreakingPointMessageFormat,
                target.Name));
    }

    private void WriteRemainingHitPoints(
        IReadOnlyList<Beast> targets)
    {
        foreach (Beast target in targets)
        {
            view.WriteLine(
                string.Format(
                    RemainingHitPointsFormat,
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
    private const int
        MinimumRemainingHitPoints =
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
    private static readonly string[]
        AttackTypes =
        [
            "Wind",
            "Light",
            "Dark"
        ];

    public ShootingStarsSkillStrategy(
        View view)
        : base(view)
    {
    }

    protected override IReadOnlyList<string>?
        SelectAttackTypes(
            Traveler traveler,
            ActiveSkillCatalogEntry skill)
    {
        return AttackTypes;
    }
}

public class NightmareChimeraSkillStrategy
    : OffensiveSkillExecutionStrategy
{
    private static readonly string[]
        AllWeapons =
        [
            "Sword",
            "Spear",
            "Dagger",
            "Axe",
            "Bow",
            "Stave"
        ];

    public NightmareChimeraSkillStrategy(
        View view)
        : base(view)
    {
    }

    protected override IReadOnlyList<string>?
        SelectAttackTypes(
            Traveler traveler,
            ActiveSkillCatalogEntry skill)
    {
        string? selectedWeapon =
            WeaponSelectionMenu.SelectWeapon(
                AllWeapons);

        if (selectedWeapon is null)
        {
            return null;
        }

        return [selectedWeapon];
    }
}