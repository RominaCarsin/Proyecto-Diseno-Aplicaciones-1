using Domain;
using DTOs;
using IRepository;
using IServices;

namespace Services.Tests;

[TestClass]
public class FixtureServiceTests
{
    private IRepository<Group> _groupRepository = null!;
    private IRepository<Match> _matchRepository = null!;

    [TestMethod]
    public void GenerateFixture_WhenLessThan48Teams_ShouldThrowInvalidOperationException()
    {
        var fixtureService = CreateFixtureService();

        Assert.ThrowsException<InvalidOperationException>(() =>
            fixtureService.GenerateFixture(42, new DateTime(2026, 6, 1)));
    }

    private FixtureService CreateFixtureService()
    {
        var groupRepo = new FakeRepository<Group>();
        var matchRepo = new FakeRepository<Match>();
        var stadiumRepo = new FakeRepository<Stadium>();
        _groupRepository = groupRepo;
        _matchRepository = matchRepo;

        var teamRepo = new FakeRepository<Team>();
        var confederationService = new ConfederationService();
        var teamService = CreateTeamService(teamRepo, confederationService);
        var stadiumService = CreateStadiumService(stadiumRepo);
        var groupService = new GroupService(groupRepo);

        return new FixtureService(teamService, stadiumService, groupService, groupRepo, matchRepo, stadiumRepo, teamRepo, null);
    }

    [TestMethod]
    public void GenerateFixture_WithLessThan4Stadiums_ShouldThrowInvalidOperationException()
    {
        var fixtureService = CreateFixtureServiceWith48TeamsNoStadiums();

        Assert.ThrowsException<InvalidOperationException>(() =>
            fixtureService.GenerateFixture(42, new DateTime(2026, 6, 1)));
    }

    private FixtureService CreateFixtureServiceWith48TeamsNoStadiums()
    {
        var groupRepo = new FakeRepository<Group>();
        var matchRepo = new FakeRepository<Match>();
        var stadiumRepo = new FakeRepository<Stadium>();
        _groupRepository = groupRepo;

        var teamRepo = new FakeRepository<Team>();
        var confederationService = new ConfederationService();
        var teamService = CreateTeamService(teamRepo, confederationService);

        for (int i = 0; i < 48; i++)
            teamRepo.Add(new Team($"TEAM_{i:00}", "UEFA", 1000 + i));

        var stadiumService = CreateStadiumService(stadiumRepo);
        var groupService = new GroupService(groupRepo);

        return new FixtureService(teamService, stadiumService, groupService, groupRepo, matchRepo, stadiumRepo, teamRepo, null);
    }

    [TestMethod]
    public void GenerateFixture_WithExistingFixture_ShouldThrowInvalidOperationException()
    {
        var fixtureService = CreateFixtureServiceReadyWithPots(48);
        fixtureService.GenerateFixture(42, new DateTime(2026, 6, 1));

        Assert.ThrowsException<InvalidOperationException>(() =>
            fixtureService.GenerateFixture(42, new DateTime(2026, 6, 1)));
    }

    [TestMethod]
    public void GenerateFixture_WithValidData_ShouldCreate12Groups()
    {
        var fixtureService = CreateFixtureServiceReadyWithPots(48);

        fixtureService.GenerateFixture(42, new DateTime(2026, 6, 1));

        var groups = _groupRepository.GetAll().ToList();
        Assert.AreEqual(12, groups.Count);
    }

    [TestMethod]
    public void GenerateFixture_WithValidData_ShouldAssign4TeamsPerGroup()
    {
        var fixtureService = CreateFixtureServiceReadyWithPots(48);

        fixtureService.GenerateFixture(42, new DateTime(2026, 6, 1));

        var groups = _groupRepository.GetAll().ToList();
        Assert.IsTrue(groups.All(g => g.Teams.Count == 4));
    }

    [TestMethod]
    public void GenerateFixture_WithValidData_ShouldCreate6MatchesPerGroup()
    {
        var fixtureService = CreateFixtureServiceReadyWithPots(48);

        fixtureService.GenerateFixture(42, new DateTime(2026, 6, 1));

        var groups = _groupRepository.GetAll().ToList();
        Assert.IsTrue(groups.All(g => g.Matches.Count == 6));
    }

    [TestMethod]
    public void GenerateFixture_WithValidData_GroupsShouldHaveLabelsAToL()
    {
        var fixtureService = CreateFixtureServiceReadyWithPots(48);

        fixtureService.GenerateFixture(42, new DateTime(2026, 6, 1));

        var labels = _groupRepository.GetAll()
            .Select(g => g.Label)
            .OrderBy(l => l)
            .ToList();

        var expected = Enumerable.Range(0, 12)
            .Select(i => (char)('A' + i))
            .ToList();

        CollectionAssert.AreEqual(expected, labels);
    }

    [TestMethod]
    public void CanGenerateFixture_WithLessThan48Teams_ShouldReturnFalse()
    {
        var fixtureService = CreateFixtureService();

        Assert.IsFalse(fixtureService.CanGenerateFixture());
    }

    [TestMethod]
    public void CanGenerateFixture_WithLessThan4Stadiums_ShouldReturnFalse()
    {
        var fixtureService = CreateFixtureServiceWith48TeamsNoStadiums();
        Assert.IsFalse(fixtureService.CanGenerateFixture());
    }

    [TestMethod]
    public void CanGenerateFixture_WithExistingFixture_ShouldReturnFalse()
    {
        var fixtureService = CreateFixtureServiceReadyWithPots(48);
        fixtureService.GenerateFixture(42, new DateTime(2026, 6, 1));
        Assert.IsFalse(fixtureService.CanGenerateFixture());
    }

    [TestMethod]
    public void GetStadiumCount_WithNoStadiums_ShouldReturnZero()
    {
        var fixtureService = CreateFixtureService();

        Assert.AreEqual(0, fixtureService.GetStadiumCount());
    }

    [TestMethod]
    public void GetStadiumCount_WithExistingStadiums_ShouldReturnCorrectCount()
    {
        var fixtureService = CreateFixtureServiceReadyWithPots(48);

        Assert.AreEqual(4, fixtureService.GetStadiumCount());
    }

    private FixtureService CreateFixtureServiceReadyWithPots(int seed = 42)
    {
        var groupRepo = new FakeRepository<Group>();
        var matchRepo = new FakeRepository<Match>();
        var potRepo = new FakeRepository<Pot>();
        var stadiumRepo = new FakeRepository<Stadium>();
        _groupRepository = groupRepo;
        _matchRepository = matchRepo;

        var teamRepo = new FakeRepository<Team>();
        var confederationService = new ConfederationService();
        var teamService = CreateTeamService(teamRepo, confederationService);

        var slots = new Dictionary<string, int>
        {
            { "UEFA", 16 }, { "CONMEBOL", 7 }, { "CONCACAF", 7 },
            { "CAF", 9 }, { "AFC", 8 }, { "OFC", 1 }
        };

        int ranking = 1500;
        foreach (var (conf, count) in slots)
            for (int i = 1; i <= count; i++)
                teamRepo.Add(new Team($"{conf}_{i:00}", conf, ranking--));

        var stadiumService = CreateStadiumService(stadiumRepo);
        for (int i = 0; i < 4; i++)
            stadiumRepo.Add(new Stadium($"STADIUM_{i}", "City", null, 50000));

        var groupService = new GroupService(groupRepo);
        var potService = new PotService(teamRepo, potRepo);

        return new FixtureService(teamService, stadiumService, groupService, groupRepo, matchRepo, stadiumRepo,teamRepo, potService);
    }

    [TestMethod]
    public void GenerateFixture_WithNoStartDate_ShouldUseDefaultStartDate()
    {
        var fixtureService = CreateFixtureServiceReadyWithPots(48);
        fixtureService.GenerateFixture(42);
        var firstMatchDate = _groupRepository.GetAll()
            .SelectMany(g => g.Matches)
            .Min(m => m.Date);
        Assert.AreEqual(new DateTime(2026, 6, 1, 14, 0, 0), firstMatchDate);
    }

    [TestMethod]
    public void CanGenerateFixture_WithAllConditionsMet_ShouldReturnTrue()
    {
        var fixtureService = CreateFixtureServiceReadyWithPots(48);
        Assert.IsTrue(fixtureService.CanGenerateFixture());
    }

    [TestMethod]
    public void GenerateFixture_WithValidData_ShouldCreate72TotalMatches()
    {
        var fixtureService = CreateFixtureServiceReadyWithPots(48);
        fixtureService.GenerateFixture(42, new DateTime(2026, 6, 1));
        var totalMatches = _groupRepository.GetAll().SelectMany(g => g.Matches).Count();
        Assert.AreEqual(72, totalMatches);
    }

    [TestMethod]
    public void GenerateFixture_ShouldAssignPhaseGroupStageToAllMatches()
    {
        var fixtureService = CreateFixtureServiceReadyWithPots(48);
        fixtureService.GenerateFixture(42, new DateTime(2026, 6, 1));
        var allMatches = _groupRepository.GetAll().SelectMany(g => g.Matches).ToList();
        Assert.IsTrue(allMatches.All(m => m.Phase == TournamentPhase.GroupStage));
    }

    [TestMethod]
    public void GenerateFixture_ShouldAssignGroupLabelToAllMatches()
    {
        var fixtureService = CreateFixtureServiceReadyWithPots(48);
        fixtureService.GenerateFixture(42, new DateTime(2026, 6, 1));
        foreach (var group in _groupRepository.GetAll())
            Assert.IsTrue(group.Matches.All(m => m.Group == group.Label));
    }

    private static TeamService CreateTeamService(
        IRepository<Team> teamRepository,
        IConfederationService confederationService)
    {
        var auditService = new AuditService(new FakeRepository<AuditLog>());
        var sessionService = new SessionService(null!, new AuthStateService());
        return new TeamService(teamRepository, confederationService, auditService, sessionService);
    }

    private static StadiumService CreateStadiumService(IRepository<Stadium> stadiumRepository)
    {
        var auditService = new AuditService(new FakeRepository<AuditLog>());
        var sessionService = new SessionService(null!, new AuthStateService());
        return new StadiumService(stadiumRepository, auditService, sessionService);
    }

    [TestMethod]
    public void SimulateGroupStage_WithNoFixtureGenerated_ShouldThrowInvalidOperationException()
    {
        var fixtureService = CreateFixtureService();
        Assert.ThrowsException<InvalidOperationException>(() =>
            fixtureService.SimulateGroupStage(42));
    }

    [TestMethod]
    public void SimulateGroupStage_ShouldSetResultOnAllMatches()
    {
        var fixtureService = CreateFixtureServiceReadyWithPots(42);
        fixtureService.GenerateFixture(42, new DateTime(2026, 6, 1));
        fixtureService.SimulateGroupStage(42);
        var allMatches = _groupRepository.GetAll().SelectMany(g => g.Matches).ToList();
        Assert.IsTrue(allMatches.All(m => m.HomeGoals.HasValue && m.AwayGoals.HasValue));
    }

    [TestMethod]
    public void SimulateGroupStage_ShouldProduceWinsNotOnlyDraws()
    {
        var fixtureService = CreateFixtureServiceReadyWithPots(42);
        fixtureService.GenerateFixture(42, new DateTime(2026, 6, 1));
        fixtureService.SimulateGroupStage(42);
        var anyMatchWithWinner = _groupRepository.GetAll()
            .SelectMany(g => g.Matches)
            .Any(m => m.Winner != null);
        Assert.IsTrue(anyMatchWithWinner);
    }

    [TestMethod]
    public void SimulateGroupStage_WithSameSeed_ShouldProduceSameWinners()
    {
        var service1 = CreateFixtureServiceReadyWithPots(42);
        service1.GenerateFixture(42, new DateTime(2026, 6, 1));
        service1.SimulateGroupStage(42);
        var winners1 = _groupRepository.GetAll()
            .SelectMany(g => g.Matches)
            .Select(m => m.Winner?.Name)
            .ToList();

        var service2 = CreateFixtureServiceReadyWithPots(42);
        service2.GenerateFixture(42, new DateTime(2026, 6, 1));
        service2.SimulateGroupStage(42);
        var winners2 = _groupRepository.GetAll()
            .SelectMany(g => g.Matches)
            .Select(m => m.Winner?.Name)
            .ToList();

        CollectionAssert.AreEqual(winners1, winners2);
    }

    [TestMethod]
    public void InitializeKnockoutStage_WithoutGroupStage_ShouldThrowInvalidOperationException()
    {
        var fixtureService = CreateFixtureService();
        Assert.ThrowsException<InvalidOperationException>(() =>
            fixtureService.InitializeKnockoutStage(42));
    }

    [TestMethod]
    public void InitializeKnockoutStage_WithGroupStageNotSimulated_ShouldThrowInvalidOperationException()
    {
        var fixtureService = CreateFixtureServiceReadyWithPots(48);
        fixtureService.GenerateFixture(42, new DateTime(2026, 6, 1));
        Assert.ThrowsException<InvalidOperationException>(() =>
            fixtureService.InitializeKnockoutStage(42));
    }

    [TestMethod]
    public void InitializeKnockoutStage_WithValidGroupStage_ShouldCreate16RoundOf32Matches()
    {
        var fixtureService = CreateFixtureServiceWithSimulatedGroupStage();
        fixtureService.InitializeKnockoutStage(42);
        var knockoutMatches = _matchRepository.GetAll()
            .Where(m => m.Phase == TournamentPhase.RoundOf32)
            .ToList();
        Assert.AreEqual(16, knockoutMatches.Count);
    }

    [TestMethod]
    public void InitializeKnockoutStage_WhenAlreadyInitialized_ShouldThrowInvalidOperationException()
    {
        var fixtureService = CreateFixtureServiceWithSimulatedGroupStage();
        fixtureService.InitializeKnockoutStage(42);
        Assert.ThrowsException<InvalidOperationException>(() =>
            fixtureService.InitializeKnockoutStage(42));
    }

    [TestMethod]
    public void InitializeKnockoutStage_ShouldUseMatchCodesA1toA8andB1toB8()
    {
        var fixtureService = CreateFixtureServiceWithSimulatedGroupStage();
        fixtureService.InitializeKnockoutStage(42);
        var roundOf32Matches = _matchRepository.GetAll()
            .Where(m => m.Phase == TournamentPhase.RoundOf32)
            .ToList();
        var aCodes = roundOf32Matches.Select(m => m.MatchCode).Where(c => c != null && c.StartsWith("A")).OrderBy(c => c).ToList();
        var bCodes = roundOf32Matches.Select(m => m.MatchCode).Where(c => c != null && c.StartsWith("B")).OrderBy(c => c).ToList();
        Assert.AreEqual(8, aCodes.Count);
        Assert.AreEqual(8, bCodes.Count);
        CollectionAssert.AreEqual(new[] { "A1", "A2", "A3", "A4", "A5", "A6", "A7", "A8" }, aCodes);
        CollectionAssert.AreEqual(new[] { "B1", "B2", "B3", "B4", "B5", "B6", "B7", "B8" }, bCodes);
    }

    [TestMethod]
    public void InitializeKnockoutStage_WithSameSeed_ShouldProduceDeterministicResults()
    {
        var service1 = CreateFixtureServiceWithSimulatedGroupStage();
        service1.InitializeKnockoutStage(42);
        var codes1 = _matchRepository.GetAll()
            .Where(m => m.Phase == TournamentPhase.RoundOf32)
            .Select(m => $"{m.MatchCode}:{m.HomeTeam.Name}-{m.AwayTeam.Name}")
            .OrderBy(c => c).ToList();

        var service2 = CreateFixtureServiceWithSimulatedGroupStage();
        service2.InitializeKnockoutStage(42);
        var codes2 = _matchRepository.GetAll()
            .Where(m => m.Phase == TournamentPhase.RoundOf32)
            .Select(m => $"{m.MatchCode}:{m.HomeTeam.Name}-{m.AwayTeam.Name}")
            .OrderBy(c => c).ToList();

        CollectionAssert.AreEqual(codes1, codes2);
    }

    [TestMethod]
    public void InitializeRoundOf16_WithoutInitializedKnockout_ShouldThrowInvalidOperationException()
    {
        var fixtureService = CreateFixtureService();
        Assert.ThrowsException<InvalidOperationException>(() => fixtureService.InitializeRoundOf16());
    }

    [TestMethod]
    public void InitializeQuarterFinals_WithoutInitializedKnockout_ShouldThrowInvalidOperationException()
    {
        var fixtureService = CreateFixtureService();
        Assert.ThrowsException<InvalidOperationException>(() => fixtureService.InitializeQuarterFinals());
    }

    [TestMethod]
    public void InitializeQuarterFinals_WhenAllPreviousPhasesAreFinished_ShouldCreateNewMatches()
    {
        var service = CreateFixtureServiceWithSimulatedGroupStage();
        service.InitializeKnockoutStage(42);
        FinishAllPhaseMatches(service, TournamentPhase.RoundOf32);
        service.InitializeRoundOf16();
        FinishAllPhaseMatches(service, TournamentPhase.RoundOf16);
        service.InitializeQuarterFinals();
        var qfMatches = _matchRepository.GetAll().Where(m => m.Phase == TournamentPhase.QuarterFinals).ToList();
        Assert.AreEqual(4, qfMatches.Count);
        CollectionAssert.AreEquivalent(new[] { "D1", "D2", "D3", "D4" }, qfMatches.Select(m => m.MatchCode).ToList());
    }

    [TestMethod]
    public void InitializeSemiFinals_WithoutInitializedKnockout_ShouldThrowInvalidOperationException()
    {
        var fixtureService = CreateFixtureService();
        Assert.ThrowsException<InvalidOperationException>(() => fixtureService.InitializeSemiFinals());
    }

    [TestMethod]
    public void InitializeSemiFinals_WhenAllPreviousPhasesAreFinished_ShouldCreateNewMatches()
    {
        var service = CreateFixtureServiceWithSimulatedGroupStage();
        service.InitializeKnockoutStage(42);
        FinishAllPhaseMatches(service, TournamentPhase.RoundOf32);
        service.InitializeRoundOf16();
        FinishAllPhaseMatches(service, TournamentPhase.RoundOf16);
        service.InitializeQuarterFinals();
        FinishAllPhaseMatches(service, TournamentPhase.QuarterFinals);
        service.InitializeSemiFinals();
        var sfMatches = _matchRepository.GetAll().Where(m => m.Phase == TournamentPhase.SemiFinals).ToList();
        Assert.AreEqual(2, sfMatches.Count);
        CollectionAssert.AreEquivalent(new[] { "S1", "S2" }, sfMatches.Select(m => m.MatchCode).ToList());
    }

    [TestMethod]
    public void InitializeThirdPlace_WithoutInitializedKnockout_ShouldThrowInvalidOperationException()
    {
        var fixtureService = CreateFixtureService();
        Assert.ThrowsException<InvalidOperationException>(() => fixtureService.InitializeThirdPlace());
    }

    [TestMethod]
    public void InitializeThirdPlace_WhenAllPreviousPhasesAreFinished_ShouldCreateNewMatch()
    {
        var service = CreateFixtureServiceWithSimulatedGroupStage();
        service.InitializeKnockoutStage(42);
        FinishAllPhaseMatches(service, TournamentPhase.RoundOf32);
        service.InitializeRoundOf16();
        FinishAllPhaseMatches(service, TournamentPhase.RoundOf16);
        service.InitializeQuarterFinals();
        FinishAllPhaseMatches(service, TournamentPhase.QuarterFinals);
        service.InitializeSemiFinals();
        FinishAllPhaseMatches(service, TournamentPhase.SemiFinals);
        service.InitializeThirdPlace();
        var tpMatches = _matchRepository.GetAll().Where(m => m.Phase == TournamentPhase.ThirdPlace).ToList();
        Assert.AreEqual(1, tpMatches.Count);
        Assert.AreEqual("T1", tpMatches.Single().MatchCode);
    }

    [TestMethod]
    public void InitializeFinal_WithoutInitializedKnockout_ShouldThrowInvalidOperationException()
    {
        var fixtureService = CreateFixtureService();
        Assert.ThrowsException<InvalidOperationException>(() => fixtureService.InitializeFinal());
    }

    [TestMethod]
    public void InitializeFinal_WhenAllPreviousPhasesAreFinished_ShouldCreateNewMatch()
    {
        var service = CreateFixtureServiceWithSimulatedGroupStage();
        service.InitializeKnockoutStage(42);
        FinishAllPhaseMatches(service, TournamentPhase.RoundOf32);
        service.InitializeRoundOf16();
        FinishAllPhaseMatches(service, TournamentPhase.RoundOf16);
        service.InitializeQuarterFinals();
        FinishAllPhaseMatches(service, TournamentPhase.QuarterFinals);
        service.InitializeSemiFinals();
        FinishAllPhaseMatches(service, TournamentPhase.SemiFinals);
        service.InitializeFinal();
        var finalMatches = _matchRepository.GetAll().Where(m => m.Phase == TournamentPhase.Final).ToList();
        Assert.AreEqual(1, finalMatches.Count);
        Assert.AreEqual("F1", finalMatches.Single().MatchCode);
    }

    [TestMethod]
    public void SetKnockoutMatchResult_WithValidData_ShouldUpdateRepositoryAndFinishMatch()
    {
        var service = CreateFixtureServiceWithSimulatedGroupStage();
        service.InitializeKnockoutStage(42);
        service.SetKnockoutMatchResult("A1", 2, 1);
        var updatedMatch = _matchRepository.GetAll().Single(m => m.MatchCode == "A1");
        Assert.AreEqual(2, updatedMatch.HomeGoals);
        Assert.AreEqual(1, updatedMatch.AwayGoals);
        Assert.AreEqual(MatchStatus.Finished, updatedMatch.Status);
        Assert.IsNotNull(updatedMatch.Winner);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void SetKnockoutMatchResult_WithDraw_ShouldThrowArgumentException()
    {
        var service = CreateFixtureServiceWithSimulatedGroupStage();
        service.InitializeKnockoutStage(42);
        service.SetKnockoutMatchResult("A1", 1, 1);
    }

    [TestMethod]
    public void InitializeRoundOf16_WhenAllRoundOf32MatchesAreFinished_ShouldCreateNewMatches()
    {
        var service = CreateFixtureServiceWithSimulatedGroupStage();
        service.InitializeKnockoutStage(42);
        var r32Matches = _matchRepository.GetAll().Where(m => m.Phase == TournamentPhase.RoundOf32).ToList();
        foreach (var match in r32Matches)
            service.SetKnockoutMatchResult(match.MatchCode!, 2, 1);
        service.InitializeRoundOf16();
        var r16Matches = _matchRepository.GetAll().Where(m => m.Phase == TournamentPhase.RoundOf16).ToList();
        Assert.AreEqual(8, r16Matches.Count);
        Assert.IsTrue(r16Matches.Any(m => m.MatchCode == "C1"));
    }

    [TestMethod]
    public void InitializeRoundOf16_WhenSomeMatchesAreNotFinished_ShouldThrowInvalidOperationException()
    {
        var service = CreateFixtureServiceWithSimulatedGroupStage();
        service.InitializeKnockoutStage(42);
        service.SetKnockoutMatchResult("A1", 2, 0);
        Assert.ThrowsException<InvalidOperationException>(() => service.InitializeRoundOf16());
    }

    [TestMethod]
    public void GetKnockoutMatches_ShouldReturnMappedResponses()
    {
        var service = CreateFixtureServiceWithSimulatedGroupStage();
        service.InitializeKnockoutStage(42);
        var responses = service.GetKnockoutMatches();
        Assert.IsInstanceOfType(responses.First(), typeof(MatchResponse));
        Assert.IsNotNull(responses.First().HomeTeam);
        Assert.IsNotNull(responses.First().MatchCode);
    }

    [TestMethod]
    public void SimulateKnockoutStage_WithoutInitializedKnockout_ShouldThrowInvalidOperationException()
    {
        var fixtureService = CreateFixtureService();
        Assert.ThrowsException<InvalidOperationException>(() => fixtureService.SimulateKnockoutStage(42));
    }

    [TestMethod]
    public void SimulateKnockoutStage_ShouldSetResultAndFinishAllKnockoutMatches()
    {
        var service = CreateFixtureServiceWithSimulatedGroupStage();
        service.InitializeKnockoutStage(42);
        service.SimulateKnockoutStage(42);
        var knockoutMatches = _matchRepository.GetAll().Where(m => m.Phase != TournamentPhase.GroupStage).ToList();
        Assert.IsTrue(knockoutMatches.All(m => m.HomeGoals.HasValue && m.AwayGoals.HasValue));
        Assert.IsTrue(knockoutMatches.All(m => m.Status == MatchStatus.Finished));
    }

    [TestMethod]
    public void SimulateKnockoutStage_ShouldProduceNoDraws()
    {
        var service = CreateFixtureServiceWithSimulatedGroupStage();
        service.InitializeKnockoutStage(42);
        service.SimulateKnockoutStage(42);
        var knockoutMatches = _matchRepository.GetAll().Where(m => m.Phase != TournamentPhase.GroupStage).ToList();
        Assert.IsTrue(knockoutMatches.All(m => m.Winner != null));
    }

    [TestMethod]
    public void SimulateKnockoutStage_WithSameSeed_ShouldProduceSameWinners()
    {
        var service1 = CreateFixtureServiceWithSimulatedGroupStage();
        service1.InitializeKnockoutStage(42);
        service1.SimulateKnockoutStage(42);
        var winners1 = _matchRepository.GetAll()
            .Where(m => m.Phase != TournamentPhase.GroupStage)
            .OrderBy(m => m.MatchCode)
            .Select(m => $"{m.MatchCode}:{m.Winner?.Name}")
            .ToList();

        var service2 = CreateFixtureServiceWithSimulatedGroupStage();
        service2.InitializeKnockoutStage(42);
        service2.SimulateKnockoutStage(42);
        var winners2 = _matchRepository.GetAll()
            .Where(m => m.Phase != TournamentPhase.GroupStage)
            .OrderBy(m => m.MatchCode)
            .Select(m => $"{m.MatchCode}:{m.Winner?.Name}")
            .ToList();

        CollectionAssert.AreEqual(winners1, winners2);
    }

    private IFixtureService CreateFixtureServiceWithSimulatedGroupStage()
    {
        var fixtureService = CreateFixtureServiceReadyWithPots(48);
        fixtureService.GenerateFixture(42, new DateTime(2026, 6, 1));
        fixtureService.SimulateGroupStage(42);
        return fixtureService;
    }

    private void FinishAllPhaseMatches(IFixtureService service, TournamentPhase phase)
    {
        var matches = _matchRepository.GetAll().Where(m => m.Phase == phase).ToList();
        foreach (var match in matches)
            service.SetKnockoutMatchResult(match.MatchCode!, 2, 1);
    }
}