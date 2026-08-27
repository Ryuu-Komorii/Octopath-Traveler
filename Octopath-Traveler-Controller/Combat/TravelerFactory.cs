namespace Octopath_Traveler;

public class TravelerFactory
{
    private readonly IReadOnlyList<TravelerCatalogEntry> travelerCatalog;

    public TravelerFactory(IReadOnlyList<TravelerCatalogEntry> travelerCatalog)
    {
        this.travelerCatalog = travelerCatalog;
    }

    public Traveler Create(TravelerTeamMember teamMember)
    {
        TravelerCatalogEntry catalogEntry = FindCatalogEntry(teamMember.Name);

        return new Traveler(catalogEntry, teamMember);
    }

    private TravelerCatalogEntry FindCatalogEntry(string travelerName)
    {
        return travelerCatalog.First(
            catalogEntry => HasName(catalogEntry, travelerName));
    }

    private bool HasName(
        TravelerCatalogEntry catalogEntry,
        string travelerName)
    {
        return catalogEntry.Name == travelerName;
    }
}