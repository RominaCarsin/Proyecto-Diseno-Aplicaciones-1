namespace Domain.Test;

[TestClass]
public class ProbabilisticEngineTests
{
    [TestMethod]
    public void ProbabilisticEngine_HasReadableName()
        => Assert.AreEqual("Probabilístico", new ProbabilisticEngine().Name);
}