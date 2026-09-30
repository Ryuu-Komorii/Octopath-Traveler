namespace Octopath_Traveler;

public interface CombatUnit
{
    string Name { get; }

    int Speed { get; }

    CombatUnitTypePriority TypePriority { get; }

    bool IsAlive();

    void BeginRound(
        int roundNumber);

    bool CanActInRound(
        int roundNumber);

    TurnPriority GetTurnPriority(
        int roundNumber);
}