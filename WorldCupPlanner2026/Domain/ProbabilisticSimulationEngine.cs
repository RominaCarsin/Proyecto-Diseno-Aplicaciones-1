namespace Domain;

public class ProbabilisticSimulationEngine : ISimulationEngine
{
    private const double DrawZone = 0.10;
    private const int MaxYellowCards = 5;
    private const int MaxRedCards = 1;

    public string Name => "Probabilístico";

    public SimulationResult Simulate(
        Team homeTeam,
        Team awayTeam,
        TournamentPhase phase,
        Random random)
    {
        return phase == TournamentPhase.GroupStage
            ? SimulateGroupStageResult(homeTeam.RankingFifa, awayTeam.RankingFifa, random)
            : SimulateKnockoutResult(homeTeam.RankingFifa, awayTeam.RankingFifa, random);
    }

    private static SimulationResult SimulateGroupStageResult(
        int homeRanking,
        int awayRanking,
        Random random)
    {
        SimulationResult goals = GetGroupStageGoals(homeRanking, awayRanking, random);
        int homeYellow = random.Next(0, MaxYellowCards + 1);
        int awayYellow = random.Next(0, MaxYellowCards + 1);
        int homeRed = random.Next(0, MaxRedCards + 1);
        int awayRed = random.Next(0, MaxRedCards + 1);

        return new SimulationResult(
            goals.HomeGoals,
            goals.AwayGoals,
            homeYellow,
            awayYellow,
            homeRed,
            awayRed);
    }

    private static SimulationResult SimulateKnockoutResult(
        int homeRanking,
        int awayRanking,
        Random random)
    {
        SimulationResult goals = GetKnockoutGoals(homeRanking, awayRanking, random);
        int homeYellow = random.Next(0, MaxYellowCards + 1);
        int awayYellow = random.Next(0, MaxYellowCards + 1);
        int homeRed = random.Next(0, MaxRedCards + 1);
        int awayRed = random.Next(0, MaxRedCards + 1);

        return new SimulationResult(
            goals.HomeGoals,
            goals.AwayGoals,
            homeYellow,
            awayYellow,
            homeRed,
            awayRed);
    }

    private static SimulationResult GetGroupStageGoals(
        int homeRanking,
        int awayRanking,
        Random random)
    {
        if (homeRanking == awayRanking)
        {
            int drawGoals = SimulateGoals(random, 0, 2);
            return new SimulationResult(drawGoals, drawGoals);
        }

        int total = homeRanking + awayRanking;
        double homeProbability = (double)homeRanking / total;
        double roll = random.NextDouble();

        if (roll < homeProbability - DrawZone)
            return new SimulationResult(
                SimulateGoals(random, 2, 4),
                SimulateGoals(random, 0, 2));

        if (roll > homeProbability + DrawZone)
            return new SimulationResult(
                SimulateGoals(random, 0, 2),
                SimulateGoals(random, 2, 4));

        int finalDrawGoals = SimulateGoals(random, 0, 2);
        return new SimulationResult(finalDrawGoals, finalDrawGoals);
    }

    private static SimulationResult GetKnockoutGoals(
        int homeRanking,
        int awayRanking,
        Random random)
    {
        int total = homeRanking + awayRanking;
        double homeProbability = (double)homeRanking / total;

        if (random.NextDouble() < homeProbability)
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