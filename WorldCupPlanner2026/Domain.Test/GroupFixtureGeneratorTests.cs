namespace Domain.Tests;

[TestClass]
public class GroupFixtureGeneratorTests
{
    [TestMethod]
    public void Generate_WithValidData_ShouldScheduleMatchesWithExpectedDatesAndTimes()
    {
        var groups = CreateGroupsWithTeams();
        var stadiums = GroupPhaseTestHelpers.BuildStadiums(4);
        var generator = new GroupFixtureGenerator(new MatchFactory(), new TournamentRules());

        generator.Generate(groups, stadiums, GroupPhaseTestHelpers.StartDate);

        var matches = groups.First().Matches.ToList();
        var baseDate = GroupPhaseTestHelpers.StartDate.Date;

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
        var groups = CreateGroupsWithTeams();
        var stadiums = GroupPhaseTestHelpers.BuildStadiums(4);
        var generator = new GroupFixtureGenerator(new MatchFactory(), new TournamentRules());

        generator.Generate(groups, stadiums, GroupPhaseTestHelpers.StartDate);

        var matches = groups.First().Matches.ToList();

        for (int i = 0; i < matches.Count; i++)
            Assert.AreSame(stadiums[i % stadiums.Count], matches[i].Stadium);
    }

    [TestMethod]
    public void Generate_WithValidData_ShouldCreate6MatchesPerGroup()
    {
        var groups = CreateGroupsWithTeams();
        var stadiums = GroupPhaseTestHelpers.BuildStadiums(4);
        var generator = new GroupFixtureGenerator(new MatchFactory(), new TournamentRules());

        generator.Generate(groups, stadiums, GroupPhaseTestHelpers.StartDate);

        foreach (var group in groups)
            Assert.AreEqual(6, group.Matches.Count);
    }

    [TestMethod]
    public void Generate_With12Groups_ShouldCreate72TotalMatchesWithGroupStagePhase()
    {
        var groups = Create12GroupsWithTeams();
        var stadiums = GroupPhaseTestHelpers.BuildStadiums(4);
        var generator = new GroupFixtureGenerator(new MatchFactory(), new TournamentRules());

        generator.Generate(groups, stadiums, GroupPhaseTestHelpers.StartDate);

        var allMatches = groups.SelectMany(g => g.Matches).ToList();

        Assert.AreEqual(72, allMatches.Count);
        Assert.IsTrue(allMatches.All(m => m.Phase == TournamentPhase.GroupStage));
    }

    private static List<Group> CreateGroupsWithTeams()
    {
        var groups = new List<Group> { new Group('A') };
        var teams = new List<Team>
        {
            new Team("T-00", "UEFA",     600),
            new Team("T-01", "CONMEBOL", 800),
            new Team("T-02", "AFC",      1000),
            new Team("T-03", "CAF",      1200),
        };

        foreach (var team in teams)
            groups[0].AddTeam(team);

        return groups;
    }

    private static List<Group> Create12GroupsWithTeams()
    {
        var groups = new List<Group>(12);

        for (int i = 0; i < 12; i++)
            groups.Add(new Group((char)('A' + i)));

        var pots = GroupPhaseTestHelpers.BuildValidPots();
        var allTeams = pots.SelectMany(p => p.Teams).ToList();

        for (int i = 0; i < allTeams.Count; i++)
            groups[i % 12].AddTeam(allTeams[i]);

        return groups;
    }
}
