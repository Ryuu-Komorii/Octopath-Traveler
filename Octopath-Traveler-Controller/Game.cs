using Octopath_Traveler_View;

namespace Octopath_Traveler;

public class Game
{
    private const string InvalidTeamMessage =
        "Archivo de equipos no válido";

    private readonly View view;
    private readonly TeamFileSelector teamFileSelector;
    private readonly TeamFileParser teamFileParser;
    private readonly TeamStructureValidator teamStructureValidator;
    private readonly CombatCatalogLoader combatCatalogLoader;
    private readonly Battle battle;

    public Game(View view, string teamsFolder)
    {
        this.view = view;

        teamFileSelector =
            new TeamFileSelector(view, teamsFolder);

        teamFileParser =
            new TeamFileParser();

        teamStructureValidator =
            new TeamStructureValidator();

        combatCatalogLoader =
            new CombatCatalogLoader();

        battle =
            new Battle(view);
    }

    public void Play()
    {
        TeamDefinition teamDefinition =
            ReadSelectedTeam();

        if (IsInvalidTeam(teamDefinition))
        {
            ShowInvalidTeamMessage();
            return;
        }

        CombatRoster combatRoster =
            CreateCombatRoster(teamDefinition);

        battle.Play(combatRoster);
    }

    private TeamDefinition ReadSelectedTeam()
    {
        string selectedTeamFile =
            teamFileSelector.SelectTeamFile();

        return teamFileParser.Parse(
            selectedTeamFile);
    }

    private bool IsInvalidTeam(
        TeamDefinition teamDefinition)
    {
        return !teamStructureValidator.IsValid(
            teamDefinition);
    }

    private void ShowInvalidTeamMessage()
    {
        view.WriteLine(
            InvalidTeamMessage);
    }

    private CombatRoster CreateCombatRoster(
        TeamDefinition teamDefinition)
    {
        IReadOnlyList<TravelerCatalogEntry> travelerCatalog =
            combatCatalogLoader.ReadTravelers();

        IReadOnlyList<BeastCatalogEntry> beastCatalog =
            combatCatalogLoader.ReadBeasts();

        TravelerFactory travelerFactory =
            new(travelerCatalog);

        BeastFactory beastFactory =
            new(beastCatalog);

        CombatRosterFactory rosterFactory =
            new(travelerFactory, beastFactory);

        return rosterFactory.Create(
            teamDefinition);
    }
}