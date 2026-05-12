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

        var teamRepo = new Repository<Team>();
        var confederationService = new ConfederationService();
        var teamService = new TeamService(teamRepo, confederationService);

        var stadiumRepo = new Repository<Stadium>();
        var stadiumService = new StadiumService(stadiumRepo);

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
        var teamService = new TeamService(teamRepo, confederationService);

        for (int i = 0; i < 48; i++)
            teamRepo.Add(new Team($"TEAM_{i:00}", "UEFA", 1000 + i));

        var stadiumRepo = new Repository<Stadium>();
        var stadiumService = new StadiumService(stadiumRepo);

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
        var teamService = new  TeamService(teamRepo1, new ConfederationService());
        teamService.AutoCompleteTeams(48);
        
        var potRepo1 = new Repository<Pot>();
        var potService1 = new PotService(teamRepo1, potRepo1);
        potService1.GeneratePots(48);
        var order1 = potRepo1.GetAll().SelectMany(p => p.Teams.Select(t => t.Name)).ToList();

        var teamRepo2 = new  Repository<Team>();
        var teamService2 = new  TeamService(teamRepo2, new ConfederationService());
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

        var service2 = CreateFixtureServiceReadyWithPots(42);
        service2.GenerateFixture(42, new DateTime(2026, 6, 1));
        var teams2 = _groupRepository.GetAll()
            .OrderBy(g => g.Label)
            .SelectMany(g => g.Teams.Select(t => t.Name))
            .ToList();

        CollectionAssert.AreEqual(teams1, teams2);
    }
    

    private FixtureService CreateFixtureServiceReadyWithPots(int seed = 42)
    {
        var groupRepo = new Repository<Group>();
        var matchRepo = new Repository<Match>();
        var potRepo = new Repository<Pot>();
        _groupRepository = groupRepo;

        var teamRepo = new Repository<Team>();
        var confederationService = new ConfederationService();
        var teamService = new TeamService(teamRepo, confederationService);

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
        var stadiumService = new StadiumService(stadiumRepo);
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

        var matchesByDay = _groupRepository.GetAll()
            .SelectMany(g => g.Matches)
            .GroupBy(m => m.Date.Date);

        Assert.IsTrue(matchesByDay.All(g => g.Count() <= 3));
    }
    
    [TestMethod]
    public void GenerateFixture_ShouldScheduleEachGroupRoundsWith3DaysBetweenRounds()
    {
        var fixtureService = CreateFixtureServiceReadyWithPots(48);
        var startDate = new DateTime(2026, 6, 1);

        fixtureService.GenerateFixture(42, startDate);

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
}