namespace Domain.Test;

[TestClass]
public class KnockoutTests
{
    private const int simulationSeed = 12345;
    private static readonly DateTime ValidMatchDate = new(2026, 7, 1);

    private Knockout CreateValidKnockout(int seed = simulationSeed) => new(seed);

    [TestMethod]
    public void GivenSimulationSeed_WhenCreateKnockout_ThenSimulationSeedIsAssigned()
    {
        var knockout = CreateValidKnockout();

        Assert.AreEqual(simulationSeed, knockout.SimulationSeed);
    }

    [TestMethod]
    public void GivenValidSeed_WhenCreateKnockout_ThenMatchesStartsEmpty()
    {
        var knockout = CreateValidKnockout();

        Assert.IsNotNull(knockout);
        Assert.AreEqual(0, knockout.Matches.Count);
    }

    [TestMethod]
    public void GivenNewSimulationSeed_WhenUpdateSimulationSeed_ThenSeedIsReplaced()
    {
        var knockout = CreateValidKnockout();
        int newSeed = 999;

        knockout.UpdateSimulationSeed(newSeed);

        Assert.AreEqual(newSeed, knockout.SimulationSeed);
    }

    [TestMethod]
    [DataRow(31)]
    [DataRow(33)]
    public void GivenQualifiedTeamListWithoutExactlyThirtyTwoTeams_WhenSetQualifiedTeams_ThenThrowsArgumentException(int teamCount)
    {
        var knockout = CreateValidKnockout();
        var qualifiedTeams = CreateQualifiedTeams(teamCount);

        Assert.ThrowsException<ArgumentException>(() => knockout.SetQualifiedTeams(qualifiedTeams));
    }

    [TestMethod]
    public void GivenThirtyTwoQualifiedTeams_WhenSetQualifiedTeams_ThenQualifiedTeamsAreAssigned()
    {
        var knockout = CreateValidKnockout();
        var qualifiedTeams = CreateQualifiedTeams(32);

        knockout.SetQualifiedTeams(qualifiedTeams);

        CollectionAssert.AreEqual(qualifiedTeams, knockout.QualifiedTeams.ToList());
    }

    [TestMethod]
    public void GivenThirtyTwoQualifiedTeams_WhenInitializeRoundOf32_ThenCreatesSixteenMatches()
    {
        var knockout = CreateValidKnockout();
        knockout.SetQualifiedTeams(CreateQualifiedTeams(32));

        knockout.InitializeRoundOf32();

        Assert.AreEqual(16, knockout.Matches.Count);
        Assert.AreEqual(32, knockout.Matches.SelectMany(m => new[] { m.HomeTeam, m.AwayTeam }).Distinct().Count());
    }

    [TestMethod]
    public void GivenThirtyTwoQualifiedTeams_WhenInitializeRoundOf32_ThenAllMatchesHaveInitialStateAndRoundOf32Phase()
    {
        var knockout = CreateValidKnockout();
        knockout.SetQualifiedTeams(CreateQualifiedTeams(32));

        knockout.InitializeRoundOf32();

        Assert.IsTrue(knockout.Matches.All(m => m.Phase == TournamentPhase.RoundOf32));
        Assert.IsTrue(knockout.Matches.All(m => m.Status == MatchStatus.NotFinished));
        Assert.IsTrue(knockout.Matches.All(m => m.Group is null));
        Assert.IsTrue(knockout.Matches.All(m => !m.HasResult));
        Assert.IsTrue(knockout.Matches.All(m => !m.IsLocked));
    }

    [TestMethod]
    public void GivenMatch_WhenAddMatch_ThenMatchIsAddedToMatches()
    {
        var knockout = CreateValidKnockout();
        var match = CreateKnockoutMatch("URUGUAY", "ARGENTINA", TournamentPhase.RoundOf32);

        knockout.AddMatch(match);

        Assert.AreEqual(1, knockout.Matches.Count);
        Assert.AreSame(match, knockout.Matches.Single());
    }

    [TestMethod]
    public void GivenUnfinishedRoundOf32Matches_WhenAddRoundOf16Match_ThenThrowsInvalidOperationException()
    {
        var knockout = CreateValidKnockout();
        knockout.SetQualifiedTeams(CreateQualifiedTeams(32));
        knockout.InitializeRoundOf32();
        var roundOf16Match = CreateKnockoutMatch("BRAZIL", "FRANCE", TournamentPhase.RoundOf16);

        Assert.ThrowsException<InvalidOperationException>(() => knockout.AddMatch(roundOf16Match));
    }

    [TestMethod]
    public void GivenSameSeedAndQualifiedTeams_WhenInitializeRoundOf32_ThenFixtureIsDeterministic()
    {
        var firstKnockout = CreateValidKnockout(seed: 777);
        var secondKnockout = CreateValidKnockout(seed: 777);
        firstKnockout.SetQualifiedTeams(CreateQualifiedTeams(32));
        secondKnockout.SetQualifiedTeams(CreateQualifiedTeams(32));

        firstKnockout.InitializeRoundOf32();
        secondKnockout.InitializeRoundOf32();

        CollectionAssert.AreEqual(
            BuildFixtureSignature(firstKnockout.Matches),
            BuildFixtureSignature(secondKnockout.Matches));
    }

    [TestMethod]
    [Ignore("Falta definir en el dominio el contrato para grupo de origen y posicion en grupo de cada clasificado.")]
    public void GivenQualifiedTeamsFromSamePreviousGroup_WhenInitializeRoundOf32_ThenNoMatchPairsTeamsFromSameGroup()
    {
        Assert.Inconclusive(
            "Necesito saber como se representa el grupo de origen y la posicion final del equipo para validar esta restriccion sin asumir nombres de propiedades.");
    }

    private static List<Team> CreateQualifiedTeams(int count) =>
        Enumerable.Range(1, count)
            .Select(i => BuildTeam(CreateTeamRequest($"TEAM {i:00}", ConfederationFor(i), rankingFifa: 1000 + i)))
            .ToList();

    private static TeamRequest CreateTeamRequest(string name, string confederation) => new()
    {
        Name = name,
        ConfederationName = confederation,
        RankingFifa = 1500
    };

    private static TeamRequest CreateTeamRequest(string name, string confederation, int rankingFifa) => new()
    {
        Name = name,
        ConfederationName = confederation,
        RankingFifa = rankingFifa
    };

    private static Team BuildTeam(TeamRequest request) =>
        new(request.Name, request.ConfederationName, request.RankingFifa);

    private static Match CreateKnockoutMatch(
        string homeTeamName,
        string awayTeamName,
        TournamentPhase phase) =>
        new(
            ValidMatchDate,
            new Stadium("CENTENARIO", "MONTEVIDEO", "Historic stadium", 60000),
            BuildTeam(CreateTeamRequest(homeTeamName, "CONMEBOL")),
            BuildTeam(CreateTeamRequest(awayTeamName, "UEFA")),
            group: null,
            phase: phase);

    private static List<string> BuildFixtureSignature(IEnumerable<Match> matches) =>
        matches
            .Select(match => $"{match.Phase}:{match.HomeTeam.Name}-{match.AwayTeam.Name}")
            .ToList();

    private static string ConfederationFor(int teamNumber)
    {
        string[] confederations = ["AFC", "CAF", "CONCACAF", "CONMEBOL", "OFC", "UEFA"];
        return confederations[(teamNumber - 1) % confederations.Length];
    }

    private sealed class TeamRequest
    {
        public string Name { get; init; } = "";
        public string ConfederationName { get; init; } = "";
        public int RankingFifa { get; init; }
    }
}
