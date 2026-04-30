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
}