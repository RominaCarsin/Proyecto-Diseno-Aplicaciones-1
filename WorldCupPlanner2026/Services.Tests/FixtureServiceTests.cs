using Domain;
using IRepository;
using IServices;
using Repository;
using Services;

namespace Services.Tests;

[TestClass]
public class FixtureServiceTests
{
    private const int RequiredTeamCount = 48;
    private const int RequiredStadiumCount = 4;
    private const int TotalGroupCount = 12;
    private const int TeamsPerGroup = 4;
    private const int MatchesPerGroup = 6;
    private const int TotalMatchCount = 72;
    private const int DefaultSeed = 42;

    private static readonly DateTime DefaultStartDate = new(2026, 6, 1);

    private IRepository<Group> _groupRepository = null!;

    // ─── GenerateFixture — validaciones ───────────────────────────────────────

    [TestMethod]
    public void GenerateFixture_WithLessThan48Teams_ShouldThrowInvalidOperationException()
    {
        var service = BuildFixtureServiceWithNoTeamsAndNoStadiums();

        Assert.ThrowsException<InvalidOperationException>(() =>
            service.GenerateFixture(DefaultSeed, DefaultStartDate));
    }

    [TestMethod]
    public void GenerateFixture_WithLessThan4Stadiums_ShouldThrowInvalidOperationException()
    {
        var service = BuildFixtureServiceWith48TeamsAndNoStadiums();

        Assert.ThrowsException<InvalidOperationException>(() =>
            service.GenerateFixture(DefaultSeed, DefaultStartDate));
    }

    [TestMethod]
    public void GenerateFixture_WithExistingFixture_ShouldThrowInvalidOperationException()
    {
        var service = BuildFixtureServiceReady();
        service.GenerateFixture(DefaultSeed, DefaultStartDate);

        Assert.ThrowsException<InvalidOperationException>(() =>
            service.GenerateFixture(DefaultSeed, DefaultStartDate));
    }

    // ─── GenerateFixture — estructura ─────────────────────────────────────────

    [TestMethod]
    public void GenerateFixture_WithValidData_ShouldCreate12Groups()
    {
        var service = BuildFixtureServiceReady();

        service.GenerateFixture(DefaultSeed, DefaultStartDate);

        Assert.AreEqual(TotalGroupCount, _groupRepository.GetAll().Count());
    }

    [TestMethod]
    public void GenerateFixture_WithValidData_ShouldAssign4TeamsPerGroup()
    {
        var service = BuildFixtureServiceReady();

        service.GenerateFixture(DefaultSeed, DefaultStartDate);

        Assert.IsTrue(_groupRepository.GetAll().All(g => g.Teams.Count == TeamsPerGroup));
    }

    [TestMethod]
    public void GenerateFixture_WithValidData_ShouldCreate6MatchesPerGroup()
    {
        var service = BuildFixtureServiceReady();

        service.GenerateFixture(DefaultSeed, DefaultStartDate);

        Assert.IsTrue(_groupRepository.GetAll().All(g => g.Matches.Count == MatchesPerGroup));
    }

    [TestMethod]
    public void GenerateFixture_WithValidData_ShouldCreate72TotalMatches()
    {
        var service = BuildFixtureServiceReady();

        service.GenerateFixture(DefaultSeed, DefaultStartDate);

        Assert.AreEqual(TotalMatchCount, GetAllMatches().Count());
    }

    [TestMethod]
    public void GenerateFixture_WithValidData_ShouldCreateGroupsWithLabelsAToL()
    {
        var service = BuildFixtureServiceReady();

        service.GenerateFixture(DefaultSeed, DefaultStartDate);

        var labels = _groupRepository.GetAll()
            .Select(g => g.Label)
            .OrderBy(l => l)
            .ToList();

        var expectedLabels = Enumerable.Range(0, TotalGroupCount)
            .Select(i => (char)('A' + i))
            .ToList();

        CollectionAssert.AreEqual(expectedLabels, labels);
    }

    // ─── GenerateFixture — confederaciones ────────────────────────────────────

    [TestMethod]
    public void GenerateFixture_ShouldNotAssignMoreThan2UefaTeamsPerGroup()
    {
        var service = BuildFixtureServiceReady();

        service.GenerateFixture(DefaultSeed, DefaultStartDate);

        Assert.IsTrue(_groupRepository.GetAll().All(g =>
            g.Teams.Count(t => t.ConfederationName == "UEFA") <= 2));
    }

    [TestMethod]
    public void GenerateFixture_ShouldNotAssignMoreThan1NonUefaTeamPerConfederationPerGroup()
    {
        var service = BuildFixtureServiceReady();

        service.GenerateFixture(DefaultSeed, DefaultStartDate);

        Assert.IsTrue(_groupRepository.GetAll().All(g =>
            g.Teams
                .Where(t => t.ConfederationName != "UEFA")
                .GroupBy(t => t.ConfederationName)
                .All(conf => conf.Count() <= 1)));
    }

    [TestMethod]
    public void GenerateFixture_WhenNoGroupCanAcceptTeam_ShouldThrowInvalidOperationException()
    {
        var service = BuildFixtureServiceWithAllTeamsFromSameNonUefaConfederation();

        Assert.ThrowsException<InvalidOperationException>(() =>
            service.GenerateFixture(DefaultSeed, DefaultStartDate));
    }

    // ─── GenerateFixture — fases y grupos ─────────────────────────────────────

    [TestMethod]
    public void GenerateFixture_ShouldAssignGroupStagePhaseToAllMatches()
    {
        var service = BuildFixtureServiceReady();

        service.GenerateFixture(DefaultSeed, DefaultStartDate);

        Assert.IsTrue(GetAllMatches().All(m => m.Phase == TournamentPhase.GroupStage));
    }

    [TestMethod]
    public void GenerateFixture_ShouldAssignGroupLabelToAllMatches()
    {
        var service = BuildFixtureServiceReady();

        service.GenerateFixture(DefaultSeed, DefaultStartDate);

        Assert.IsTrue(_groupRepository.GetAll()
            .All(g => g.Matches.All(m => m.Group?.Label == g.Label)));
    }

    // ─── GenerateFixture — calendario ─────────────────────────────────────────

    [TestMethod]
    public void GenerateFixture_ShouldNotCreateMoreThan3MatchesPerDay()
    {
        var service = BuildFixtureServiceReady();

        service.GenerateFixture(DefaultSeed, DefaultStartDate);

        var matchesByDay = GetAllMatches().GroupBy(m => m.Date.Date);
        Assert.IsTrue(matchesByDay.All(g => g.Count() <= 3));
    }

    [TestMethod]
    public void GenerateFixture_ShouldStartMatchesAt14Hours()
    {
        var service = BuildFixtureServiceReady();

        service.GenerateFixture(DefaultSeed, DefaultStartDate);

        Assert.IsTrue(GetAllMatches().All(m => m.Date.Hour >= 14));
    }

    [TestMethod]
    public void GenerateFixture_ShouldSeparateMatchesWith4HoursBetweenMatchesInSameDay()
    {
        var service = BuildFixtureServiceReady();

        service.GenerateFixture(DefaultSeed, DefaultStartDate);

        foreach (var group in _groupRepository.GetAll())
        {
            var matchesPerDay = group.Matches
                .GroupBy(m => m.Date.Date)
                .Where(g => g.Count() > 1)
                .Where(g => g.Select(m => m.Date).Distinct().Count() > 1);

            foreach (var day in matchesPerDay)
            {
                var times = day.Select(m => m.Date.TimeOfDay).OrderBy(t => t).ToList();
                for (int i = 1; i < times.Count; i++)
                    Assert.AreEqual(TimeSpan.FromHours(4), times[i] - times[i - 1]);
            }
        }
    }

    [TestMethod]
    public void GenerateFixture_ShouldScheduleEachGroupRoundsWith3DaysBetweenRounds()
    {
        var service = BuildFixtureServiceReady();

        service.GenerateFixture(DefaultSeed, DefaultStartDate);

        foreach (var group in _groupRepository.GetAll())
        {
            var matches = group.Matches.ToList();
            var round1Date = matches[0].Date.Date;
            var round2Date = matches[2].Date.Date;
            var round3Date = matches[4].Date.Date;

            Assert.AreEqual(round1Date.AddDays(3), round2Date);
            Assert.AreEqual(round2Date.AddDays(3), round3Date);
        }
    }

    [TestMethod]
    public void GenerateFixture_ShouldScheduleLastRoundMatchesAtSameDateAndTime()
    {
        var service = BuildFixtureServiceReady();

        service.GenerateFixture(DefaultSeed, DefaultStartDate);

        foreach (var group in _groupRepository.GetAll())
        {
            var matches = group.Matches.ToList();
            Assert.AreEqual(matches[4].Date, matches[5].Date);
        }
    }

    [TestMethod]
    public void GenerateFixture_ShouldScheduleGroupsWithSevenDayOffset()
    {
        var service = BuildFixtureServiceReady();

        service.GenerateFixture(DefaultSeed, DefaultStartDate);

        var groups = _groupRepository.GetAll().OrderBy(g => g.Label).ToList();

        for (int i = 0; i < groups.Count; i++)
        {
            var expectedStart = DefaultStartDate.Date.AddDays(i * 7);
            var firstMatchDate = groups[i].Matches.Min(m => m.Date.Date);
            Assert.AreEqual(expectedStart, firstMatchDate);
        }
    }

    [TestMethod]
    public void GenerateFixture_WithNoStartDate_ShouldUseDefaultStartDate()
    {
        var service = BuildFixtureServiceReady();

        service.GenerateFixture(DefaultSeed);

        var firstMatchDate = GetAllMatches().Min(m => m.Date);
        Assert.AreEqual(new DateTime(2026, 6, 1, 14, 0, 0), firstMatchDate);
    }

    // ─── GenerateFixture — estadios ───────────────────────────────────────────

    [TestMethod]
    public void GenerateFixture_ShouldAssignStadiumsInCircularOrder()
    {
        var service = BuildFixtureServiceReady();

        service.GenerateFixture(DefaultSeed, DefaultStartDate);

        var allMatches = GetAllMatches().ToList();
        var expectedStadiums = Enumerable.Range(0, RequiredStadiumCount)
            .Select(i => $"STADIUM_{i}")
            .OrderBy(s => s)
            .ToList();

        Assert.IsTrue(allMatches.All(m => expectedStadiums.Contains(m.Stadium.Name)));
        for (int i = 0; i < allMatches.Count; i++)
            Assert.AreEqual(expectedStadiums[i % expectedStadiums.Count], allMatches[i].Stadium.Name);
    }

    // ─── GenerateFixture — determinismo ───────────────────────────────────────

    [TestMethod]
    public void GenerateFixture_WithSameSeed_ShouldProduceSameGroupAssignment()
    {
        var service1 = BuildFixtureServiceReady();
        service1.GenerateFixture(DefaultSeed, DefaultStartDate);
        var teams1 = GetTeamNamesOrderedByGroup();

        var service2 = BuildFixtureServiceReady();
        service2.GenerateFixture(DefaultSeed, DefaultStartDate);
        var teams2 = GetTeamNamesOrderedByGroup();

        CollectionAssert.AreEqual(teams1, teams2);
    }

    [TestMethod]
    public void GenerateFixture_WithDifferentSeeds_ShouldProduceDifferentGroupAssignment()
    {
        var service1 = BuildFixtureServiceReadyWithTiedRankings();
        service1.GenerateFixture(seed: 1, DefaultStartDate);
        var teams1 = GetTeamNamesOrderedByGroup();

        var service2 = BuildFixtureServiceReadyWithTiedRankings();
        service2.GenerateFixture(seed: 99, DefaultStartDate);
        var teams2 = GetTeamNamesOrderedByGroup();

        CollectionAssert.AreNotEqual(teams1, teams2);
    }

    // ─── CanGenerateFixture ───────────────────────────────────────────────────

    [TestMethod]
    public void CanGenerateFixture_WithAllConditionsMet_ShouldReturnTrue()
    {
        var service = BuildFixtureServiceReady();

        Assert.IsTrue(service.CanGenerateFixture());
    }

    [TestMethod]
    public void CanGenerateFixture_WithLessThan48Teams_ShouldReturnFalse()
    {
        var service = BuildFixtureServiceWithNoTeamsAndNoStadiums();

        Assert.IsFalse(service.CanGenerateFixture());
    }

    [TestMethod]
    public void CanGenerateFixture_WithLessThan4Stadiums_ShouldReturnFalse()
    {
        var service = BuildFixtureServiceWith48TeamsAndNoStadiums();

        Assert.IsFalse(service.CanGenerateFixture());
    }

    [TestMethod]
    public void CanGenerateFixture_WithExistingFixture_ShouldReturnFalse()
    {
        var service = BuildFixtureServiceReady();
        service.GenerateFixture(DefaultSeed, DefaultStartDate);

        Assert.IsFalse(service.CanGenerateFixture());
    }

    // ─── GetStadiumCount ──────────────────────────────────────────────────────

    [TestMethod]
    public void GetStadiumCount_WithNoStadiums_ShouldReturnZero()
    {
        var service = BuildFixtureServiceWithNoTeamsAndNoStadiums();

        Assert.AreEqual(0, service.GetStadiumCount());
    }

    [TestMethod]
    public void GetStadiumCount_WithExistingStadiums_ShouldReturnCorrectCount()
    {
        var service = BuildFixtureServiceReady();

        Assert.AreEqual(RequiredStadiumCount, service.GetStadiumCount());
    }

    // ─── GeneratePots ─────────────────────────────────────────────────────────

    [TestMethod]
    public void GeneratePots_WithSameSeed_ShouldProduceSameTeamOrder()
    {
        var teamRepo1 = new Repository<Team>();
        BuildTeamService(teamRepo1).AutoCompleteTeams(RequiredTeamCount);
        var potService1 = new PotService(teamRepo1, new Repository<Pot>());
        potService1.GeneratePots(DefaultSeed);
        var order1 = potService1.GetPots()
            .SelectMany(p => p.Teams.Select(t => t.Name))
            .ToList();

        var teamRepo2 = new Repository<Team>();
        BuildTeamService(teamRepo2).AutoCompleteTeams(RequiredTeamCount);
        var potService2 = new PotService(teamRepo2, new Repository<Pot>());
        potService2.GeneratePots(DefaultSeed);
        var order2 = potService2.GetPots()
            .SelectMany(p => p.Teams.Select(t => t.Name))
            .ToList();

        CollectionAssert.AreEqual(order1, order2);
    }

    [TestMethod]
    public void GeneratePots_WithDifferentSeeds_ShouldProduceDifferentTeamOrder()
    {
        var teamRepo1 = BuildTeamRepoWithTiedRankings();
        var potService1 = new PotService(teamRepo1, new Repository<Pot>());
        potService1.GeneratePots(seed: 48);
        var order1 = potService1.GetPots()
            .SelectMany(p => p.Teams.Select(t => t.Name))
            .ToList();

        var teamRepo2 = BuildTeamRepoWithTiedRankings();
        var potService2 = new PotService(teamRepo2, new Repository<Pot>());
        potService2.GeneratePots(seed: 99);
        var order2 = potService2.GetPots()
            .SelectMany(p => p.Teams.Select(t => t.Name))
            .ToList();

        CollectionAssert.AreNotEqual(order1, order2);
    }

    // ─── SimulateGroupStage ───────────────────────────────────────────────────

    [TestMethod]
    public void SimulateGroupStage_WithNoFixtureGenerated_ShouldThrowInvalidOperationException()
    {
        var service = BuildFixtureServiceWithNoTeamsAndNoStadiums();

        Assert.ThrowsException<InvalidOperationException>(() =>
            service.SimulateGroupStage(DefaultSeed));
    }

    [TestMethod]
    public void SimulateGroupStage_ShouldSetResultOnAllMatches()
    {
        var service = BuildFixtureServiceReady();
        service.GenerateFixture(DefaultSeed, DefaultStartDate);

        service.SimulateGroupStage(DefaultSeed);

        Assert.IsTrue(GetAllMatches().All(m => m.HomeGoals.HasValue && m.AwayGoals.HasValue));
    }

    [TestMethod]
    public void SimulateGroupStage_ShouldProduceAtLeastOneMatchWithWinner()
    {
        var service = BuildFixtureServiceReady();
        service.GenerateFixture(DefaultSeed, DefaultStartDate);

        service.SimulateGroupStage(DefaultSeed);

        Assert.IsTrue(GetAllMatches().Any(m => m.Winner != null));
    }

    [TestMethod]
    public void SimulateGroupStage_WithSameSeed_ShouldProduceSameWinners()
    {
        var service1 = BuildFixtureServiceReady();
        service1.GenerateFixture(DefaultSeed, DefaultStartDate);
        service1.SimulateGroupStage(DefaultSeed);
        var winners1 = GetAllMatches().Select(m => m.Winner?.Name).ToList();

        var service2 = BuildFixtureServiceReady();
        service2.GenerateFixture(DefaultSeed, DefaultStartDate);
        service2.SimulateGroupStage(DefaultSeed);
        var winners2 = GetAllMatches().Select(m => m.Winner?.Name).ToList();

        CollectionAssert.AreEqual(winners1, winners2);
    }

    // ─── Builders ─────────────────────────────────────────────────────────────

    private FixtureService BuildFixtureServiceWithNoTeamsAndNoStadiums()
    {
        var groupRepo = new Repository<Group>();
        _groupRepository = groupRepo;

        return BuildFixtureService(
            teamRepo: new Repository<Team>(),
            stadiumRepo: new Repository<Stadium>(),
            groupRepo: groupRepo,
            potRepo: null);
    }

    private FixtureService BuildFixtureServiceWith48TeamsAndNoStadiums()
    {
        var groupRepo = new Repository<Group>();
        _groupRepository = groupRepo;

        var teamRepo = new Repository<Team>();
        AddTeamsWithDistinctConfederations(teamRepo);

        return BuildFixtureService(
            teamRepo: teamRepo,
            stadiumRepo: new Repository<Stadium>(),
            groupRepo: groupRepo,
            potRepo: null);
    }

    private FixtureService BuildFixtureServiceReady()
    {
        var groupRepo = new Repository<Group>();
        _groupRepository = groupRepo;

        var teamRepo = new Repository<Team>();
        AddTeamsWithDistinctConfederations(teamRepo);

        var stadiumRepo = new Repository<Stadium>();
        AddStadiums(stadiumRepo, RequiredStadiumCount);

        return BuildFixtureService(teamRepo, stadiumRepo, groupRepo, new Repository<Pot>());
    }

    private FixtureService BuildFixtureServiceReadyWithTiedRankings()
    {
        var groupRepo = new Repository<Group>();
        _groupRepository = groupRepo;

        var teamRepo = BuildTeamRepoWithTiedRankings();

        var stadiumRepo = new Repository<Stadium>();
        AddStadiums(stadiumRepo, RequiredStadiumCount);

        return BuildFixtureService(teamRepo, stadiumRepo, groupRepo, new Repository<Pot>());
    }

    private FixtureService BuildFixtureServiceWithAllTeamsFromSameNonUefaConfederation()
    {
        var groupRepo = new Repository<Group>();
        _groupRepository = groupRepo;

        var teamRepo = new Repository<Team>();
        for (int i = 1; i <= RequiredTeamCount; i++)
            teamRepo.Add(new Team($"AFC_{i:00}", "AFC", 1000 - i));

        var stadiumRepo = new Repository<Stadium>();
        AddStadiums(stadiumRepo, RequiredStadiumCount);

        return BuildFixtureService(teamRepo, stadiumRepo, groupRepo, new Repository<Pot>());
    }

    private static FixtureService BuildFixtureService(
        IRepository<Team> teamRepo,
        IRepository<Stadium> stadiumRepo,
        IRepository<Group> groupRepo,
        IRepository<Pot>? potRepo)
    {
        var teamService = BuildTeamService(teamRepo);
        var stadiumService = BuildStadiumService(stadiumRepo);
        var groupService = new GroupService(groupRepo);
        var matchRepo = new Repository<Match>();
        var potService = potRepo is not null
            ? new PotService(teamRepo, potRepo)
            : null;

        return new FixtureService(
            teamService, stadiumService, groupService,
            groupRepo, matchRepo, potService!);
    }

    private static TeamService BuildTeamService(IRepository<Team> teamRepository)
    {
        var auditService = new AuditService(new Repository<AuditLog>());
        var sessionService = new SessionService(null!, new AuthStateService());
        return new TeamService(teamRepository, new ConfederationService(), auditService, sessionService);
    }

    private static StadiumService BuildStadiumService(IRepository<Stadium> stadiumRepository)
    {
        var auditService = new AuditService(new Repository<AuditLog>());
        var sessionService = new SessionService(null!, new AuthStateService());
        return new StadiumService(stadiumRepository, auditService, sessionService);
    }

    private static Repository<Team> BuildTeamRepoWithTiedRankings()
    {
        var repo = new Repository<Team>();
        var slots = new Dictionary<string, int>
        {
            { "UEFA", 16 }, { "CONMEBOL", 7 }, { "CONCACAF", 7 },
            { "CAF", 9 }, { "AFC", 8 }, { "OFC", 1 }
        };
        foreach (var (conf, count) in slots)
            for (int i = 1; i <= count; i++)
                repo.Add(new Team($"{conf}_{i:00}", conf, 1000));
        return repo;
    }

    private static void AddTeamsWithDistinctConfederations(IRepository<Team> teamRepo)
    {
        var slots = new Dictionary<string, int>
        {
            { "UEFA", 16 }, { "CONMEBOL", 7 }, { "CONCACAF", 7 },
            { "CAF", 9 }, { "AFC", 8 }, { "OFC", 1 }
        };
        int ranking = 1500;
        foreach (var (conf, count) in slots)
            for (int i = 1; i <= count; i++)
                teamRepo.Add(new Team($"{conf}_{i:00}", conf, ranking--));
    }

    private static void AddStadiums(IRepository<Stadium> stadiumRepo, int count)
    {
        for (int i = 0; i < count; i++)
            stadiumRepo.Add(new Stadium($"STADIUM_{i}", "CITY", null, 50000));
    }

    private IEnumerable<Match> GetAllMatches() =>
        _groupRepository.GetAll().SelectMany(g => g.Matches);

    private List<string> GetTeamNamesOrderedByGroup() =>
        _groupRepository.GetAll()
            .OrderBy(g => g.Label)
            .SelectMany(g => g.Teams.Select(t => t.Name))
            .ToList();
}