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
    private Confederation _confederation;

    [TestInitialize]
    public void Setup()
    {
        _mockRepository = new Mock<IRepository<Team>>();
        _service = new TeamService(_mockRepository.Object);

        _confederation = new Confederation("CONMBEBOL", 6);
    }
    
    [TestMethod]
    public void AddTeam_WithValidTeam_ShouldAddTeam()
    {
        var team = new Team("Uruguay", _confederation, 8, 0, 0, 0);
        _service.AddTeam(team);
        
        _mockRepository.Verify(r => r.Add(team), Times.Once);
    }
    
    [TestMethod]
    public void GetTeams_ShouldReturnAllTeams()
    {
        var teams = new List<Team>
        {
            new Team("Uruguay", _confederation, 8, 0, 0, 0),
            new Team("Argentina", _confederation, 3, 0, 0, 0)
        };
        
        _mockRepository.Setup(r => r.GetAll()).Returns(teams);
        
        var result = _service.GetTeams();
        
        Assert.AreEqual(2, result.Count());
        Assert.AreEqual("Uruguay", result.First().Name);
        Assert.AreEqual("Argentina", result.Last().Name);
    }
}