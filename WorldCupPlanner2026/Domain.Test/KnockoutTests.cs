namespace Domain.Test;

[TestClass]
public class KnockoutTests
{
    private const int SimulationSeed = 12345;
    private const int SemillaCrucesFase = 98765;
    private static readonly DateTime ValidMatchDate = new(2026, 7, 1);

    private static Knockout CreateValidKnockout(
        int simulationSeed = SimulationSeed,
        int semillaCrucesFase = SemillaCrucesFase) =>
        new(simulationSeed, semillaCrucesFase);

    [TestMethod]
    public void GivenSimulationSeed_WhenCreateKnockout_ThenSimulationSeedIsAssigned()
    {
        var knockout = CreateValidKnockout();

        Assert.AreEqual(SimulationSeed, knockout.SimulationSeed);
    }

    [TestMethod]
    public void GivenSemillaCrucesFase_WhenCreateKnockout_ThenSemillaCrucesFaseIsAssigned()
    {
        var knockout = CreateValidKnockout();

        Assert.AreEqual(SemillaCrucesFase, knockout.SemillaCrucesFase);
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
        const int newSeed = 999;

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
        var qualifiedTeams = CreateOrderedQualifiedTeams();

        knockout.SetQualifiedTeams(qualifiedTeams);

        CollectionAssert.AreEqual(qualifiedTeams, knockout.QualifiedTeams.ToList());
    }

    [TestMethod]
    public void GivenOrderedQualifiedTeams_WhenInitializeRoundOf32_ThenCreatesMatchesWithExpectedCodesAndPotSizes()
    {
        var knockout = CreateValidKnockout();
        var qualifiedTeams = CreateOrderedQualifiedTeams();
        knockout.SetQualifiedTeams(qualifiedTeams);

        knockout.InitializeRoundOf32();

        Assert.AreEqual(16, knockout.Matches.Count);
        CollectionAssert.AreEquivalent(ExpectedRoundOf32Codes(), knockout.Matches.Select(match => match.MatchCode).ToList());

        AssertMatchesUseExpectedPots(
            knockout.Matches.Where(match => match.MatchCode!.StartsWith("A")).ToList(),
            qualifiedTeams.Skip(4).Take(8),
            qualifiedTeams.Skip(24).Take(8));

        AssertMatchesUseExpectedPots(
            knockout.Matches.Where(match => IsMatchCodeBetween(match.MatchCode, "B1", "B4")).ToList(),
            qualifiedTeams.Take(4),
            qualifiedTeams.Skip(20).Take(4));

        AssertMatchesUseExpectedPotSplit(
            knockout.Matches.Where(match => IsMatchCodeBetween(match.MatchCode, "B5", "B8")).ToList(),
            qualifiedTeams.Skip(12).Take(8));
    }

    [TestMethod]
    public void GivenBestCaseGroupDistribution_WhenInitializeRoundOf32_ThenAvoidsPairingTeamsFromSameGroup()
    {
        var knockout = CreateValidKnockout();
        knockout.SetQualifiedTeams(CreateOrderedQualifiedTeams());

        knockout.InitializeRoundOf32();

        Assert.IsTrue(knockout.Matches.All(match => match.HomeTeam.GroupLabel != match.AwayTeam.GroupLabel));
    }

    [TestMethod]
    public void GivenSameSemillaCrucesFase_WhenInitializeRoundOf32_ThenFixtureIsDeterministic()
    {
        var firstKnockout = CreateValidKnockout(simulationSeed: 111, semillaCrucesFase: 777);
        var secondKnockout = CreateValidKnockout(simulationSeed: 999, semillaCrucesFase: 777);
        firstKnockout.SetQualifiedTeams(CreateOrderedQualifiedTeams());
        secondKnockout.SetQualifiedTeams(CreateOrderedQualifiedTeams());

        firstKnockout.InitializeRoundOf32();
        secondKnockout.InitializeRoundOf32();

        CollectionAssert.AreEqual(
            BuildFixtureSignature(firstKnockout.Matches),
            BuildFixtureSignature(secondKnockout.Matches));
    }

    [TestMethod]
    public void GivenDifferentSemillaCrucesFase_WhenInitializeRoundOf32_ThenFixtureCanChange()
    {
        var firstKnockout = CreateValidKnockout(semillaCrucesFase: 101);
        var secondKnockout = CreateValidKnockout(semillaCrucesFase: 202);
        firstKnockout.SetQualifiedTeams(CreateOrderedQualifiedTeams());
        secondKnockout.SetQualifiedTeams(CreateOrderedQualifiedTeams());

        firstKnockout.InitializeRoundOf32();
        secondKnockout.InitializeRoundOf32();

        CollectionAssert.AreNotEqual(
            BuildFixtureSignature(firstKnockout.Matches),
            BuildFixtureSignature(secondKnockout.Matches));
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
        knockout.SetQualifiedTeams(CreateOrderedQualifiedTeams());
        knockout.InitializeRoundOf32();
        var roundOf16Match = CreateKnockoutMatch("BRAZIL", "FRANCE", TournamentPhase.RoundOf16);

        Assert.ThrowsException<InvalidOperationException>(() => knockout.AddMatch(roundOf16Match));
    }

    private static List<Team> CreateOrderedQualifiedTeams()
    {
        var teams = new List<Team>();

        teams.AddRange(CreateTeams("FIRST-BEST", 1, 4, ["A", "B", "C", "D"]));
        teams.AddRange(CreateTeams("FIRST-WORST", 1, 8, ["E", "F", "G", "H", "I", "J", "K", "L"]));
        teams.AddRange(CreateTeams("SECOND-BEST", 1, 8, ["A", "B", "C", "D", "E", "F", "G", "H"]));
        teams.AddRange(CreateTeams("SECOND-WORST", 1, 4, ["I", "J", "K", "L"]));
        teams.AddRange(CreateTeams("THIRD", 1, 8, ["A", "B", "C", "D", "E", "F", "G", "H"]));

        return teams;
    }

    private static List<Team> CreateQualifiedTeams(int count) =>
        Enumerable.Range(1, count)
            .Select(number => BuildTeam(CreateTeamRequest(
                $"TEAM-{number:00}",
                ConfederationFor(number),
                rankingFifa: 1000 + number,
                groupLabel: ((char)('A' + (number - 1) % 12)).ToString())))
            .ToList();

    private static IEnumerable<Team> CreateTeams(string prefix, int startNumber, int count, string[] groupLabels) =>
        Enumerable.Range(startNumber, count)
            .Select((number, index) => BuildTeam(CreateTeamRequest(
                $"{prefix}-{number:00}",
                ConfederationFor(number + prefix.Length),
                rankingFifa: 1000 + number,
                groupLabel: groupLabels[index])));

    private static TeamRequest CreateTeamRequest(
        string name,
        string confederation,
        int rankingFifa = 1500,
        string groupLabel = "A") => new()
    {
        Name = name,
        ConfederationName = confederation,
        RankingFifa = rankingFifa,
        GroupLabel = groupLabel
    };

    private static Team BuildTeam(TeamRequest request) =>
        new(request.Name, request.ConfederationName, request.RankingFifa, request.GroupLabel);

    private static Match CreateKnockoutMatch(
        string homeTeamName,
        string awayTeamName,
        TournamentPhase phase) =>
        new(
            ValidMatchDate,
            new Stadium("CENTENARIO", "MONTEVIDEO", "Historic stadium", 60000),
            BuildTeam(CreateTeamRequest(homeTeamName, "CONMEBOL", groupLabel: "A")),
            BuildTeam(CreateTeamRequest(awayTeamName, "UEFA", groupLabel: "B")),
            group: null,
            phase: phase,
            matchCode: "A1");

    private static void AssertMatchesUseExpectedPots(
        IReadOnlyCollection<Match> matches,
        IEnumerable<Team> expectedHomePot,
        IEnumerable<Team> expectedAwayPot)
    {
        var homeNames = matches.Select(match => match.HomeTeam.Name).ToList();
        var awayNames = matches.Select(match => match.AwayTeam.Name).ToList();

        CollectionAssert.AreEquivalent(expectedHomePot.Select(team => team.Name).ToList(), homeNames);
        CollectionAssert.AreEquivalent(expectedAwayPot.Select(team => team.Name).ToList(), awayNames);
    }

    private static void AssertMatchesUseExpectedPotSplit(
        IReadOnlyCollection<Match> matches,
        IEnumerable<Team> expectedPot)
    {
        var matchedTeamNames = matches
            .SelectMany(match => new[] { match.HomeTeam.Name, match.AwayTeam.Name })
            .ToList();

        CollectionAssert.AreEquivalent(expectedPot.Select(team => team.Name).ToList(), matchedTeamNames);
    }

    private static List<string> ExpectedRoundOf32Codes() =>
    [
        "A1", "A2", "A3", "A4", "A5", "A6", "A7", "A8",
        "B1", "B2", "B3", "B4", "B5", "B6", "B7", "B8"
    ];

    private static bool IsMatchCodeBetween(string? matchCode, string from, string to) =>
        matchCode is not null
        && string.Compare(matchCode, from, StringComparison.Ordinal) >= 0
        && string.Compare(matchCode, to, StringComparison.Ordinal) <= 0;

    private static List<string> BuildFixtureSignature(IEnumerable<Match> matches) =>
        matches
            .OrderBy(match => match.MatchCode)
            .Select(match => $"{match.MatchCode}:{match.HomeTeam.Name}-{match.AwayTeam.Name}")
            .ToList();

    private static string ConfederationFor(int seed)
    {
        string[] confederations = ["AFC", "CAF", "CONCACAF", "CONMEBOL", "OFC", "UEFA"];
        return confederations[Math.Abs(seed) % confederations.Length];
    }

    private sealed class TeamRequest
    {
        public string Name { get; init; } = "";
        public string ConfederationName { get; init; } = "";
        public int RankingFifa { get; init; }
        public string GroupLabel { get; init; } = "";
    }
}
