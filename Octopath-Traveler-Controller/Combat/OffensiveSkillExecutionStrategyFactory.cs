using Octopath_Traveler_View;

namespace Octopath_Traveler;

public class OffensiveSkillExecutionStrategyFactory
{
    private const string LastStandName =
        "Last Stand";

    private const string MercyStrikeName =
        "Mercy Strike";

    private const string ShootingStarsName =
        "Shooting Stars";

    private const string NightmareChimeraName =
        "Nightmare Chimera";

    private readonly View view;

    public OffensiveSkillExecutionStrategyFactory(
        View view)
    {
        this.view = view;
    }

    public OffensiveSkillExecutionStrategy Create(
        ActiveSkillCatalogEntry skill)
    {
        return skill.Name switch
        {
            LastStandName =>
                new LastStandSkillStrategy(view),

            MercyStrikeName =>
                new MercyStrikeSkillStrategy(view),

            ShootingStarsName =>
                new ShootingStarsSkillStrategy(view),

            NightmareChimeraName =>
                new NightmareChimeraSkillStrategy(view),

            _ =>
                new StandardOffensiveSkillStrategy(view)
        };
    }
}