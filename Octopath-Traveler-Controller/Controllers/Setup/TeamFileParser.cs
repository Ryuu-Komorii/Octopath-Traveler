namespace Octopath_Traveler;

public class TeamFileParser
{
    private const string PlayerTeamHeader = "Player Team";
    private const string EnemyTeamHeader = "Enemy Team";
    private const int HeaderLineCount = 1;

    private readonly TravelerTeamMemberParser travelerParser = new();

    public TeamDefinition Parse(string teamFilePath)
    {
        string[] teamFileLines = File.ReadAllLines(teamFilePath);
        TravelerTeamMember[] travelers = ParseTravelers(teamFileLines);
        string[] beastNames = ParseBeastNames(teamFileLines);

        return new TeamDefinition(travelers, beastNames);
    }

    private TravelerTeamMember[] ParseTravelers(string[] teamFileLines)
    {
        return teamFileLines
            .SkipWhile(IsNotPlayerTeamHeader)
            .Skip(HeaderLineCount)
            .TakeWhile(IsNotEnemyTeamHeader)
            .Where(HasContent)
            .Select(travelerParser.Parse)
            .ToArray();
    }

    private string[] ParseBeastNames(string[] teamFileLines)
    {
        return teamFileLines
            .SkipWhile(IsNotEnemyTeamHeader)
            .Skip(HeaderLineCount)
            .Where(HasContent)
            .ToArray();
    }

    private bool IsNotPlayerTeamHeader(string line)
    {
        return line != PlayerTeamHeader;
    }

    private bool IsNotEnemyTeamHeader(string line)
    {
        return line != EnemyTeamHeader;
    }

    private bool HasContent(string line)
    {
        return !string.IsNullOrWhiteSpace(line);
    }
}