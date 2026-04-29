using Domain;
using DTOs;
using Services;

namespace Services.Tests;

[TestClass]
public class ConfederationServiceTest
{
    private ConfederationService _service;

    [TestInitialize]
    public void Setup()
    {
        _service = new ConfederationService();
    }

    [TestMethod]
    public void GetAll_ShouldContainAllConfederationsWithCorrectSlots()
    {
        var expected = new Dictionary<string, int>
        {
            { "UEFA",     16 },
            { "CAF",       9 },
            { "AFC",       8 },
            { "CONCACAF",  7 },
            { "CONMEBOL",  7 },
            { "OFC",       1 },
        };

        var result = _service.GetAll().ToList();

        foreach (var (name, slots) in expected)
        {
            var conf = result.FirstOrDefault(c => c.Name == name);
            Assert.IsNotNull(conf, $"Confederation {name} not found");
            Assert.AreEqual(slots, conf.Slots, $"{name} should have {slots} slots");
        }
    }
    
    
    [TestMethod]
    public void AddTeam_NotFullConfederation_ShouldAddTeamToConfederation()
    {
        var team = new Team("Spain", "UEFA", 500);

        _service.AddTeamToConfederation(team);

        var uefa = _service.GetAll().First(c => c.Name == "UEFA");
        Assert.AreEqual(15, uefa.RemainingSlots);
    }

    [TestMethod]
    public void AddTeam_FullConfederation_ShouldThrowException()
    {
        var firstTeam = new Team("NewZealand", "OFC", 500);
        _service.AddTeamToConfederation(firstTeam);

        var secondTeam = new Team("Fiji", "OFC", 600);
        Assert.ThrowsException<ArgumentException>(() => _service.AddTeamToConfederation(secondTeam));
    }

    [TestMethod]
    public void UpdateTeamConfederation_ShouldRemoveTeamFromOldConfederation()
    {
        var team = new Team("Spain", "UEFA", 500);
        _service.AddTeamToConfederation(team);

        _service.UpdateTeamConfederation(team, "CONMEBOL");

        var uefa = _service.GetAll().First(c => c.Name == "UEFA");
        Assert.AreEqual(16, uefa.RemainingSlots);
    }

    [TestMethod]
    public void UpdateTeamConfederation_ShouldAddTeamToNewConfederation()
    {
        var team = new Team("Spain", "UEFA", 500);
        _service.AddTeamToConfederation(team);

        _service.UpdateTeamConfederation(team, "CONMEBOL");

        var conmebol = _service.GetAll().First(c => c.Name == "CONMEBOL");
        Assert.AreEqual(6, conmebol.RemainingSlots);
    }

    [TestMethod]
    public void UpdateTeamConfederation_WhenNewConfederationIsFull_ShouldThrowAndNotMove()
    {
        var existing = new Team("NewZealand", "OFC", 500);
        _service.AddTeamToConfederation(existing);

        var team = new Team("Spain", "UEFA", 500);
        _service.AddTeamToConfederation(team);

        Assert.ThrowsException<ArgumentException>(() => _service.UpdateTeamConfederation(team, "OFC"));

        var uefa = _service.GetAll().First(c => c.Name == "UEFA");
        Assert.AreEqual(15, uefa.RemainingSlots);
    }

    [TestMethod]
    public void UpdateTeamConfederation_WhenNewConfederationNotFound_ShouldThrowException()
    {
        var team = new Team("Spain", "UEFA", 500);
        _service.AddTeamToConfederation(team);

        Assert.ThrowsException<ArgumentException>(() => _service.UpdateTeamConfederation(team, "INVALID"));
    }
    

    [TestMethod]
    public void UpdateTeamConfederation_WhenSameConfederation_ShouldNotMoveTeam()
    {
        var team = new Team("Spain", "UEFA", 500);
        _service.AddTeamToConfederation(team);

        _service.UpdateTeamConfederation(team, "UEFA");

        var uefa = _service.GetAll().First(c => c.Name == "UEFA");
        Assert.AreEqual(15, uefa.RemainingSlots);
    }
    
    
    [TestMethod]
    public void RemoveTeamFromConfederation_WhenTeamExists_ShouldOnlyRemoveThatTeam()
    {
        var team  = new Team("Spain",  "UEFA", 500);
        var team2 = new Team("Bosnia", "UEFA", 600);
        _service.AddTeamToConfederation(team);
        _service.AddTeamToConfederation(team2);

        _service.RemoveTeamFromConfederation(team);

        var uefa = _service.GetAll().First(c => c.Name == "UEFA");
        Assert.AreEqual(15, uefa.RemainingSlots);
    }
    
    
}