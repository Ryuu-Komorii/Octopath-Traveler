namespace Octopath_Traveler;

public class CombatCatalogLoader
{
    private readonly TravelerCatalogReader travelerCatalogReader = new();
    private readonly BeastCatalogReader beastCatalogReader = new();

    public IReadOnlyList<TravelerCatalogEntry> ReadTravelers()
    {
        return travelerCatalogReader.Read(GameDataPaths.Travelers);
    }

    public IReadOnlyList<BeastCatalogEntry> ReadBeasts()
    {
        return beastCatalogReader.Read(GameDataPaths.Beasts);
    }
}