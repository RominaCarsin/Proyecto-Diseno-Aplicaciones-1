namespace Domain.Test;

[TestClass]
public class ProbabilisticSimulationEngineTests
{
    private const int Seed = 42;

    private static Team CreateTeam(string name, int ranking) =>
        new(name, "CONMEBOL", ranking);

    [TestMethod]
    public void Name_ShouldReturnProbabilistic()
    {
        var engine = new ProbabilisticSimulationEngine();

        Assert.AreEqual("Probabilístico", engine.Name);
    }

    [TestMethod]
    public void Simulate_GroupStageWithEqualRankings_ShouldReturnDraw()
    {
        var engine = new ProbabilisticSimulationEngine();
        var home = CreateTeam("HOME", 1500);
        var away = CreateTeam("AWAY", 1500);

        var result = engine.Simulate(
            home,
            away,
            TournamentPhase.GroupStage,
            new Random(Seed));

        Assert.AreEqual(result.HomeGoals, result.AwayGoals);
    }

    [TestMethod]
    public void Simulate_KnockoutWithEqualRankings_ShouldNotReturnDraw()
    {
        var engine = new ProbabilisticSimulationEngine();
        var home = CreateTeam("HOME", 1500);
        var away = CreateTeam("AWAY", 1500);

        var result = engine.Simulate(
            home,
            away,
            TournamentPhase.RoundOf16,
            new Random(Seed));

        Assert.AreNotEqual(result.HomeGoals, result.AwayGoals);
    }

    [TestMethod]
    public void Simulate_WithSameSeed_ShouldReturnSameResult()
    {
        var engine = new ProbabilisticSimulationEngine();
        var home = CreateTeam("HOME", 1800);
        var away = CreateTeam("AWAY", 1200);

        var firstResult = engine.Simulate(
            home,
            away,
            TournamentPhase.GroupStage,
            new Random(Seed));

        var secondResult = engine.Simulate(
            home,
            away,
            TournamentPhase.GroupStage,
            new Random(Seed));

        Assert.AreEqual(firstResult, secondResult);
    }
}  