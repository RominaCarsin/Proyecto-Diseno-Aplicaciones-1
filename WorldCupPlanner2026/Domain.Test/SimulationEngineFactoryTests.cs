namespace Domain.Test;

[TestClass]
public class SimulationEngineFactoryTests
{
    [TestMethod]
    public void Create_WithProbabilisticType_ShouldReturnProbabilisticEngine()
    {
        var engine = SimulationEngineFactory.Create(SimulationEngineType.Probabilistic);

        Assert.IsInstanceOfType(engine, typeof(ProbabilisticSimulationEngine));
        Assert.AreEqual("Probabilístico", engine.Name);
    }
}