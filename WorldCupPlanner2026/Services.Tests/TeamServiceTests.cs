using Domain;
using DTOs;
using IServices;
using IRepository;
using Repository;

namespace Services.Tests;

[TestClass]
public class TeamServiceTests
{
    private TeamService _service;
    private  IRepository<Team> _repository;
    private IConfederationService _confederationService;
    
    private TeamRequest _validTeamRequest = new TeamRequest { Name = "Uruguay", ConfederationName = "CONMEBOL", RankingFifa = 500 };
    private TeamRequest _otherValidTeamRequest = new TeamRequest { Name = "Argentina", ConfederationName = "CONMEBOL", RankingFifa = 700 };
    
    [TestInitialize]
    public void Setup()
    {
        _repository = new Repository<Team>();
        _confederationService = new ConfederationService();
        _service = new TeamService(_repository, _confederationService);
    }

    [TestMethod]
    public void AddTeam_WithValidTeamRequest_ShouldAddTeamNormalized()
    {
        _service.AddTeam(_validTeamRequest);
        
        var team = _repository.GetAll().FirstOrDefault(t => t.Name == "URUGUAY");
        Assert.AreEqual("URUGUAY", team.Name);
        
    }
    

    [TestMethod]
    public void UpdateTeam_WhithInvalidTeamRequest_ShouldThrowArgumentExceptionTeamNotFound()
    {
        _service.AddTeam(_validTeamRequest);
        
        Assert.ThrowsException<ArgumentException>(() => _service.UpdateTeam("Inexistente", _validTeamRequest));
    }
    
    [TestMethod]
    public void AddTeam_WithAlreadyExistingName_ShouldThrowException()
    {
        _service.AddTeam(_validTeamRequest);
        
        var duplicate = new TeamRequest { Name = "URUGUAY", ConfederationName = "CONMEBOL", RankingFifa = 600 };
        
        Assert.ThrowsException<ArgumentException>(() => _service.AddTeam(duplicate));
    }
    

    [TestMethod]
    public void UpdateTeam_WithValidData_ShouldUpdateAndCallConfederationService()
    {
        _service.AddTeam(_validTeamRequest);
        var request = new TeamRequest { Name = "Uruguay", ConfederationName = "UEFA", RankingFifa = 600 };
        
        _service.UpdateTeam("URUGUAY", request);
        var updatedTeam = _repository.GetAll().FirstOrDefault(t => t.Name == "URUGUAY");
        
        Assert.AreEqual(600, updatedTeam.RankingFifa);
        Assert.AreEqual("UEFA", updatedTeam.ConfederationName);
    }
    
    [TestMethod]
    public void UpdateTeam_WhenNewNameAlreadyTakenByAnotherTeam_ShouldThrowArgumentException()
    {
        _service.AddTeam(_validTeamRequest);
        _service.AddTeam(_otherValidTeamRequest);
        
        var request = new TeamRequest { Name = "ARGENTINA", ConfederationName = "CONMEBOL", RankingFifa = 500 };
        
        Assert.ThrowsException<ArgumentException>(() => _service.UpdateTeam("URUGUAY", request));
    }
    
    [TestMethod]
    public void UpdateTeam_WhenConfederationChanged_ShouldUpdateTeamConfederation()
    {
        _service.AddTeam(_validTeamRequest);
        var request = new TeamRequest { Name = "URUGUAY", ConfederationName = "UEFA", RankingFifa = 600 };
        
        _service.UpdateTeam("URUGUAY", request);
        
        var updatedTeam = _repository.GetAll().FirstOrDefault(t => t.Name == "URUGUAY");
        
        Assert.AreEqual(600, updatedTeam.RankingFifa);
        Assert.AreEqual("UEFA", updatedTeam.ConfederationName);
    }

    [TestMethod]
    public void DeleteTeam_WhenTeamExists_ShouldRemoveAndFreeSlot()
    {
        _service.AddTeam(_validTeamRequest);

        _service.DeleteTeam("URUGUAY");

        var team = _repository.GetAll().FirstOrDefault(t => t.Name == "URUGUAY");
        Assert.IsNull(team);
    }
    
    [TestMethod]
    public void DeleteTeam_WhenTeamNotFound_ShouldThrowArgumentException()
    {
        
        Assert.ThrowsException<ArgumentException>(() => _service.DeleteTeam("INEXISTENTE"));
        
    }
    
    [TestMethod]
    public void GetTeams_ShouldReturnAllTeamsFromRepository()
    {
        _service.AddTeam(_validTeamRequest);
        _service.AddTeam(_otherValidTeamRequest);
        
        var result = _service.GetTeams().ToList();
        
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("URUGUAY", result[0].Name);
        Assert.AreEqual("ARGENTINA", result[1].Name);
    }

    [TestMethod]
    public void AutoCompleteTeams_WhenCalled_FillsExactly48Teams()
    {
        AddTeamsToService(
            ("Uruguay", "CONMEBOL", 500),
            ("Argentina", "CONMEBOL", 700),
            ("Brazil", "CONMEBOL", 800),
            ("France", "UEFA", 900),
            ("Spain", "UEFA", 1000),
            ("Germany", "UEFA", 1100),
            ("Morocco", "CAF", 1200),
            ("Japan", "AFC", 1300),
            ("Mexico", "CONCACAF", 1400),
            ("New Zealand", "OFC", 1500));

        _service.AutoCompleteTeams(10);

        var result = _service.GetTeams().ToList();
        Assert.AreEqual(48, result.Count);
        Assert.AreEqual(16, result.Count(team => team.ConfederationName == "UEFA"));
        Assert.AreEqual(9, result.Count(team => team.ConfederationName == "CAF"));
        Assert.AreEqual(8, result.Count(team => team.ConfederationName == "AFC"));
        Assert.AreEqual(7, result.Count(team => team.ConfederationName == "CONCACAF"));
        Assert.AreEqual(7, result.Count(team => team.ConfederationName == "CONMEBOL"));
        Assert.AreEqual(1, result.Count(team => team.ConfederationName == "OFC"));
    }

    [TestMethod]
    public void AutoCompleteTeams_GeneratesDeterministicNames()
    {
        _service.AutoCompleteTeams(10);

        var firstGeneratedUefaTeam = _service.GetTeams()
            .FirstOrDefault(team => team.ConfederationName == "UEFA" && team.Name.StartsWith("UEFA_"));

        Assert.IsNotNull(firstGeneratedUefaTeam);
        Assert.AreEqual("UEFA_01", firstGeneratedUefaTeam.Name);
    }

    [TestMethod]
    public void AutoCompleteTeams_WithSameSeed_GeneratesIdenticalRankings()
    {
        var firstService = CreateTeamService();
        var secondService = CreateTeamService();

        firstService.AutoCompleteTeams(123);
        secondService.AutoCompleteTeams(123);

        var firstRankings = GetGeneratedRankings(firstService);
        var secondRankings = GetGeneratedRankings(secondService);

        CollectionAssert.AreEqual(firstRankings, secondRankings);
    }

    private static TeamService CreateTeamService()
    {
        return new TeamService(new Repository<Team>(), new ConfederationService());
    }

    private static int[] GetGeneratedRankings(TeamService service)
    {
        return service.GetTeams()
            .Where(team => team.Name.Contains('_'))
            .OrderBy(team => team.Name)
            .Select(team => team.RankingFifa)
            .ToArray();
    }

    private void AddTeamsToService(params (string Name, string ConfederationName, int RankingFifa)[] teams)
    {
        foreach (var team in teams)
        {
            _service.AddTeam(new TeamRequest
            {
                Name = team.Name,
                ConfederationName = team.ConfederationName,
                RankingFifa = team.RankingFifa
            });
        }
    }
}
