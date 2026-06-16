namespace Domain.Tests;

[TestClass]
public class GroupPhaseTests
{
    [TestMethod]
    public void CreateFixture_WithValidPotsStadiumsAndStartDate_ShouldCreateWithExpectedProperties()
    {
        var fixture = new GroupPhase(
            GroupPhaseTestHelpers.BuildValidPots(),
            GroupPhaseTestHelpers.BuildStadiums(count: 4),
            GroupPhaseTestHelpers.StartDate);

        Assert.IsNotNull(fixture);
        Assert.IsNotNull(fixture.Groups);
        Assert.AreEqual(0, fixture.Groups.Count);
    }

    [TestMethod]
    public void Generate_WithLessThan48Teams_ShouldThrowInvalidOperationException()
    {
        var fixture = new GroupPhase(
            GroupPhaseTestHelpers.BuildPotsWithLessThanRequiredTeams(),
            GroupPhaseTestHelpers.BuildStadiums(count: 4),
            GroupPhaseTestHelpers.StartDate);

        var exception = Assert.ThrowsException<InvalidOperationException>(() =>
            fixture.Generate());

        Assert.AreEqual("Se necesitan 48 equipos para generar el fixture.", exception.Message);
    }

    [TestMethod]
    public void Generate_WithLessThan4Stadiums_ShouldThrowInvalidOperationException()
    {
        var fixture = new GroupPhase(
            GroupPhaseTestHelpers.BuildValidPots(),
            GroupPhaseTestHelpers.BuildStadiums(count: 3),
            GroupPhaseTestHelpers.StartDate);

        var exception = Assert.ThrowsException<InvalidOperationException>(() =>
            fixture.Generate());

        Assert.AreEqual("Se necesitan al menos 4 estadios para generar el fixture.", exception.Message);
    }

    [TestMethod]
    public void Generate_WithAllTeamsFromSameNonUefaConfederation_ShouldThrowInvalidOperationException()
    {
        var fixture = new GroupPhase(
            GroupPhaseTestHelpers.BuildPotsWithOnlyConmebolTeams(),
            GroupPhaseTestHelpers.BuildStadiums(count: 4),
            GroupPhaseTestHelpers.StartDate);

        var exception = Assert.ThrowsException<InvalidOperationException>(() =>
            fixture.Generate());

        StringAssert.StartsWith(exception.Message, "No se pudo asignar el equipo");
    }

    [TestMethod]
    public void Generate_WithMoreThan48Teams_ShouldThrowInvalidOperationException()
    {
        var fixture = new GroupPhase(
            GroupPhaseTestHelpers.BuildPotsWithMoreThanRequiredTeams(),
            GroupPhaseTestHelpers.BuildStadiums(count: 4),
            GroupPhaseTestHelpers.StartDate);

        var exception = Assert.ThrowsException<InvalidOperationException>(() =>
            fixture.Generate());

        StringAssert.StartsWith(exception.Message, "No se pudo asignar el equipo");
    }

    [TestMethod]
    public void Generate_WithValidData_ShouldCreate12Groups()
    {
        var fixture = new GroupPhase(
            GroupPhaseTestHelpers.BuildValidPots(),
            GroupPhaseTestHelpers.BuildStadiums(count: 4),
            GroupPhaseTestHelpers.StartDate);

        fixture.Generate();

        Assert.AreEqual(12, fixture.Groups.Count);
    }

    [TestMethod]
    public void Generate_WithValidData_ShouldAssign4TeamsPerGroup()
    {
        var fixture = new GroupPhase(
            GroupPhaseTestHelpers.BuildValidPots(),
            GroupPhaseTestHelpers.BuildStadiums(count: 4),
            GroupPhaseTestHelpers.StartDate);

        fixture.Generate();

        foreach (var group in fixture.Groups)
            Assert.AreEqual(4, group.Teams.Count);
    }

    [TestMethod]
    public void Generate_WithMoreThan4Stadiums_ShouldCreateFixtureSuccessfully()
    {
        var fixture = new GroupPhase(
            GroupPhaseTestHelpers.BuildValidPots(),
            GroupPhaseTestHelpers.BuildStadiums(count: 5),
            GroupPhaseTestHelpers.StartDate);

        fixture.Generate();

        Assert.AreEqual(12, fixture.Groups.Count);
    }

    [TestMethod]
    public void Generate_WithValidData_ShouldDistributeTeamsByConfederationAcrossGroups()
    {
        var fixture = new GroupPhase(
            GroupPhaseTestHelpers.BuildValidPots(),
            GroupPhaseTestHelpers.BuildStadiums(count: 4),
            GroupPhaseTestHelpers.StartDate);

        fixture.Generate();

        foreach (var confederation in new[] { "UEFA", "CONMEBOL", "AFC" })
        {
            var teamsPerGroup = fixture.Groups
                .Select(g => g.Teams.Count(t => t.ConfederationName == confederation))
                .ToList();

            Assert.IsTrue(
                teamsPerGroup.Max() - teamsPerGroup.Min() <= 1,
                $"Unbalanced distribution for {confederation}: {teamsPerGroup.Max()} vs {teamsPerGroup.Min()}.");
        }

        foreach (var group in fixture.Groups)
        {
            Assert.IsTrue(group.Teams.Count(t => t.ConfederationName == "UEFA") <= 2);

            foreach (var confederation in new[] { "CONMEBOL", "AFC", "CAF", "CONCACAF", "OFC" })
                Assert.IsTrue(group.Teams.Count(t => t.ConfederationName == confederation) <= 1);
        }
    }
}
