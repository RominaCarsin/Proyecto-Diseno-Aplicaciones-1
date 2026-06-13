namespace Domain.Test;

[TestClass]
public class PureRandomSimulationEngineTests
{
    [TestMethod]
    public void Name_ShouldReturnPureRandom()
    {
        var engine = new PureRandomSimulationEngine();

        Assert.AreEqual("Aleatorio Puro", engine.Name);
    }
}