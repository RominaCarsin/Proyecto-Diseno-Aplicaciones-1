namespace Domain.Test;

[TestClass]
public class KnockoutTests
{
    private const int simulationSeed = 12345;
    
    private Knockout CreateValidKnockout(int seed = simulationSeed) => new(seed);

    [TestMethod]
    public void CreateKnockout_WithSimulationSeed_ShouldHaveAsAtributeSimulationSeed()
    {
        var knockout = CreateValidKnockout();

        Assert.AreEqual(simulationSeed, knockout.SimulationSeed);
    }

    [TestMethod]
    public void CreateKnockout_WithoutSimulationSeed_ShouldSucced()
    {
        var knockout = CreateValidKnockout();
        
        Assert.IsNotNull(knockout);
    }

    [TestMethod]
    public void UpdateSimulationSeed_WithValidSeed_ShouldSucceed()
    {
        var knockout = CreateValidKnockout();
        int newSeed = 999;
        
        knockout.UpdateSimulationSeed(newSeed);
        
        Assert.AreEqual(newSeed, knockout.SimulationSeed);
    }
}