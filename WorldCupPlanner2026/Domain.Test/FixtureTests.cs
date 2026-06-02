namespace Domain.Tests;

[TestClass]
public class FixtureTests
{
    private static readonly DateTime StartDate = new(2026, 6, 1);

    private static List<Team> BuildTeams(string confederation, string prefix, int count, int startRanking)
    {
        var teams = new List<Team>(count);

        for (int i = 0; i < count; i++)
            teams.Add(new Team($"{prefix}{i:00}", confederation, startRanking + i * 7));

        return teams;
    }

    private static List<Pot> BuildValidPots() => new()
    {
        new Pot(1, BuildTeams("UEFA", "UEA-", 12, 600)),
        new Pot(2, BuildTeams("UEFA", "UEB-", 12, 800)),
        new Pot(3, BuildTeams("CONMEBOL", "CON-", 12, 1000)),
        new Pot(4, BuildTeams("AFC", "AFC-", 12, 1200)),
    };

    private static List<Stadium> BuildStadiums(int count)
    {
        var stadiums = new List<Stadium>(count);

        for (int i = 0; i < count; i++)
            stadiums.Add(new Stadium($"Stadium {i:00}", "City", "desc", 30000));

        return stadiums;
    }

    [TestMethod]
    public void CreateFixture_WithValidPotsStadiumsAndStartDate_ShouldCreateWithExpectedProperties()
    {
        var fixture = new Fixture(BuildValidPots(), BuildStadiums(4), StartDate);

        Assert.IsNotNull(fixture);
        Assert.IsNotNull(fixture.Groups);
        Assert.AreEqual(0, fixture.Groups.Count);
    }
    
    [TestMethod]
    public void Generate_WithValidData_ShouldCreateTwelveGroups()
    {
        var fixture = new Fixture(BuildValidPots(), BuildStadiums(4), StartDate);

        fixture.Generate();

        Assert.AreEqual(12, fixture.Groups.Count);
    }
    
    [TestMethod]
    public void Generate_WithLessThanFortyEightTeams_ShouldThrowException()
    {
        var fixture = new Fixture(BuildPotsWithLessThanRequiredTeams(), BuildStadiums(4), StartDate);

        var exception = Assert.ThrowsException<InvalidOperationException>(() => fixture.Generate());

        Assert.AreEqual("Se necesitan 48 equipos para generar el fixture.", exception.Message);
    }
    
    private static List<Pot> BuildPotsWithLessThanRequiredTeams() => new()
    {
        new Pot(1, BuildTeams("UEFA", "A-", 12, 600)),
        new Pot(2, BuildTeams("CONMEBOL", "B-", 12, 800)),
        new Pot(3, BuildTeams("AFC", "C-", 12, 1000)),
    };
    
    [TestMethod]
    public void Generate_WithLessThanFourStadiums_ShouldThrowException()
    {
        var fixture = new Fixture(BuildValidPots(), BuildStadiums(3), StartDate);

        var exception = Assert.ThrowsException<InvalidOperationException>(() => fixture.Generate());

        Assert.AreEqual("Se necesitan al menos 4 estadios para generar el fixture.", exception.Message);
    }
    
    [TestMethod]
    public void Generate_WithValidData_ShouldAssignFourTeamsPerGroup()
    {
        var fixture = new Fixture(BuildValidPots(), BuildStadiums(4), StartDate);

        fixture.Generate();

        foreach (var group in fixture.Groups)
            Assert.AreEqual(4, group.Teams.Count);
    }
    
    [TestMethod]
    public void Generate_WithValidData_ShouldBalanceTeamsByConfederation()
    {
        var fixture = new Fixture(BuildValidPots(), BuildStadiums(4), StartDate);

        fixture.Generate();

        foreach (var confederation in new[] { "UEFA", "CONMEBOL", "AFC" })
        {
            var perGroup = fixture.Groups
                .Select(g => g.Teams.Count(t => t.ConfederationName == confederation))
                .ToList();

            Assert.IsTrue(
                perGroup.Max() - perGroup.Min() <= 1,
                $"Reparto desbalanceado para {confederation}: {perGroup.Max()} vs {perGroup.Min()}.");
        }

        foreach (var group in fixture.Groups)
        {
            Assert.IsTrue(group.Teams.Count(t => t.ConfederationName == "UEFA") <= 2);

            foreach (var other in new[] { "CONMEBOL", "AFC", "CAF", "CONCACAF", "OFC" })
                Assert.IsTrue(group.Teams.Count(t => t.ConfederationName == other) <= 1);
        }
    }
    
    [TestMethod]
    public void Generate_WithValidData_ShouldCreateExpectedGroupStageMatches()
    {
        var fixture = new Fixture(BuildValidPots(), BuildStadiums(4), StartDate);

        fixture.Generate();

        foreach (var group in fixture.Groups)
            Assert.AreEqual(6, group.Matches.Count);

        var allMatches = fixture.Groups.SelectMany(g => g.Matches).ToList();

        Assert.AreEqual(72, allMatches.Count);

        foreach (var match in allMatches)
            Assert.AreEqual(TournamentPhase.GroupStage, match.Phase);
    }
    
    [TestMethod]
    public void Generate_WithValidData_ShouldAssignMatchDatesUsingExpectedSchedule()
    {
        var fixture = new Fixture(BuildValidPots(), BuildStadiums(4), StartDate);

        fixture.Generate();

        var matches = fixture.Groups.First().Matches.ToList();
        var baseDate = StartDate.Date;

        Assert.AreEqual(baseDate.AddHours(14), matches[0].Date);

        Assert.AreEqual(baseDate.AddHours(18), matches[1].Date);
        Assert.AreEqual(matches[0].Date.AddHours(4), matches[1].Date);

        Assert.AreEqual(baseDate.AddDays(3).AddHours(14), matches[2].Date);
        Assert.AreEqual(baseDate.AddDays(3).AddHours(18), matches[3].Date);
        Assert.AreEqual(matches[2].Date.AddHours(4), matches[3].Date);

        Assert.AreEqual(baseDate.AddDays(6).AddHours(14), matches[4].Date);
        Assert.AreEqual(baseDate.AddDays(6).AddHours(14), matches[5].Date);
    }
    
    [TestMethod]
    public void Generate_WithValidData_ShouldAssignStadiumsRoundRobin()
    {
        var stadiums = BuildStadiums(4);
        var fixture = new Fixture(BuildValidPots(), stadiums, StartDate);

        fixture.Generate();

        var matches = fixture.Groups.First().Matches.ToList();

        for (int i = 0; i < matches.Count; i++)
            Assert.AreSame(stadiums[i % stadiums.Count], matches[i].Stadium);
    }
    
    [TestMethod]
    public void SimulateGroupStage_WithValidSeed_ShouldUpdateGroupStandings()
    {
        var fixture = new Fixture(BuildValidPots(), BuildStadiums(4), StartDate);
        fixture.Generate();

        fixture.SimulateGroupStage(42);

        var allMatches = fixture.Groups.SelectMany(g => g.Matches).ToList();
        var allTeams = fixture.Groups.SelectMany(g => g.Teams).ToList();

        foreach (var match in allMatches)
            Assert.IsTrue(match.HasResult);

        Assert.IsTrue(allTeams.Any(team => team.Points > 0));
    }
}