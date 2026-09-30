namespace Octopath_Traveler;

public class ActiveSkillCatalogEntry
{
    public string Name { get; set; } = string.Empty;
    public int SP { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Target { get; set; } = string.Empty;
    public double Modifier { get; set; }
    public string Boost { get; set; } = string.Empty;
}