namespace Octopath_Traveler;

public class TurnOrderBuilder
{
    private const int TravelerTypePriority = 0;
    private const int BeastTypePriority = 1;

    public IReadOnlyList<CombatUnit> Build(
        CombatRoster combatRoster,
        int roundNumber)
    {
        IEnumerable<TurnOrderEntry> travelerEntries =
            CreateTravelerEntries(
                combatRoster.Travelers);

        IEnumerable<TurnOrderEntry> beastEntries =
            CreateBeastEntries(
                combatRoster.Beasts);

        return travelerEntries
            .Concat(beastEntries)
            .Where(entry =>
                CanAct(
                    entry,
                    roundNumber))
            .OrderBy(entry =>
                GetTurnPriority(
                    entry,
                    roundNumber))
            .ThenByDescending(
                GetSpeed)
            .ThenBy(
                GetTypePriority)
            .ThenBy(
                GetBoardPosition)
            .Select(
                GetUnit)
            .ToArray();
    }

    public IReadOnlyList<CombatUnit> ReorderPending(
        CombatRoster combatRoster,
        IReadOnlyCollection<CombatUnit> pendingTurns,
        int roundNumber)
    {
        return Build(
                combatRoster,
                roundNumber)
            .Where(
                pendingTurns.Contains)
            .ToArray();
    }

    private IEnumerable<TurnOrderEntry>
        CreateTravelerEntries(
            IReadOnlyList<Traveler> travelers)
    {
        return travelers.Select(
            (traveler, boardPosition) =>
                CreateTravelerEntry(
                    traveler,
                    boardPosition));
    }

    private IEnumerable<TurnOrderEntry>
        CreateBeastEntries(
            IReadOnlyList<Beast> beasts)
    {
        return beasts.Select(
            (beast, boardPosition) =>
                CreateBeastEntry(
                    beast,
                    boardPosition));
    }

    private TurnOrderEntry CreateTravelerEntry(
        Traveler traveler,
        int boardPosition)
    {
        return new TurnOrderEntry(
            traveler,
            TravelerTypePriority,
            boardPosition);
    }

    private TurnOrderEntry CreateBeastEntry(
        Beast beast,
        int boardPosition)
    {
        return new TurnOrderEntry(
            beast,
            BeastTypePriority,
            boardPosition);
    }

    private bool CanAct(
        TurnOrderEntry entry,
        int roundNumber)
    {
        return entry.Unit switch
        {
            Traveler traveler =>
                traveler.CanActInRound(
                    roundNumber),

            Beast beast =>
                beast.CanActInRound(
                    roundNumber),

            _ => false
        };
    }

    private TurnPriority GetTurnPriority(
        TurnOrderEntry entry,
        int roundNumber)
    {
        return entry.Unit switch
        {
            Traveler traveler =>
                traveler.GetTurnPriority(
                    roundNumber),

            Beast beast =>
                beast.GetTurnPriority(
                    roundNumber),

            _ =>
                TurnPriority.Normal
        };
    }

    private int GetSpeed(
        TurnOrderEntry entry)
    {
        return entry.Unit.Speed;
    }

    private int GetTypePriority(
        TurnOrderEntry entry)
    {
        return entry.TypePriority;
    }

    private int GetBoardPosition(
        TurnOrderEntry entry)
    {
        return entry.BoardPosition;
    }

    private CombatUnit GetUnit(
        TurnOrderEntry entry)
    {
        return entry.Unit;
    }

    private class TurnOrderEntry
    {
        public CombatUnit Unit { get; }
        public int TypePriority { get; }
        public int BoardPosition { get; }

        public TurnOrderEntry(
            CombatUnit unit,
            int typePriority,
            int boardPosition)
        {
            Unit = unit;
            TypePriority =
                typePriority;

            BoardPosition =
                boardPosition;
        }
    }
}