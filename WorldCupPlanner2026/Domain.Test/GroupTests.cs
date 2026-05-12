namespace Domain.Test;

[TestClass]
public class GroupTests
{
    private const char ValidLabel = 'A';
    private const char LowercaseLabel = 'a';
    private const char InvalidLabel = 'Z';

    private Group CreateValidGroup() => new(ValidLabel);

    private Team CreateUruguayTeam() =>
        new("URUGUAY", "CONMEBOL", 1500);

    private Team CreateArgentinaTeam() =>
        new("ARGENTINA", "UEFA", 1700);

    private Team CreateBrazilTeam() =>
        new("BRAZIL", "AFC", 1800);

    private Team CreateChileTeam() =>
        new("CHILE", "CAF", 1400);

    private Match CreateMatch(Group group, Team home, Team away) =>
        new Match(
            date: DateTime.Today,
            stadium: new Stadium("Centenario", "Montevideo", "ValidDescription", 20000),
            homeTeam: home,
            awayTeam: away
        );
    

    [TestMethod]
    public void CreateGroup_WithValidUppercaseLabel_ShouldCreateGroup()
    {
        var group = CreateValidGroup();

        Assert.AreEqual(ValidLabel, group.Label);
        Assert.AreEqual(0, group.Teams.Count);
    }

    [TestMethod]
    public void CreateGroup_WithLowercaseLabel_ShouldStoreUppercaseLabel()
    {
        var group = new Group(LowercaseLabel);

        Assert.AreEqual(ValidLabel, group.Label);
    }

    [TestMethod]
    public void UpdateLabel_WithLowercaseLabel_ShouldStoreUppercaseLabel()
    {
        var group = CreateValidGroup();

        group.UpdateLabel('b');

        Assert.AreEqual('B', group.Label);
    }

    [TestMethod]
    public void UpdateLabel_WithInvalidLabel_ShouldThrowArgumentException()
    {
        var group = CreateValidGroup();

        Assert.ThrowsException<ArgumentException>(() =>
            group.UpdateLabel(InvalidLabel));
    }
    

    [TestMethod]
    public void AddTeam_WithValidTeam_ShouldAddTeam()
    {
        var group = CreateValidGroup();
        var team = CreateUruguayTeam();

        group.AddTeam(team);

        Assert.AreEqual(1, group.Teams.Count);
        Assert.AreSame(team, group.Teams[0]);
    }

    [TestMethod]
    public void AddTeam_WithNullTeam_ShouldThrowArgumentException()
    {
        var group = CreateValidGroup();

        Assert.ThrowsException<ArgumentException>(() =>
            group.AddTeam(null));
    }
    
    [TestMethod]
    public void Add2UEFATeamsToGroup_ShouldSucced()
    {
        var group = CreateValidGroup();

        group.AddTeam(new Team("URUGUAY", "UEFA", 1500));
        group.AddTeam(new Team("ARGENTINA", "UEFA", 1700));

        Assert.AreEqual(2, group.Teams.Count);
    }

    [TestMethod]
    public void Add3UEFATeamsToGroup_ShouldThrowException()
    {
        var group = CreateValidGroup();

        group.AddTeam(new Team("URUGUAY", "UEFA", 1500));
        group.AddTeam(new Team("ARGENTINA", "UEFA", 1700));

        Assert.ThrowsException<ArgumentException>(() =>
            group.AddTeam(new Team("BOLIVIA", "UEFA", 1400)));
    }
    
    [TestMethod]
    public void AddTeam_WhenGroupIsFull_ShouldThrowArgumentException()
    {
        var group = CreateValidGroup();
        AddFourTeams(group);

        var extraTeam = new Team("EXTRA", "OFC", 2000);

        Assert.ThrowsException<ArgumentException>(() =>
            group.AddTeam(extraTeam));
    }

    [TestMethod]
    public void RemoveTeam_WithExistingTeam_ShouldRemoveTeam()
    {
        var group = CreateValidGroup();
        var team = CreateArgentinaTeam();

        group.AddTeam(team);
        group.RemoveTeam(team);

        Assert.AreEqual(0, group.Teams.Count);
    }

    [TestMethod]
    public void RemoveTeam_WithNonExistingTeam_ShouldThrowArgumentException()
    {
        var group = CreateValidGroup();
        var team = CreateChileTeam();

        Assert.ThrowsException<ArgumentException>(() =>
            group.RemoveTeam(team));
    }

    [TestMethod]
    public void RemoveTeam_WithNullTeam_ShouldThrowArgumentException()
    {
        var group = CreateValidGroup();

        Assert.ThrowsException<ArgumentException>(() =>
            group.RemoveTeam(null));
    }

    [TestMethod]
    public void RemoveTeam_WithMultipleTeams_ShouldRemoveOnlySpecifiedTeam()
    {
        var group = CreateValidGroup();
        var argentina = CreateArgentinaTeam();
        var brazil = CreateBrazilTeam();

        group.AddTeam(argentina);
        group.AddTeam(brazil);
        group.RemoveTeam(argentina);

        Assert.AreEqual(1, group.Teams.Count);
        Assert.AreEqual(brazil, group.Teams[0]);
    }

    [TestMethod]
    public void Add2NonUEFATeamsFromSameConfederationToGroup_ShouldThrowException()
    {
        var group = CreateValidGroup();

        group.AddTeam(new Team("URUGUAY", "CONMEBOL", 1500));

        Assert.ThrowsException<ArgumentException>(() =>
            group.AddTeam(new Team("ARGENTINA", "CONMEBOL", 1700)));
    }
    
    [TestMethod]
    public void AddTeamsFromDifferentNonUEFAConfederationsToGroup_ShouldSucceed()
    {
        var group = CreateValidGroup();

        group.AddTeam(new Team("URUGUAY", "CONMEBOL", 1500));
        group.AddTeam(new Team("MEXICO", "CONCACAF", 1600));

        Assert.AreEqual(2, group.Teams.Count);
    }
    
    [TestMethod]
    public void AddMatch_WithValidMatch_ShouldAddMatchToGroup()
    {
        var group = CreateValidGroup();
        var uruguay = CreateUruguayTeam();
        var argentina = CreateArgentinaTeam();

        group.AddTeam(uruguay);
        group.AddTeam(argentina);

        var match = CreateMatch(group, uruguay, argentina);
        group.AddMatch(match);

        Assert.AreEqual(1, group.Matches.Count);
    }

    [TestMethod]
    public void AddMatch_WithNullMatch_ShouldThrowArgumentException()
    {
        var group = CreateValidGroup();

        Assert.ThrowsException<ArgumentException>(() =>
            group.AddMatch(null));
    }
    

    [TestMethod]
    public void SetMatchResult_WhenHomeTeamWins_ShouldUpdateTeamStats()
    {
        var group = CreateValidGroup();
        var uruguay = CreateUruguayTeam();
        var argentina = CreateArgentinaTeam();

        group.AddTeam(uruguay);
        group.AddTeam(argentina);

        var match = CreateMatch(group, uruguay, argentina);
        group.AddMatch(match);

        group.SetMatchResult(match, 3, 1);

        Assert.AreEqual(3, uruguay.Points);
        Assert.AreEqual(0, argentina.Points);
        Assert.AreEqual(2, uruguay.GoalDifference);
        Assert.AreEqual(-2, argentina.GoalDifference);
        Assert.AreEqual(3, uruguay.GoalsInFavor);
        Assert.AreEqual(1, argentina.GoalsInFavor);
    }

    [TestMethod]
    public void SetMatchResult_WhenAwayTeamWins_ShouldUpdateTeamStats()
    {
        var group = CreateValidGroup();
        var uruguay = CreateUruguayTeam();
        var argentina = CreateArgentinaTeam();

        group.AddTeam(uruguay);
        group.AddTeam(argentina);

        var match = CreateMatch(group, uruguay, argentina);
        group.AddMatch(match);

        group.SetMatchResult(match, 0, 2);

        Assert.AreEqual(0, uruguay.Points);
        Assert.AreEqual(3, argentina.Points);
        Assert.AreEqual(-2, uruguay.GoalDifference);
        Assert.AreEqual(2, argentina.GoalDifference);
    }

    [TestMethod]
    public void SetMatchResult_WhenDraw_ShouldGiveBothTeamsOnePoint()
    {
        var group = CreateValidGroup();
        var uruguay = CreateUruguayTeam();
        var argentina = CreateArgentinaTeam();

        group.AddTeam(uruguay);
        group.AddTeam(argentina);

        var match = CreateMatch(group, uruguay, argentina);
        group.AddMatch(match);

        group.SetMatchResult(match, 1, 1);

        Assert.AreEqual(1, uruguay.Points);
        Assert.AreEqual(1, argentina.Points);
        Assert.AreEqual(0, uruguay.GoalDifference);
        Assert.AreEqual(0, argentina.GoalDifference);
    }
    

    [TestMethod]
    public void GetStandings_WithNoMatches_ShouldReturnAllTeamsWithZeroPoints()
    {
        var group = CreateValidGroup();
        var uruguay = CreateUruguayTeam();
        var argentina = CreateArgentinaTeam();

        group.AddTeam(uruguay);
        group.AddTeam(argentina);

        var standings = group.GetStandings().ToList();

        Assert.AreEqual(2, standings.Count);
        Assert.IsTrue(standings.All(t => t.Points == 0));
    }

    [TestMethod]
    public void GetStandings_ShouldOrderByPoints()
    {
        var group = CreateValidGroup();
        var uruguay = CreateUruguayTeam();
        var argentina = CreateArgentinaTeam();

        group.AddTeam(uruguay);
        group.AddTeam(argentina);

        var match = CreateMatch(group, uruguay, argentina);
        match.SetResult(2, 0);
        group.AddMatch(match);

        var standings = group.GetStandings().ToList();

        Assert.AreEqual(uruguay, standings[0]);
    }

    [TestMethod]
    public void GetStandings_WhenSamePoints_ShouldOrderByGoalDifference()
    {
        var group = CreateValidGroup();
        var uruguay = CreateUruguayTeam();
        var argentina = CreateArgentinaTeam();
        var brazil = CreateBrazilTeam();
        var chile = CreateChileTeam();

        group.AddTeam(uruguay);
        group.AddTeam(argentina);
        group.AddTeam(brazil);
        group.AddTeam(chile);
        
        var match1 = CreateMatch(group, uruguay, chile);
        match1.SetResult(1, 0);
        group.AddMatch(match1);

        var match2 = CreateMatch(group, argentina, brazil);
        match2.SetResult(3, 0);
        group.AddMatch(match2);

        var standings = group.GetStandings().ToList();

        Assert.AreEqual(argentina, standings[0]);
        Assert.AreEqual(uruguay, standings[1]);
    }

    [TestMethod]
    public void GetStandings_WhenSamePointsAndGoalDifference_ShouldOrderByGoalsFor()
    {
        var group = CreateValidGroup();
        var uruguay = CreateUruguayTeam();
        var argentina = CreateArgentinaTeam();
        var brazil = CreateBrazilTeam();
        var chile = CreateChileTeam();

        group.AddTeam(uruguay);
        group.AddTeam(argentina);
        group.AddTeam(brazil);
        group.AddTeam(chile);

        var match1 = CreateMatch(group, uruguay, argentina);
        match1.SetResult(2, 1);
        group.AddMatch(match1);

        var match2 = CreateMatch(group, brazil, chile);
        match2.SetResult(3, 2);
        group.AddMatch(match2);

        var standings = group.GetStandings().ToList();

        Assert.AreEqual(brazil, standings[0]);
        Assert.AreEqual(uruguay, standings[1]);
    }

    [TestMethod]
    public void GetStandings_AfterMultipleMatches_ShouldReflectAccumulatedStats()
    {
        var group = CreateValidGroup();
        var uruguay = CreateUruguayTeam();
        var argentina = CreateArgentinaTeam();
        var brazil = CreateBrazilTeam();
        var chile = CreateChileTeam();

        group.AddTeam(uruguay);
        group.AddTeam(argentina);
        group.AddTeam(brazil);
        group.AddTeam(chile);

        var match1 = CreateMatch(group, uruguay, argentina);
        match1.SetResult(1, 0);
        group.AddMatch(match1);

        var match2 = CreateMatch(group, uruguay, brazil);
        match2.SetResult(2, 0);
        group.AddMatch(match2);

        var standings = group.GetStandings().ToList();

        Assert.AreEqual(uruguay, standings[0]);
        Assert.AreEqual(6, uruguay.Points);
    }
    

    [TestMethod]
    public void GetTeamByPosition_WithValidPosition_ShouldReturnTeamFromStandings()
    {
        var group = CreateValidGroup();
        var uruguay = CreateUruguayTeam();
        var argentina = CreateArgentinaTeam();

        group.AddTeam(uruguay);
        group.AddTeam(argentina);

        var match = CreateMatch(group, uruguay, argentina);
        match.SetResult(3, 0);
        group.AddMatch(match);

        var result = group.GetTeamByPosition(1);

        Assert.AreEqual(uruguay, result);
    }

    [TestMethod]
    public void GetTeamByPosition_WithInvalidPosition_ShouldThrowArgumentException()
    {
        var group = CreateValidGroup();

        Assert.ThrowsException<ArgumentException>(() =>
            group.GetTeamByPosition(0));

        Assert.ThrowsException<ArgumentException>(() =>
            group.GetTeamByPosition(5));
    }
    
    [TestMethod]
    public void SimulateMatches_WithPendingMatch_ShouldSetMatchResult()
    {
        var group = CreateValidGroup();
        var uruguay = CreateUruguayTeam();
        var argentina = CreateArgentinaTeam();

        group.AddTeam(uruguay);
        group.AddTeam(argentina);

        var match = CreateMatch(group, uruguay, argentina);
        group.AddMatch(match);

        group.SimulateMatches(42);

        Assert.IsNotNull(match.HomeGoals);
        Assert.IsNotNull(match.AwayGoals);
    }
    
    [TestMethod]
    public void SimulateMatches_WithPendingMatch_ShouldUpdateTeamStats()
    {
        var group = CreateValidGroup();
        var uruguay = CreateUruguayTeam();
        var argentina = CreateArgentinaTeam();

        group.AddTeam(uruguay);
        group.AddTeam(argentina);

        var match = CreateMatch(group, uruguay, argentina);
        group.AddMatch(match);

        group.SimulateMatches(42);

        Assert.AreEqual(2, uruguay.Points + argentina.Points);
    }
    
    [TestMethod]
    public void SimulateMatches_WithMultipleMatches_ShouldSetResultForAllMatches()
    {
        var group = CreateValidGroup();

        var uruguay = CreateUruguayTeam();
        var argentina = CreateArgentinaTeam();
        var brazil = CreateBrazilTeam();
        var chile = CreateChileTeam();

        group.AddTeam(uruguay);
        group.AddTeam(argentina);
        group.AddTeam(brazil);
        group.AddTeam(chile);

        var firstMatch = CreateMatch(group, uruguay, argentina);
        var secondMatch = CreateMatch(group, brazil, chile);

        group.AddMatch(firstMatch);
        group.AddMatch(secondMatch);

        group.SimulateMatches(42);

        Assert.IsNotNull(firstMatch.HomeGoals);
        Assert.IsNotNull(firstMatch.AwayGoals);
        Assert.IsNotNull(secondMatch.HomeGoals);
        Assert.IsNotNull(secondMatch.AwayGoals);
    }
    
    [TestMethod]
    public void SimulateMatches_WithSameSeed_ShouldGenerateSameResult()
    {
        var firstGroup = CreateValidGroup();
        var secondGroup = CreateValidGroup();

        var firstHome = CreateUruguayTeam();
        var firstAway = CreateArgentinaTeam();

        var secondHome = CreateUruguayTeam();
        var secondAway = CreateArgentinaTeam();

        firstGroup.AddTeam(firstHome);
        firstGroup.AddTeam(firstAway);

        secondGroup.AddTeam(secondHome);
        secondGroup.AddTeam(secondAway);

        var firstMatch = CreateMatch(firstGroup, firstHome, firstAway);
        var secondMatch = CreateMatch(secondGroup, secondHome, secondAway);

        firstGroup.AddMatch(firstMatch);
        secondGroup.AddMatch(secondMatch);

        firstGroup.SimulateMatches(42);
        secondGroup.SimulateMatches(42);

        Assert.AreEqual(firstMatch.HomeGoals, secondMatch.HomeGoals);
        Assert.AreEqual(firstMatch.AwayGoals, secondMatch.AwayGoals);
    }

    private static void AddFourTeams(Group group)
    {
        group.AddTeam(new Team("TEAM0", "AFC", 1000));
        group.AddTeam(new Team("TEAM1", "CAF", 1001));
        group.AddTeam(new Team("TEAM2", "CONCACAF", 1002));
        group.AddTeam(new Team("TEAM3", "UEFA", 1003));
    }
}