using Octopath_Traveler_View;

namespace Octopath_Traveler;

public class BeastSkillExecutionStrategyFactory
{
    private readonly BeastSkillResultWriter
        resultWriter;

    public BeastSkillExecutionStrategyFactory(
        View view)
    {
        resultWriter =
            new BeastSkillResultWriter(
                view);
    }

    public BeastSkillExecutionStrategy Create(
        string skillName)
    {
        return skillName switch
        {
            BeastSkillNames.Attack =>
                CreatePhysical(
                    new HighestHitPointsTargetSelector()),

            BeastSkillNames.BefuddlingClaw =>
                CreatePhysical(
                    new HighestElementalAttackTargetSelector()),

            BeastSkillNames.Stampede or
            BeastSkillNames.Rampage =>
                CreatePhysical(
                    new AllLivingTravelersTargetSelector()),

            BeastSkillNames.IceBlast or
            BeastSkillNames.Incinerate or
            BeastSkillNames.BlackGale or
            BeastSkillNames.Galestorm =>
                CreateElemental(
                    new AllLivingTravelersTargetSelector()),

            BeastSkillNames.Stab or
            BeastSkillNames.BoarRush or
            BeastSkillNames.VorpalFang =>
                CreatePhysical(
                    new LowestPhysicalDefenseTargetSelector()),

            BeastSkillNames.MeteorStorm or
            BeastSkillNames.Freeze or
            BeastSkillNames.Luminescence or
            BeastSkillNames.Enshadow or
            BeastSkillNames.WindSlash =>
                CreateElemental(
                    new HighestSpeedTargetSelector()),

            BeastSkillNames.Windshot or
            BeastSkillNames.Firesand or
            BeastSkillNames.Thundershot or
            BeastSkillNames.Lightshot or
            BeastSkillNames.Iceshot or
            BeastSkillNames.Shadowshot =>
                CreateElemental(
                    new LowestElementalDefenseTargetSelector()),

            BeastSkillNames.VortalClaw =>
                new VortalClawExecutionStrategy(
                    resultWriter),

            _ =>
                throw CreateUnsupportedSkillException(
                    skillName)
        };
    }

    private BeastSkillExecutionStrategy CreatePhysical(
        TravelerTargetSelector targetSelector)
    {
        return new DamageBeastSkillExecutionStrategy(
            resultWriter,
            BeastAttackKind.Physical,
            targetSelector);
    }

    private BeastSkillExecutionStrategy CreateElemental(
        TravelerTargetSelector targetSelector)
    {
        return new DamageBeastSkillExecutionStrategy(
            resultWriter,
            BeastAttackKind.Elemental,
            targetSelector);
    }

    private ArgumentException
        CreateUnsupportedSkillException(
            string skillName)
    {
        return new ArgumentException(
            $"Habilidad de bestia no soportada: {skillName}");
    }
}