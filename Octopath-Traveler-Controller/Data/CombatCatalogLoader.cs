namespace Octopath_Traveler;

public class CombatCatalogLoader
{
    private readonly TravelerCatalogReader travelerCatalogReader = new();
    private readonly BeastCatalogReader beastCatalogReader = new();
    private readonly ActiveSkillCatalogReader activeSkillCatalogReader = new();
    private readonly PassiveSkillCatalogReader passiveSkillCatalogReader = new();
    private readonly BeastSkillCatalogReader beastSkillCatalogReader = new();

    public IReadOnlyList<TravelerCatalogEntry> ReadTravelers()
    {
        return travelerCatalogReader.Read(GameDataPaths.Travelers);
    }

    public IReadOnlyList<BeastCatalogEntry> ReadBeasts()
    {
        return beastCatalogReader.Read(GameDataPaths.Beasts);
    }

    public IReadOnlyList<ActiveSkillCatalogEntry> ReadActiveSkills()
    {
        return activeSkillCatalogReader.Read(GameDataPaths.ActiveSkills);
    }

    public IReadOnlyList<PassiveSkillCatalogEntry> ReadPassiveSkills()
    {
        return passiveSkillCatalogReader.Read(GameDataPaths.PassiveSkills);
    }

    public IReadOnlyList<BeastSkillCatalogEntry> ReadBeastSkills()
    {
        return beastSkillCatalogReader.Read(GameDataPaths.BeastSkills);
    }
}