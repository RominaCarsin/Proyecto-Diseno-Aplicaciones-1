namespace Domain.Tests;

[TestClass]
public class FixtureTests
{
    private static readonly DateTime StartDate = new(2026, 6, 1);
    
    [TestMethod]
    public void CreateFixture_WithValidPotsStadiumsAndStartDate_ShouldCreateWithExpectedProperties()
    {
        var fixture = new Fixture(BuildValidPots(), BuildStadiums(count: 4), StartDate);

        Assert.IsNotNull(fixture);
        Assert.IsNotNull(fixture.Groups);
        Assert.AreEqual(0, fixture.Groups.Count);
    }
    
    [TestMethod]
    public void Generate_WithLessThan48Teams_ShouldThrowInvalidOperationException()
    {
        var fixture = new Fixture(BuildPotsWithLessThanRequiredTeams(), BuildStadiums(count: 4), StartDate);

        var exception = Assert.ThrowsException<InvalidOperationException>(() =>
            fixture.Generate());

        Assert.AreEqual("Se necesitan 48 equipos para generar el fixture.", exception.Message);
    }

    [TestMethod]
    public void Generate_WithLessThan4Stadiums_ShouldThrowInvalidOperationException()
    {
        var fixture = new Fixture(BuildValidPots(), BuildStadiums(count: 3), StartDate);

        var exception = Assert.ThrowsException<InvalidOperationException>(() =>
            fixture.Generate());

        Assert.AreEqual("Se necesitan al menos 4 estadios para generar el fixture.", exception.Message);
    }

    [TestMethod]
    public void Generate_WithAllTeamsFromSameNonUefaConfederation_ShouldThrowInvalidOperationException()
    {
        var fixture = new Fixture(BuildPotsWithOnlyConmebolTeams(), BuildStadiums(count: 4), StartDate);

        var exception = Assert.ThrowsException<InvalidOperationException>(() =>
            fixture.Generate());

        StringAssert.StartsWith(exception.Message, "No se pudo asignar el equipo");
    }

    [TestMethod]
    public void Generate_WithMoreThan48Teams_ShouldThrowInvalidOperationException()
    {
        var fixture = new Fixture(BuildPotsWithMoreThanRequiredTeams(), BuildStadiums(count: 4), StartDate);

        var exception = Assert.ThrowsException<InvalidOperationException>(() =>
            fixture.Generate());

        StringAssert.StartsWith(exception.Message, "No se pudo asignar el equipo");
    }
    
    [TestMethod]
    public void Generate_WithValidData_ShouldCreate12Groups()
    {
        var fixture = new Fixture(BuildValidPots(), BuildStadiums(count: 4), StartDate);

        fixture.Generate();

        Assert.AreEqual(12, fixture.Groups.Count);
    }

    [TestMethod]
    public void Generate_WithValidData_ShouldAssign4TeamsPerGroup()
    {
        var fixture = new Fixture(BuildValidPots(), BuildStadiums(count: 4), StartDate);

        fixture.Generate();

        foreach (var group in fixture.Groups)
            Assert.AreEqual(4, group.Teams.Count);
    }

    [TestMethod]
    public void Generate_WithValidData_ShouldCreate6MatchesPerGroup()
    {
        var fixture = new Fixture(BuildValidPots(), BuildStadiums(count: 4), StartDate);

        fixture.Generate();

        foreach (var group in fixture.Groups)
            Assert.AreEqual(6, group.Matches.Count);
    }

    [TestMethod]
    public void Generate_WithValidData_ShouldCreate72TotalMatchesWithGroupStagePhase()
    {
        var fixture = new Fixture(BuildValidPots(), BuildStadiums(count: 4), StartDate);

        fixture.Generate();

        var allMatches = fixture.Groups.SelectMany(g => g.Matches).ToList();

        Assert.AreEqual(72, allMatches.Count);
        Assert.IsTrue(allMatches.All(m => m.Phase == TournamentPhase.GroupStage));
    }

    [TestMethod]
    public void Generate_WithMoreThan4Stadiums_ShouldCreateFixtureSuccessfully()
    {
        var fixture = new Fixture(BuildValidPots(), BuildStadiums(count: 5), StartDate);

        fixture.Generate();

        Assert.AreEqual(12, fixture.Groups.Count);
    }
    
    [TestMethod]
    public void Generate_WithValidData_ShouldDistributeTeamsByConfederationAcrossGroups()
    {
        var fixture = new Fixture(BuildValidPots(), BuildStadiums(count: 4), StartDate);

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
    
    [TestMethod]
    public void Generate_WithValidData_ShouldScheduleMatchesWithExpectedDatesAndTimes()
    {
        var fixture = new Fixture(BuildValidPots(), BuildStadiums(count: 4), StartDate);

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
    public void Generate_WithValidData_ShouldAssignStadiumsInRoundRobinOrder()
    {
        var stadiums = BuildStadiums(count: 4);
        var fixture = new Fixture(BuildValidPots(), stadiums, StartDate);

        fixture.Generate();

        var matches = fixture.Groups.First().Matches.ToList();

        for (int i = 0; i < matches.Count; i++)
            Assert.AreSame(stadiums[i % stadiums.Count], matches[i].Stadium);
    }
    
    [TestMethod]
    public void SimulateGroupStage_WithValidSeed_ShouldSetResultOnAllMatchesAndUpdateStandings()
    {
        var fixture = new Fixture(BuildValidPots(), BuildStadiums(count: 4), StartDate);
        fixture.Generate();

        fixture.SimulateGroupStage(seed: 42);

        var allMatches = fixture.Groups.SelectMany(g => g.Matches).ToList();
        var allTeams = fixture.Groups.SelectMany(g => g.Teams).ToList();

        Assert.IsTrue(allMatches.All(m => m.HasResult));
        Assert.IsTrue(allTeams.Any(t => t.Points > 0));
    }

    [TestMethod]
    public void SimulateGroupStage_WithoutGeneratedFixture_ShouldThrowInvalidOperationException()
    {
        var fixture = new Fixture(BuildValidPots(), BuildStadiums(count: 4), StartDate);

        var exception = Assert.ThrowsException<InvalidOperationException>(() =>
            fixture.SimulateGroupStage(seed: 1));

        Assert.AreEqual("No se ha generado el fixture todavía.", exception.Message);
    }
    
    [TestMethod]
    public void FromExistingGroups_WithValidGroups_ShouldCreateFixtureWithThoseGroups()
    {
        var groups = new List<Group> { new('A'), new('B') };

        var fixture = Fixture.FromExistingGroups(groups);

        Assert.AreEqual(2, fixture.Groups.Count);
        Assert.AreSame(groups[0], fixture.Groups.ElementAt(0));
        Assert.AreSame(groups[1], fixture.Groups.ElementAt(1));
    }
    
    private static List<Pot> BuildValidPots() => new()
    {
        new Pot(1, BuildTeams("UEFA",     "UEA-", count: 12, startRanking:  600)),
        new Pot(2, BuildTeams("UEFA",     "UEB-", count: 12, startRanking:  800)),
        new Pot(3, BuildTeams("CONMEBOL", "CON-", count: 12, startRanking: 1000)),
        new Pot(4, BuildTeams("AFC",      "AFC-", count: 12, startRanking: 1200)),
    };

    private static List<Pot> BuildPotsWithLessThanRequiredTeams() => new()
    {
        new Pot(1, BuildTeams("UEFA",     "A-", count: 12, startRanking:  600)),
        new Pot(2, BuildTeams("CONMEBOL", "B-", count: 12, startRanking:  800)),
        new Pot(3, BuildTeams("AFC",      "C-", count: 12, startRanking: 1000)),
    };

    private static List<Pot> BuildPotsWithMoreThanRequiredTeams() => new()
    {
        new Pot(1, BuildTeams("UEFA",     "UEA-", count: 12, startRanking:  600)),
        new Pot(2, BuildTeams("UEFA",     "UEB-", count: 12, startRanking:  800)),
        new Pot(3, BuildTeams("CONMEBOL", "CON-", count: 12, startRanking: 1000)),
        new Pot(4, BuildTeams("AFC",      "AFC-", count: 12, startRanking: 1200)),
        new Pot(4, BuildTeams("CAF",      "CAF-", count: 12, startRanking: 1400)),
    };

    private static List<Pot> BuildPotsWithOnlyConmebolTeams() => new()
    {
        new Pot(1, BuildTeams("CONMEBOL", "CON-A-", count: 12, startRanking:  600)),
        new Pot(2, BuildTeams("CONMEBOL", "CON-B-", count: 12, startRanking:  800)),
        new Pot(3, BuildTeams("CONMEBOL", "CON-C-", count: 12, startRanking: 1000)),
        new Pot(4, BuildTeams("CONMEBOL", "CON-D-", count: 12, startRanking: 1200)),
    };

    private static List<Team> BuildTeams(
        string confederation,
        string prefix,
        int count,
        int startRanking)
    {
        var teams = new List<Team>(count);

        for (int i = 0; i < count; i++)
            teams.Add(new Team($"{prefix}{i:00}", confederation, startRanking + i * 7));

        return teams;
    }

    private static List<Stadium> BuildStadiums(int count)
    {
        var stadiums = new List<Stadium>(count);

        for (int i = 0; i < count; i++)
            stadiums.Add(new Stadium($"STADIUM_{i:00}", "CITY", "Historic stadium", 30000));

        return stadiums;
    }
}