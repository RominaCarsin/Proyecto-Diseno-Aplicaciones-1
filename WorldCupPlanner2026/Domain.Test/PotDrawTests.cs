namespace Domain.Test;

[TestClass]
public class PotDrawTests
{
    private const int DefaultSeed = 42;

    [TestMethod]
    public void ShuffleByRanking_ShouldOnlyShuffleWithinTiedGroups()
    {
        var teams = new List<Team>
        {
            new("A", "UEFA", 1000),
            new("B", "UEFA", 1000),
            new("C", "UEFA", 1000),
            new("D", "UEFA", 500),
            new("E", "UEFA", 500),
        };

        new PotDraw().ShuffleByRanking(teams, DefaultSeed);

        Assert.IsTrue(teams.Take(3).All(t => t.RankingFifa == 1000));
        Assert.IsTrue(teams.Skip(3).All(t => t.RankingFifa == 500));
    }
}
