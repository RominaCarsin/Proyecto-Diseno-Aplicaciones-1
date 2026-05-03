using Domain;
using Services;
using IRepository;
using Repository;

[TestClass]
public class PotServiceTests
{
    private PotService _service;
    private IRepository<Team> _teamRepository;
    private IRepository<Pot> _potRepository;
    private ShuffleService _shuffleService;

    [TestInitialize]
    public void Setup()
    {
        _teamRepository = new Repository<Team>();
        _potRepository = new Repository<Pot>();
        _shuffleService = new ShuffleService(42);
        _service = new PotService(_teamRepository, _potRepository, _shuffleService);
    }

    private Pot GetPotByNumber(int number)
    {
        foreach (var pot in _potRepository.GetAll())
            if (pot.Number == number)
                return pot;
        return null;
    }

    private bool AllTeamsHaveRankingAtMost(Pot pot, int max)
    {
        foreach (var team in pot.Teams)
            if (team.RankingFifa > max)
                return false;
        return true;
    }

    private bool AllTeamsHaveRankingAtLeast(Pot pot, int min)
    {
        foreach (var team in pot.Teams)
            if (team.RankingFifa < min)
                return false;
        return true;
    }

    [TestMethod]
    public void GeneratePots_WithLessThan48Teams_ShouldThrowArgumentException()
    {
        for (int i = 0; i < 47; i++)
            _teamRepository.Add(new Team("Equipo " + i, "UEFA", i + 300));

        Assert.ThrowsException<ArgumentException>(() => _service.GeneratePots());
    }

    [TestMethod]
    public void GeneratePots_WithMoreThan48Teams_ShouldThrowArgumentException()
    {
        for (int i = 0; i < 49; i++)
            _teamRepository.Add(new Team("Equipo " + i, "UEFA", i + 300));

        Assert.ThrowsException<ArgumentException>(() => _service.GeneratePots());
    }

    [TestMethod]
    public void GeneratePots_WithNoTeams_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() => _service.GeneratePots());
    }

    [TestMethod]
    public void GeneratePots_ShouldCreateFourPots()
    {
        for (int i = 1; i <= 48; i++)
            _teamRepository.Add(new Team("Team " + i, "UEFA", 300 + i));

        _service.GeneratePots();

        int count = 0;
        foreach (var _ in _potRepository.GetAll())
            count++;

        Assert.AreEqual(4, count);
    }

    [TestMethod]
    public void GeneratePots_EachPotShouldHaveTwelveTeams()
    {
        for (int i = 1; i <= 48; i++)
            _teamRepository.Add(new Team("Team " + i, "UEFA", 300 + i));

        _service.GeneratePots();

        foreach (var pot in _potRepository.GetAll())
            Assert.AreEqual(12, pot.Teams.Count);
    }

    [TestMethod]
    public void GeneratePots_Pot1ShouldHaveHighestRankedTeams()
    {
        for (int i = 1; i <= 48; i++)
            _teamRepository.Add(new Team("Team " + i, "UEFA", 300 + i));

        _service.GeneratePots();

        var pot1 = GetPotByNumber(1);
        Assert.IsNotNull(pot1);
        Assert.IsTrue(AllTeamsHaveRankingAtLeast(pot1, 337)); // rankings 337-348 son los 12 más altos
    }

    [TestMethod]
    public void GeneratePots_Pot4ShouldHaveLowestRankedTeams()
    {
        for (int i = 1; i <= 48; i++)
            _teamRepository.Add(new Team("Team " + i, "UEFA", 300 + i));

        _service.GeneratePots();

        var pot4 = GetPotByNumber(4);
        Assert.IsNotNull(pot4);
        Assert.IsTrue(AllTeamsHaveRankingAtMost(pot4, 312)); // rankings 301-312 son los 12 más bajos
    }

    [TestMethod]
    public void GeneratePots_PotNumbersShouldBe1Through4()
    {
        for (int i = 1; i <= 48; i++)
            _teamRepository.Add(new Team("Team " + i, "UEFA", 300 + i));

        _service.GeneratePots();

        for (int number = 1; number <= 4; number++)
        {
            var pot = GetPotByNumber(number);
            Assert.IsNotNull(pot, $"No se encontró el bombo {number}");
        }
    }

    [TestMethod]
    public void GeneratePots_WithTiedRankings_SameSeedShouldProduceSameOrder()
    {
        for (int i = 1; i <= 48; i++)
            _teamRepository.Add(new Team("Team " + i, "UEFA", 500));

        _service.GeneratePots();
        var pot1FirstRun = GetPotByNumber(1);
        var firstTeamFirstRun = pot1FirstRun.Teams[0].Name;
        
        var teamRepo2 = new Repository<Team>();
        var potRepo2  = new Repository<Pot>();
        for (int i = 1; i <= 48; i++)
            teamRepo2.Add(new Team("Team " + i, "UEFA", 500));

        var service2 = new PotService(
            teamRepo2, potRepo2,
            new ShuffleService(42));

        service2.GeneratePots();

        Pot pot1SecondRun = null;
        foreach (var pot in potRepo2.GetAll())
            if (pot.Number == 1) { pot1SecondRun = pot; break; }

        Assert.AreEqual(firstTeamFirstRun, pot1SecondRun.Teams[0].Name);
    }

    [TestMethod]
    public void GeneratePots_WithTiedRankings_DifferentSeedShouldProduceDifferentOrder()
    {
        for (int i = 1; i <= 48; i++)
            _teamRepository.Add(new Team("Team " + i, "UEFA", 500));

        _service.GeneratePots();
        var firstTeamSeed42 = GetPotByNumber(1).Teams[0].Name;

        var teamRepo2 = new Repository<Team>();
        var potRepo2  = new Repository<Pot>();
        for (int i = 1; i <= 48; i++)
            teamRepo2.Add(new Team("Team " + i, "UEFA", 500));

        var service2 = new PotService(
            teamRepo2, potRepo2,
            new ShuffleService(99));

        service2.GeneratePots();

        Pot pot1Seed99 = null;
        foreach (var pot in potRepo2.GetAll())
            if (pot.Number == 1) { pot1Seed99 = pot; break; }

        Assert.AreNotEqual(firstTeamSeed42, pot1Seed99.Teams[0].Name);
    }
}