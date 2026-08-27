namespace Octopath_Traveler;

public class TurnOrderBuilder
{
    private const int TravelerPriority = 0;
    private const int BeastPriority = 1;

    public IReadOnlyList<CombatUnit> Build(
        CombatRoster combatRoster)
    {
        IEnumerable<TurnOrderEntry> travelerEntries =
            CreateTravelerEntries(combatRoster.Travelers);

        IEnumerable<TurnOrderEntry> beastEntries =
            CreateBeastEntries(combatRoster.Beasts);

        return travelerEntries
            .Concat(beastEntries)
            .Where(IsAlive)
            .OrderByDescending(GetSpeed)
            .ThenBy(GetTypePriority)
            .ThenBy(GetBoardPosition)
            .Select(GetUnit)
            .ToArray();
    }

    private IEnumerable<TurnOrderEntry> CreateTravelerEntries(
        IReadOnlyList<Traveler> travelers)
    {
        return travelers.Select(
            (traveler, boardPosition) =>
                CreateTravelerEntry(
                    traveler,
                    boardPosition));
    }

    private IEnumerable<TurnOrderEntry> CreateBeastEntries(
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
            TravelerPriority,
            boardPosition);
    }

    private TurnOrderEntry CreateBeastEntry(
        Beast beast,
        int boardPosition)
    {
        return new TurnOrderEntry(
            beast,
            BeastPriority,
            boardPosition);
    }

    private bool IsAlive(TurnOrderEntry entry)
    {
        return entry.Unit.IsAlive();
    }

    private int GetSpeed(TurnOrderEntry entry)
    {
        return entry.Unit.Speed;
    }

    private int GetTypePriority(TurnOrderEntry entry)
    {
        return entry.TypePriority;
    }

    private int GetBoardPosition(TurnOrderEntry entry)
    {
        return entry.BoardPosition;
    }

    private CombatUnit GetUnit(TurnOrderEntry entry)
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
            TypePriority = typePriority;
            BoardPosition = boardPosition;
        }
    }
}