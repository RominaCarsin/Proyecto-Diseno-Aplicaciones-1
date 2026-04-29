using Domain;

namespace Domain.Test;

[TestClass]
public class FisherYatesTests
{
    [TestMethod]
    public void Shuffle_WithSameSeed_ShouldReturnSameOrder()
    {
        var list1 = new List<int> { 1, 2, 3, 4, 5 };
        var list2 = new List<int> { 1, 2, 3, 4, 5 };

        var result1 = FisherYates.Shuffle(list1, 42);
        var result2 = FisherYates.Shuffle(list2, 42);

        CollectionAssert.AreEqual(result1, result2);
    }

    [TestMethod]
    public void Shuffle_WithDifferentSeed_ShouldReturnDifferentOrder()
    {
        var list1 = new List<int> { 1, 2, 3, 4, 5 };
        var list2 = new List<int> { 1, 2, 3, 4, 5 };

        var result1 = FisherYates.Shuffle(list1, 42);
        var result2 = FisherYates.Shuffle(list2, 99);

        CollectionAssert.AreNotEqual(result1, result2);
    }

    [TestMethod]
    public void Shuffle_ShouldContainAllOriginalElements()
    {
        var list = new List<int> { 1, 2, 3, 4, 5 };

        var result = FisherYates.Shuffle(list, 42);

        CollectionAssert.AreEquivalent(list, result);
    }

    [TestMethod]
    public void Shuffle_EmptyList_ShouldReturnEmptyList()
    {
        var list = new List<int>();

        var result = FisherYates.Shuffle(list, 42);

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void Shuffle_SingleElement_ShouldReturnSameElement()
    {
        var list = new List<int> { 99 };

        var result = FisherYates.Shuffle(list, 42);

        Assert.AreEqual(99, result[0]);
    }
}