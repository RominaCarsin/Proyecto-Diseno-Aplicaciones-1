namespace Domain;

public class PureRandomSimulationEngine : ISimulationEngine
{
    public string Name => "Aleatorio Puro";

    public SimulationResult Simulate(
        Team homeTeam,
        Team awayTeam,
        TournamentPhase phase,
        Random random)
    {
        return phase == TournamentPhase.GroupStage
            ? SimulateGroupStageResult(random)
            : SimulateKnockoutResult(random);
    }

    private static SimulationResult SimulateGroupStageResult(Random random)
    {
        return new SimulationResult(
            SimulateGoals(random, 0, 4),
            SimulateGoals(random, 0, 4));
    }

    private static SimulationResult SimulateKnockoutResult(Random random)
    {
        bool homeWins = random.Next(0, 2) == 0;

        if (homeWins)
            return new SimulationResult(
                SimulateGoals(random, 2, 3),
                SimulateGoals(random, 0, 1));

        return new SimulationResult(
            SimulateGoals(random, 0, 1),
            SimulateGoals(random, 2, 3));
    }

    private static int SimulateGoals(Random random, int min, int max) =>
        random.Next(min, max + 1);
}