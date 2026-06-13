namespace Domain.Test;

[TestClass]
public class ProbabilisticEngineTests
{
    [TestMethod]
    public void ProbabilisticEngine_HasReadableName()
        => Assert.AreEqual("Probabilístico", new ProbabilisticEngine().Name);
    
    [TestMethod]
    public void SimulateResult_IsDeterministic_ForSameSeed()
    {
        var engine = new ProbabilisticEngine();
        var first  = engine.SimulateResult(1500, 1200, allowDraw: true, new Random(7));
        var second = engine.SimulateResult(1500, 1200, allowDraw: true, new Random(7));
        Assert.AreEqual(first, second);
    }
}