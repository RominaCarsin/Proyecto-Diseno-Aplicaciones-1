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

    [TestMethod]
    public void Simulate_GroupStage_ShouldReturnYellowCardsWithinValidRange()
    {
        var engine = new PureRandomSimulationEngine();
        var home = CreateTeam("HOME", 2500);
        var away = CreateTeam("AWAY", 300);

        var result = engine.Simulate(
            home,
            away,
            TournamentPhase.GroupStage,
            new Random(Seed));

        Assert.IsTrue(result.HomeYellowCards >= 0 && result.HomeYellowCards <= 5,
            $"HomeYellowCards {result.HomeYellowCards} is out of range [0, 5]");
        Assert.IsTrue(result.AwayYellowCards >= 0 && result.AwayYellowCards <= 5,
            $"AwayYellowCards {result.AwayYellowCards} is out of range [0, 5]");
    }

    [TestMethod]
    public void Simulate_GroupStage_ShouldReturnRedCardsWithinValidRange()
    {
        var engine = new PureRandomSimulationEngine();
        var home = CreateTeam("HOME", 2500);
        var away = CreateTeam("AWAY", 300);

        var result = engine.Simulate(
            home,
            away,
            TournamentPhase.GroupStage,
            new Random(Seed));

        Assert.IsTrue(result.HomeRedCards >= 0 && result.HomeRedCards <= 1,
            $"HomeRedCards {result.HomeRedCards} is out of range [0, 1]");
        Assert.IsTrue(result.AwayRedCards >= 0 && result.AwayRedCards <= 1,
            $"AwayRedCards {result.AwayRedCards} is out of range [0, 1]");
    }

    [TestMethod]
    public void Simulate_Knockout_ShouldReturnYellowCardsWithinValidRange()
    {
        var engine = new PureRandomSimulationEngine();
        var home = CreateTeam("HOME", 2500);
        var away = CreateTeam("AWAY", 300);

        var result = engine.Simulate(
            home,
            away,
            TournamentPhase.RoundOf16,
            new Random(Seed));

        Assert.IsTrue(result.HomeYellowCards >= 0 && result.HomeYellowCards <= 5);
        Assert.IsTrue(result.AwayYellowCards >= 0 && result.AwayYellowCards <= 5);
    }

    [TestMethod]
    public void Simulate_Knockout_ShouldReturnRedCardsWithinValidRange()
    {
        var engine = new PureRandomSimulationEngine();
        var home = CreateTeam("HOME", 2500);
        var away = CreateTeam("AWAY", 300);

        var result = engine.Simulate(
            home,
            away,
            TournamentPhase.RoundOf16,
            new Random(Seed));

        Assert.IsTrue(result.HomeRedCards >= 0 && result.HomeRedCards <= 1);
        Assert.IsTrue(result.AwayRedCards >= 0 && result.AwayRedCards <= 1);
    }

    [TestMethod]
    public void Simulate_GroupStageWithSameSeed_ShouldReturnSameResultIncludingCards()
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

        Assert.AreEqual(firstResult.HomeYellowCards, secondResult.HomeYellowCards);
        Assert.AreEqual(firstResult.AwayYellowCards, secondResult.AwayYellowCards);
        Assert.AreEqual(firstResult.HomeRedCards, secondResult.HomeRedCards);
        Assert.AreEqual(firstResult.AwayRedCards, secondResult.AwayRedCards);
    }

    [TestMethod]
    public void Simulate_Knockout_WithSeed42_ShouldHaveAwayWin()
    {
        var engine = new PureRandomSimulationEngine();
        var home = CreateTeam("HOME", 1500);
        var away = CreateTeam("AWAY", 1500);

        var result = engine.Simulate(home, away, TournamentPhase.RoundOf16, new Random(42));

        Assert.IsTrue(result.AwayGoals >= 2, "Expected away team to win with seed 42");
    }

    [TestMethod]
    public void Simulate_Knockout_WithSeed1_ShouldHaveHomeWin()
    {
        var engine = new PureRandomSimulationEngine();
        var home = CreateTeam("HOME", 1500);
        var away = CreateTeam("AWAY", 1500);

        var result = engine.Simulate(home, away, TournamentPhase.RoundOf16, new Random(1));

        Assert.IsTrue(result.HomeGoals >= 2, "Expected home team to win with seed 1");
    }
}