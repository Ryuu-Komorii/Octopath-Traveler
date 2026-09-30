namespace Octopath_Traveler;

public class TurnOrderBuilder
{
    public IReadOnlyList<CombatUnit> Build(
        CombatRoster combatRoster,
        int roundNumber)
    {
        IEnumerable<TurnOrderEntry> travelerEntries =
            CreateEntries(
                combatRoster.Travelers);

        IEnumerable<TurnOrderEntry> beastEntries =
            CreateEntries(
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

    private IEnumerable<TurnOrderEntry> CreateEntries<T>(
        IReadOnlyList<T> units)
        where T : CombatUnit
    {
        return units.Select(
            (unit, boardPosition) =>
                new TurnOrderEntry(
                    unit,
                    boardPosition));
    }

    private bool CanAct(
        TurnOrderEntry entry,
        int roundNumber)
    {
        return entry.Unit.CanActInRound(
            roundNumber);
    }

    private TurnPriority GetTurnPriority(
        TurnOrderEntry entry,
        int roundNumber)
    {
        return entry.Unit.GetTurnPriority(
            roundNumber);
    }

    private int GetSpeed(
        TurnOrderEntry entry)
    {
        return entry.Unit.Speed;
    }

    private CombatUnitTypePriority GetTypePriority(
        TurnOrderEntry entry)
    {
        return entry.Unit.TypePriority;
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

        public int BoardPosition { get; }

        public TurnOrderEntry(
            CombatUnit unit,
            int boardPosition)
        {
            Unit =
                unit;

            BoardPosition =
                boardPosition;
        }
    }
}