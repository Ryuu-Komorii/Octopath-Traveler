namespace Octopath_Traveler;

public class BattleOutcomeEvaluator
{
    public BattleOutcome Evaluate(
        CombatRoster roster)
    {
        if (HasNoLivingTravelers(roster))
        {
            return BattleOutcome.EnemyWon;
        }

        if (HasNoLivingBeasts(roster))
        {
            return BattleOutcome.PlayerWon;
        }

        return BattleOutcome.InProgress;
    }

    public bool HasBattleEnded(
        BattleOutcome outcome)
    {
        return outcome !=
               BattleOutcome.InProgress;
    }

    public bool IsBattleInProgress(
        BattleOutcome outcome)
    {
        return outcome ==
               BattleOutcome.InProgress;
    }

    private bool HasNoLivingTravelers(
        CombatRoster roster)
    {
        return !roster.Travelers
            .Any(IsAlive);
    }

    private bool HasNoLivingBeasts(
        CombatRoster roster)
    {
        return !roster.Beasts
            .Any(IsAlive);
    }

    private bool IsAlive(
        Traveler traveler)
    {
        return traveler.IsAlive();
    }

    private bool IsAlive(
        Beast beast)
    {
        return beast.IsAlive();
    }
}