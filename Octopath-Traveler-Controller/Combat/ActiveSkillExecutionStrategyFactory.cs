using Octopath_Traveler_View;

namespace Octopath_Traveler;

public class ActiveSkillExecutionStrategyFactory
{
    private const string HealWoundsName =
        "Heal Wounds";

    private const string HealMoreName =
        "Heal More";

    private const string FirstAidName =
        "First Aid";

    private const string ReviveName =
        "Revive";

    private const string VivifyName =
        "Vivify";

    private const string SpearheadName =
        "Spearhead";

    private const string LegholdTrapName =
        "Leghold Trap";

    private readonly View view;

    public ActiveSkillExecutionStrategyFactory(
        View view)
    {
        this.view = view;
    }

    public ActiveSkillExecutionStrategy Create(
        ActiveSkillCatalogEntry skill)
    {
        return skill.Name switch
        {
            HealWoundsName or
            HealMoreName or
            FirstAidName =>
                new HealingSkillExecutionStrategy(
                    view),

            ReviveName =>
                new ReviveSkillExecutionStrategy(
                    view),

            VivifyName =>
                new VivifySkillExecutionStrategy(
                    view),

            SpearheadName =>
                new SpearheadSkillExecutionStrategy(
                    view),

            LegholdTrapName =>
                new LegholdTrapSkillExecutionStrategy(
                    view),

            _ =>
                new OffensiveActiveSkillExecutionStrategy(
                    view)
        };
    }
}