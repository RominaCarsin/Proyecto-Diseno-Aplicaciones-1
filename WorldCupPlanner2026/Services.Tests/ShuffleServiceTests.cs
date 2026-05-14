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
    public void ShuffleRange_ShouldOnlyAffectElementsWithinRange()
    {
        var list = new List<string> { "A", "B", "C", "D", "E" };

        new ShuffleService(DefaultSeed).ShuffleRange(list, startIndex: 1, endIndex: 4);

        Assert.AreEqual("A", list[0]);
        Assert.AreEqual("E", list[4]);
    }

    [TestMethod]
    public void ShuffleRange_ShouldPreserveAllElementsWithinRange()
    {
        var list = new List<string> { "A", "B", "C", "D", "E" };

        new ShuffleService(DefaultSeed).ShuffleRange(list, startIndex: 1, endIndex: 4);

        CollectionAssert.Contains(list, "B");
        CollectionAssert.Contains(list, "C");
        CollectionAssert.Contains(list, "D");
    }

    [TestMethod]
    public void ShuffleRange_WithSameSeed_ShouldProduceSameOrder()
    {
        var list1 = new List<string> { "A", "B", "C", "D", "E" };
        var list2 = new List<string> { "A", "B", "C", "D", "E" };

        new ShuffleService(DefaultSeed).ShuffleRange(list1, startIndex: 0, endIndex: 5);
        new ShuffleService(DefaultSeed).ShuffleRange(list2, startIndex: 0, endIndex: 5);

        CollectionAssert.AreEqual(list1, list2);
    }

    [TestMethod]
    public void ShuffleRange_WithSingleElementRange_ShouldNotChangeList()
    {
        var list = new List<int> { 1, 2, 3 };

        new ShuffleService(DefaultSeed).ShuffleRange(list, startIndex: 1, endIndex: 2);

        Assert.AreEqual(1, list[0]);
        Assert.AreEqual(2, list[1]);
        Assert.AreEqual(3, list[2]);
    }
    
    private static List<string> BuildStringList() =>
        new List<string> { "URUGUAY", "ARGENTINA", "BRAZIL", "FRANCE" };
}