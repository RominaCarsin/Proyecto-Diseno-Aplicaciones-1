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
    public void AddTeam_WithValidTeam_ShouldAddTeam()
    {
        var request = new TeamRequest { Name = "Uruguay", ConfederationName = "CONMEBOL", RankingFifa = 500 };

        _service.AddTeam(request);

        _mockRepository.Verify(r => r.Add(It.Is<Team>(t => t.Name == "Uruguay")), Times.Once);
    }

    [TestMethod]
    public void GetTeams_ShouldReturnTeamResponses()
    {
        var teams = new List<Team>
        {
            new Team("Uruguay", "CONMEBOL", 500)
        };
        _mockRepository.Setup(r => r.GetAll()).Returns(teams);

        var result = _service.GetTeams().ToList();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Uruguay", result[0].Name);
        Assert.IsInstanceOfType(result[0], typeof(TeamResponse));
    }

    [TestMethod]
    public void AddTeam_WithAlreadyExistingName_ShouldThrowException()
    {
        var existingTeams = new List<Team>
        {
            new Team("Uruguay", _confederation.Name, 500)
        };
        _mockRepository.Setup(r => r.GetAll()).Returns(existingTeams);

        var duplicate = new TeamRequest { Name = "Uruguay", ConfederationName = _confederation.Name, RankingFifa = 600 };
        Assert.ThrowsException<ArgumentException>(() => _service.AddTeam(duplicate));
    }

    [TestMethod]
public void UpdateTeam_WhenTeamExists_ShouldUpdateNameAndRanking()
{
    var existing = new Team("Uruguay", _confederation.Name, 500);
    _mockRepository.Setup(r => r.GetAll()).Returns(new List<Team> { existing });

    var request = new TeamRequest { Name = "Uruguay Updated", ConfederationName = _confederation.Name, RankingFifa = 1200 };
    _service.UpdateTeam("Uruguay", request);

    Assert.AreEqual("Uruguay Updated", existing.Name);
    Assert.AreEqual(1200, existing.RankingFifa);
}

[TestMethod]
public void UpdateTeam_WhenTeamNotFound_ShouldThrowArgumentException()
{
    _mockRepository.Setup(r => r.GetAll()).Returns(new List<Team>());

    var request = new TeamRequest { Name = "Uruguay Updated", ConfederationName = _confederation.Name, RankingFifa = 1200 };
    Assert.ThrowsException<ArgumentException>(() => _service.UpdateTeam("Uruguay", request));
}

[TestMethod]
public void UpdateTeam_WhenNewNameAlreadyTakenByAnotherTeam_ShouldThrowArgumentException()
{
    var uruguay = new Team("Uruguay", _confederation.Name, 500);
    var argentina = new Team("Argentina", _confederation.Name, 600);
    _mockRepository.Setup(r => r.GetAll()).Returns(new List<Team> { uruguay, argentina });

    var request = new TeamRequest { Name = "Argentina", ConfederationName = _confederation.Name, RankingFifa = 500 };
    Assert.ThrowsException<ArgumentException>(() => _service.UpdateTeam("Uruguay", request));
}

[TestMethod]
public void UpdateTeam_WhenNewNameIsSameAsCurrentName_ShouldSucceed()
{
    var existing = new Team("Uruguay", _confederation.Name, 500);
    _mockRepository.Setup(r => r.GetAll()).Returns(new List<Team> { existing });

    var request = new TeamRequest { Name = "Uruguay", ConfederationName = _confederation.Name, RankingFifa = 1200 };
    _service.UpdateTeam("Uruguay", request);

    Assert.AreEqual("Uruguay", existing.Name);
    Assert.AreEqual(1200, existing.RankingFifa);
}

[TestMethod]
public void UpdateTeam_WhenChangingConfederation_ShouldCallUpdateTeamConfederation()
{
    var existing = new Team("Uruguay", "CONMEBOL", 500);
    _mockRepository.Setup(r => r.GetAll()).Returns(new List<Team> { existing });

    var request = new TeamRequest { Name = "Uruguay", ConfederationName = "UEFA", RankingFifa = 500 };
    _service.UpdateTeam("Uruguay", request);

    _mockConfederationService.Verify(c => c.UpdateTeamConfederation(existing, "UEFA"), Times.Once);
}

[TestMethod]
public void UpdateTeam_WhenConfederationNotChanged_ShouldNotCallUpdateTeamConfederation()
{
    var existing = new Team("Uruguay", "CONMEBOL", 500);
    _mockRepository.Setup(r => r.GetAll()).Returns(new List<Team> { existing });

    var request = new TeamRequest { Name = "Uruguay", ConfederationName = "CONMEBOL", RankingFifa = 1200 };
    _service.UpdateTeam("Uruguay", request);

    _mockConfederationService.Verify(c =>
        c.UpdateTeamConfederation(It.IsAny<Team>(), It.IsAny<string>()), Times.Never);
}

    [TestMethod]
    public void DeleteTeam_WhenTeamExists_ShouldRemoveFromRepository()
    {
        var existing = new Team("Uruguay", _confederation.Name, 500);
        _mockRepository.Setup(r => r.GetAll()).Returns(new List<Team> { existing });

        _service.DeleteTeam("Uruguay");

        _mockRepository.Verify(r => r.Delete(existing), Times.Once);
    }

    [TestMethod]
    public void DeleteTeam_WhenTeamExists_ShouldFreeSlotInConfederation()
    {
        var existing = new Team("Uruguay", _confederation.Name, 500);
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