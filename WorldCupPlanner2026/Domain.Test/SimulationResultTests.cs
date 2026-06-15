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

    [TestMethod]
    public void CreateSimulationResult_WithGoalsOnly_ShouldDefaultCardsToZero()
    {
        var result = new SimulationResult(2, 1);

        Assert.AreEqual(0, result.HomeYellowCards);
        Assert.AreEqual(0, result.AwayYellowCards);
        Assert.AreEqual(0, result.HomeRedCards);
        Assert.AreEqual(0, result.AwayRedCards);
    }

    [TestMethod]
    public void CreateSimulationResult_WithGoalsAndCards_ShouldStoreAllValues()
    {
        var result = new SimulationResult(2, 1, 3, 2, 1, 0);

        Assert.AreEqual(2, result.HomeGoals);
        Assert.AreEqual(1, result.AwayGoals);
        Assert.AreEqual(3, result.HomeYellowCards);
        Assert.AreEqual(2, result.AwayYellowCards);
        Assert.AreEqual(1, result.HomeRedCards);
        Assert.AreEqual(0, result.AwayRedCards);
    }
}