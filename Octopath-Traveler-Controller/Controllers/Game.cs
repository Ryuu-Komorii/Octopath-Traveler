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

    public Game(
        View view,
        string teamsFolder)
    {
        this.view = view;

        teamFileSelector =
            new TeamFileSelector(
                view,
                teamsFolder);

        teamFileParser =
            new TeamFileParser();

        teamStructureValidator =
            new TeamStructureValidator();

        combatCatalogLoader =
            new CombatCatalogLoader();

        ActiveSkillCatalog activeSkillCatalog =
            CreateActiveSkillCatalog();

        BeastSkillCatalog beastSkillCatalog =
            CreateBeastSkillCatalog();

        battle =
            new Battle(
                view,
                activeSkillCatalog,
                beastSkillCatalog);
    }

    public void Play()
    {
        TeamDefinition teamDefinition =
            ReadSelectedTeam();

        if (IsInvalidTeam(
            teamDefinition))
        {
            WriteInvalidTeamMessage();
            return;
        }

        CombatRoster combatRoster =
            CreateCombatRoster(
                teamDefinition);

        battle.Play(
            combatRoster);
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

    private void WriteInvalidTeamMessage()
    {
        view.WriteLine(
            InvalidTeamMessage);
    }

    private ActiveSkillCatalog CreateActiveSkillCatalog()
    {
        IReadOnlyList<ActiveSkillCatalogEntry> activeSkills =
            combatCatalogLoader.ReadActiveSkills();

        return new ActiveSkillCatalog(
            activeSkills);
    }

    private BeastSkillCatalog CreateBeastSkillCatalog()
    {
        IReadOnlyList<BeastSkillCatalogEntry> beastSkills =
            combatCatalogLoader.ReadBeastSkills();

        return new BeastSkillCatalog(
            beastSkills);
    }

    private CombatRoster CreateCombatRoster(
        TeamDefinition teamDefinition)
    {
        TravelerFactory travelerFactory =
            CreateTravelerFactory();

        BeastFactory beastFactory =
            CreateBeastFactory();

        CombatRosterFactory rosterFactory =
            new CombatRosterFactory(
                travelerFactory,
                beastFactory);

        return rosterFactory.Create(
            teamDefinition);
    }

    private TravelerFactory CreateTravelerFactory()
    {
        IReadOnlyList<TravelerCatalogEntry> travelerCatalog =
            combatCatalogLoader.ReadTravelers();

        return new TravelerFactory(
            travelerCatalog);
    }

    private BeastFactory CreateBeastFactory()
    {
        IReadOnlyList<BeastCatalogEntry> beastCatalog =
            combatCatalogLoader.ReadBeasts();

        return new BeastFactory(
            beastCatalog);
    }
}