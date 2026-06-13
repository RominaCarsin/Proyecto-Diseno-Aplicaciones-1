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

    [TestMethod]
    public void Simulate_GroupStageWithSameSeed_ShouldReturnSameResult()
    {
        var engine = new PureRandomSimulationEngine();
        var home = CreateTeam("HOME", 2500);
        var away = CreateTeam("AWAY", 300);

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

    [TestMethod]
    public void Simulate_GroupStage_ShouldIgnoreRanking()
    {
        var engine = new PureRandomSimulationEngine();

        var strongHome = CreateTeam("STRONG_HOME", 2500);
        var weakAway = CreateTeam("WEAK_AWAY", 300);

        var weakHome = CreateTeam("WEAK_HOME", 300);
        var strongAway = CreateTeam("STRONG_AWAY", 2500);

        var firstResult = engine.Simulate(
            strongHome,
            weakAway,
            TournamentPhase.GroupStage,
            new Random(Seed));

        var secondResult = engine.Simulate(
            weakHome,
            strongAway,
            TournamentPhase.GroupStage,
            new Random(Seed));

        Assert.AreEqual(firstResult, secondResult);
    }

    [TestMethod]
    public void Simulate_Knockout_ShouldNeverReturnDraw()
    {
        var engine = new PureRandomSimulationEngine();
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
    public void Simulate_Knockout_ShouldReturnGoalsWithinValidRange()
    {
        var engine = new PureRandomSimulationEngine();
        var home = CreateTeam("HOME", 2500);
        var away = CreateTeam("AWAY", 300);

        var result = engine.Simulate(
            home,
            away,
            TournamentPhase.RoundOf16,
            new Random(Seed));

        Assert.IsTrue(result.HomeGoals is >= 0 and <= 3);
        Assert.IsTrue(result.AwayGoals is >= 0 and <= 3);
    }
}