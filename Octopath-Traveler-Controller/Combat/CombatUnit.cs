namespace Octopath_Traveler;

public interface CombatUnit
{
    string Name { get; }
    int Speed { get; }

    bool IsAlive();
}