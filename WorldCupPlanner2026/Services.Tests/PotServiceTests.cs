using Domain;
using IRepository;
using Repository;

namespace Services.Tests;

[TestClass]
public class PotServiceTests
{
    private const int DefaultSeed = 42;
    private const int TiedRanking = 500;
    private const int BaseRanking = 300;

    private PotService _service = null!;
    private IRepository<Team> _teamRepository = null!;
    private IRepository<Pot> _potRepository = null!;

    [TestInitialize]
    public void Setup()
    {
        _teamRepository = new FakeRepository<Team>();
        _potRepository = new FakeRepository<Pot>();
        _service = new PotService(_teamRepository, _potRepository);
    }
    
    [TestMethod]
    public void GeneratePots_WithNoTeams_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _service.GeneratePots(DefaultSeed));
    }

    [TestMethod]
    public void GeneratePots_WithLessThan48Teams_ShouldThrowArgumentException()
    {
        AddTeamsWithDistinctRankings(count: 47);

        Assert.ThrowsException<ArgumentException>(() =>
            _service.GeneratePots(DefaultSeed));
    }

    [TestMethod]
    public void GeneratePots_WithMoreThan48Teams_ShouldThrowArgumentException()
    {
        AddTeamsWithDistinctRankings(count: 49);

        Assert.ThrowsException<ArgumentException>(() =>
            _service.GeneratePots(DefaultSeed));
    }
    
    [TestMethod]
    public void GeneratePots_WithExactly48Teams_ShouldCreateFourPots()
    {
        AddTeamsWithDistinctRankings(count: 48);

        _service.GeneratePots(DefaultSeed);

        Assert.AreEqual(4, _potRepository.GetAll().Count());
    }

    [TestMethod]
    public void GeneratePots_WithExactly48Teams_ShouldAssign12TeamsPerPot()
    {
        AddTeamsWithDistinctRankings(count: 48);

        _service.GeneratePots(DefaultSeed);

        Assert.IsTrue(_potRepository.GetAll().All(p => p.Teams.Count == 12));
    }

    [TestMethod]
    public void GeneratePots_WithExactly48Teams_ShouldCreatePotsNumbered1Through4()
    {
        AddTeamsWithDistinctRankings(count: 48);

        _service.GeneratePots(DefaultSeed);

        for (int number = Pot.MinNumber; number <= Pot.MaxNumber; number++)
            Assert.IsNotNull(GetPotByNumber(number));
    }
    
    [TestMethod]
    public void GeneratePots_Pot1ShouldContainHighestRankedTeams()
    {
        AddTeamsWithDistinctRankings(count: 48);

        _service.GeneratePots(DefaultSeed);

        var pot1 = GetPotByNumber(1);
        Assert.IsNotNull(pot1);
        Assert.IsTrue(pot1.Teams.All(t => t.RankingFifa >= 337));
    }

    [TestMethod]
    public void GeneratePots_Pot4ShouldContainLowestRankedTeams()
    {
        AddTeamsWithDistinctRankings(count: 48);

        _service.GeneratePots(DefaultSeed);

        var pot4 = GetPotByNumber(4);
        Assert.IsNotNull(pot4);
        Assert.IsTrue(pot4.Teams.All(t => t.RankingFifa <= 312));
    }
    
    [TestMethod]
    public void GeneratePots_WithTiedRankingsAndSameSeed_ShouldProduceSameTeamOrder()
    {
        AddTeamsWithTiedRankings(_teamRepository, count: 48);
        _service.GeneratePots(DefaultSeed);
        var firstTeamFirstRun = GetPotByNumber(1)!.Teams[0].Name;

        var secondService = BuildServiceWithTiedRankingTeams();
        secondService.GeneratePots(DefaultSeed);
        var firstTeamSecondRun = GetPotByNumberFrom(secondService, 1)!.Teams[0].Name;

        Assert.AreEqual(firstTeamFirstRun, firstTeamSecondRun);
    }

    [TestMethod]
    public void GeneratePots_WithTiedRankingsAndDifferentSeeds_ShouldProduceDifferentTeamOrder()
    {
        AddTeamsWithTiedRankings(_teamRepository, count: 48);
        _service.GeneratePots(seed: 42);
        var firstTeamSeed42 = GetPotByNumber(1)!.Teams[0].Name;

        var secondService = BuildServiceWithTiedRankingTeams();
        secondService.GeneratePots(seed: 99);
        var firstTeamSeed99 = GetPotByNumberFrom(secondService, 1)!.Teams[0].Name;

        Assert.AreNotEqual(firstTeamSeed42, firstTeamSeed99);
    }
    
    private void AddTeamsWithDistinctRankings(int count)
    {
        for (int i = 1; i <= count; i++)
            _teamRepository.Add(new Team($"TEAM_{i:00}", "UEFA", BaseRanking + i));
    }

    private static void AddTeamsWithTiedRankings(IRepository<Team> teamRepository, int count)
    {
        for (int i = 1; i <= count; i++)
            teamRepository.Add(new Team($"TEAM_{i:00}", "UEFA", TiedRanking));
    }

    private static PotService BuildServiceWithTiedRankingTeams()
    {
        var teamRepo = new FakeRepository<Team>();
        var potRepo = new FakeRepository<Pot>();
        AddTeamsWithTiedRankings(teamRepo, count: 48);
        return new PotService(teamRepo, potRepo);
    }

    private Pot? GetPotByNumber(int number) =>
        _potRepository.GetAll().FirstOrDefault(p => p.Number == number);

    private static Pot? GetPotByNumberFrom(PotService service, int number) =>
        service.GetPots().FirstOrDefault(p => p.Number == number);
}