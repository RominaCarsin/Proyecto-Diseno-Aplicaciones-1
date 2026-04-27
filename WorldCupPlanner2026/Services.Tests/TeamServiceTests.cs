using Domain;
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
    public void AddTeam_WithValidTeam_ShouldAddTeam()
    {
        var team = new Team("Uruguay", _confederation.Name, 500, 0, 0, 0);
        _service.AddTeam(team);
        
        _mockRepository.Verify(r => r.Add(team), Times.Once);
        _mockConfederationService.Verify(c => c.AddTeamToConfederation(team), Times.Once);
    }
    
    [TestMethod]
    public void GetTeams_ShouldReturnAllTeams()
    {
        var teams = new List<Team>
        {
            new Team("Uruguay", _confederation.Name, 500, 0, 0, 0),
            new Team("Argentina", _confederation.Name, 501, 0, 0, 0)
        };
        
        _mockRepository.Setup(r => r.GetAll()).Returns(teams);
        
        var result = _service.GetTeams();
        
        Assert.AreEqual(2, result.Count());
        _mockRepository.Verify(r => r.GetAll(), Times.Once);
    }

    [TestMethod]
    public void AddTeam_WithAlreadyExistingName_ShouldThrowException()
    {
        var existingTeams = new List<Team>
        {
            new Team("Uruguay", _confederation.Name, 500, 0, 0, 0)
        };
        _mockRepository.Setup(r => r.GetAll()).Returns(existingTeams);
        
        var duplicate = new Team("Uruguay", _confederation.Name, 600, 0, 0, 0);
        Assert.ThrowsException<ArgumentException>(() => _service.AddTeam(duplicate));
    }

    [TestMethod]
    public void UpdateTeam_WhenTeamExists_ShouldUpdateNameAndRanking()
    {
        var existing = new Team("Uruguay", _confederation.Name, 500, 0, 0, 0);
        _mockRepository.Setup(r => r.GetAll()).Returns(new List<Team> { existing });

        _service.UpdateTeam("Uruguay", "Uruguay Updated", 1200);

        Assert.AreEqual("Uruguay Updated", existing.Name);
        Assert.AreEqual(1200, existing.RankingFifa);
    }

    [TestMethod]
    public void UpdateTeam_WhenTeamNotFound_ShouldThrowArgumentException()
    {
        _mockRepository.Setup(r => r.GetAll()).Returns(new List<Team>());

        Assert.ThrowsException<ArgumentException>(() => _service.UpdateTeam("Uruguay", "Uruguay Updated", 1200));
    }

    [TestMethod]
    public void UpdateTeam_WhenNewNameAlreadyTakenByAnotherTeam_ShouldThrowArgumentException()
    {
        var uruguay = new Team("Uruguay", _confederation.Name, 500, 0, 0, 0);
        var argentina = new Team("Argentina", _confederation.Name, 600, 0, 0, 0);
        _mockRepository.Setup(r => r.GetAll()).Returns(new List<Team> { uruguay, argentina });

        Assert.ThrowsException<ArgumentException>(() => _service.UpdateTeam("Uruguay", "Argentina", 500));
    }

    [TestMethod]
    public void UpdateTeam_WhenNewNameIsSameAsCurrentName_ShouldSucceed()
    {
        var existing = new Team("Uruguay", _confederation.Name, 500, 0, 0, 0);
        _mockRepository.Setup(r => r.GetAll()).Returns(new List<Team> { existing });

        _service.UpdateTeam("Uruguay", "Uruguay", 1200);

        Assert.AreEqual("Uruguay", existing.Name);
        Assert.AreEqual(1200, existing.RankingFifa);
    }
    

    [TestMethod]
    public void DeleteTeam_WhenTeamExists_ShouldRemoveFromRepository()
    {
        var existing = new Team("Uruguay", _confederation.Name, 500, 0, 0, 0);
        _mockRepository.Setup(r => r.GetAll()).Returns(new List<Team> { existing });

        _service.DeleteTeam("Uruguay");

        _mockRepository.Verify(r => r.Delete(existing), Times.Once);
    }

    [TestMethod]
    public void DeleteTeam_WhenTeamExists_ShouldFreeSlotInConfederation()
    {
        var existing = new Team("Uruguay", _confederation.Name, 500, 0, 0, 0);
        _mockRepository.Setup(r => r.GetAll()).Returns(new List<Team> { existing });

        _service.DeleteTeam("Uruguay");

        _mockConfederationService.Verify(c => c.RemoveTeamFromConfederation(existing), Times.Once);
    }

    [TestMethod]
    public void DeleteTeam_WhenTeamNotFound_ShouldThrowArgumentException()
    {
        _mockRepository.Setup(r => r.GetAll()).Returns(new List<Team>());

        Assert.ThrowsException<ArgumentException>(() => _service.DeleteTeam("Uruguay"));
    }
}