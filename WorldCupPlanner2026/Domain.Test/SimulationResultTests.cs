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

    [TestMethod]
    public void Equals_SameValues_ShouldReturnTrue()
    {
        var a = new SimulationResult(2, 1, 3, 2, 1, 0);
        var b = new SimulationResult(2, 1, 3, 2, 1, 0);

        Assert.IsTrue(a.Equals(b));
    }

    [TestMethod]
    public void Equals_SameReference_ShouldReturnTrue()
    {
        var a = new SimulationResult(2, 1);

        Assert.IsTrue(a.Equals(a));
    }

    [TestMethod]
    public void Equals_Null_ShouldReturnFalse()
    {
        var a = new SimulationResult(2, 1);

        Assert.IsFalse(a.Equals(null));
    }

    [TestMethod]
    public void Equals_DifferentType_ShouldReturnFalse()
    {
        var a = new SimulationResult(2, 1);

        Assert.IsFalse(a.Equals("string"));
    }

    [TestMethod]
    public void Equals_DifferentHomeGoals_ShouldReturnFalse()
    {
        var a = new SimulationResult(2, 1);
        var b = new SimulationResult(3, 1);

        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void Equals_DifferentAwayGoals_ShouldReturnFalse()
    {
        var a = new SimulationResult(2, 1);
        var b = new SimulationResult(2, 2);

        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void Equals_DifferentHomeYellowCards_ShouldReturnFalse()
    {
        var a = new SimulationResult(0, 0, 1, 0, 0, 0);
        var b = new SimulationResult(0, 0, 2, 0, 0, 0);

        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void Equals_DifferentAwayYellowCards_ShouldReturnFalse()
    {
        var a = new SimulationResult(0, 0, 0, 1, 0, 0);
        var b = new SimulationResult(0, 0, 0, 2, 0, 0);

        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void Equals_DifferentHomeRedCards_ShouldReturnFalse()
    {
        var a = new SimulationResult(0, 0, 0, 0, 1, 0);
        var b = new SimulationResult(0, 0, 0, 0, 2, 0);

        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void Equals_DifferentAwayRedCards_ShouldReturnFalse()
    {
        var a = new SimulationResult(0, 0, 0, 0, 0, 1);
        var b = new SimulationResult(0, 0, 0, 0, 0, 2);

        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void GetHashCode_EqualObjects_ShouldBeEqual()
    {
        var a = new SimulationResult(2, 1, 3, 2, 1, 0);
        var b = new SimulationResult(2, 1, 3, 2, 1, 0);

        Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void GetHashCode_DifferentObjects_ShouldDiffer()
    {
        var a = new SimulationResult(2, 1);
        var b = new SimulationResult(1, 2);

        Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
    }
}