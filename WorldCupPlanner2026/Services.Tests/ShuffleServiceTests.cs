namespace Services.Tests;

[TestClass]
public class ShuffleServiceTests
{
    // ─── Shuffle ──────────────────────────────────────────────────────────────

    [TestMethod]
    public void Shuffle_WithSameSeed_ShouldProduceSameOrder()
    {
        var list1 = new List<string> { "Uruguay", "Argentina", "Brasil", "Francia" };
        var list2 = new List<string> { "Uruguay", "Argentina", "Brasil", "Francia" };

        new ShuffleService(42).Shuffle(list1);
        new ShuffleService(42).Shuffle(list2);

        CollectionAssert.AreEqual(list1, list2);
    }

    [TestMethod]
    public void Shuffle_WithDifferentSeeds_ShouldProduceDifferentOrders()
    {
        var list1 = new List<string> { "A", "B", "C", "D", "E", "F", "G", "H" };
        var list2 = new List<string> { "A", "B", "C", "D", "E", "F", "G", "H" };

        new ShuffleService(1).Shuffle(list1);
        new ShuffleService(2).Shuffle(list2);

        CollectionAssert.AreNotEqual(list1, list2);
    }

    [TestMethod]
    public void Shuffle_ShouldContainSameElements()
    {
        var original = new List<string> { "Uruguay", "Argentina", "Brasil", "Francia" };
        var list     = new List<string> { "Uruguay", "Argentina", "Brasil", "Francia" };

        new ShuffleService(42).Shuffle(list);

        CollectionAssert.AreEquivalent(original, list);
    }

    [TestMethod]
    public void Shuffle_SingleElement_ShouldNotChangeList()
    {
        var list = new List<int> { 10 };

        new ShuffleService(42).Shuffle(list);

        Assert.AreEqual(1, list.Count);
        Assert.AreEqual(10, list[0]);
    }

    [TestMethod]
    public void Shuffle_EmptyList_ShouldNotThrow()
    {
        var list = new List<int>();

        new ShuffleService(42).Shuffle(list);

        Assert.AreEqual(0, list.Count);
    }

    // ─── ShuffleRange ─────────────────────────────────────────────────────────

    [TestMethod]
    public void ShuffleRange_ShouldOnlyAffectElementsInRange()
    {
        var list = new List<string> { "A", "B", "C", "D", "E" };

        new ShuffleService(42).ShuffleRange(list, 1, 4); // solo índices 1, 2, 3

        // A y E no deben moverse
        Assert.AreEqual("A", list[0]);
        Assert.AreEqual("E", list[4]);
    }

    [TestMethod]
    public void ShuffleRange_ShouldContainSameElementsInRange()
    {
        var list = new List<string> { "A", "B", "C", "D", "E" };

        new ShuffleService(42).ShuffleRange(list, 1, 4);

        // Los elementos del rango siguen estando, solo en otro orden
        CollectionAssert.Contains(list, "B");
        CollectionAssert.Contains(list, "C");
        CollectionAssert.Contains(list, "D");
    }

    [TestMethod]
    public void ShuffleRange_WithSameSeed_ShouldProduceSameOrder()
    {
        var list1 = new List<string> { "A", "B", "C", "D", "E" };
        var list2 = new List<string> { "A", "B", "C", "D", "E" };

        new ShuffleService(42).ShuffleRange(list1, 0, 5);
        new ShuffleService(42).ShuffleRange(list2, 0, 5);

        CollectionAssert.AreEqual(list1, list2);
    }

    [TestMethod]
    public void ShuffleRange_SingleElement_ShouldNotChangeList()
    {
        var list = new List<int> { 1, 2, 3 };

        new ShuffleService(42).ShuffleRange(list, 1, 2); // rango de un solo elemento

        Assert.AreEqual(1, list[0]);
        Assert.AreEqual(2, list[1]);
        Assert.AreEqual(3, list[2]);
    }
}