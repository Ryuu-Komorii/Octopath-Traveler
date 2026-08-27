namespace Octopath_Traveler;

public class CombatRosterFactory
{
    private readonly TravelerFactory travelerFactory;
    private readonly BeastFactory beastFactory;

    public CombatRosterFactory(
        TravelerFactory travelerFactory,
        BeastFactory beastFactory)
    {
        this.travelerFactory = travelerFactory;
        this.beastFactory = beastFactory;
    }

    public CombatRoster Create(TeamDefinition teamDefinition)
    {
        Traveler[] travelers = CreateTravelers(teamDefinition);
        Beast[] beasts = CreateBeasts(teamDefinition);

        return new CombatRoster(travelers, beasts);
    }

    private Traveler[] CreateTravelers(TeamDefinition teamDefinition)
    {
        return teamDefinition.Travelers
            .Select(travelerFactory.Create)
            .ToArray();
    }

    private Beast[] CreateBeasts(TeamDefinition teamDefinition)
    {
        return teamDefinition.BeastNames
            .Select(beastFactory.Create)
            .ToArray();
    }
}