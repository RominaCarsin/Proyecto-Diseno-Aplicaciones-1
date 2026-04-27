using Domain;

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
        // Arrange
        var team = new Team("Spain", "UEFA", 500, 0, 0, 0);
        
        // Act
        _service.AddTeamToConfederation(team);
        
        // Assert - Obtener la confederación UEFA del servicio (no local)
        var uefa = _service.GetAll().First(c => c.Name == "UEFA");
        Assert.IsTrue(uefa.Teams.Contains(team), "Team should be added to the confederation");
    }
    
    [TestMethod]
    public void AddTeam_FullConfederation_ShouldThrowException()
    {
        var ofc = _service.GetAll().First(c => c.Name == "OFC");
        var firstTeam = new Team("NewZealand", "OFC", 500, 0, 0, 0);
        _service.AddTeamToConfederation(firstTeam);
        
        var secondTeam = new Team("Fiji", "OFC", 600, 0, 0, 0);
        Assert.ThrowsException<ArgumentException>(() => _service.AddTeamToConfederation(secondTeam));
    }
    
    [TestMethod]
    public void AddTeam_InvalidConfederation_ShouldThrowException()
    {
        var team = new Team("InvalidTeam", "INVALID", 500, 0, 0, 0);
        
        Assert.ThrowsException<ArgumentException>(() => _service.AddTeamToConfederation(team));
    }
}