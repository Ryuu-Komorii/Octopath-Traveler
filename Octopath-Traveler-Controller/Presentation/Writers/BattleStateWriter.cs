using Octopath_Traveler_View;

namespace Octopath_Traveler;

public class BattleStateWriter
{
    private const string PlayerTeamHeader =
        "Equipo del jugador";

    private const string EnemyTeamHeader =
        "Equipo del enemigo";

    private const string BoardPositionFormat =
        "{0}-{1}";

    private const char FirstBoardPosition =
        'A';

    private readonly View view;

    public BattleStateWriter(
        View view)
    {
        this.view =
            view;
    }

    public void Write(
        CombatRoster combatRoster)
    {
        WriteTravelers(
            combatRoster.Travelers);

        WriteBeasts(
            combatRoster.Beasts);
    }

    private void WriteTravelers(
        IReadOnlyList<Traveler> travelers)
    {
        view.WriteLine(
            PlayerTeamHeader);

        IEnumerable<string> travelerLines =
            travelers.Select(
                FormatTraveler);

        WriteLines(
            travelerLines);
    }

    private void WriteBeasts(
        IReadOnlyList<Beast> beasts)
    {
        view.WriteLine(
            EnemyTeamHeader);

        IEnumerable<string> beastLines =
            beasts.Select(
                FormatBeast);

        WriteLines(
            beastLines);
    }

    private string FormatTraveler(
        Traveler traveler,
        int boardPosition)
    {
        string travelerStatus =
            string.Format(
                CombatText.TravelerStatusFormat,
                traveler.Name,
                traveler.CurrentHP,
                traveler.MaxHP,
                traveler.CurrentSP,
                traveler.MaxSP,
                traveler.BoostPoints);

        return AddBoardPosition(
            travelerStatus,
            boardPosition);
    }

    private string FormatBeast(
        Beast beast,
        int boardPosition)
    {
        string beastStatus =
            string.Format(
                CombatText.BeastStatusFormat,
                beast.Name,
                beast.CurrentHP,
                beast.MaxHP,
                beast.Shields);

        return AddBoardPosition(
            beastStatus,
            boardPosition);
    }

    private string AddBoardPosition(
        string unitStatus,
        int boardPosition)
    {
        char position =
            GetBoardPosition(
                boardPosition);

        return string.Format(
            BoardPositionFormat,
            position,
            unitStatus);
    }

    private char GetBoardPosition(
        int boardPosition)
    {
        return (char)(
            FirstBoardPosition +
            boardPosition);
    }

    private void WriteLines(
        IEnumerable<string> lines)
    {
        foreach (string line in lines)
        {
            view.WriteLine(
                line);
        }
    }
}