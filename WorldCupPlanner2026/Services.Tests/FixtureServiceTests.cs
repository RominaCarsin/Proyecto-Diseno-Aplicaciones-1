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

        return new FixtureService(teamService, stadiumService, groupService, groupRepo, matchRepo);
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

        return new FixtureService(teamService, stadiumService, groupService, groupRepo, matchRepo);
    }
    
    [TestMethod]
    public void GenerateFixture_WithExistingFixture_ShouldThrowInvalidOperationException()
    {
        var fixtureService = CreateFixtureServiceReady();
        fixtureService.GenerateFixture(42, new DateTime(2026, 6, 1));

        Assert.ThrowsException<InvalidOperationException>(() =>
            fixtureService.GenerateFixture(42, new DateTime(2026, 6, 1)));
    }

    private FixtureService CreateFixtureServiceReady()
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

        for (int i = 0; i < 4; i++)
            stadiumRepo.Add(new Stadium($"STADIUM_{i}", "City", null, 50000));

        var groupService = new GroupService(groupRepo);

        return new FixtureService(teamService, stadiumService, groupService, groupRepo, matchRepo);
    }
    
    [TestMethod]
    public void GenerateFixture_WithValidData_ShouldCreate12Groups()
    {
        var fixtureService = CreateFixtureServiceReady();

        fixtureService.GenerateFixture(42, new DateTime(2026, 6, 1));

        var groups = _groupRepository.GetAll().ToList();
        Assert.AreEqual(12, groups.Count);
    }
    
    [TestMethod]
    public void GenerateFixture_WithValidData_ShouldAssign4TeamsPerGroup()
    {
        var fixtureService = CreateFixtureServiceReady();

        fixtureService.GenerateFixture(42, new DateTime(2026, 6, 1));

        var groups = _groupRepository.GetAll().ToList();
        Assert.IsTrue(groups.All(g => g.Teams.Count == 4));
    }
    
    [TestMethod]
    public void GenerateFixture_WithValidData_ShouldCreate6MatchesPerGroup()
    {
        var fixtureService = CreateFixtureServiceReady();

        fixtureService.GenerateFixture(42, new DateTime(2026, 6, 1));

        var groups = _groupRepository.GetAll().ToList();
        Assert.IsTrue(groups.All(g => g.Matches.Count == 6));
    }
    
    [TestMethod]
    public void GenerateFixture_WithValidData_GroupsShouldHaveLabelsAToL()
    {
        var fixtureService = CreateFixtureServiceReady();

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
        var fixtureService = CreateFixtureServiceReady(); 
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
        var fixtureService = CreateFixtureServiceReady();

        Assert.AreEqual(4, fixtureService.GetStadiumCount());
    }
    
    
    [TestMethod]
    public void GeneratePots_WithSameSeed_ShouldProduceSameOrder()
    {
        var teamRepo1 = CreateTeamRepo();
        var potRepo1 = new Repository<Pot>();
        var potService1 = new PotService(teamRepo1, potRepo1, new ShuffleService(42));
        potService1.GeneratePots();
        var order1 = potRepo1.GetAll().SelectMany(p => p.Teams.Select(t => t.Name)).ToList();

        var teamRepo2 = CreateTeamRepo();
        var potRepo2 = new Repository<Pot>();
        var potService2 = new PotService(teamRepo2, potRepo2, new ShuffleService(42));
        potService2.GeneratePots();
        var order2 = potRepo2.GetAll().SelectMany(p => p.Teams.Select(t => t.Name)).ToList();

        CollectionAssert.AreEqual(order1, order2);
    }
}