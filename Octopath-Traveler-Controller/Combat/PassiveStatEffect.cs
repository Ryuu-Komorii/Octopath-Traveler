namespace Octopath_Traveler;

public abstract class PassiveStatEffect
{
    public abstract void Apply(TravelerStatValues stats);
}

public class ElementalAugmentationEffect : PassiveStatEffect
{
    private const int ElementalAttackIncrease = 50;

    public override void Apply(TravelerStatValues stats)
    {
        stats.IncreaseElementalAttack(
            ElementalAttackIncrease);
    }
}

public class SummonStrengthEffect : PassiveStatEffect
{
    private const int PhysicalAttackIncrease = 50;

    public override void Apply(TravelerStatValues stats)
    {
        stats.IncreasePhysicalAttack(
            PhysicalAttackIncrease);
    }
}

public class HaleAndHeartyEffect : PassiveStatEffect
{
    private const int HitPointIncrease = 500;

    public override void Apply(TravelerStatValues stats)
    {
        stats.IncreaseMaxHitPoints(
            HitPointIncrease);
    }
}

public class FleefootEffect : PassiveStatEffect
{
    private const int SpeedIncrease = 50;

    public override void Apply(TravelerStatValues stats)
    {
        stats.IncreaseSpeed(
            SpeedIncrease);
    }
}

public class InnerStrengthEffect : PassiveStatEffect
{
    private const int SkillPointIncrease = 50;

    public override void Apply(TravelerStatValues stats)
    {
        stats.IncreaseMaxSkillPoints(
            SkillPointIncrease);
    }
}

public class NoPassiveStatEffect : PassiveStatEffect
{
    public override void Apply(TravelerStatValues stats)
    {
    }
}