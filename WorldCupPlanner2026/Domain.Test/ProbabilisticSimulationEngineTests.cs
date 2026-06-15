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

    [TestMethod]
    public void Simulate_GroupStage_ShouldReturnYellowCardsWithinValidRange()
    {
        var engine = new ProbabilisticSimulationEngine();
        var home = CreateTeam("HOME", 1500);
        var away = CreateTeam("AWAY", 1700);
        var random = new Random(Seed);

        var result = engine.Simulate(
            home,
            away,
            TournamentPhase.GroupStage,
            random);

        Assert.IsTrue(result.HomeYellowCards >= 0 && result.HomeYellowCards <= 5,
            $"HomeYellowCards {result.HomeYellowCards} is out of range [0, 5]");
        Assert.IsTrue(result.AwayYellowCards >= 0 && result.AwayYellowCards <= 5,
            $"AwayYellowCards {result.AwayYellowCards} is out of range [0, 5]");
    }

    [TestMethod]
    public void Simulate_GroupStage_ShouldReturnRedCardsWithinValidRange()
    {
        var engine = new ProbabilisticSimulationEngine();
        var home = CreateTeam("HOME", 1500);
        var away = CreateTeam("AWAY", 1700);
        var random = new Random(Seed);

        var result = engine.Simulate(
            home,
            away,
            TournamentPhase.GroupStage,
            random);

        Assert.IsTrue(result.HomeRedCards >= 0 && result.HomeRedCards <= 1,
            $"HomeRedCards {result.HomeRedCards} is out of range [0, 1]");
        Assert.IsTrue(result.AwayRedCards >= 0 && result.AwayRedCards <= 1,
            $"AwayRedCards {result.AwayRedCards} is out of range [0, 1]");
    }

    [TestMethod]
    public void Simulate_Knockout_ShouldReturnYellowCardsWithinValidRange()
    {
        var engine = new ProbabilisticSimulationEngine();
        var home = CreateTeam("HOME", 1500);
        var away = CreateTeam("AWAY", 1700);
        var random = new Random(Seed);

        var result = engine.Simulate(
            home,
            away,
            TournamentPhase.RoundOf16,
            random);

        Assert.IsTrue(result.HomeYellowCards >= 0 && result.HomeYellowCards <= 5);
        Assert.IsTrue(result.AwayYellowCards >= 0 && result.AwayYellowCards <= 5);
    }

    [TestMethod]
    public void Simulate_Knockout_ShouldReturnRedCardsWithinValidRange()
    {
        var engine = new ProbabilisticSimulationEngine();
        var home = CreateTeam("HOME", 1500);
        var away = CreateTeam("AWAY", 1700);
        var random = new Random(Seed);

        var result = engine.Simulate(
            home,
            away,
            TournamentPhase.RoundOf16,
            random);

        Assert.IsTrue(result.HomeRedCards >= 0 && result.HomeRedCards <= 1);
        Assert.IsTrue(result.AwayRedCards >= 0 && result.AwayRedCards <= 1);
    }

    [TestMethod]
    public void Simulate_WithSameSeed_ShouldReturnSameResultIncludingCards()
    {
        var engine = new ProbabilisticSimulationEngine();
        var home = CreateTeam("HOME", 1800);
        var away = CreateTeam("AWAY", 1200);

        var result1 = engine.Simulate(
            home,
            away,
            TournamentPhase.GroupStage,
            new Random(Seed));

        var result2 = engine.Simulate(
            home,
            away,
            TournamentPhase.GroupStage,
            new Random(Seed));

        Assert.AreEqual(result1.HomeYellowCards, result2.HomeYellowCards);
        Assert.AreEqual(result1.AwayYellowCards, result2.AwayYellowCards);
        Assert.AreEqual(result1.HomeRedCards, result2.HomeRedCards);
        Assert.AreEqual(result1.AwayRedCards, result2.AwayRedCards);
    }
}
