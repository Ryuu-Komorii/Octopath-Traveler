namespace Octopath_Traveler;

public abstract class TravelerTargetSelector
{
    public abstract IReadOnlyList<Traveler> Select(
        IReadOnlyList<Traveler> travelers);

    protected IEnumerable<Traveler> GetLivingTravelers(
        IReadOnlyList<Traveler> travelers)
    {
        return travelers.Where(
            IsAlive);
    }

    protected IReadOnlyList<Traveler> CreateSingleTarget(
        Traveler traveler)
    {
        return [traveler];
    }

    private bool IsAlive(
        Traveler traveler)
    {
        return traveler.IsAlive();
    }
}

public class HighestHitPointsTargetSelector
    : TravelerTargetSelector
{
    public override IReadOnlyList<Traveler> Select(
        IReadOnlyList<Traveler> travelers)
    {
        Traveler target =
            GetLivingTravelers(travelers)
                .OrderByDescending(
                    GetCurrentHitPoints)
                .First();

        return CreateSingleTarget(
            target);
    }

    private int GetCurrentHitPoints(
        Traveler traveler)
    {
        return traveler.CurrentHP;
    }
}

public class HighestElementalAttackTargetSelector
    : TravelerTargetSelector
{
    public override IReadOnlyList<Traveler> Select(
        IReadOnlyList<Traveler> travelers)
    {
        Traveler target =
            GetLivingTravelers(travelers)
                .OrderByDescending(
                    GetElementalAttack)
                .First();

        return CreateSingleTarget(
            target);
    }

    private int GetElementalAttack(
        Traveler traveler)
    {
        return traveler.ElementalAttack;
    }
}

public class LowestPhysicalDefenseTargetSelector
    : TravelerTargetSelector
{
    public override IReadOnlyList<Traveler> Select(
        IReadOnlyList<Traveler> travelers)
    {
        Traveler target =
            GetLivingTravelers(travelers)
                .OrderBy(
                    GetPhysicalDefense)
                .First();

        return CreateSingleTarget(
            target);
    }

    private int GetPhysicalDefense(
        Traveler traveler)
    {
        return traveler.PhysicalDefense;
    }
}

public class HighestSpeedTargetSelector
    : TravelerTargetSelector
{
    public override IReadOnlyList<Traveler> Select(
        IReadOnlyList<Traveler> travelers)
    {
        Traveler target =
            GetLivingTravelers(travelers)
                .OrderByDescending(
                    GetSpeed)
                .First();

        return CreateSingleTarget(
            target);
    }

    private int GetSpeed(
        Traveler traveler)
    {
        return traveler.Speed;
    }
}

public class LowestElementalDefenseTargetSelector
    : TravelerTargetSelector
{
    public override IReadOnlyList<Traveler> Select(
        IReadOnlyList<Traveler> travelers)
    {
        Traveler target =
            GetLivingTravelers(travelers)
                .OrderBy(
                    GetElementalDefense)
                .First();

        return CreateSingleTarget(
            target);
    }

    private int GetElementalDefense(
        Traveler traveler)
    {
        return traveler.ElementalDefense;
    }
}

public class AllLivingTravelersTargetSelector
    : TravelerTargetSelector
{
    public override IReadOnlyList<Traveler> Select(
        IReadOnlyList<Traveler> travelers)
    {
        return GetLivingTravelers(
                travelers)
            .ToArray();
    }
}