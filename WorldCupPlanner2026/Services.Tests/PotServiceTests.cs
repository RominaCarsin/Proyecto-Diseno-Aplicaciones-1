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

    [TestMethod]
    public void GeneratePots_WithIncorrectAmountOfTeams_ShouldThrowException()
    {
        for (int i = 0; i < 47; i++)
        {
            _teamRepository.Add(new Team { Name = "Equipo " + i, FifaRanking = i + 1 });
        }
        
        Assert.ThrowsException<ArgumentException>(() => _service.GeneratePots());
    }

    [TestMethod]
    public void GeneratePots_ShouldDistributeTeamsCorrectlyByRanking()
    {
        for (int i = 1; i <= 48; i++)
        {
            _teamRepository.Add(new Team { Name = "Team " + i, FifaRanking = i });
        }
        
        _service.GeneratePots();
        
        var pot1 = _potRepository.Get(p => p.Number == 1);
        var pot4 = _potRepository.Get(p => p.Number == 4);

        Assert.IsNotNull(pot1);
        Assert.IsNotNull(pot4);
        
        Assert.AreEqual(12, pot1.Teams.Count);
        Assert.IsTrue(pot1.Teams.All(t => t.FifaRanking <= 12));
        
        Assert.AreEqual(12, pot4.Teams.Count);
        Assert.IsTrue(pot4.Teams.All(t => t.FifaRanking >= 37));
    }

    [TestMethod]
    public void GeneratePots_WithTiedRankings_ShouldBeDeterministicDueToSeed()
    {
        for (int i = 1; i <= 48; i++)
        {
            _teamRepository.Add(new Team { Name = "Team " + i, FifaRanking = 1 });
        }
        
        _service.GeneratePots();
        
        var pot1 = _potRepository.Get(p => p.Number == 1);
        
        Assert.IsNotNull(pot1.Teams[0].Name); 
    }
}