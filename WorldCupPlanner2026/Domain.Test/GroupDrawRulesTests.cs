namespace Domain.Tests;

[TestClass]
public class GroupDrawRulesTests
{
    [TestMethod]
    public void AssignTeams_With48TeamsAnd12Groups_ShouldAssignAllTeams()
    {
        var groups = Enumerable.Range(0, 12).Select(i => new Group((char)('A' + i))).ToList();
        var pots = GroupPhaseTestHelpers.BuildValidPots();
        var service = new GroupDrawRules(new ConfederationBalancedStrategy(), new TournamentRules());

        service.AssignTeams(groups, pots);

        int totalAssigned = groups.Sum(g => g.Teams.Count);
        Assert.AreEqual(48, totalAssigned);
    }

    [TestMethod]
    public void AssignTeams_With48TeamsAnd12Groups_ShouldAssign4PerGroup()
    {
        var groups = Enumerable.Range(0, 12).Select(i => new Group((char)('A' + i))).ToList();
        var pots = GroupPhaseTestHelpers.BuildValidPots();
        var service = new GroupDrawRules(new ConfederationBalancedStrategy(), new TournamentRules());

        service.AssignTeams(groups, pots);

        foreach (var group in groups)
            Assert.AreEqual(4, group.Teams.Count);
    }

    [TestMethod]
    public void AssignTeams_WithAllSameConfederation_ShouldThrowInvalidOperationException()
    {
        var groups = Enumerable.Range(0, 12).Select(i => new Group((char)('A' + i))).ToList();
        var pots = GroupPhaseTestHelpers.BuildPotsWithOnlyConmebolTeams();
        var service = new GroupDrawRules(new ConfederationBalancedStrategy(), new TournamentRules());

        var exception = Assert.ThrowsException<InvalidOperationException>(() =>
            service.AssignTeams(groups, pots));

        StringAssert.StartsWith(exception.Message, "No se pudo asignar el equipo");
    }

    [TestMethod]
    public void AssignTeams_WithMoreThan48Teams_ShouldThrowInvalidOperationException()
    {
        var groups = Enumerable.Range(0, 12).Select(i => new Group((char)('A' + i))).ToList();
        var pots = GroupPhaseTestHelpers.BuildPotsWithMoreThanRequiredTeams();
        var service = new GroupDrawRules(new ConfederationBalancedStrategy(), new TournamentRules());

        var exception = Assert.ThrowsException<InvalidOperationException>(() =>
            service.AssignTeams(groups, pots));

        StringAssert.StartsWith(exception.Message, "No se pudo asignar el equipo");
    }

    [TestMethod]
    public void AssignTeams_ShouldDistributeByConfederationEvenly()
    {
        var groups = Enumerable.Range(0, 12).Select(i => new Group((char)('A' + i))).ToList();
        var pots = GroupPhaseTestHelpers.BuildValidPots();
        var service = new GroupDrawRules(new ConfederationBalancedStrategy(), new TournamentRules());

        service.AssignTeams(groups, pots);

        foreach (var confederation in new[] { "UEFA", "CONMEBOL", "AFC" })
        {
            var teamsPerGroup = groups
                .Select(g => g.Teams.Count(t => t.ConfederationName == confederation))
                .ToList();

            Assert.IsTrue(
                teamsPerGroup.Max() - teamsPerGroup.Min() <= 1,
                $"Unbalanced distribution for {confederation}: {teamsPerGroup.Max()} vs {teamsPerGroup.Min()}.");
        }

        foreach (var group in groups)
        {
            Assert.IsTrue(group.Teams.Count(t => t.ConfederationName == "UEFA") <= 2);

            foreach (var confederation in new[] { "CONMEBOL", "AFC", "CAF", "CONCACAF", "OFC" })
                Assert.IsTrue(group.Teams.Count(t => t.ConfederationName == confederation) <= 1);
        }
    }
}
