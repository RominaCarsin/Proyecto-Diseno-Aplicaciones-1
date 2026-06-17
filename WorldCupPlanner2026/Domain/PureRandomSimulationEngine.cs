namespace Domain;

public class PureRandomSimulationEngine : ISimulationEngine
{
    private const int MaxYellowCards = 5;
    private const int MaxRedCards = 1;

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
        int homeGoals = SimulateGoals(random, 0, 4);
        int awayGoals = SimulateGoals(random, 0, 4);
        int homeYellow = random.Next(0, MaxYellowCards + 1);
        int awayYellow = random.Next(0, MaxYellowCards + 1);
        int homeRed = random.Next(0, MaxRedCards + 1);
        int awayRed = random.Next(0, MaxRedCards + 1);

        return new SimulationResult(
            homeGoals,
            awayGoals,
            homeYellow,
            awayYellow,
            homeRed,
            awayRed);
    }

    private static SimulationResult SimulateKnockoutResult(Random random)
    {
        bool homeWins = random.Next(0, 2) == 0;
        int homeGoals;
        int awayGoals;

        if (homeWins)
        {
            homeGoals = SimulateGoals(random, 2, 3);
            awayGoals = SimulateGoals(random, 0, 1);
        }
        else
        {
            homeGoals = SimulateGoals(random, 0, 1);
            awayGoals = SimulateGoals(random, 2, 3);
        }

        int homeYellow = random.Next(0, MaxYellowCards + 1);
        int awayYellow = random.Next(0, MaxYellowCards + 1);
        int homeRed = random.Next(0, MaxRedCards + 1);
        int awayRed = random.Next(0, MaxRedCards + 1);

        return new SimulationResult(
            homeGoals,
            awayGoals,
            homeYellow,
            awayYellow,
            homeRed,
            awayRed);
    }

    private static int SimulateGoals(Random random, int min, int max) =>
        random.Next(min, max + 1);
}