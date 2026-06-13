namespace Domain.Test;

[TestClass]
public class PureRandomSimulationEngineTests
{
    private const int Seed = 42;

    private static Team CreateTeam(string name, int ranking) =>
        new(name, "CONMEBOL", ranking);
    [TestMethod]
    public void Name_ShouldReturnPureRandom()
    {
        var engine = new PureRandomSimulationEngine();

        Assert.AreEqual("Aleatorio Puro", engine.Name);
    }
    
    [TestMethod]
    public void Simulate_GroupStage_ShouldReturnGoalsWithinValidRange()
    {
        var engine = new PureRandomSimulationEngine();
        var home = CreateTeam("HOME", 2500);
        var away = CreateTeam("AWAY", 300);

        var result = engine.Simulate(
            home,
            away,
            TournamentPhase.GroupStage,
            new Random(Seed));

        Assert.IsTrue(result.HomeGoals is >= 0 and <= 4);
        Assert.IsTrue(result.AwayGoals is >= 0 and <= 4);
    }
}