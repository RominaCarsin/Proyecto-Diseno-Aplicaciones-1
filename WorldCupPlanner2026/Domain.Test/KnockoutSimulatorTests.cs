namespace Domain.Test;

[TestClass]
public class KnockoutSimulatorTests
{
    private const int Seed = 42;
    private static readonly DateTime MatchDate = new(2026, 7, 1);

    private static Team CreateTeam(string name, int rankingFifa) =>
        new(name, "CONMEBOL", rankingFifa, "A");

    private static Stadium CreateStadium() =>
        new("CENTENARIO", "MONTEVIDEO", "Historic stadium", 60000);

    private static Match CreateMatch(Team home, Team away, string matchCode = "C1") =>
        new(MatchDate, CreateStadium(), home, away, group: null, phase: TournamentPhase.RoundOf16, matchCode: matchCode);

    [TestMethod]
    public void Simulate_WithNoMatches_ShouldNotThrow()
    {
        new KnockoutSimulator(Seed).Simulate(Enumerable.Empty<Match>());
    }

    [TestMethod]
    public void Simulate_OnUnfinishedMatch_ShouldSetResult()
    {
        var match = CreateMatch(CreateTeam("HOME", 1500), CreateTeam("AWAY", 1700));

        new KnockoutSimulator(Seed).Simulate(new[] { match });

        Assert.IsNotNull(match.HomeGoals);
        Assert.IsNotNull(match.AwayGoals);
    }

    [TestMethod]
    public void Simulate_OnUnfinishedMatch_ShouldKeepMatchEditable()
    {
        var match = CreateMatch(CreateTeam("HOME", 1500), CreateTeam("AWAY", 1700));

        new KnockoutSimulator(Seed).Simulate(new[] { match });

        Assert.AreEqual(MatchStatus.NotFinished, match.Status);
        Assert.IsFalse(match.IsFinished);
    }

    [TestMethod]
    public void Simulate_ShouldNeverProduceADraw()
    {
        var match = CreateMatch(CreateTeam("HOME", 1500), CreateTeam("AWAY", 1700));

        new KnockoutSimulator(Seed).Simulate(new[] { match });

        Assert.AreNotEqual(match.HomeGoals, match.AwayGoals);
        Assert.IsNotNull(match.Winner);
    }

    [TestMethod]
    public void Simulate_WithEqualRankings_ShouldStillProduceAWinner()
    {
        // A diferencia del simulador de grupos, acá no hay rama de empate por ranking igual:
        // los rangos de goles (2-3 / 0-1) son siempre disjuntos.
        var match = CreateMatch(CreateTeam("HOME", 1500), CreateTeam("AWAY", 1500));

        new KnockoutSimulator(Seed).Simulate(new[] { match });

        Assert.AreNotEqual(match.HomeGoals, match.AwayGoals);
        Assert.IsNotNull(match.Winner);
    }

    [TestMethod]
    public void Simulate_WhenMatchAlreadyFinished_ShouldNotThrow()
    {
        var match = CreateMatch(CreateTeam("HOME", 1500), CreateTeam("AWAY", 1700));
        match.SetResult(2, 1);
        match.Finish();

        new KnockoutSimulator(Seed).Simulate(new[] { match });
    }

    [TestMethod]
    public void Simulate_WhenMatchAlreadyFinished_ShouldNotOverwriteResult()
    {
        var match = CreateMatch(CreateTeam("HOME", 1500), CreateTeam("AWAY", 1700));
        match.SetResult(2, 1);
        match.Finish();

        new KnockoutSimulator(Seed).Simulate(new[] { match });

        Assert.AreEqual(2, match.HomeGoals);
        Assert.AreEqual(1, match.AwayGoals);
    }

    [TestMethod]
    public void Simulate_WithMixOfFinishedAndUnfinishedMatches_ShouldOnlySimulateUnfinishedAndKeepItEditable()
    {
        var finishedMatch = CreateMatch(CreateTeam("HOME1", 1500), CreateTeam("AWAY1", 1700), "C1");
        finishedMatch.SetResult(3, 0);
        finishedMatch.Finish();

        var pendingMatch = CreateMatch(CreateTeam("HOME2", 1800), CreateTeam("AWAY2", 1400), "C2");

        new KnockoutSimulator(Seed).Simulate(new[] { finishedMatch, pendingMatch });

        Assert.AreEqual(3, finishedMatch.HomeGoals);
        Assert.AreEqual(0, finishedMatch.AwayGoals);
        Assert.AreEqual(MatchStatus.Finished, finishedMatch.Status);

        Assert.IsNotNull(pendingMatch.HomeGoals);
        Assert.IsNotNull(pendingMatch.AwayGoals);
        Assert.AreEqual(MatchStatus.NotFinished, pendingMatch.Status);
        Assert.IsFalse(pendingMatch.IsFinished);
    }

    [TestMethod]
    public void Simulate_WithMultipleUnfinishedMatches_ShouldSetResultForAllAndKeepThemEditable()
    {
        var match1 = CreateMatch(CreateTeam("HOME1", 1500), CreateTeam("AWAY1", 1700), "C1");
        var match2 = CreateMatch(CreateTeam("HOME2", 1800), CreateTeam("AWAY2", 1400), "C2");

        new KnockoutSimulator(Seed).Simulate(new[] { match1, match2 });

        Assert.IsTrue(match1.HasResult);
        Assert.IsTrue(match2.HasResult);

        Assert.AreEqual(MatchStatus.NotFinished, match1.Status);
        Assert.AreEqual(MatchStatus.NotFinished, match2.Status);

        Assert.IsFalse(match1.IsFinished);
        Assert.IsFalse(match2.IsFinished);
    }

    [TestMethod]
    public void Simulate_WithSameSeed_ShouldProduceDeterministicResults()
    {
        var match1 = CreateMatch(CreateTeam("HOME", 1500), CreateTeam("AWAY", 1700), "C1");
        var match2 = CreateMatch(CreateTeam("HOME", 1500), CreateTeam("AWAY", 1700), "C1");

        new KnockoutSimulator(Seed).Simulate(new[] { match1 });
        new KnockoutSimulator(Seed).Simulate(new[] { match2 });

        Assert.AreEqual(match1.HomeGoals, match2.HomeGoals);
        Assert.AreEqual(match1.AwayGoals, match2.AwayGoals);
    }

    [TestMethod]
    public void Simulate_WithDifferentSeed_ShouldProduceDifferentResults()
    {
        var matches1 = Enumerable.Range(1, 8)
            .Select(i => CreateMatch(CreateTeam($"HOME{i}", 1500), CreateTeam($"AWAY{i}", 1700), $"C{i}"))
            .ToList();
        var matches2 = Enumerable.Range(1, 8)
            .Select(i => CreateMatch(CreateTeam($"HOME{i}", 1500), CreateTeam($"AWAY{i}", 1700), $"C{i}"))
            .ToList();

        new KnockoutSimulator(1).Simulate(matches1);
        new KnockoutSimulator(2).Simulate(matches2);

        var results1 = matches1.Select(m => $"{m.HomeGoals}-{m.AwayGoals}").ToList();
        var results2 = matches2.Select(m => $"{m.HomeGoals}-{m.AwayGoals}").ToList();

        CollectionAssert.AreNotEqual(results1, results2,
            "Distinta semilla debe producir resultados diferentes (altamente probable con 8 partidos).");
    }

    [TestMethod]
    public void Simulate_GoalsShouldBeWithinValidRange()
    {
        var match = CreateMatch(CreateTeam("HOME", 1500), CreateTeam("AWAY", 1700));

        new KnockoutSimulator(Seed).Simulate(new[] { match });

        Assert.IsTrue(match.HomeGoals is >= 0 and <= 3);
        Assert.IsTrue(match.AwayGoals is >= 0 and <= 3);
    }
    
    [TestMethod]
    public void Simulate_WhenMatchHasResultButIsNotFinished_ShouldAllowResultUpdate()
    {
        var match = CreateMatch(CreateTeam("HOME", 1500), CreateTeam("AWAY", 1700));
        match.SetResult(3, 0);

        new KnockoutSimulator(Seed).Simulate(new[] { match });

        Assert.IsTrue(match.HasResult);
        Assert.AreEqual(MatchStatus.NotFinished, match.Status);
        Assert.IsFalse(match.IsFinished);
        Assert.IsNotNull(match.Winner);
    }
    
    [TestMethod]
    public void Simulate_WithCustomEngine_ShouldUseEngineResult()
    {
        var match = CreateMatch(CreateTeam("HOME", 1500), CreateTeam("AWAY", 1700));
        var engine = new FixedSimulationEngine(homeGoals: 2, awayGoals: 1);

        new KnockoutSimulator(Seed, engine).Simulate(new[] { match });

        Assert.AreEqual(2, match.HomeGoals);
        Assert.AreEqual(1, match.AwayGoals);
        Assert.AreEqual(MatchStatus.NotFinished, match.Status);
    }
    
    private sealed class FixedSimulationEngine : ISimulationEngine
    {
        private readonly int _homeGoals;
        private readonly int _awayGoals;

        public FixedSimulationEngine(int homeGoals, int awayGoals)
        {
            _homeGoals = homeGoals;
            _awayGoals = awayGoals;
        }

        public string Name => "Fixed";

        public SimulationResult Simulate(
            Team homeTeam,
            Team awayTeam,
            TournamentPhase phase,
            Random random)
        {
            return new SimulationResult(_homeGoals, _awayGoals);
        }
    }
}