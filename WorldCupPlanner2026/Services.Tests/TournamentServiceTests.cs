using Domain;
using DTOs;
using IRepository;
using IServices;
using Services;

namespace Services.Tests;

[TestClass]
public class TournamentServiceTests
{
    private const int DefaultSeed = 42;

    private const string HomeTeamName = "ARGENTINA";
    private const string AwayTeamName = "BRAZIL";

    private const string MainStadiumName = "CENTENARIO";
    private const string MainStadiumCity = "MONTEVIDEO";

    private static readonly DateTime ValidMatchDate = new(2026, 6, 1, 18, 0, 0);
    private static readonly DateTime UpdatedMatchDate = new(2026, 6, 10, 18, 0, 0);

    private ITournamentService _service = null!;

    private IRepository<Group> _groupRepository = null!;
    private IRepository<Match> _matchRepository = null!;
    private IRepository<Stadium> _stadiumRepository = null!;
    private IRepository<Team> _teamRepository = null!;
    private IRepository<Pot> _potRepository = null!;
    private IRepository<AuditLog> _auditRepository = null!;

    private IAuditService _auditService = null!;
    private ISessionService _sessionService = null!;
    private IUpdateRankingService _updateRankingService = null!;

    [TestInitialize]
    public void Setup()
    {
        _groupRepository = new FakeRepository<Group>();
        _matchRepository = new FakeRepository<Match>();
        _stadiumRepository = new FakeRepository<Stadium>();
        _teamRepository = new FakeRepository<Team>();
        _potRepository = new FakeRepository<Pot>();
        _auditRepository = new FakeRepository<AuditLog>();

        _auditService = new AuditService(_auditRepository);
        _sessionService = new SessionService(null!, new AuthStateService());
        _updateRankingService = new UpdateRankingService(_auditService);

        _service = new TournamentService(
            _groupRepository,
            _matchRepository,
            _stadiumRepository,
            _teamRepository,
            _potRepository,
            _auditService,
            _sessionService,
            _updateRankingService);
    }

    // ─────────────────────────────────────────────
    // Fixture / Pots
    // ─────────────────────────────────────────────

    [TestMethod]
    public void GenerateFixture_WhenLessThan48Teams_ShouldThrowInvalidOperationException()
    {
        AddStadiums(4);

        Assert.ThrowsException<InvalidOperationException>(() =>
            _service.GenerateFixture(DefaultSeed, new DateTime(2026, 6, 1)));
    }

    [TestMethod]
    public void GenerateFixture_WithLessThan4Stadiums_ShouldThrowInvalidOperationException()
    {
        AddTeams(48);
        AddStadiums(3);

        Assert.ThrowsException<InvalidOperationException>(() =>
            _service.GenerateFixture(DefaultSeed, new DateTime(2026, 6, 1)));
    }

    [TestMethod]
    public void GenerateFixture_WithExistingFixture_ShouldThrowInvalidOperationException()
    {
        AddTeams(48);
        AddStadiums(4);

        _service.GenerateFixture(DefaultSeed, new DateTime(2026, 6, 1));

        Assert.ThrowsException<InvalidOperationException>(() =>
            _service.GenerateFixture(DefaultSeed, new DateTime(2026, 6, 1)));
    }

    [TestMethod]
    public void GenerateFixture_WithValidData_ShouldCreate12Groups()
    {
        AddTeams(48);
        AddStadiums(4);

        _service.GenerateFixture(DefaultSeed, new DateTime(2026, 6, 1));

        Assert.AreEqual(12, _groupRepository.GetAll().Count());
    }


    [TestMethod]
    public void GenerateFixture_WithValidData_ShouldCreateFourPots()
    {
        AddTeams(48);
        AddStadiums(4);

        _service.GenerateFixture(DefaultSeed, new DateTime(2026, 6, 1));

        Assert.AreEqual(4, _potRepository.GetAll().Count());
    }

    [TestMethod]
    public void GenerateFixture_WithValidData_ShouldAssign12TeamsPerPot()
    {
        AddTeams(48);
        AddStadiums(4);

        _service.GenerateFixture(DefaultSeed, new DateTime(2026, 6, 1));

        Assert.IsTrue(_potRepository.GetAll().All(pot => pot.Teams.Count == 12));
    }

    [TestMethod]
    public void GenerateFixture_WithValidData_ShouldAssign4TeamsPerGroup()
    {
        AddTeams(48);
        AddStadiums(4);

        _service.GenerateFixture(DefaultSeed, new DateTime(2026, 6, 1));

        Assert.IsTrue(_groupRepository.GetAll().All(group => group.Teams.Count == 4));
    }

    [TestMethod]
    public void GenerateFixture_WithValidData_ShouldCreate72GroupStageMatches()
    {
        AddTeams(48);
        AddStadiums(4);

        _service.GenerateFixture(DefaultSeed, new DateTime(2026, 6, 1));

        var totalMatches = _groupRepository.GetAll()
            .SelectMany(group => group.Matches)
            .Count();

        Assert.AreEqual(72, totalMatches);
    }

    [TestMethod]
    public void CanGenerateFixture_WithAllConditionsMet_ShouldReturnTrue()
    {
        AddTeams(48);
        AddStadiums(4);

        Assert.IsTrue(_service.CanGenerateFixture());
    }

    [TestMethod]
    public void CanGenerateFixture_WithExistingFixture_ShouldReturnFalse()
    {
        AddTeams(48);
        AddStadiums(4);

        _service.GenerateFixture(DefaultSeed, new DateTime(2026, 6, 1));

        Assert.IsFalse(_service.CanGenerateFixture());
    }

    [TestMethod]
    public void CanGenerateFixture_WithInsufficientTeams_ShouldReturnFalse()
    {
        AddStadiums(4);

        Assert.IsFalse(_service.CanGenerateFixture());
    }

    [TestMethod]
    public void GetStadiumCount_WithExistingStadiums_ShouldReturnCorrectCount()
    {
        AddStadiums(4);

        Assert.AreEqual(4, _service.GetStadiumCount());
    }

    // ─────────────────────────────────────────────
    // Groups
    // ─────────────────────────────────────────────

    [TestMethod]
    public void GetGroups_WithGeneratedFixture_ShouldReturnTwelveGroups()
    {
        AddTeams(48);
        AddStadiums(4);

        _service.GenerateFixture(DefaultSeed, new DateTime(2026, 6, 1));

        Assert.AreEqual(12, _service.GetGroups().Count());
    }

    [TestMethod]
    public void GetGroupByLabel_WithExistingLabel_ShouldReturnCorrectGroup()
    {
        AddTeams(48);
        AddStadiums(4);
        _service.GenerateFixture(DefaultSeed, new DateTime(2026, 6, 1));

        var group = _service.GetGroupByLabel('A');

        Assert.AreEqual('A', group.Label);
    }

    [TestMethod]
    public void GetGroupByLabel_WithNonExistingLabel_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _service.GetGroupByLabel('Z'));
    }

    [TestMethod]
    public void GetGroupStandings_WithGeneratedFixture_ShouldReturnFourTeams()
    {
        AddTeams(48);
        AddStadiums(4);
        _service.GenerateFixture(DefaultSeed, new DateTime(2026, 6, 1));

        var standings = _service.GetGroupStandings('A').ToList();

        Assert.AreEqual(4, standings.Count);
    }

    [TestMethod]
    public void SimulateGroupStage_WithNoFixtureGenerated_ShouldThrowInvalidOperationException()
    {
        Assert.ThrowsException<InvalidOperationException>(() =>
            _service.SimulateGroupStage(DefaultSeed));
    }

    [TestMethod]
    public void SimulateGroupStage_WithGeneratedFixture_ShouldFinishGroupMatches()
    {
        AddTeams(48);
        AddStadiums(4);
        _service.GenerateFixture(DefaultSeed, new DateTime(2026, 6, 1));

        _service.SimulateGroupStage(DefaultSeed);

        var matches = _groupRepository.GetAll()
            .SelectMany(group => group.Matches)
            .ToList();

        Assert.IsTrue(matches.All(match => match.HomeGoals.HasValue));
        Assert.IsTrue(matches.All(match => match.AwayGoals.HasValue));
    }

    [TestMethod]
    public void SetGroupMatchResult_WithGeneratedFixture_ShouldApplyResultToMatch()
    {
        AddTeams(48);
        AddStadiums(4);
        _service.GenerateFixture(DefaultSeed, new DateTime(2026, 6, 1));

        var firstGroup = _groupRepository.GetAll().First();
        var firstMatch = firstGroup.Matches.First();

        _service.SetGroupMatchResult(firstGroup.Label, firstMatch.MatchId, 2, 1);

        Assert.AreEqual(2, firstMatch.HomeGoals);
        Assert.AreEqual(1, firstMatch.AwayGoals);
    }

    [TestMethod]
    public void SetGroupMatchResult_WithNonExistingGroup_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _service.SetGroupMatchResult('Z', 1, 1, 0));
    }

    [TestMethod]
    public void FinishGroupMatch_WithGeneratedFixture_ShouldFinishMatch()
    {
        AddTeams(48);
        AddStadiums(4);
        _service.GenerateFixture(DefaultSeed, new DateTime(2026, 6, 1));

        var firstGroup = _groupRepository.GetAll().First();
        var firstMatch = firstGroup.Matches.First();

        _service.FinishGroupMatch(firstGroup.Label, firstMatch.MatchId);

        Assert.AreEqual(MatchStatus.Finished, firstMatch.Status);
    }

    [TestMethod]
    public void FinishGroupMatch_WithNonExistingGroup_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _service.FinishGroupMatch('Z', 1));
    }

    [TestMethod]
    public void GetQualifiedTeams_WithSimulatedGroupStage_ShouldReturn32Teams()
    {
        AddTeams(48);
        AddStadiums(4);
        _service.GenerateFixture(DefaultSeed, new DateTime(2026, 6, 1));
        _service.SimulateGroupStage(DefaultSeed);

        var qualifiedTeams = _service.GetQualifiedTeams().ToList();

        Assert.AreEqual(32, qualifiedTeams.Count);
    }

    [TestMethod]
    public void GetQualifiedTeams_WithoutTwelveGroups_ShouldThrowInvalidOperationException()
    {
        Assert.ThrowsException<InvalidOperationException>(() =>
            _service.GetQualifiedTeams());
    }

    // ─────────────────────────────────────────────
    // Matches
    // ─────────────────────────────────────────────

    [TestMethod]
    public void AddMatch_WithValidRequest_ShouldPersistOneMatch()
    {
        _service.AddMatch(BuildValidMatchRequest());

        Assert.AreEqual(1, _service.GetMatches().Count());
    }

    [TestMethod]
    public void AddMatch_WithNullRequest_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _service.AddMatch(null!));
    }

    [TestMethod]
    public void GetMatchById_WithExistingId_ShouldReturnMatchWithExpectedProperties()
    {
        var saved = AddValidMatchAndGetResponse();

        var result = _service.GetMatchById(saved.Id);

        Assert.AreEqual(saved.Id, result.Id);
        Assert.AreEqual(HomeTeamName, result.HomeTeam.Name);
        Assert.AreEqual(AwayTeamName, result.AwayTeam.Name);
    }

    [TestMethod]
    public void GetMatchById_WithNonExistingId_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _service.GetMatchById(999));
    }

    [TestMethod]
    public void SetMatchResult_WithHomeTeamScoringMore_ShouldSetHomeTeamAsWinner()
    {
        var saved = AddValidMatchAndGetResponse();

        _service.SetMatchResult(saved.Id, 3, 1);

        var result = _service.GetMatchById(saved.Id);

        Assert.AreEqual(3, result.HomeGoals);
        Assert.AreEqual(1, result.AwayGoals);
        Assert.AreEqual(HomeTeamName, result.Winner!.Name);
    }

    [TestMethod]
    public void SetMatchResult_WithAwayTeamScoringMore_ShouldSetAwayTeamAsWinner()
    {
        var saved = AddValidMatchAndGetResponse();

        _service.SetMatchResult(saved.Id, 0, 2);

        var result = _service.GetMatchById(saved.Id);

        Assert.AreEqual(AwayTeamName, result.Winner!.Name);
    }

    [TestMethod]
    public void SetMatchResult_WithDraw_ShouldSetNullWinner()
    {
        var saved = AddValidMatchAndGetResponse();

        _service.SetMatchResult(saved.Id, 1, 1);

        var result = _service.GetMatchById(saved.Id);

        Assert.IsNull(result.Winner);
    }

    [TestMethod]
    public void UpdateMatchDate_WithValidDate_ShouldReplaceMatchDate()
    {
        var saved = AddValidMatchAndGetResponse();

        _service.UpdateMatchDate(saved.Id, UpdatedMatchDate);

        var result = _service.GetMatchById(saved.Id);

        Assert.AreEqual(UpdatedMatchDate, result.Date);
    }

    [TestMethod]
    public void UpdateMatchStadium_WithValidRequest_ShouldUpdateStadium()
    {
        var saved = AddValidMatchAndGetResponse();

        var newStadium = BuildStadiumRequest("WEMBLEY", "LONDON");

        _service.UpdateMatchStadium(saved.Id, newStadium);

        var result = _service.GetMatchById(saved.Id);

        Assert.AreEqual("WEMBLEY", result.Stadium.Name);
    }

    [TestMethod]
    public void UpdateMatchStadium_WithNullRequest_ShouldThrowArgumentException()
    {
        var saved = AddValidMatchAndGetResponse();

        Assert.ThrowsException<ArgumentException>(() =>
            _service.UpdateMatchStadium(saved.Id, null!));
    }

    [TestMethod]
    public void FinishMatch_WithExistingId_ShouldFinishMatch()
    {
        var saved = AddValidMatchAndGetResponse();

        _service.FinishMatch(saved.Id);

        var domainMatch = _matchRepository.GetAll()
            .First(match => match.MatchId == saved.Id);

        Assert.AreEqual(MatchStatus.Finished, domainMatch.Status);
    }

    [TestMethod]
    public void GetAvailableStadiumsForMatch_WhenAllCandidatesAreFree_ShouldReturnAllCandidates()
    {
        var saved = AddValidMatchAndGetResponse();

        var candidates = new[]
        {
            BuildStadiumRequest("MARACANA", "RIO DE JANEIRO"),
            BuildStadiumRequest("AZTECA", "CIUDAD DE MEXICO")
        };

        var result = _service.GetAvailableStadiumsForMatch(saved.Id, candidates).ToList();

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void GetAvailableStadiumsForMatch_WhenCandidateConflictsWithExisting_ShouldExcludeIt()
    {
        // Tres partidos en el mismo estadio y día → el estadio queda lleno (máx 3 por día),
        // por lo que ese estadio debe ser rechazado como candidato.
        var conflictStadium = BuildStadiumRequest("CONFLICT", "CITY");

        _service.AddMatch(BuildValidMatchRequest(
            homeTeamName: "TEAM_A", awayTeamName: "TEAM_B",
            date: new DateTime(2026, 7, 1, 10, 0, 0), stadiumRequest: conflictStadium));
        _service.AddMatch(BuildValidMatchRequest(
            homeTeamName: "TEAM_C", awayTeamName: "TEAM_D",
            date: new DateTime(2026, 7, 1, 15, 0, 0), stadiumRequest: conflictStadium));
        _service.AddMatch(BuildValidMatchRequest(
            homeTeamName: "TEAM_E", awayTeamName: "TEAM_F",
            date: new DateTime(2026, 7, 1, 20, 0, 0), stadiumRequest: conflictStadium));

        // Partido objetivo en otra fecha (no cuenta para el conflicto del día lleno).
        _service.AddMatch(BuildValidMatchRequest(
            homeTeamName: "TEAM_G", awayTeamName: "TEAM_H",
            date: new DateTime(2026, 8, 1, 12, 0, 0),
            stadiumRequest: BuildStadiumRequest("OTHER", "CITY")));

        var target = _service.GetMatches().Last();

        // Candidato en el día lleno: debe ser excluido. Lo evaluamos contra la fecha del partido objetivo,
        // pero el conflicto se mide sobre los partidos del estadio CONFLICT en su propio día.
        var candidates = new[] { conflictStadium };

        // Para que el conflicto se dispare, el partido objetivo debe estar ese día lleno:
        _service.UpdateMatchDate(target.Id, new DateTime(2026, 7, 1, 8, 0, 0));
        var refreshed = _service.GetMatches().Last();

        var result = _service.GetAvailableStadiumsForMatch(refreshed.Id, candidates).ToList();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void AddMatch_WhenStadiumHasThreeMatchesSameDay_ShouldThrowInvalidOperationException()
    {
        var stadium = BuildStadiumRequest("FULL", "CITY");
        var day = new DateTime(2026, 7, 1);

        _service.AddMatch(BuildValidMatchRequest(
            homeTeamName: "T1", awayTeamName: "T2",
            date: day.AddHours(8), stadiumRequest: stadium));
        _service.AddMatch(BuildValidMatchRequest(
            homeTeamName: "T3", awayTeamName: "T4",
            date: day.AddHours(13), stadiumRequest: stadium));
        _service.AddMatch(BuildValidMatchRequest(
            homeTeamName: "T5", awayTeamName: "T6",
            date: day.AddHours(18), stadiumRequest: stadium));

        Assert.ThrowsException<InvalidOperationException>(() =>
            _service.AddMatch(BuildValidMatchRequest(
                homeTeamName: "T7", awayTeamName: "T8",
                date: day.AddHours(22), stadiumRequest: stadium)));
    }

    [TestMethod]
    public void AddMatch_WhenMatchesAreTooCloseInSameStadium_ShouldThrowInvalidOperationException()
    {
        var stadium = BuildStadiumRequest("CLOSE", "CITY");
        var day = new DateTime(2026, 7, 1);

        _service.AddMatch(BuildValidMatchRequest(
            homeTeamName: "T1", awayTeamName: "T2",
            date: day.AddHours(12), stadiumRequest: stadium));

        // Menos de 4 horas de diferencia en el mismo estadio.
        Assert.ThrowsException<InvalidOperationException>(() =>
            _service.AddMatch(BuildValidMatchRequest(
                homeTeamName: "T3", awayTeamName: "T4",
                date: day.AddHours(14), stadiumRequest: stadium)));
    }

    // ─────────────────────────────────────────────
    // Knockout
    // ─────────────────────────────────────────────

    [TestMethod]
    public void InitializeKnockoutStage_WithNoFixtureGenerated_ShouldThrowInvalidOperationException()
    {
        Assert.ThrowsException<InvalidOperationException>(() =>
            _service.InitializeKnockoutStage(DefaultSeed));
    }

    [TestMethod]
    public void InitializeKnockoutStage_WhenGroupStageNotSimulated_ShouldThrowInvalidOperationException()
    {
        AddTeams(48);
        AddStadiums(4);
        _service.GenerateFixture(DefaultSeed, new DateTime(2026, 6, 1));

        // Fixture generado pero NO simulado → los matches no tienen resultado.
        Assert.ThrowsException<InvalidOperationException>(() =>
            _service.InitializeKnockoutStage(DefaultSeed));
    }

    [TestMethod]
    public void InitializeKnockoutStage_WhenAlreadyInitialized_ShouldThrowInvalidOperationException()
    {
        CreateFixtureAndSimulateGroupStage();
        _service.InitializeKnockoutStage(DefaultSeed);

        Assert.ThrowsException<InvalidOperationException>(() =>
            _service.InitializeKnockoutStage(DefaultSeed));
    }

    [TestMethod]
    public void InitializeKnockoutStage_WithSimulatedGroupStage_ShouldCreateKnockoutMatches()
    {
        CreateFixtureAndSimulateGroupStage();

        _service.InitializeKnockoutStage(DefaultSeed);

        var knockoutMatches = _matchRepository.GetAll()
            .Where(match => match.Phase != TournamentPhase.GroupStage)
            .ToList();

        Assert.IsTrue(knockoutMatches.Any());
    }

    [TestMethod]
    public void GetKnockoutMatches_WithInitializedKnockout_ShouldReturnMappedResponses()
    {
        CreateFixtureAndSimulateGroupStage();

        _service.InitializeKnockoutStage(DefaultSeed);

        var matches = _service.GetKnockoutMatches().ToList();

        Assert.IsTrue(matches.Any());
        Assert.IsNotNull(matches.First().HomeTeam);
        Assert.IsNotNull(matches.First().AwayTeam);
        Assert.IsFalse(string.IsNullOrWhiteSpace(matches.First().MatchCode));
    }
    
    [TestMethod]
    public void FinishKnockoutMatch_WithResult_ShouldFinishMatchAndRecalculateRanking()
    {
        CreateFixtureAndSimulateGroupStage();
        _service.InitializeKnockoutStage(DefaultSeed);

        var knockoutMatch = _matchRepository.GetAll()
            .First(match => match.Phase != TournamentPhase.GroupStage);

        var matchCode = knockoutMatch.MatchCode!;

        _service.SetKnockoutMatchResult(matchCode, 2, 0);

        var homeRankingBefore = knockoutMatch.HomeTeam.RankingFifa;

        _service.FinishKnockoutMatch(matchCode);

        Assert.AreEqual(MatchStatus.Finished, knockoutMatch.Status);
        Assert.AreNotEqual(homeRankingBefore, knockoutMatch.HomeTeam.RankingFifa);
    }
    
    [TestMethod]
    public void FinishAllKnockoutMatches_WithResults_ShouldFinishAllKnockoutMatchesAndRecalculateRanking()
    {
        CreateFixtureAndSimulateGroupStage();
        _service.InitializeKnockoutStage(DefaultSeed);

        var knockoutMatches = _matchRepository.GetAll()
            .Where(match => match.Phase != TournamentPhase.GroupStage)
            .ToList();

        foreach (var match in knockoutMatches)
            _service.SetKnockoutMatchResult(match.MatchCode!, 1, 0);

        var sampleMatch = knockoutMatches.First();
        var homeRankingBefore = sampleMatch.HomeTeam.RankingFifa;

        _service.FinishAllKnockoutMatches();

        Assert.IsTrue(knockoutMatches.All(match => match.Status == MatchStatus.Finished));
        Assert.AreNotEqual(homeRankingBefore, sampleMatch.HomeTeam.RankingFifa);
    }
    
    [TestMethod]
    public void FinishAllKnockoutMatches_WithMatchesWithoutResult_ShouldNotFinishThem()
    {
        CreateFixtureAndSimulateGroupStage();
        _service.InitializeKnockoutStage(DefaultSeed);

        _service.FinishAllKnockoutMatches();

        var knockoutMatches = _matchRepository.GetAll()
            .Where(match => match.Phase != TournamentPhase.GroupStage)
            .ToList();

        Assert.IsTrue(knockoutMatches.All(match => match.Status == MatchStatus.NotFinished));
        Assert.IsTrue(knockoutMatches.All(match => !match.HasResult));
    }
    
    [TestMethod]
    public void SetKnockoutMatchResult_WithInitializedKnockout_ShouldApplyResult()
    {
        CreateFixtureAndSimulateGroupStage();
        _service.InitializeKnockoutStage(DefaultSeed);

        var firstKnockoutMatch = _matchRepository.GetAll()
            .First(match => match.Phase != TournamentPhase.GroupStage);
        var code = firstKnockoutMatch.MatchCode!;

        _service.SetKnockoutMatchResult(code, 2, 1);

        var updated = _matchRepository.GetAll()
            .First(match => match.MatchCode == code);

        Assert.AreEqual(2, updated.HomeGoals);
        Assert.AreEqual(1, updated.AwayGoals);
    }

    [TestMethod]
    public void SimulateKnockoutStage_WithoutInitializedKnockout_ShouldThrowInvalidOperationException()
    {
        Assert.ThrowsException<InvalidOperationException>(() =>
            _service.SimulateKnockoutStage(DefaultSeed));
    }

    [TestMethod]
    public void SimulateKnockoutStage_WithInitializedKnockout_ShouldSetResultsWithoutFinishingMatches()
    {
        CreateFixtureAndSimulateGroupStage();
        _service.InitializeKnockoutStage(DefaultSeed);

        _service.SimulateKnockoutStage(DefaultSeed);

        var knockoutMatches = _matchRepository.GetAll()
            .Where(match => match.Phase != TournamentPhase.GroupStage)
            .ToList();

        Assert.IsTrue(knockoutMatches.All(match => match.HasResult));
        Assert.IsTrue(knockoutMatches.All(match => match.Winner != null));
        Assert.IsTrue(knockoutMatches.All(match => match.Status == MatchStatus.NotFinished));
    }

    [TestMethod]
    public void InitializeRoundOf16_AfterRoundOf32Finished_ShouldCreateRoundOf16Matches()
    {
        CreateFixtureAndSimulateGroupStage();
        _service.InitializeKnockoutStage(DefaultSeed);
        SimulateAndFinishKnockoutStage();

        _service.InitializeRoundOf16();

        var roundOf16 = _matchRepository.GetAll()
            .Where(match => match.Phase == TournamentPhase.RoundOf16)
            .ToList();

        Assert.AreEqual(8, roundOf16.Count);
    }

    [TestMethod]
    public void InitializeQuarterFinals_AfterRoundOf16Finished_ShouldCreateQuarterFinalMatches()
    {
        CreateFixtureAndSimulateGroupStage();
        _service.InitializeKnockoutStage(DefaultSeed);

        SimulateAndFinishKnockoutStage();
        _service.InitializeRoundOf16();

        SimulateAndFinishKnockoutStage();
        _service.InitializeQuarterFinals();

        var quarterFinals = _matchRepository.GetAll()
            .Where(match => match.Phase == TournamentPhase.QuarterFinals)
            .ToList();

        Assert.AreEqual(4, quarterFinals.Count);
    }

    [TestMethod]
    public void InitializeSemiFinals_AfterQuarterFinalsFinished_ShouldCreateSemiFinalMatches()
    {
        CreateFixtureAndSimulateGroupStage();
        _service.InitializeKnockoutStage(DefaultSeed);

        SimulateAndFinishKnockoutStage();
        _service.InitializeRoundOf16();

        SimulateAndFinishKnockoutStage();
        _service.InitializeQuarterFinals();

        SimulateAndFinishKnockoutStage();
        _service.InitializeSemiFinals();

        var semiFinals = _matchRepository.GetAll()
            .Where(match => match.Phase == TournamentPhase.SemiFinals)
            .ToList();

        Assert.AreEqual(2, semiFinals.Count);
    }

    [TestMethod]
    public void InitializeThirdPlace_AfterSemiFinalsFinished_ShouldCreateThirdPlaceMatch()
    {
        CreateFixtureAndSimulateGroupStage();
        _service.InitializeKnockoutStage(DefaultSeed);

        SimulateAndFinishKnockoutStage();
        _service.InitializeRoundOf16();

        SimulateAndFinishKnockoutStage();
        _service.InitializeQuarterFinals();

        SimulateAndFinishKnockoutStage();
        _service.InitializeSemiFinals();

        SimulateAndFinishKnockoutStage();
        _service.InitializeThirdPlace();

        var thirdPlace = _matchRepository.GetAll()
            .Where(match => match.Phase == TournamentPhase.ThirdPlace)
            .ToList();

        Assert.AreEqual(1, thirdPlace.Count);
    }

    [TestMethod]
    public void InitializeFinal_AfterSemiFinalsFinished_ShouldCreateFinalMatch()
    {
        CreateFixtureAndSimulateGroupStage();
        _service.InitializeKnockoutStage(DefaultSeed);

        SimulateAndFinishKnockoutStage();
        _service.InitializeRoundOf16();

        SimulateAndFinishKnockoutStage();
        _service.InitializeQuarterFinals();

        SimulateAndFinishKnockoutStage();
        _service.InitializeSemiFinals();

        SimulateAndFinishKnockoutStage();
        _service.InitializeFinal();

        var final = _matchRepository.GetAll()
            .Where(match => match.Phase == TournamentPhase.Final)
            .ToList();

        Assert.AreEqual(1, final.Count);
    }  

    // ─────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────

    private void CreateFixtureAndSimulateGroupStage()
    {
        AddTeams(48);
        AddStadiums(4);

        _service.GenerateFixture(DefaultSeed, new DateTime(2026, 6, 1));
        _service.SimulateGroupStage(DefaultSeed);
    }

    private MatchResponse AddValidMatchAndGetResponse()
    {
        _service.AddMatch(BuildValidMatchRequest());
        return _service.GetMatches().Last();
    }

    private void AddTeams(int count)
    {
        var slots = new Dictionary<string, int>
        {
            { "UEFA", 16 },
            { "CONMEBOL", 7 },
            { "CONCACAF", 7 },
            { "CAF", 9 },
            { "AFC", 8 },
            { "OFC", 1 }
        };

        int ranking = 1500;
        int added = 0;

        foreach (var slot in slots)
        {
            for (int i = 1; i <= slot.Value && added < count; i++)
            {
                _teamRepository.Add(new Team($"{slot.Key}_{i:00}", slot.Key, ranking--));
                added++;
            }

            if (added == count)
                return;
        }

        while (added < count)
        {
            _teamRepository.Add(new Team($"TEAM_{added:00}", "UEFA", ranking--));
            added++;
        }
    }

    private void AddStadiums(int count)
    {
        for (int i = 1; i <= count; i++)
        {
            _stadiumRepository.Add(new Stadium(
                $"STADIUM_{i:00}",
                "City",
                "Test stadium",
                50000));
        }
    }

    private static MatchRequest BuildValidMatchRequest(
        string homeTeamName = HomeTeamName,
        string awayTeamName = AwayTeamName,
        DateTime? date = null,
        StadiumRequest? stadiumRequest = null) =>
        new()
        {
            HomeTeam = BuildTeamRequest(homeTeamName),
            AwayTeam = BuildTeamRequest(awayTeamName),
            Stadium = stadiumRequest ?? BuildStadiumRequest(MainStadiumName, MainStadiumCity),
            Date = date ?? ValidMatchDate
        };

    private static TeamRequest BuildTeamRequest(string name) =>
        new()
        {
            Name = name,
            ConfederationName = "CONMEBOL",
            RankingFifa = 1800
        };

    private static StadiumRequest BuildStadiumRequest(
        string name,
        string city,
        string description = "Historic stadium",
        int capacity = 60000) =>
        new()
        {
            Name = name,
            City = city,
            Description = description,
            Capacity = capacity
        };
    
    // ─── GenerateFixture sobrecarga (requiere agregar la firma a ITournamentService) ───
    [TestMethod]
    public void GenerateFixture_WithDefaultDateOverload_ShouldCreate12Groups()
    {
        AddTeams(48);
        AddStadiums(4);

        _service.GenerateFixture(DefaultSeed);   // sobrecarga de un parámetro

        Assert.AreEqual(12, _groupRepository.GetAll().Count());
    }

// ─── AddMatchToGroup: match con grupo asignado ───
    [TestMethod]
    public void AddMatch_WithGroupAssigned_ShouldAddMatchToThatGroup()
    {
        AddTeams(48);
        AddStadiums(4);
        _service.GenerateFixture(DefaultSeed, new DateTime(2026, 6, 1));

        var groupLabel = _groupRepository.GetAll().First().Label;

        var request = BuildValidMatchRequest(
            homeTeamName: "EXTRA_HOME",
            awayTeamName: "EXTRA_AWAY",
            date: new DateTime(2026, 9, 1, 12, 0, 0));
        request.Group = groupLabel;   // ← clave: dispara AddMatchToGroup

        _service.AddMatch(request);

        var group = _groupRepository.GetAll().First(g => g.Label == groupLabel);
        Assert.IsTrue(group.Matches.Any(m =>
            m.HomeTeam.Name == "EXTRA_HOME" && m.AwayTeam.Name == "EXTRA_AWAY"));
    }
    
    [TestMethod]
    public void RegisterMatchIncidence_YellowForHomeTeam_ShouldBeCountedOnDomainMatch()
    {
        var saved = AddValidMatchAndGetResponse();

        _service.RegisterMatchIncidence(saved.Id, "Yellow", isHomeTeam: true, minute: 30);

        var domainMatch = _matchRepository.GetAll().First(m => m.MatchId == saved.Id);
        Assert.AreEqual(1, domainMatch.YellowCardsOf(domainMatch.HomeTeam));
        Assert.AreEqual(0, domainMatch.YellowCardsOf(domainMatch.AwayTeam));
    }
    
    [TestMethod]
    public void RegisterMatchIncidence_RedForAwayTeam_ShouldBeCountedSeparately()
    {
        var saved = AddValidMatchAndGetResponse();

        _service.RegisterMatchIncidence(saved.Id, "Red", isHomeTeam: false, minute: 80);

        var domainMatch = _matchRepository.GetAll().First(m => m.MatchId == saved.Id);
        Assert.AreEqual(1, domainMatch.RedCardsOf(domainMatch.AwayTeam));
        Assert.AreEqual(0, domainMatch.RedCardsOf(domainMatch.HomeTeam));
    }
    
    [TestMethod]
    public void RegisterMatchIncidence_WithInvalidType_ShouldThrowArgumentException()
    {
        var saved = AddValidMatchAndGetResponse();

        Assert.ThrowsException<ArgumentException>(() =>
            _service.RegisterMatchIncidence(saved.Id, "Penalty", isHomeTeam: true, minute: 30));
    }
    
    [TestMethod]
    public void RegisterMatchIncidence_WithNonExistingMatch_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _service.RegisterMatchIncidence(999, "Yellow", isHomeTeam: true, minute: 30));
    }
    
    [TestMethod]
    public void FinishMatch_WithResult_ShouldRecalculateBothTeamsRanking()
    {
        var saved = AddValidMatchAndGetResponse();
        // El partido se crea con equipos de ranking 1800 (ver BuildTeamRequest).
        _service.SetMatchResult(saved.Id, 2, 0);  // gana el local

        var domainMatch = _matchRepository.GetAll().First(m => m.MatchId == saved.Id);
        var homeBefore = domainMatch.HomeTeam.RankingFifa;
        var awayBefore = domainMatch.AwayTeam.RankingFifa;

        _service.FinishMatch(saved.Id);

        // Tras finalizar, el ganador sube y el perdedor baja.
        Assert.IsTrue(domainMatch.HomeTeam.RankingFifa > homeBefore);
        Assert.IsTrue(domainMatch.AwayTeam.RankingFifa < awayBefore);
    }
    
    [TestMethod]
    public void FinishGroupMatch_WithResult_ShouldRecalculateRanking()
    {
        AddTeams(48);
        AddStadiums(4);
        _service.GenerateFixture(DefaultSeed, new DateTime(2026, 6, 1));

        var group = _groupRepository.GetAll().First();
        var match = group.Matches.First();
        _service.SetGroupMatchResult(group.Label, match.MatchId, 3, 0);

        var homeBefore = match.HomeTeam.RankingFifa;

        _service.FinishGroupMatch(group.Label, match.MatchId);

        Assert.AreNotEqual(homeBefore, match.HomeTeam.RankingFifa);
    }
    
    
    [TestMethod]
    public void FinishAllMatches_ShouldFinishMatchesWithResultAndRecalculateRanking()
    {
        AddTeams(48);
        AddStadiums(4);
        _service.GenerateFixture(DefaultSeed, new DateTime(2026, 6, 1));
        _service.SimulateGroupStage(DefaultSeed);  // deja resultados, NO finaliza
    
        // Tomamos un equipo cualquiera y su ranking antes de finalizar.
        var sampleMatch = _groupRepository.GetAll().First().Matches.First();
        var homeBefore = sampleMatch.HomeTeam.RankingFifa;
    
        _service.FinishAllMatches();
    
        // Todos los partidos de grupo quedaron finalizados.
        var allGroupMatches = _groupRepository.GetAll().SelectMany(g => g.Matches).ToList();
        Assert.IsTrue(allGroupMatches.All(m => m.Status == MatchStatus.Finished));
    
        // Y el ranking del equipo de muestra cambió (se recalculó).
        Assert.AreNotEqual(homeBefore, sampleMatch.HomeTeam.RankingFifa);
    }
    
    
    [TestMethod]
    public void SimulateKnockoutStage_ShouldNotRecalculateRankingBeforeMatchesAreFinished()
    {
        CreateFixtureAndSimulateGroupStage();
        _service.InitializeKnockoutStage(DefaultSeed);

        var knockoutMatch = _matchRepository.GetAll()
            .First(match => match.Phase != TournamentPhase.GroupStage);

        var homeRankingBefore = knockoutMatch.HomeTeam.RankingFifa;

        _service.SimulateKnockoutStage(DefaultSeed);

        Assert.AreEqual(homeRankingBefore, knockoutMatch.HomeTeam.RankingFifa);
    }
    
    private void SimulateAndFinishKnockoutStage()
    {
        _service.SimulateKnockoutStage(DefaultSeed);
        _service.FinishAllKnockoutMatches();
    }
}