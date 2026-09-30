namespace Octopath_Traveler;

public class BeastFactory
{
    private readonly IReadOnlyList<BeastCatalogEntry> beastCatalog;

    public BeastFactory(IReadOnlyList<BeastCatalogEntry> beastCatalog)
    {
        this.beastCatalog = beastCatalog;
    }

    public Beast Create(string beastName)
    {
        BeastCatalogEntry catalogEntry = FindCatalogEntry(beastName);

        return new Beast(catalogEntry);
    }

    private BeastCatalogEntry FindCatalogEntry(string beastName)
    {
        return beastCatalog.First(
            catalogEntry => HasName(catalogEntry, beastName));
    }

    private bool HasName(
        BeastCatalogEntry catalogEntry,
        string beastName)
    {
        return catalogEntry.Name == beastName;
    }
}