namespace Domain.Test;

[TestClass]
public class GroupConfederationRuleTests
{
    [TestMethod]
    public void EnsureCanAdd_With2UefaTeams_ShouldNotThrow()
    {
        var policy = new GroupConfederationRule();
        var currentTeams = new List<Team> { new("URUGUAY", "UEFA", 1500) };
        var candidate = new Team("ARGENTINA", "UEFA", 1700);

        policy.EnsureCanAdd(currentTeams, candidate);
    }

    [TestMethod]
    public void EnsureCanAdd_With3UefaTeams_ShouldThrowArgumentException()
    {
        var policy = new GroupConfederationRule();
        var currentTeams = new List<Team>
        {
            new("URUGUAY", "UEFA", 1500),
            new("ARGENTINA", "UEFA", 1700)
        };
        var candidate = new Team("BOLIVIA", "UEFA", 1400);

        Assert.ThrowsException<ArgumentException>(() =>
            policy.EnsureCanAdd(currentTeams, candidate));
    }

    [TestMethod]
    public void EnsureCanAdd_With2NonUefaTeamsFromSameConfederation_ShouldThrowArgumentException()
    {
        var policy = new GroupConfederationRule();
        var currentTeams = new List<Team> { new("URUGUAY", "CONMEBOL", 1500) };
        var candidate = new Team("ARGENTINA", "CONMEBOL", 1700);

        Assert.ThrowsException<ArgumentException>(() =>
            policy.EnsureCanAdd(currentTeams, candidate));
    }

    [TestMethod]
    public void EnsureCanAdd_WithTeamsFromDifferentNonUefaConfederations_ShouldNotThrow()
    {
        var policy = new GroupConfederationRule();
        var currentTeams = new List<Team> { new("URUGUAY", "CONMEBOL", 1500) };
        var candidate = new Team("MEXICO", "CONCACAF", 1600);

        policy.EnsureCanAdd(currentTeams, candidate);
    }
}
