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
    
    [TestMethod]
    public void Create_WithPureRandomType_ShouldReturnPureRandomEngine()
    {
        var engine = SimulationEngineFactory.Create(SimulationEngineType.PureRandom);

        Assert.IsInstanceOfType(engine, typeof(PureRandomSimulationEngine));
        Assert.AreEqual("Aleatorio Puro", engine.Name);
    }
}