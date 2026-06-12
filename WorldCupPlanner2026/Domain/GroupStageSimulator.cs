namespace Domain;

public class GroupStageSimulator
{
    private const double DrawZone = 0.10;

    private readonly int _seed;

    public GroupStageSimulator(int seed)
    {
        _seed = seed;
    }

    public void Simulate(IEnumerable<Group> groups)
    {
        foreach (var group in groups)
            SimulateGroup(group);
    }

    private void SimulateGroup(Group group)
    {
        var random = new Random(_seed + group.Label);

        foreach (var match in group.Matches)
        {
            var (homeGoals, awayGoals) = SimulateResult(
                match.HomeTeam.RankingFifa,
                match.AwayTeam.RankingFifa,
                random);

            group.SetMatchResult(match, homeGoals, awayGoals);
        }
    }

    private static (int homeGoals, int awayGoals) SimulateResult(
        int homeRanking,
        int awayRanking,
        Random random)
    {
        if (homeRanking == awayRanking)
        {
            int drawGoals = SimulateGoals(random, 0, 2);
            return (drawGoals, drawGoals);
        }

        int total = homeRanking + awayRanking;
        double homeProbability = (double)homeRanking / total;
        double roll = random.NextDouble();

        if (roll < homeProbability - DrawZone)
            return (SimulateGoals(random, 2, 4), SimulateGoals(random, 0, 2));

        if (roll > homeProbability + DrawZone)
            return (SimulateGoals(random, 0, 2), SimulateGoals(random, 2, 4));

        int finalDrawGoals = SimulateGoals(random, 0, 2);
        return (finalDrawGoals, finalDrawGoals);
    }

    private static int SimulateGoals(Random random, int min, int max) =>
        random.Next(min, max + 1);
}