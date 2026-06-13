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
    
    [TestMethod]
    public void SimulateResult_CanProduceDraw_WhenDrawsAllowed()
    {
        var engine = new ProbabilisticEngine();
        var random = new Random(99);
        bool sawDraw = false;
        for (int i = 0; i < 300 && !sawDraw; i++)
        {
            var (home, away) = engine.SimulateResult(1500, 1450, allowDraw: true, random);
            sawDraw = home == away;
        }
        Assert.IsTrue(sawDraw);
    }
    
    [TestMethod]
    public void Probabilistic_AlwaysDraws_WhenRankingsEqualAndDrawsAllowed()
    {
        var engine = new ProbabilisticEngine();
        var random = new Random(50);
        for (int i = 0; i < 300; i++)
        {
            var (home, away) = engine.SimulateResult(1500, 1500, allowDraw: true, random);
            Assert.AreEqual(home, away);
        }
    }
    
}