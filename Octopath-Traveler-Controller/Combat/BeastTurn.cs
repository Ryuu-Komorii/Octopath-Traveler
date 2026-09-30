using Octopath_Traveler_View;

namespace Octopath_Traveler;

public class BeastTurn
{
    private readonly BeastSkillCatalog
        beastSkillCatalog;

    private readonly BeastSkillExecutionStrategyFactory
        strategyFactory;

    public BeastTurn(
        View view,
        BeastSkillCatalog beastSkillCatalog)
    {
        this.beastSkillCatalog =
            beastSkillCatalog;

        strategyFactory =
            new BeastSkillExecutionStrategyFactory(
                view);
    }

    public void Execute(
        Beast beast,
        IReadOnlyList<Traveler> travelers)
    {
        BeastSkillCatalogEntry skill =
            beastSkillCatalog.Find(
                beast.SkillName);

        BeastSkillExecutionStrategy strategy =
            strategyFactory.Create(
                skill.Name);

        strategy.Execute(
            beast,
            travelers,
            skill);
    }
}