using Octopath_Traveler_View;

namespace Octopath_Traveler;

public class BeastSkillExecutionStrategyFactory
{
    private readonly View view;

    public BeastSkillExecutionStrategyFactory(
        View view)
    {
        this.view = view;
    }

    public BeastSkillExecutionStrategy Create(
        string skillName)
    {
        return skillName switch
        {
            "Attack" =>
                CreatePhysical(
                    new HighestHitPointsTargetSelector()),

            "Befuddling claw" =>
                CreatePhysical(
                    new HighestElementalAttackTargetSelector()),

            "Stampede" or
            "Rampage" =>
                CreatePhysical(
                    new AllLivingTravelersTargetSelector()),

            "Ice blast" or
            "Incinerate" or
            "Black Gale" or
            "Galestorm" =>
                CreateElemental(
                    new AllLivingTravelersTargetSelector()),

            "Stab" or
            "Boar Rush" or
            "Vorpal Fang" =>
                CreatePhysical(
                    new LowestPhysicalDefenseTargetSelector()),

            "Meteor Storm" or
            "Freeze" or
            "Luminescence" or
            "Enshadow" or
            "Wind slash" =>
                CreateElemental(
                    new HighestSpeedTargetSelector()),

            "Windshot" or
            "Firesand" or
            "Thundershot" or
            "Lightshot" or
            "Iceshot" or
            "Shadowshot" =>
                CreateElemental(
                    new LowestElementalDefenseTargetSelector()),

            "Vortal Claw" =>
                new VortalClawExecutionStrategy(
                    view),

            _ =>
                throw new ArgumentException(
                    $"Habilidad de bestia no soportada: {skillName}")
        };
    }

    private BeastSkillExecutionStrategy CreatePhysical(
        TravelerTargetSelector targetSelector)
    {
        return new DamageBeastSkillExecutionStrategy(
            view,
            BeastAttackKind.Physical,
            targetSelector);
    }

    private BeastSkillExecutionStrategy CreateElemental(
        TravelerTargetSelector targetSelector)
    {
        return new DamageBeastSkillExecutionStrategy(
            view,
            BeastAttackKind.Elemental,
            targetSelector);
    }
}