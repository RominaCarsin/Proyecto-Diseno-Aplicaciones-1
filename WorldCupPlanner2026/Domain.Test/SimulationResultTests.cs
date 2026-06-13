namespace Domain.Test;

[TestClass]
public class SimulationResultTests
{
    [TestMethod]
    public void CreateSimulationResult_WithGoals_ShouldStoreValues()
    {
        var result = new SimulationResult(2, 1);

        Assert.AreEqual(2, result.HomeGoals);
        Assert.AreEqual(1, result.AwayGoals);
    }
}