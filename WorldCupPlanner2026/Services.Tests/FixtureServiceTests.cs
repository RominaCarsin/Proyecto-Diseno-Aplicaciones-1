using Domain;
using IRepository;
using IServices;
using Repository;
using Services;

namespace Services.Tests;

[TestClass]
public class FixtureServiceTests
{
    private IRepository<Group> _groupRepository = null!;
    private IRepository<Match> _matchRepository;

    [TestMethod]
    public void GenerateFixture_WhenLessThan48Teams_ShouldThrowInvalidOperationException()
    {
        var fixtureService = CreateFixtureService();

        Assert.ThrowsException<InvalidOperationException>(() =>
            fixtureService.GenerateFixture(42, new DateTime(2026, 6, 1)));
    }

    private FixtureService CreateFixtureService()
    {
        var groupRepo = new Repository<Group>();
        var matchRepo = new Repository<Match>();
        _groupRepository = groupRepo;
        _matchRepository = matchRepo;

        var teamRepo = new Repository<Team>();
        var confederationService = new ConfederationService();
        var teamService = CreateTeamService(teamRepo, confederationService);

        var stadiumRepo = new Repository<Stadium>();
        var stadiumService = CreateStadiumService(stadiumRepo);

        var groupService = new GroupService(groupRepo);

        return new FixtureService(teamService, stadiumService, groupService, groupRepo, matchRepo, null);
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
        var groupRepo = new Repository<Group>();
        var matchRepo = new Repository<Match>();
        _groupRepository = groupRepo;

        var teamRepo = new Repository<Team>();
        var confederationService = new ConfederationService();
        var teamService = CreateTeamService(teamRepo, confederationService);

        for (int i = 0; i < 48; i++)
            teamRepo.Add(new Team($"TEAM_{i:00}", "UEFA", 1000 + i));

        var stadiumRepo = new Repository<Stadium>();
        var stadiumService = CreateStadiumService(stadiumRepo);

        var groupService = new GroupService(groupRepo);

        return new FixtureService(teamService, stadiumService, groupService, groupRepo, matchRepo, null);
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
    
    [TestMethod] public void CanGenerateFixture_WithExistingFixture_ShouldReturnFalse() 
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
    
    
    [TestMethod]
    public void GeneratePots_WithSameSeed_ShouldProduceSameOrder()
    {
        var teamRepo1 = new  Repository<Team>();
        var teamService = CreateTeamService(teamRepo1, new ConfederationService());
        teamService.AutoCompleteTeams(48);
        
        var potRepo1 = new Repository<Pot>();
        var potService1 = new PotService(teamRepo1, potRepo1);
        potService1.GeneratePots(48);
        var order1 = potRepo1.GetAll().SelectMany(p => p.Teams.Select(t => t.Name)).ToList();

        var teamRepo2 = new  Repository<Team>();
        var teamService2 = CreateTeamService(teamRepo2, new ConfederationService());
        teamService2.AutoCompleteTeams(48);
        
        var potRepo2 = new Repository<Pot>();
        var potService2 = new PotService(teamRepo2, potRepo2);
        potService2.GeneratePots(48);
        var order2 = potRepo2.GetAll().SelectMany(p => p.Teams.Select(t => t.Name)).ToList();

        CollectionAssert.AreEqual(order1, order2);
    }
    
    [TestMethod]
    public void GeneratePots_WithDifferentSeeds_ShouldProduceDifferentOrder_WhenRankingsAreTied()
    {
        var teamRepo1 = CreateTeamRepoWithTiedRankings();

        var potRepo1 = new Repository<Pot>();
        var potService1 = new PotService(teamRepo1, potRepo1);

        potService1.GeneratePots(48);

        var order1 = potRepo1.GetAll()
            .SelectMany(p => p.Teams.Select(t => t.Name))
            .ToList();

        var teamRepo2 = CreateTeamRepoWithTiedRankings();

        var potRepo2 = new Repository<Pot>();
        var potService2 = new PotService(teamRepo2, potRepo2);

        potService2.GeneratePots(99);

        var order2 = potRepo2.GetAll()
            .SelectMany(p => p.Teams.Select(t => t.Name))
            .ToList();

        CollectionAssert.AreNotEqual(order1, order2);
    }
    
    private Repository<Team> CreateTeamRepoWithTiedRankings()
    {
        var repo = new Repository<Team>();

        for (int i = 1; i <= 48; i++)
        {
            repo.Add(new Team(
                $"Team {i}",
                "CONMEBOL",
                1000
            ));
        }

        return repo;
    }
    
    [TestMethod]
    public void GenerateFixture_WithSameSeed_ShouldProduceSameGroupAssignment()
    {
        var service1 = CreateFixtureServiceReadyWithPots(42);
        service1.GenerateFixture(42, new DateTime(2026, 6, 1));
        var teams1 = _groupRepository.GetAll()
            .OrderBy(g => g.Label)
            .SelectMany(g => g.Teams.Select(t => t.Name))
            .ToList();

        var service2 = CreateFixtureServiceReadyWithPots(50);
        service2.GenerateFixture(42, new DateTime(2026, 6, 1));
        var teams2 = _groupRepository.GetAll()
            .OrderBy(g => g.Label)
            .SelectMany(g => g.Teams.Select(t => t.Name))
            .ToList();

        CollectionAssert.AreEqual(teams1, teams2);
    }
    //
    [TestMethod]
    public void GroupAdds3UEFATeams_ShouldInsertTeamInNextAvailableGroup()
    {
        var fixtureService = CreateFixtureServiceReadyWithPots(48);

        fixtureService.GenerateFixture(48, new DateTime(2026, 6, 1));

        var groups = _groupRepository.GetAll().ToList();

        Assert.IsTrue(groups.All(g =>
            g.Teams.Count(t => t.ConfederationName == "UEFA") <= 2));
    }
    
    [TestMethod]
    public void GroupAdds2NotUEFATeams_ShouldInsertTeamInNextAvailableGroup()
    {
        var fixtureService = CreateFixtureServiceReadyWithPots(48);

        fixtureService.GenerateFixture(48, new DateTime(2026, 6, 1));

        var groups = _groupRepository.GetAll().ToList();

        Assert.IsTrue(groups.All(g =>
            g.Teams
                .Where(t => t.ConfederationName != "UEFA")
                .GroupBy(t => t.ConfederationName)
                .All(conf => conf.Count() <= 1)));
    }

    private FixtureService CreateFixtureServiceReadyWithPots(int seed = 42)
    {
        var groupRepo = new Repository<Group>();
        var matchRepo = new Repository<Match>();
        var potRepo = new Repository<Pot>();
        _groupRepository = groupRepo;
        _matchRepository = matchRepo;

        var teamRepo = new Repository<Team>();
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

        var stadiumRepo = new Repository<Stadium>();
        var stadiumService = CreateStadiumService(stadiumRepo);
        for (int i = 0; i < 4; i++)
            stadiumRepo.Add(new Stadium($"STADIUM_{i}", "City", null, 50000));

        var groupService = new GroupService(groupRepo);
        var potService = new PotService(teamRepo, potRepo);

        return new FixtureService(teamService, stadiumService, groupService, groupRepo, matchRepo, potService);
    }
    
    [TestMethod]
    public void GenerateFixture_ShouldNotCreateMoreThan3MatchesPerDay()
    {
        var fixtureService = CreateFixtureServiceReadyWithPots(48);
        var startDate = new DateTime(2026, 6, 1);

        fixtureService.GenerateFixture(42, startDate);

        var matchesByStadiumAndDay = _matchRepository.GetAll()
            .GroupBy(m => new
            {
                Date = m.Date.Date,
                Stadium = m.Stadium.Name
            });

        Assert.IsTrue(matchesByStadiumAndDay.All(g => g.Count() <= 3));
    }
    
    [TestMethod]
    public void GenerateFixture_ShouldScheduleEachGroupRoundsWith3DaysBetweenRounds()
    {
        var fixtureService = CreateFixtureServiceReadyWithPots(48);
        var startDate = new DateTime(2026, 6, 1);

        fixtureService.GenerateFixture(42, startDate);

        foreach (var group in _groupRepository.GetAll())
        {
            var matches = group.Matches.OrderBy(m => m.Date).ToList();

            var round1Date = matches[0].Date.Date;
            var round2Date = matches[2].Date.Date;
            var round3Date = matches[4].Date.Date;

            Assert.AreEqual(round1Date.AddDays(3), round2Date);
            Assert.AreEqual(round2Date.AddDays(3), round3Date);
        }
    }
   
    [TestMethod]
    public void GenerateFixture_WithGroupStage_ShouldScheduleLastRoundMatchesAtSameDateAndTime()
    {
        var fixtureService = CreateFixtureServiceReadyWithPots(48);

        fixtureService.GenerateFixture(42, new DateTime(2026, 6, 1));

        foreach (var group in _groupRepository.GetAll())
        {
            var matches = group.Matches.ToList();

            Assert.AreEqual(matches[4].Date, matches[5].Date);
        }
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
    public void GenerateFixture_WithDifferentSeeds_ShouldProduceDifferentGroupAssignment()
    {
        var service1 = CreateFixtureServiceReadyWithPotsWithTiedRankings();
        service1.GenerateFixture(1, new DateTime(2026, 6, 1));
        var teams1 = _groupRepository.GetAll()
            .OrderBy(g => g.Label)
            .SelectMany(g => g.Teams.Select(t => t.Name))
            .ToList();

        var service2 = CreateFixtureServiceReadyWithPotsWithTiedRankings();
        service2.GenerateFixture(99, new DateTime(2026, 6, 1));
        var teams2 = _groupRepository.GetAll()
            .OrderBy(g => g.Label)
            .SelectMany(g => g.Teams.Select(t => t.Name))
            .ToList();

        CollectionAssert.AreNotEqual(teams1, teams2);
    }
    

[TestMethod]
public void GenerateFixture_ShouldStartMatchesAt14Hours()
{
    var fixtureService = CreateFixtureServiceReadyWithPots(48);

    fixtureService.GenerateFixture(42, new DateTime(2026, 6, 1));

    var allMatches = _groupRepository.GetAll()
        .SelectMany(g => g.Matches)
        .ToList();

    Assert.IsTrue(allMatches.All(m => m.Date.Hour >= 14));
}

[TestMethod]
public void GenerateFixture_ShouldSeparateMatchesWith4HoursBetweenMatchesInSameDay()
{
    var fixtureService = CreateFixtureServiceReadyWithPots(48);
    fixtureService.GenerateFixture(42, new DateTime(2026, 6, 1));

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
public void GenerateFixture_ShouldAssignStadiumsInCircularOrder()
{
    var fixtureService = CreateFixtureServiceReadyWithPots(48);

    fixtureService.GenerateFixture(42, new DateTime(2026, 6, 1));

    var allMatches = _groupRepository.GetAll()
        .SelectMany(g => g.Matches)
        .ToList();

    var stadiumNames = allMatches
        .Select(m => m.Stadium.Name)
        .ToList();

    var expectedStadiums = Enumerable.Range(0, 4)
        .Select(i => $"STADIUM_{i}")
        .OrderBy(s => s)
        .ToList();

    // Verificar que todos los estadios usados pertenecen al conjunto esperado
    Assert.IsTrue(stadiumNames.All(s => expectedStadiums.Contains(s)));

    // Verificar distribución circular: partido N usa estadio[N % 4]
    for (int i = 0; i < allMatches.Count; i++)
        Assert.AreEqual(expectedStadiums[i % expectedStadiums.Count], allMatches[i].Stadium.Name);
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

    var totalMatches = _groupRepository.GetAll()
        .SelectMany(g => g.Matches)
        .Count();

    Assert.AreEqual(72, totalMatches);
}

[TestMethod]
public void GenerateFixture_ShouldAssignPhaseGroupStageToAllMatches()
{
    var fixtureService = CreateFixtureServiceReadyWithPots(48);

    fixtureService.GenerateFixture(42, new DateTime(2026, 6, 1));

    var allMatches = _groupRepository.GetAll()
        .SelectMany(g => g.Matches)
        .ToList();

    Assert.IsTrue(allMatches.All(m => m.Phase == TournamentPhase.GroupStage));
}

[TestMethod]
public void GenerateFixture_ShouldAssignGroupLabelToAllMatches()
{
    var fixtureService = CreateFixtureServiceReadyWithPots(48);

    fixtureService.GenerateFixture(42, new DateTime(2026, 6, 1));

    foreach (var group in _groupRepository.GetAll())
    {
        Assert.IsTrue(group.Matches.All(m => m.Group == group.Label));
    }
}


[TestMethod]
public void GenerateFixture_WhenNoGroupCanAcceptTeam_ShouldThrowInvalidOperationException()
{
    // Crear 48 equipos todos de la misma confederación no-UEFA
    // Así los primeros 12 grupos quedan llenos y los siguientes no pueden aceptar
    // más equipos de esa confederación
    var groupRepo = new Repository<Group>();
    var matchRepo = new Repository<Match>();
    var potRepo = new Repository<Pot>();
    _groupRepository = groupRepo;

    var teamRepo = new Repository<Team>();

    // 48 equipos AFC: cada grupo solo admite 1, pero hay 12 grupos → 12 caben
    // Con 48 equipos del mismo confederación no-UEFA, el pot 2 en adelante falla
    for (int i = 1; i <= 48; i++)
        teamRepo.Add(new Team($"AFC_{i:00}", "AFC", 1000 - i));

    var stadiumRepo = new Repository<Stadium>();
    var stadiumService = CreateStadiumService(stadiumRepo);
    for (int i = 0; i < 4; i++)
        stadiumRepo.Add(new Stadium($"STADIUM_{i}", "City", null, 50000));

    var confederationService = new ConfederationService();
    var groupService = new GroupService(groupRepo);
    var potService = new PotService(teamRepo, potRepo);

    var fixtureService = new FixtureService(
        CreateTeamService(teamRepo, confederationService),
        stadiumService, groupService, groupRepo, matchRepo, potService);

    Assert.ThrowsException<InvalidOperationException>(() =>
        fixtureService.GenerateFixture(42, new DateTime(2026, 6, 1)));
}

    private FixtureService CreateFixtureServiceReadyWithPotsWithTiedRankings()
    {
        var groupRepo = new Repository<Group>();
        var matchRepo = new Repository<Match>();
        var potRepo = new Repository<Pot>();
        _groupRepository = groupRepo;

        var teamRepo = new Repository<Team>();
        var confederationService = new ConfederationService();

        var slots = new Dictionary<string, int>
        {
            { "UEFA", 16 }, { "CONMEBOL", 7 }, { "CONCACAF", 7 },
            { "CAF", 9 }, { "AFC", 8 }, { "OFC", 1 }
        };

        foreach (var (conf, count) in slots)
            for (int i = 1; i <= count; i++)
                teamRepo.Add(new Team($"{conf}_{i:00}", conf, 1000));

        var stadiumRepo = new Repository<Stadium>();
        var stadiumService = CreateStadiumService(stadiumRepo);
        for (int i = 0; i < 4; i++)
            stadiumRepo.Add(new Stadium($"STADIUM_{i}", "City", null, 50000));

        var groupService = new GroupService(groupRepo);
        var potService = new PotService(teamRepo, potRepo);

        return new FixtureService(
            CreateTeamService(teamRepo, confederationService),
            stadiumService, groupService, groupRepo, matchRepo, potService);
    }

    private static TeamService CreateTeamService(
        IRepository<Team> teamRepository,
        IConfederationService confederationService)
    {
        var auditService = new AuditService(new Repository<AuditLog>());
        var sessionService = new SessionService(null!, new AuthStateService());
        return new TeamService(teamRepository, confederationService, auditService, sessionService);
    }

    private static StadiumService CreateStadiumService(IRepository<Stadium> stadiumRepository)
    {
        var auditService = new AuditService(new Repository<AuditLog>());
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

        var allMatches = _groupRepository.GetAll()
            .SelectMany(g => g.Matches)
            .ToList();

        Assert.IsTrue(allMatches.All(m => m.HomeGoals.HasValue && m.AwayGoals.HasValue));
    }
    
    [TestMethod]
    public void SimulateGroupStage_ShouldProduceWinsNotOnlyDraws()
    {
        var fixtureService = CreateFixtureServiceReadyWithPots(42);
        fixtureService.GenerateFixture(42, new DateTime(2026, 6, 1));

        fixtureService.SimulateGroupStage(42);

        // Con 0,0 hardcoded todos tienen puntos múltiplos de 1 (solo empates)
        // Con simulación real debe haber al menos un partido con ganador
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
    public void GenerateFixture_ShouldScheduleGroupsWithThreeDayOffset()
    {
        var fixtureService = CreateFixtureServiceReadyWithPots(48);
        var startDate = new DateTime(2026, 6, 1);

        fixtureService.GenerateFixture(42, startDate);

        var groups = _groupRepository.GetAll()
            .OrderBy(g => g.Label)
            .ToList();

        for (int i = 0; i < groups.Count; i++)
        {
            var expectedStart = startDate.Date.AddDays(i * 3);

            var firstMatchDate = groups[i]
                .Matches
                .Min(m => m.Date.Date);

            Assert.AreEqual(expectedStart, firstMatchDate);
        }
    }
}
