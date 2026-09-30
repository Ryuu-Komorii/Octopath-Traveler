using Octopath_Traveler_View;

namespace Octopath_Traveler;

public class OffensiveSkillExecutor
{
    private readonly OffensiveSkillExecutionStrategyFactory
        strategyFactory;

    public OffensiveSkillExecutor(View view)
    {
        strategyFactory =
            new OffensiveSkillExecutionStrategyFactory(view);
    }

    public ActionExecutionResult Execute(
        ActiveSkillExecutionContext context)
    {
        OffensiveSkillExecutionStrategy strategy =
            strategyFactory.Create(context.Skill);

        return strategy.Execute(context);
    }
}