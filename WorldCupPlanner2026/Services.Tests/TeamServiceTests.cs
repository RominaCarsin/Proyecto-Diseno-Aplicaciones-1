using Domain;
using DTOs;
using IServices;
using Services;
using IRepository;
using Moq;

namespace Services.Tests;

[TestClass]
public class TeamServiceTests
{
    private TeamService _service;
    private Mock<IRepository<Team>> _mockRepository;
    private Mock<IConfederationService> _mockConfederationService;
    private Confederation _confederation;

    [TestInitialize]
    public void Setup()
    {
        _mockRepository = new Mock<IRepository<Team>>();
        _mockConfederationService = new Mock<IConfederationService>();
        _service = new TeamService(_mockRepository.Object, _mockConfederationService.Object);
        _confederation = new Confederation("CONMEBOL", 6);
    }

    [TestMethod]
    public void AddTeam_WithValidTeam_ShouldAddTeamNormalized()
    {
        
        var request = new TeamRequest { Name = "Uruguay", ConfederationName = "conmebol", RankingFifa = 500 };

        _service.AddTeam(request);
        
        _mockRepository.Verify(r => r.Add(It.Is<Team>(t => t.Name == "URUGUAY" && t.ConfederationName == "CONMEBOL")), Times.Once);
    }
    

    [TestMethod]
    public void UpdateTeam_WhenTeamNotFound_ShouldThrowArgumentException()
    {
        _mockRepository.Setup(r => r.GetAll()).Returns(new List<Team>());
        var request = new TeamRequest { Name = "Cualquier cosa", ConfederationName = "UEFA", RankingFifa = 500 };
        
        var ex = Assert.ThrowsException<ArgumentException>(() => 
            _service.UpdateTeam("Inexistente", request));
    
        Assert.AreEqual("Team not found.", ex.Message);
    }
    
    [TestMethod]
    public void AddTeam_WithAlreadyExistingName_ShouldThrowException()
    {
        var existingTeams = new List<Team>
        {
            new Team("Uruguay", "CONMEBOL", 500)
        };
        _mockRepository.Setup(r => r.GetAll()).Returns(existingTeams);
        
        var duplicate = new TeamRequest { Name = "URUGUAY", ConfederationName = "CONMEBOL", RankingFifa = 600 };
        Assert.ThrowsException<ArgumentException>(() => _service.AddTeam(duplicate));
    }
    

    [TestMethod]
    public void UpdateTeam_WithValidData_ShouldUpdateAndCallConfederationService()
    {
        var existing = new Team("URUGUAY", "CONMEBOL", 500);
        _mockRepository.Setup(r => r.GetAll()).Returns(new List<Team> { existing });
    
        var request = new TeamRequest { Name = "Uruguay", ConfederationName = "UEFA", RankingFifa = 600 };
        
        _service.UpdateTeam("URUGUAY", request);
        
        Assert.AreEqual(600, existing.RankingFifa);
        Assert.AreEqual("UEFA", existing.ConfederationName);
        _mockConfederationService.Verify(c => c.UpdateTeamConfederation(existing, "UEFA"), Times.Once);
    }
    
    [TestMethod]
    public void UpdateTeam_WhenNewNameAlreadyTakenByAnotherTeam_ShouldThrowArgumentException()
    {
        var uruguay = new Team("URUGUAY", "CONMEBOL", 500);
        var argentina = new Team("ARGENTINA", "CONMEBOL", 600);
        _mockRepository.Setup(r => r.GetAll()).Returns(new List<Team> { uruguay, argentina });

        
        var request = new TeamRequest { Name = "ARGENTINA", ConfederationName = "CONMEBOL", RankingFifa = 500 };
        Assert.ThrowsException<ArgumentException>(() => _service.UpdateTeam("URUGUAY", request));
    }
    
    [TestMethod]
    public void UpdateTeam_WhenConfederationNotChanged_ShouldNotUpdateTeamConfederation()
    {
        var existing = new Team("URUGUAY", "CONMEBOL", 500);
        _mockRepository.Setup(r => r.GetAll()).Returns(new List<Team> { existing });
        var request = new TeamRequest { Name = "URUGUAY", ConfederationName = "CONMEBOL", RankingFifa = 600 };
        
        _service.UpdateTeam("URUGUAY", request);
        
        _mockConfederationService.Verify(c => 
            c.UpdateTeamConfederation(It.IsAny<Team>(), It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public void DeleteTeam_WhenTeamExists_ShouldRemoveAndFreeSlot()
    {
        var existing = new Team("URUGUAY", "CONMEBOL", 500);
        _mockRepository.Setup(r => r.GetAll()).Returns(new List<Team> { existing });

        _service.DeleteTeam("URUGUAY");

        _mockRepository.Verify(r => r.Delete(existing), Times.Once);
        _mockConfederationService.Verify(c => c.RemoveTeamFromConfederation(existing), Times.Once);
    }
    
    [TestMethod]
    public void DeleteTeam_WhenTeamNotFound_ShouldThrowArgumentException()
    {
        _mockRepository.Setup(r => r.GetAll()).Returns(new List<Team>());
        
        var ex = Assert.ThrowsException<ArgumentException>(() => 
            _service.DeleteTeam("Inexistente"));
    
        Assert.AreEqual("Team not found.", ex.Message);
    }
    
    [TestMethod]
    public void GetTeams_ShouldReturnAllTeamsFromRepository()
    {
        var teams = new List<Team> 
        { 
            new Team("URUGUAY", "CONMEBOL", 500),
            new Team("ARGENTINA", "CONMEBOL", 600)
        };
        _mockRepository.Setup(r => r.GetAll()).Returns(teams);
        
        var result = _service.GetTeams().ToList();
        
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("URUGUAY", result[0].Name);
        Assert.AreEqual("ARGENTINA", result[1].Name);
    }
}