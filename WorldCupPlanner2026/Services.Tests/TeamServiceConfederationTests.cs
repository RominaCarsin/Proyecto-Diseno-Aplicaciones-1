using Domain;
using IRepository;
using Repository;
using Services;

namespace Services.Tests;

[TestClass]
public class TeamServiceConfederationTests
{
    private TeamService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _service = BuildService();
    }

    [TestMethod]
    public void GetConfederations_ShouldContainAllConfederations()
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
            .GetConfederations()
            .ToDictionary(confederation => confederation.Name, confederation => confederation.Slots);

        foreach (var expectedConfederation in expectedSlotsByConfederation)
        {
            Assert.IsTrue(confederations.ContainsKey(expectedConfederation.Key));
            Assert.AreEqual(expectedConfederation.Value, confederations[expectedConfederation.Key]);
        }
    }

    [TestMethod]
    public void AddTeam_WhenConfederationHasAvailableSlots_ShouldReduceRemainingSlots()
    {
        AddTeam("Spain", "UEFA");

        AssertRemainingSlots("UEFA", 15);
    }

    [TestMethod]
    public void AddTeam_WhenConfederationIsFull_ShouldThrowException()
    {
        AddTeam("NewZealand", "OFC");

        Assert.ThrowsException<ArgumentException>(() => AddTeam("Fiji", "OFC", 600));
    }

    [TestMethod]
    public void UpdateTeam_WhenConfederationChanges_ShouldRemoveTeamFromOldConfederation()
    {
        AddTeam("Spain", "UEFA");

        _service.UpdateTeam("Spain", BuildRequest("Spain", "CONMEBOL"));

        AssertRemainingSlots("UEFA", 16);
    }

    [TestMethod]
    public void UpdateTeam_WhenConfederationChanges_ShouldAddTeamToNewConfederation()
    {
        AddTeam("Spain", "UEFA");

        _service.UpdateTeam("Spain", BuildRequest("Spain", "CONMEBOL"));

        AssertRemainingSlots("CONMEBOL", 6);
    }

    [TestMethod]
    public void UpdateTeam_WhenNewConfederationIsFull_ShouldThrowExceptionAndKeepTeamInOldConfederation()
    {
        AddTeam("NewZealand", "OFC");
        AddTeam("Spain", "UEFA");

        Assert.ThrowsException<ArgumentException>(() =>
            _service.UpdateTeam("Spain", BuildRequest("Spain", "OFC")));

        AssertRemainingSlots("UEFA", 15);
    }

    [TestMethod]
    public void UpdateTeam_WhenNewConfederationDoesNotExist_ShouldThrowException()
    {
        AddTeam("Spain", "UEFA");

        Assert.ThrowsException<ArgumentException>(() =>
            _service.UpdateTeam("Spain", BuildRequest("Spain", "INVALID")));
    }

    [TestMethod]
    public void UpdateTeam_WhenNewConfederationIsSameAsCurrent_ShouldNotMoveTeam()
    {
        AddTeam("Spain", "UEFA");

        _service.UpdateTeam("Spain", BuildRequest("Spain", "UEFA"));

        AssertRemainingSlots("UEFA", 15);
    }

    [TestMethod]
    public void DeleteTeam_WhenTeamExists_ShouldFreeConfederationSlot()
    {
        AddTeam("Spain", "UEFA");
        AddTeam("Bosnia", "UEFA", 600);

        _service.DeleteTeam("Spain");

        AssertRemainingSlots("UEFA", 15);
    }

    private static TeamService BuildService()
    {
        IRepository<Team> repository = new FakeRepository<Team>();
        var auditService = new AuditService(new FakeRepository<AuditLog>());
        var sessionService = new SessionService(null!, new AuthStateService());

        return new TeamService(repository, auditService, sessionService);
    }

    private void AddTeam(string name, string confederationName, int ranking = 500)
    {
        _service.AddTeam(BuildRequest(name, confederationName, ranking));
    }

    private static DTOs.TeamRequest BuildRequest(string name, string confederationName, int ranking = 500) =>
        new()
        {
            Name = name,
            ConfederationName = confederationName,
            RankingFifa = ranking
        };

    private void AssertRemainingSlots(string confederationName, int expectedRemainingSlots)
    {
        var confederation = _service.GetConfederations().First(c => c.Name == confederationName);

        Assert.AreEqual(expectedRemainingSlots, confederation.RemainingSlots);
    }
}
