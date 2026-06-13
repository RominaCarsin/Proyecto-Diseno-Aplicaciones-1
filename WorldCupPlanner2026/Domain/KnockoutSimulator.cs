namespace Domain;

public class KnockoutSimulator
{
    private readonly int _seed;

    public KnockoutSimulator(int seed)
    {
        _seed = seed;
    }

    public void Simulate(IEnumerable<Match> matches)
    {
        var random = new Random(_seed);

        foreach (var match in matches)
        {
            if (match.Status == MatchStatus.Finished)
                continue;

            var (homeGoals, awayGoals) = SimulateResult(
                match.HomeTeam.RankingFifa,
                match.AwayTeam.RankingFifa,
                random);

            match.SetResult(homeGoals, awayGoals);
        }
    }

    private static (int homeGoals, int awayGoals) SimulateResult(
        int homeRanking, int awayRanking, Random random)
    {
        int total = homeRanking + awayRanking;
        double homeProbability = (double)homeRanking / total;

        if (random.NextDouble() < homeProbability)
            return (SimulateGoals(random, 2, 3), SimulateGoals(random, 0, 1));

        return (SimulateGoals(random, 0, 1), SimulateGoals(random, 2, 3));
    }

    private static int SimulateGoals(Random random, int min, int max) =>
        random.Next(min, max + 1);
}