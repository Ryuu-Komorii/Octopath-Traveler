namespace Octopath_Traveler;

public class CombatCatalogLoader
{
    private readonly JsonCatalogReader<TravelerCatalogEntry>
        travelerCatalogReader = new();

    private readonly JsonCatalogReader<BeastCatalogEntry>
        beastCatalogReader = new();

    private readonly JsonCatalogReader<ActiveSkillCatalogEntry>
        activeSkillCatalogReader = new();

    private readonly JsonCatalogReader<PassiveSkillCatalogEntry>
        passiveSkillCatalogReader = new();

    private readonly JsonCatalogReader<BeastSkillCatalogEntry>
        beastSkillCatalogReader = new();

    public IReadOnlyList<TravelerCatalogEntry> ReadTravelers()
    {
        return travelerCatalogReader.Read(
            GameDataPaths.Travelers);
    }

    public IReadOnlyList<BeastCatalogEntry> ReadBeasts()
    {
        return beastCatalogReader.Read(
            GameDataPaths.Beasts);
    }

    public IReadOnlyList<ActiveSkillCatalogEntry> ReadActiveSkills()
    {
        return activeSkillCatalogReader.Read(
            GameDataPaths.ActiveSkills);
    }

    public IReadOnlyList<PassiveSkillCatalogEntry> ReadPassiveSkills()
    {
        return passiveSkillCatalogReader.Read(
            GameDataPaths.PassiveSkills);
    }

    public IReadOnlyList<BeastSkillCatalogEntry> ReadBeastSkills()
    {
        return beastSkillCatalogReader.Read(
            GameDataPaths.BeastSkills);
    }
}