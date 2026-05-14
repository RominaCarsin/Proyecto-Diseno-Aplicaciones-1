using Domain;

namespace Services.Tests;

[TestClass]
public class ConfederationServiceTest
{
    private ConfederationService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _service = new ConfederationService();
    }

    [TestMethod]
    public void GetAll_ShouldContainAllConfederations()
    {
        var expectedSlotsByConfederation = new Dictionary<string, int>
        {
            { "UEFA", 16 },
            { "CAF", 9 },
            { "AFC", 8 },
            { "CONCACAF", 7 },
            { "CONMEBOL", 7 },
            { "OFC", 1 },
        };

        var confederations = _service
            .GetAll()
            .ToDictionary(confederation => confederation.Name, confederation => confederation.Slots);

        foreach (var expectedConfederation in expectedSlotsByConfederation)
        {
            Assert.IsTrue(confederations.ContainsKey(expectedConfederation.Key));
            Assert.AreEqual(expectedConfederation.Value, confederations[expectedConfederation.Key]);
        }
    }

    [TestMethod]
    public void AddTeamToConfederation_WhenConfederationHasAvailableSlots_ShouldAddTeam()
    {
        var team = CreateTeam("Spain", "UEFA");

        _service.AddTeamToConfederation(team);

        AssertRemainingSlots("UEFA", 15);
    }

    [TestMethod]
    public void AddTeamToConfederation_WhenConfederationIsFull_ShouldThrowException()
    {
        _service.AddTeamToConfederation(CreateTeam("NewZealand", "OFC"));

        var team = CreateTeam("Fiji", "OFC", 600);

        Assert.ThrowsException<ArgumentException>(() => _service.AddTeamToConfederation(team));
    }

    [TestMethod]
    public void UpdateTeamConfederation_ShouldRemoveTeamFromOldConfederation()
    {
        var team = AddTeam("Spain", "UEFA");

        _service.UpdateTeamConfederation(team, "CONMEBOL");

        AssertRemainingSlots("UEFA", 16);
    }

    [TestMethod]
    public void UpdateTeamConfederation_ShouldAddTeamToNewConfederation()
    {
        var team = AddTeam("Spain", "UEFA");

        _service.UpdateTeamConfederation(team, "CONMEBOL");

        AssertRemainingSlots("CONMEBOL", 6);
    }

    [TestMethod]
    public void UpdateTeamConfederation_WhenNewConfederationIsFull_ShouldThrowExceptionAndKeepTeamInOldConfederation()
    {
        AddTeam("NewZealand", "OFC");
        var team = AddTeam("Spain", "UEFA");

        Assert.ThrowsException<ArgumentException>(() => _service.UpdateTeamConfederation(team, "OFC"));

        AssertRemainingSlots("UEFA", 15);
    }

    [TestMethod]
    public void UpdateTeamConfederation_WhenNewConfederationDoesNotExist_ShouldThrowException()
    {
        var team = AddTeam("Spain", "UEFA");

        Assert.ThrowsException<ArgumentException>(() => _service.UpdateTeamConfederation(team, "INVALID"));
    }

    [TestMethod]
    public void UpdateTeamConfederation_WhenNewConfederationIsSameAsCurrent_ShouldNotMoveTeam()
    {
        var team = AddTeam("Spain", "UEFA");

        _service.UpdateTeamConfederation(team, "UEFA");

        AssertRemainingSlots("UEFA", 15);
    }

    [TestMethod]
    public void RemoveTeamFromConfederation_WhenTeamExists_ShouldRemoveOnlyThatTeam()
    {
        var team = AddTeam("Spain", "UEFA");
        AddTeam("Bosnia", "UEFA", 600);

        _service.RemoveTeamFromConfederation(team);

        AssertRemainingSlots("UEFA", 15);
    }

    private Team AddTeam(string name, string confederationName, int ranking = 500)
    {
        var team = CreateTeam(name, confederationName, ranking);
        _service.AddTeamToConfederation(team);

        return team;
    }

    private static Team CreateTeam(string name, string confederationName, int ranking = 500)
    {
        return new Team(name, confederationName, ranking);
    }

    private void AssertRemainingSlots(string confederationName, int expectedRemainingSlots)
    {
        var confederation = _service.GetAll().First(c => c.Name == confederationName);

        Assert.AreEqual(expectedRemainingSlots, confederation.RemainingSlots);
    }
}