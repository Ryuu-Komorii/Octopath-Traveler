using System.Text.Json;

namespace Octopath_Traveler;

public class JsonCatalogReader<TCatalogEntry>
{
    public IReadOnlyList<TCatalogEntry> Read(string catalogFilePath)
    {
        string catalogJson = File.ReadAllText(catalogFilePath);

        return JsonSerializer.Deserialize<TCatalogEntry[]>(catalogJson)
               ?? [];
    }
}