using Domain;
using DTOs;
using IRepository;
using Repository;
using Services;

namespace Services.Tests;

[TestClass]
public class TeamServiceTests
{
    private const string ValidTeamName = "URUGUAY";
    private const string OtherTeamName = "ARGENTINA";
    private const string ValidConfederation = "CONMEBOL";
    private const int ValidRanking = 500;
    private const int OtherRanking = 700;

    private TeamService _service = null!;
    private IRepository<Team> _repository = null!;

    [TestInitialize]
    public void Setup()
    {
        _repository = new FakeRepository<Team>();
        var auditService = new AuditService(new FakeRepository<AuditLog>());
        var sessionService = new SessionService(null!, new AuthStateService());

        _service = new TeamService(_repository, auditService, sessionService);
    }
    
    [TestMethod]
    public void AddTeam_WithValidRequest_ShouldPersistTeamNormalized()
    {
        _service.AddTeam(BuildTeamRequest(ValidTeamName, ValidConfederation));

        var team = FindTeamByName(ValidTeamName);
        Assert.IsNotNull(team);
        Assert.AreEqual(ValidTeamName, team.Name);
    }

    [TestMethod]
    public void AddTeam_WithAlreadyExistingName_ShouldThrowArgumentException()
    {
        _service.AddTeam(BuildTeamRequest(ValidTeamName, ValidConfederation));

        Assert.ThrowsException<ArgumentException>(() =>
            _service.AddTeam(BuildTeamRequest(ValidTeamName, ValidConfederation, ranking: 600)));
    }
    
    [TestMethod]
    public void UpdateTeam_WithValidData_ShouldUpdateRankingAndConfederation()
    {
        _service.AddTeam(BuildTeamRequest(ValidTeamName, ValidConfederation));

        _service.UpdateTeam(ValidTeamName, BuildTeamRequest(ValidTeamName, "UEFA", ranking: 600));

        var updatedTeam = FindTeamByName(ValidTeamName);
        Assert.AreEqual(600, updatedTeam!.RankingFifa);
        Assert.AreEqual("UEFA", updatedTeam.ConfederationName);
    }

    [TestMethod]
    public void UpdateTeam_WhenConfederationChanged_ShouldUpdateTeamConfederation()
    {
        _service.AddTeam(BuildTeamRequest(ValidTeamName, ValidConfederation));

        _service.UpdateTeam(ValidTeamName, BuildTeamRequest(ValidTeamName, "UEFA", ranking: 600));

        var updatedTeam = FindTeamByName(ValidTeamName);
        Assert.AreEqual("UEFA", updatedTeam!.ConfederationName);
    }

    [TestMethod]
    public void UpdateTeam_WithNonExistingName_ShouldThrowArgumentException()
    {
        _service.AddTeam(BuildTeamRequest(ValidTeamName, ValidConfederation));

        Assert.ThrowsException<ArgumentException>(() =>
            _service.UpdateTeam("NONEXISTENT", BuildTeamRequest(ValidTeamName, ValidConfederation)));
    }

    [TestMethod]
    public void UpdateTeam_WhenNewNameAlreadyTakenByAnotherTeam_ShouldThrowArgumentException()
    {
        _service.AddTeam(BuildTeamRequest(ValidTeamName, ValidConfederation));
        _service.AddTeam(BuildTeamRequest(OtherTeamName, ValidConfederation, ranking: OtherRanking));

        Assert.ThrowsException<ArgumentException>(() =>
            _service.UpdateTeam(ValidTeamName, BuildTeamRequest(OtherTeamName, ValidConfederation)));
    }
    
    [TestMethod]
    public void DeleteTeam_WithExistingTeam_ShouldRemoveTeamFromRepository()
    {
        _service.AddTeam(BuildTeamRequest(ValidTeamName, ValidConfederation));

        _service.DeleteTeam(ValidTeamName);

        Assert.IsNull(FindTeamByName(ValidTeamName));
    }

    [TestMethod]
    public void DeleteTeam_WithNonExistingName_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _service.DeleteTeam("NONEXISTENT"));
    }
    
    [TestMethod]
    public void GetTeams_WithMultipleTeams_ShouldReturnAllTeams()
    {
        _service.AddTeam(BuildTeamRequest(ValidTeamName, ValidConfederation));
        _service.AddTeam(BuildTeamRequest(OtherTeamName, ValidConfederation, ranking: OtherRanking));

        var result = _service.GetTeams().ToList();

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual(ValidTeamName, result[0].Name);
        Assert.AreEqual(OtherTeamName, result[1].Name);
    }
    
    [TestMethod]
    public void AutoCompleteTeams_ShouldFillExactly48TeamsWithCorrectConfederationDistribution()
    {
        AddInitialTeams();

        _service.AutoCompleteTeams(seed: 10);

        var result = _service.GetTeams().ToList();
        Assert.AreEqual(48, result.Count);
        Assert.AreEqual(16, result.Count(t => t.ConfederationName == "UEFA"));
        Assert.AreEqual(9, result.Count(t => t.ConfederationName == "CAF"));
        Assert.AreEqual(8, result.Count(t => t.ConfederationName == "AFC"));
        Assert.AreEqual(7, result.Count(t => t.ConfederationName == "CONCACAF"));
        Assert.AreEqual(7, result.Count(t => t.ConfederationName == "CONMEBOL"));
        Assert.AreEqual(1, result.Count(t => t.ConfederationName == "OFC"));
    }

    [TestMethod]
    public void AutoCompleteTeams_ShouldGenerateDeterministicNames()
    {
        _service.AutoCompleteTeams(seed: 10);

        var firstGeneratedUefaTeam = _service.GetTeams()
            .FirstOrDefault(t => t.ConfederationName == "UEFA" && t.Name.StartsWith("UEFA_"));

        Assert.IsNotNull(firstGeneratedUefaTeam);
        Assert.AreEqual("UEFA_01", firstGeneratedUefaTeam.Name);
    }

    [TestMethod]
    public void AutoCompleteTeams_WithSameSeed_ShouldGenerateIdenticalRankings()
    {
        var firstService = BuildTeamService();
        var secondService = BuildTeamService();

        firstService.AutoCompleteTeams(seed: 123);
        secondService.AutoCompleteTeams(seed: 123);

        var firstRankings = GetGeneratedTeamRankings(firstService);
        var secondRankings = GetGeneratedTeamRankings(secondService);

        CollectionAssert.AreEqual(firstRankings, secondRankings);
    }
    
    [TestMethod]
    public void GetTeamsOrdered_WithoutTies_ShouldReturnTeamsOrderedByRankingDescending()
    {
        AddTeams(
            ("URUGUAY", "CONMEBOL", 1700),
            ("FRANCE", "UEFA", 2100),
            ("JAPAN", "AFC", 1300),
            ("MOROCCO", "CAF", 1600));

        var result = _service.GetTeamsOrdered(seed: 5).ToList();

        CollectionAssert.AreEqual(
            new[] { 2100, 1700, 1600, 1300 },
            result.Select(t => t.RankingFifa).ToArray());
    }

    [TestMethod]
    public void GetTeamsOrdered_WithTies_ShouldShuffleTiedTeamsDeterministically()
    {
        AddTeams(
            ("ALPHA", "UEFA", 1800),
            ("BETA", "UEFA", 1800),
            ("GAMMA", "UEFA", 1800),
            ("DELTA", "CAF", 1500));

        var firstResult = _service.GetTeamsOrdered(seed: 7).ToList();
        var secondResult = _service.GetTeamsOrdered(seed: 7).ToList();

        var expectedTiedTeams = new List<Team>
        {
            new("ALPHA", "UEFA", 1800),
            new("BETA", "UEFA", 1800),
            new("GAMMA", "UEFA", 1800),
        };
        new ShuffleService(seed: 7).ShuffleByRanking(expectedTiedTeams);
        var expectedTiedNames = expectedTiedTeams.Select(t => t.Name).ToList();

        CollectionAssert.AreEqual(
            firstResult.Select(t => t.Name).ToArray(),
            secondResult.Select(t => t.Name).ToArray());
        CollectionAssert.AreEqual(
            expectedTiedNames.ToArray(),
            firstResult.Take(3).Select(t => t.Name).ToArray());
        Assert.AreEqual("DELTA", firstResult[3].Name);
    }

    [TestMethod]
    public void GetTeamsOrdered_WithDifferentSeeds_ShouldProduceDifferentTieBreakOrder()
    {
        AddTeams(
            ("ALPHA", "UEFA", 1800),
            ("BETA", "UEFA", 1800),
            ("GAMMA", "UEFA", 1800),
            ("DELTA", "UEFA", 1800));

        var firstOrder = _service.GetTeamsOrdered(seed: 1).Select(t => t.Name).ToArray();
        var secondOrder = _service.GetTeamsOrdered(seed: 2).Select(t => t.Name).ToArray();

        CollectionAssert.AreNotEqual(firstOrder, secondOrder);
    }
    
    [TestMethod]
    public void GetTeamsByRanking_ShouldReturnTeamsOrderedByRankingDescending()
    {
        AddTeams(
            ("URUGUAY", "CONMEBOL", 1700),
            ("FRANCE", "UEFA", 2100),
            ("JAPAN", "AFC", 1300),
            ("MOROCCO", "CAF", 1600));

        var result = _service.GetTeamsByRanking().ToList();

        CollectionAssert.AreEqual(
            new[] { 2100, 1700, 1600, 1300 },
            result.Select(t => t.RankingFifa).ToArray());
    }
    
    private static TeamRequest BuildTeamRequest(
        string name,
        string confederationName,
        int ranking = ValidRanking) =>
        new TeamRequest
        {
            Name = name,
            ConfederationName = confederationName,
            RankingFifa = ranking
        };

    private static TeamService BuildTeamService()
    {
        var repository = new FakeRepository<Team>();
        var auditService = new AuditService(new FakeRepository<AuditLog>());
        var sessionService = new SessionService(null!, new AuthStateService());
        return new TeamService(repository, auditService, sessionService);
    }

    private Team? FindTeamByName(string name) =>
        _repository.GetAll().FirstOrDefault(t => t.Name == name);

    private void AddTeams(params (string Name, string Confederation, int Ranking)[] teams)
    {
        foreach (var (name, confederation, ranking) in teams)
            _service.AddTeam(BuildTeamRequest(name, confederation, ranking));
    }

    private void AddInitialTeams()
    {
        AddTeams(
            ("URUGUAY",      "CONMEBOL",  500),
            ("ARGENTINA",    "CONMEBOL",  700),
            ("BRAZIL",       "CONMEBOL",  800),
            ("FRANCE",       "UEFA",      900),
            ("SPAIN",        "UEFA",     1000),
            ("GERMANY",      "UEFA",     1100),
            ("MOROCCO",      "CAF",      1200),
            ("JAPAN",        "AFC",      1300),
            ("MEXICO",       "CONCACAF", 1400),
            ("NEW ZEALAND",  "OFC",      1500));
    }

    private static int[] GetGeneratedTeamRankings(TeamService service) =>
        service.GetTeams()
            .Where(t => t.Name.Contains('_'))
            .OrderBy(t => t.Name)
            .Select(t => t.RankingFifa)
            .ToArray();
}
