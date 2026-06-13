namespace Domain.Test;

[TestClass]
public class KnockoutTests
{
    private const int SimulationSeed = 12345;
    private const int SemillaCrucesFase = 98765;
    private static readonly DateTime ValidMatchDate = new(2026, 7, 1);

    private static Knockout CreateValidKnockout(
        int simulationSeed = SimulationSeed,
        int semillaCrucesFase = SemillaCrucesFase)
    {
        var shuffleService = new TestShuffleService(semillaCrucesFase);
        return new Knockout(simulationSeed, shuffleService, semillaCrucesFase);
    }

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
    public void GivenNullQualifiedTeams_WhenSetQualifiedTeams_ThenThrowsArgumentException()
    {
        var knockout = CreateValidKnockout();

        Assert.ThrowsException<ArgumentException>(() => knockout.SetQualifiedTeams(null!));
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
    public void GivenNoQualifiedTeamsSet_WhenInitializeRoundOf32_ThenThrowsInvalidOperationException()
    {
        var knockout = CreateValidKnockout();

        Assert.ThrowsException<InvalidOperationException>(() => knockout.InitializeRoundOf32());
    }

    [TestMethod]
    public void GivenAlreadyInitializedRoundOf32_WhenInitializeRoundOf32Again_ThenThrowsInvalidOperationException()
    {
        var knockout = CreateValidKnockout();
        knockout.SetQualifiedTeams(CreateOrderedQualifiedTeams());
        knockout.InitializeRoundOf32();

        Assert.ThrowsException<InvalidOperationException>(() => knockout.InitializeRoundOf32());
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
    public void GivenAllSecondBestTeamsFromSameGroup_WhenInitializeRoundOf32_ThenFallbackToFirstOpponent()
    {
        var knockout = CreateValidKnockout();
        var teams = CreateOrderedQualifiedTeams().ToList();
        for (int i = 12; i <= 19; i++)
        {
            var t = teams[i];
            teams[i] = new Team(t.Name, t.ConfederationName, t.RankingFifa, "A");
        }
        knockout.SetQualifiedTeams(teams);

        knockout.InitializeRoundOf32();

        var b5ToB8 = knockout.Matches
            .Where(m => IsMatchCodeBetween(m.MatchCode, "B5", "B8"))
            .ToList();
        Assert.AreEqual(4, b5ToB8.Count);
        Assert.IsTrue(b5ToB8.All(m => m.HomeTeam is not null && m.AwayTeam is not null));
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
    public void GivenNullMatch_WhenAddMatch_ThenThrowsArgumentException()
    {
        var knockout = CreateValidKnockout();

        Assert.ThrowsException<ArgumentException>(() => knockout.AddMatch(null!));
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
    
    // Round of 16 -------------
    
    [TestMethod]
    public void GivenFinishedRoundOf32_WhenInitializeRoundOf16_ThenCreatesMatchesWithExpectedCodesAndWinners()
    {
        var knockout = CreateKnockoutWithFinishedRoundOf32();
    
        knockout.InitializeRoundOf16();
    
        Assert.AreEqual(8, knockout.Matches.Count(m => m.Phase == TournamentPhase.RoundOf16));
    
        var roundOf16Matches = knockout.Matches
            .Where(m => m.Phase == TournamentPhase.RoundOf16)
            .ToList();
    
        CollectionAssert.AreEquivalent(
            new[] { "C1", "C2", "C3", "C4", "C5", "C6", "C7", "C8" }, 
            roundOf16Matches.Select(m => m.MatchCode).ToList());
    
        // Verifica emparejamiento: Ganador Ai vs Ganador Bi
        for (int i = 1; i <= 8; i++)
        {
            var matchC = roundOf16Matches.Single(m => m.MatchCode == $"C{i}");
            var matchA = knockout.Matches.Single(m => m.MatchCode == $"A{i}");
            var matchB = knockout.Matches.Single(m => m.MatchCode == $"B{i}");
        
            Assert.AreEqual(matchA.Winner, matchC.HomeTeam);
            Assert.AreEqual(matchB.Winner, matchC.AwayTeam);
        }
    }

    [TestMethod]
    public void GivenUnfinishedRoundOf32_WhenInitializeRoundOf16_ThenThrowsInvalidOperationException()
    {
        var knockout = CreateValidKnockout();
        var qualifiedTeams = CreateOrderedQualifiedTeams();
        knockout.SetQualifiedTeams(qualifiedTeams);
        knockout.InitializeRoundOf32();
        // No finalizamos los partidos
    
        Assert.ThrowsException<InvalidOperationException>(() => knockout.InitializeRoundOf16());
    }

    [TestMethod]
    public void GivenAlreadyInitializedRoundOf16_WhenInitializeRoundOf16Again_ThenThrowsInvalidOperationException()
    {
        var knockout = CreateKnockoutWithFinishedRoundOf32();
        knockout.InitializeRoundOf16();
    
        Assert.ThrowsException<InvalidOperationException>(() => knockout.InitializeRoundOf16());
    }
    
    //QuarterFinals--------------
    
    [TestMethod]
    public void GivenFinishedRoundOf16_WhenInitializeQuarterFinals_ThenCreatesMatchesWithExpectedCodesAndWinners()
    {
        var knockout = CreateKnockoutWithFinishedRoundOf16();
    
        knockout.InitializeQuarterFinals();
    
        Assert.AreEqual(4, knockout.Matches.Count(m => m.Phase == TournamentPhase.QuarterFinals));
    
        var quarterFinalMatches = knockout.Matches
            .Where(m => m.Phase == TournamentPhase.QuarterFinals)
            .ToList();
    
        CollectionAssert.AreEquivalent(
            new[] { "D1", "D2", "D3", "D4" }, 
            quarterFinalMatches.Select(m => m.MatchCode).ToList());
    
        // Verifica emparejamientos consecutivos: C1vsC2, C3vsC4, C5vsC6, C7vsC8
        var pairings = new[] { (1, 2), (3, 4), (5, 6), (7, 8) };
        for (int d = 0; d < pairings.Length; d++)
        {
            var (c1, c2) = pairings[d];
            var matchD = quarterFinalMatches.Single(m => m.MatchCode == $"D{d + 1}");
            var matchC1 = knockout.Matches.Single(m => m.MatchCode == $"C{c1}");
            var matchC2 = knockout.Matches.Single(m => m.MatchCode == $"C{c2}");
        
            Assert.AreEqual(matchC1.Winner, matchD.HomeTeam);
            Assert.AreEqual(matchC2.Winner, matchD.AwayTeam);
        }
    }

    [TestMethod]
    public void GivenUnfinishedRoundOf16_WhenInitializeQuarterFinals_ThenThrowsInvalidOperationException()
    {
        var knockout = CreateKnockoutWithFinishedRoundOf32();
        knockout.InitializeRoundOf16();
        // No finalizamos los partidos de octavos
    
        Assert.ThrowsException<InvalidOperationException>(() => knockout.InitializeQuarterFinals());
    }
    
    //Semifinals---------
    
    [TestMethod]
    public void GivenFinishedQuarterFinals_WhenInitializeSemiFinals_ThenCreatesMatchesWithExpectedCodesAndWinners()
    {
        var knockout = CreateKnockoutWithFinishedQuarterFinals();
    
        knockout.InitializeSemiFinals();
    
        Assert.AreEqual(2, knockout.Matches.Count(m => m.Phase == TournamentPhase.SemiFinals));
    
        var semiFinalMatches = knockout.Matches
            .Where(m => m.Phase == TournamentPhase.SemiFinals)
            .ToList();
    
        CollectionAssert.AreEquivalent(
            new[] { "S1", "S2" }, 
            semiFinalMatches.Select(m => m.MatchCode).ToList());
    
        // S1: Ganador D1 vs Ganador D2
        var matchS1 = semiFinalMatches.Single(m => m.MatchCode == "S1");
        var matchD1 = knockout.Matches.Single(m => m.MatchCode == "D1");
        var matchD2 = knockout.Matches.Single(m => m.MatchCode == "D2");
        Assert.AreEqual(matchD1.Winner, matchS1.HomeTeam);
        Assert.AreEqual(matchD2.Winner, matchS1.AwayTeam);
    
        // S2: Ganador D3 vs Ganador D4
        var matchS2 = semiFinalMatches.Single(m => m.MatchCode == "S2");
        var matchD3 = knockout.Matches.Single(m => m.MatchCode == "D3");
        var matchD4 = knockout.Matches.Single(m => m.MatchCode == "D4");
        Assert.AreEqual(matchD3.Winner, matchS2.HomeTeam);
        Assert.AreEqual(matchD4.Winner, matchS2.AwayTeam);
    }

    [TestMethod]
    public void GivenUnfinishedQuarterFinals_WhenInitializeSemiFinals_ThenThrowsInvalidOperationException()
    {
        var knockout = CreateKnockoutWithFinishedRoundOf16();
        knockout.InitializeQuarterFinals();
    
        Assert.ThrowsException<InvalidOperationException>(() => knockout.InitializeSemiFinals());
    }
    
    //ThirdPlace---------
    
    [TestMethod]
    public void GivenFinishedSemiFinals_WhenInitializeThirdPlace_ThenCreatesMatchWithLosers()
    {
        var knockout = CreateKnockoutWithFinishedSemiFinals();
    
        knockout.InitializeThirdPlace();
    
        Assert.AreEqual(1, knockout.Matches.Count(m => m.Phase == TournamentPhase.ThirdPlace));
    
        var thirdPlaceMatch = knockout.Matches.Single(m => m.Phase == TournamentPhase.ThirdPlace);
        Assert.AreEqual("T1", thirdPlaceMatch.MatchCode);
    
        var matchS1 = knockout.Matches.Single(m => m.MatchCode == "S1");
        var matchS2 = knockout.Matches.Single(m => m.MatchCode == "S2");
    
        Assert.AreEqual(matchS1.Loser, thirdPlaceMatch.HomeTeam);
        Assert.AreEqual(matchS2.Loser, thirdPlaceMatch.AwayTeam);
    }

    [TestMethod]
    public void GivenUnfinishedSemiFinals_WhenInitializeThirdPlace_ThenThrowsInvalidOperationException()
    {
        var knockout = CreateKnockoutWithFinishedQuarterFinals();
        knockout.InitializeSemiFinals();
    
        Assert.ThrowsException<InvalidOperationException>(() => knockout.InitializeThirdPlace());
    }
    
    //Final ----------------------
    
    [TestMethod]
    public void GivenFinishedSemiFinals_WhenInitializeFinal_ThenCreatesMatchWithWinners()
    {
        var knockout = CreateKnockoutWithFinishedSemiFinals();
    
        knockout.InitializeFinal();
    
        Assert.AreEqual(1, knockout.Matches.Count(m => m.Phase == TournamentPhase.Final));
    
        var finalMatch = knockout.Matches.Single(m => m.Phase == TournamentPhase.Final);
        Assert.AreEqual("F1", finalMatch.MatchCode);
    
        var matchS1 = knockout.Matches.Single(m => m.MatchCode == "S1");
        var matchS2 = knockout.Matches.Single(m => m.MatchCode == "S2");
    
        Assert.AreEqual(matchS1.Winner, finalMatch.HomeTeam);
        Assert.AreEqual(matchS2.Winner, finalMatch.AwayTeam);
    }

    [TestMethod]
    public void GivenUnfinishedSemiFinals_WhenInitializeFinal_ThenThrowsInvalidOperationException()
    {
        var knockout = CreateKnockoutWithFinishedQuarterFinals();
        knockout.InitializeSemiFinals();
    
        Assert.ThrowsException<InvalidOperationException>(() => knockout.InitializeFinal());
    }

    [TestMethod]
    public void GivenInitializedThirdPlace_WhenInitializeFinal_ThenDoesNotThrowException()
    {
        var knockout = CreateKnockoutWithFinishedSemiFinals();
        knockout.InitializeThirdPlace();
    
        // Final y tercer puesto pueden coexistir porque ambos dependen de semifinales
        knockout.InitializeFinal();
    }
    
    //Extra -----------------
    
    [TestMethod]
    public void GivenFinalAndThirdPlaceInitialized_WhenAllOtherPhasesExist_ThenBracketIsComplete()
    {
        var knockout = CreateKnockoutWithFinishedSemiFinals();
        knockout.InitializeThirdPlace();
        knockout.InitializeFinal();
    
        var phases = knockout.Matches.Select(m => m.Phase).Distinct().ToList();
    
        CollectionAssert.Contains(phases, TournamentPhase.RoundOf32);
        CollectionAssert.Contains(phases, TournamentPhase.RoundOf16);
        CollectionAssert.Contains(phases, TournamentPhase.QuarterFinals);
        CollectionAssert.Contains(phases, TournamentPhase.SemiFinals);
        CollectionAssert.Contains(phases, TournamentPhase.ThirdPlace);
        CollectionAssert.Contains(phases, TournamentPhase.Final);
        Assert.AreEqual(16 + 8 + 4 + 2 + 1 + 1, knockout.Matches.Count);
    }

    // ──────────────────────────────────────────────
    // RED: SetMatchResult — casos de prueba
    // ──────────────────────────────────────────────

    [TestMethod]
    public void SetMatchResult_WhenMatchExists_ShouldUpdateGoalsAndFinishMatch()
    {
        var knockout = CreateValidKnockout();
        var qualifiedTeams = CreateOrderedQualifiedTeams();
        knockout.SetQualifiedTeams(qualifiedTeams);
        knockout.InitializeRoundOf32();

        string targetCode = "A1";
        int homeGoals = 2;
        int awayGoals = 1;

        knockout.SetMatchResult(targetCode, homeGoals, awayGoals);

        var match = knockout.Matches.Single(m => m.MatchCode == targetCode);
        Assert.AreEqual(homeGoals, match.HomeGoals);
        Assert.AreEqual(awayGoals, match.AwayGoals);
        Assert.AreEqual(MatchStatus.Finished, match.Status);
        Assert.IsNotNull(match.Winner);
    }

    [TestCategory("UpdateResults")]
    [TestMethod]
    public void SetMatchResult_WhenMatchDoesNotExist_ShouldThrowArgumentException()
    {
        var knockout = CreateValidKnockout();
        knockout.SetQualifiedTeams(CreateOrderedQualifiedTeams());
        knockout.InitializeRoundOf32();

        Assert.ThrowsException<ArgumentException>(() =>
            knockout.SetMatchResult("NON-EXISTENT", 1, 0),
            "Debe fallar si el código de partido no existe en la fase de knockout.");
    }

    [TestCategory("UpdateResults")]
    [TestMethod]
    public void SetMatchResult_WhenResultIsDraw_ShouldThrowArgumentException()
    {
        var knockout = CreateValidKnockout();
        knockout.SetQualifiedTeams(CreateOrderedQualifiedTeams());
        knockout.InitializeRoundOf32();

        Assert.ThrowsException<ArgumentException>(() =>
            knockout.SetMatchResult("A1", 1, 1),
            "Los partidos de knockout no pueden terminar en empate.");
    }

    [TestCategory("UpdateResults")]
    [TestMethod]
    public void SetMatchResult_WhenMatchIsAlreadyFinished_ShouldThrowInvalidOperationException()
    {
        var knockout = CreateValidKnockout();
        knockout.SetQualifiedTeams(CreateOrderedQualifiedTeams());
        knockout.InitializeRoundOf32();
        knockout.SetMatchResult("A1", 3, 0);

        Assert.ThrowsException<InvalidOperationException>(() =>
            knockout.SetMatchResult("A1", 1, 2),
            "No se debería poder cambiar el resultado de un partido ya finalizado (bloqueo de integridad).");
    }

    //Helpers--------------------------------
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

    private Knockout CreateKnockoutWithFinishedRoundOf32()
    {
        var knockout = CreateValidKnockout();
        knockout.SetQualifiedTeams(CreateOrderedQualifiedTeams());
        knockout.InitializeRoundOf32();
        FinishAllMatches(knockout.Matches.Where(m => m.Phase == TournamentPhase.RoundOf32));
        return knockout;
    }

    private Knockout CreateKnockoutWithFinishedRoundOf16()
    {
        var knockout = CreateKnockoutWithFinishedRoundOf32();
        knockout.InitializeRoundOf16();
        FinishAllMatches(knockout.Matches.Where(m => m.Phase == TournamentPhase.RoundOf16));
        return knockout;
    }

    private Knockout CreateKnockoutWithFinishedQuarterFinals()
    {
        var knockout = CreateKnockoutWithFinishedRoundOf16();
        knockout.InitializeQuarterFinals();
        FinishAllMatches(knockout.Matches.Where(m => m.Phase == TournamentPhase.QuarterFinals));
        return knockout;
    }

    private Knockout CreateKnockoutWithFinishedSemiFinals()
    {
        var knockout = CreateKnockoutWithFinishedQuarterFinals();
        knockout.InitializeSemiFinals();
        FinishAllMatches(knockout.Matches.Where(m => m.Phase == TournamentPhase.SemiFinals));
        return knockout;
    }

    private static void FinishAllMatches(IEnumerable<Match> matches)
    {
        foreach (var match in matches)
        {
            match.SetResult(1, 0);
            match.Finish();
        }
    }

    private sealed class TestShuffleService : IShuffleService
    {
        private readonly int _seed;

        public TestShuffleService(int seed)
        {
            _seed = seed;
        }

        public void Shuffle<T>(IList<T> list)
        {
            var random = new Random(_seed);
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        public void ShuffleByRanking(IList<Team> teams)
        {
            var random = new Random(_seed);
            int start = 0;

            while (start < teams.Count)
            {
                int end = FindEndOfTiedGroup(teams, start);

                if (end - start > 1)
                    for (int i = end - 1; i > start; i--)
                    {
                        int j = random.Next(start, i + 1);
                        (teams[i], teams[j]) = (teams[j], teams[i]);
                    }

                start = end;
            }
        }

        private static int FindEndOfTiedGroup(IList<Team> teams, int start)
        {
            int end = start;
            while (end < teams.Count && teams[end].RankingFifa == teams[start].RankingFifa)
                end++;
            return end;
        }
    }

    private sealed class TeamRequest
    {
        public string Name { get; init; } = "";
        public string ConfederationName { get; init; } = "";
        public int RankingFifa { get; init; }
        public string GroupLabel { get; init; } = "";
    }
}
