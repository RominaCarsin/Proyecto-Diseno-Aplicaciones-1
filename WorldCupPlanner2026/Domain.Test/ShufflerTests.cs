namespace Domain.Test;

[TestClass]
public class ShufflerTests
{
    private const int DefaultSeed = 42;

    [TestMethod]
    public void Shuffle_WithSameSeed_ShouldProduceSameOrder()
    {
        var list1 = BuildStringList();
        var list2 = BuildStringList();

        new Shuffler(DefaultSeed).Shuffle(list1);
        new Shuffler(DefaultSeed).Shuffle(list2);

        CollectionAssert.AreEqual(list1, list2);
    }

    [TestMethod]
    public void Shuffle_WithDifferentSeeds_ShouldProduceDifferentOrders()
    {
        var list1 = new List<string> { "A", "B", "C", "D", "E", "F", "G", "H" };
        var list2 = new List<string> { "A", "B", "C", "D", "E", "F", "G", "H" };

        new Shuffler(seed: 1).Shuffle(list1);
        new Shuffler(seed: 2).Shuffle(list2);

        CollectionAssert.AreNotEqual(list1, list2);
    }

    [TestMethod]
    public void Shuffle_ShouldPreserveAllElements()
    {
        var original = BuildStringList();
        var shuffled = BuildStringList();

        new Shuffler(DefaultSeed).Shuffle(shuffled);

        CollectionAssert.AreEquivalent(original, shuffled);
    }

    [TestMethod]
    public void Shuffle_WithSingleElement_ShouldNotChangeList()
    {
        var list = new List<int> { 10 };

        new Shuffler(DefaultSeed).Shuffle(list);

        Assert.AreEqual(1, list.Count);
        Assert.AreEqual(10, list[0]);
    }

    [TestMethod]
    public void Shuffle_WithEmptyList_ShouldNotThrow()
    {
        var emptyList = new List<int>();

        new Shuffler(DefaultSeed).Shuffle(emptyList);

        Assert.AreEqual(0, emptyList.Count);
    }

    private static List<string> BuildStringList() =>
        new List<string> { "URUGUAY", "ARGENTINA", "BRAZIL", "FRANCE" };
}
