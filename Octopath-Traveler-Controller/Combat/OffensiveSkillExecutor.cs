using Octopath_Traveler_View;

namespace Octopath_Traveler;

public class OffensiveSkillExecutor
{
    private readonly OffensiveSkillExecutionStrategyFactory
        strategyFactory;

    public OffensiveSkillExecutor(
        View view)
    {
        strategyFactory =
            new OffensiveSkillExecutionStrategyFactory(
                view);
    }

    public ActionExecutionResult Execute(
        Traveler traveler,
        IReadOnlyList<Beast> beasts,
        ActiveSkillCatalogEntry skill)
    {
        OffensiveSkillExecutionStrategy strategy =
            strategyFactory.Create(
                skill);

        return strategy.Execute(
            traveler,
            beasts,
            skill);
    }
}