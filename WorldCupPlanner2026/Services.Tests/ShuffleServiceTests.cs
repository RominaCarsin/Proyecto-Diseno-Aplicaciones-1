using Domain;

namespace Services.Tests;

[TestClass]
public class ShuffleServiceTests
{
    private const int DefaultSeed = 42;
    
    [TestMethod]
    public void Shuffle_WithSameSeed_ShouldProduceSameOrder()
    {
        var list1 = BuildStringList();
        var list2 = BuildStringList();

        new ShuffleService(DefaultSeed).Shuffle(list1);
        new ShuffleService(DefaultSeed).Shuffle(list2);

        CollectionAssert.AreEqual(list1, list2);
    }

    [TestMethod]
    public void Shuffle_WithDifferentSeeds_ShouldProduceDifferentOrders()
    {
        var list1 = new List<string> { "A", "B", "C", "D", "E", "F", "G", "H" };
        var list2 = new List<string> { "A", "B", "C", "D", "E", "F", "G", "H" };

        new ShuffleService(seed: 1).Shuffle(list1);
        new ShuffleService(seed: 2).Shuffle(list2);

        CollectionAssert.AreNotEqual(list1, list2);
    }

    [TestMethod]
    public void Shuffle_ShouldPreserveAllElements()
    {
        var original = BuildStringList();
        var shuffled = BuildStringList();

        new ShuffleService(DefaultSeed).Shuffle(shuffled);

        CollectionAssert.AreEquivalent(original, shuffled);
    }

    [TestMethod]
    public void Shuffle_WithSingleElement_ShouldNotChangeList()
    {
        var list = new List<int> { 10 };

        new ShuffleService(DefaultSeed).Shuffle(list);

        Assert.AreEqual(1, list.Count);
        Assert.AreEqual(10, list[0]);
    }

    [TestMethod]
    public void Shuffle_WithEmptyList_ShouldNotThrow()
    {
        var emptyList = new List<int>();

        new ShuffleService(DefaultSeed).Shuffle(emptyList);

        Assert.AreEqual(0, emptyList.Count);
    }

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

        new ShuffleService(DefaultSeed).ShuffleByRanking(teams);

        Assert.IsTrue(teams.Take(3).All(t => t.RankingFifa == 1000));
        Assert.IsTrue(teams.Skip(3).All(t => t.RankingFifa == 500));
    }
    
    private static List<string> BuildStringList() =>
        new List<string> { "URUGUAY", "ARGENTINA", "BRAZIL", "FRANCE" };
}
