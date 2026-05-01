using Services;

namespace Services.Tests;

[TestClass]
public class ShuffleServiceTests
{
    [TestMethod]
    public void Shuffle_WithKnownSeed_ShouldProduceDeterministicOrder()
    {
        int seed = 42;
        var list1 = new List<string> { "Uruguay", "Argentina", "Brasil", "Francia" };
        var list2 = new List<string> { "Uruguay", "Argentina", "Brasil", "Francia" };
        
        var service1 = new ShuffleService(seed);
        var service2 = new ShuffleService(seed);
        
        service1.Shuffle(list1);
        service2.Shuffle(list2);

        CollectionAssert.AreEqual(list1, list2);
    }

    [TestMethod]
    public void Shuffle_WithDifferentSeeds_ShouldProduceDifferentOrders()
    {
        var list1 = Enumerable.Range(1, 50).ToList();
        var list2 = Enumerable.Range(1, 50).ToList();
        
        var service1 = new ShuffleService(123);
        var service2 = new ShuffleService(999);
        
        service1.Shuffle(list1);
        service2.Shuffle(list2);
        
        CollectionAssert.AreNotEqual(list1, list2);
    }
    

    [TestMethod]
    public void Shuffle_SingleElement_ShouldNotThrow()
    {
        var service = new ShuffleService(42);
        var list = new List<int> { 10 };
        
        service.Shuffle(list);
        
        Assert.AreEqual(1, list.Count);
        Assert.AreEqual(10, list[0]);
    }
}