using Octopath_Traveler_View;

namespace Octopath_Traveler;

public class TeamFileSelector
{
    private const string TeamFileSearchPattern = "*.txt";
    private const string TeamSelectionPrompt = "Elige un archivo para cargar los equipos";
    private const int FirstTeamOption = 0;

    private readonly View view;
    private readonly string teamsFolder;

    public TeamFileSelector(View view, string teamsFolder)
    {
        this.view = view;
        this.teamsFolder = teamsFolder;
    }

    public string SelectTeamFile()
    {
        string[] teamFiles = GetTeamFilesInOrder();
        ShowTeamFiles(teamFiles);
        int selectedTeamIndex = ReadSelectedTeamIndex();

        return teamFiles[selectedTeamIndex];
    }

    private string[] GetTeamFilesInOrder()
    {
        return Directory
            .GetFiles(teamsFolder, TeamFileSearchPattern, SearchOption.TopDirectoryOnly)
            .OrderBy(Path.GetFileName)
            .ToArray();
    }

    private void ShowTeamFiles(string[] teamFiles)
    {
        view.WriteLine(TeamSelectionPrompt);

        int optionNumber = FirstTeamOption;

        foreach (string teamFile in teamFiles)
        {
            ShowTeamFileOption(optionNumber, teamFile);
            optionNumber++;
        }
    }

    private void ShowTeamFileOption(int optionNumber, string teamFile)
    {
        string fileName = Path.GetFileName(teamFile);
        view.WriteLine($"{optionNumber}: {fileName}");
    }

    private int ReadSelectedTeamIndex()
    {
        return int.Parse(view.ReadLine());
    }
}