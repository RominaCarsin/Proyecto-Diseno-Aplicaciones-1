namespace Domain.Test;

[TestClass]
public class GroupTests
{
    private const char ValidLabel = 'A';
    private const char LowercaseLabel = 'a';
    private Group CreateValidGroup() => new(ValidLabel);

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
    public void AddTeam_WithValidTeam_ShouldAddTeam()
    {
        var group = CreateValidGroup();
        var team = TeamTestData.CreateUruguayTeam();

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
    public void AddTeam_WhenGroupIsFull_ShouldThrowArgumentException()
    {
        var group = CreateValidGroup();
        AddFourTeams(group);

        var extraTeam = new Team("EXTRA", "OFC", 2000);

        Assert.ThrowsException<ArgumentException>(() =>
            group.AddTeam(extraTeam));
    }

    [TestMethod]
    public void AddMatch_WithValidMatch_ShouldAddMatchToGroup()
    {
        var group = CreateValidGroup();
        var uruguay = TeamTestData.CreateUruguayTeam();
        var argentina = TeamTestData.CreateArgentinaTeam();

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
        var uruguay = TeamTestData.CreateUruguayTeam();
        var argentina = TeamTestData.CreateArgentinaTeam();

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
        var uruguay = TeamTestData.CreateUruguayTeam();
        var argentina = TeamTestData.CreateArgentinaTeam();

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
        var uruguay = TeamTestData.CreateUruguayTeam();
        var argentina = TeamTestData.CreateArgentinaTeam();

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
    public void GetTeamByPosition_WithValidPosition_ShouldReturnTeamFromStandings()
    {
        var group = CreateValidGroup();
        var uruguay = TeamTestData.CreateUruguayTeam();
        var argentina = TeamTestData.CreateArgentinaTeam();

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
    public void FinishMatch_WithValidMatch_ShouldMarkMatchAsFinished()
    {
        var group = CreateValidGroup();
        var uruguay = TeamTestData.CreateUruguayTeam();
        var argentina = TeamTestData.CreateArgentinaTeam();

        group.AddTeam(uruguay);
        group.AddTeam(argentina);

        var match = CreateMatch(group, uruguay, argentina);

        group.FinishMatch(match);

        Assert.AreEqual(MatchStatus.Finished, match.Status);
    }
    
    [TestMethod]
    public void FinishPlayedMatches_ShouldFinishOnlyMatchesWithResultAndReturnThem()
    {
        var group = CreateValidGroup();
        var uruguay = TeamTestData.CreateUruguayTeam();
        var argentina = TeamTestData.CreateArgentinaTeam();
        var brazil = TeamTestData.CreateBrazilTeam();
        var chile = TeamTestData.CreateChileTeam();

        group.AddTeam(uruguay);
        group.AddTeam(argentina);
        group.AddTeam(brazil);
        group.AddTeam(chile);

        var matchWithResult = CreateMatch(group, uruguay, argentina);
        var matchWithoutResult = CreateMatch(group, brazil, chile);
        group.AddMatch(matchWithResult);
        group.AddMatch(matchWithoutResult);

        // Solo uno tiene resultado cargado.
        group.SetMatchResult(matchWithResult, 2, 1);

        var finished = group.FinishPlayedMatches().ToList();

        // El que tenía resultado quedó finalizado y fue devuelto.
        Assert.AreEqual(1, finished.Count);
        Assert.AreSame(matchWithResult, finished[0]);
        Assert.AreEqual(MatchStatus.Finished, matchWithResult.Status);

        // El que NO tenía resultado sigue sin finalizar.
        Assert.AreEqual(MatchStatus.NotFinished, matchWithoutResult.Status);
    }
    
    [TestMethod]
    public void FinishPlayedMatches_CalledTwice_ShouldNotReturnAlreadyFinishedMatches()
    {
        var group = CreateValidGroup();
        var uruguay = TeamTestData.CreateUruguayTeam();
        var argentina = TeamTestData.CreateArgentinaTeam();
        var brazil = TeamTestData.CreateBrazilTeam();
        var chile = TeamTestData.CreateChileTeam();

        group.AddTeam(uruguay);
        group.AddTeam(argentina);
        group.AddTeam(brazil);
        group.AddTeam(chile);

        var match1 = CreateMatch(group, uruguay, argentina);
        var match2 = CreateMatch(group, brazil, chile);
        group.AddMatch(match1);
        group.AddMatch(match2);
        group.SetMatchResult(match1, 1, 0);
        group.SetMatchResult(match2, 0, 2);

        var firstCall = group.FinishPlayedMatches().ToList();
        var secondCall = group.FinishPlayedMatches().ToList();

        Assert.AreEqual(2, firstCall.Count);   // ambos se finalizaron la primera vez
        Assert.AreEqual(0, secondCall.Count);  // ya finalizados → no se devuelven de nuevo
    }
    
    [TestMethod]
    public void FinishPlayedMatches_WithNoPlayedMatches_ShouldReturnEmpty()
    {
        var group = CreateValidGroup();
        var uruguay = TeamTestData.CreateUruguayTeam();
        var argentina = TeamTestData.CreateArgentinaTeam();

        group.AddTeam(uruguay);
        group.AddTeam(argentina);

        // Un partido agregado pero SIN resultado.
        var match = CreateMatch(group, uruguay, argentina);
        group.AddMatch(match);

        var finished = group.FinishPlayedMatches().ToList();

        Assert.AreEqual(0, finished.Count);
        Assert.AreEqual(MatchStatus.NotFinished, match.Status);
    }

    [TestMethod]
    public void AddTeam_With3UefaTeams_ShouldThrowArgumentException()
    {
        var group = CreateValidGroup();

        group.AddTeam(new Team("URUGUAY", "UEFA", 1500));
        group.AddTeam(new Team("ARGENTINA", "UEFA", 1700));

        Assert.ThrowsException<ArgumentException>(() =>
            group.AddTeam(new Team("BOLIVIA", "UEFA", 1400)));
    }

    [TestMethod]
    public void GetOrderedTeamsByPosition_WithMatches_ShouldReturnOrderedStandings()
    {
        var group = CreateValidGroup();
        var uruguay = TeamTestData.CreateUruguayTeam();
        var argentina = TeamTestData.CreateArgentinaTeam();

        group.AddTeam(uruguay);
        group.AddTeam(argentina);

        var match = CreateMatch(group, uruguay, argentina);
        match.SetResult(2, 0);
        group.AddMatch(match);

        var standings = group.GetOrderedTeamsByPosition().ToList();

        Assert.AreEqual(2, standings.Count);
        Assert.AreEqual(uruguay, standings[0]);
    }

    [TestMethod]
    public void CreateGroup_GroupId_ShouldDefaultToZero()
    {
        var group = CreateValidGroup();

        Assert.AreEqual(0, group.GroupId);
    }

    [TestMethod]
    public void CreateGroup_WithLabelAtMinBoundary_ShouldSucceed()
    {
        var group = new Group('A');

        Assert.AreEqual('A', group.Label);
    }

    [TestMethod]
    public void CreateGroup_WithLabelAtMaxBoundary_ShouldSucceed()
    {
        var group = new Group('L');

        Assert.AreEqual('L', group.Label);
    }

    [TestMethod]
    public void CreateGroup_WithLabelBelowMinBoundary_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Group('@'));
    }

    [TestMethod]
    public void CreateGroup_WithLabelAboveMaxBoundary_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Group('M'));
    }

    [TestMethod]
    public void CreateGroup_WithLowercaseMaxLabel_ShouldNormalizeToUppercase()
    {
        var group = new Group('l');

        Assert.AreEqual('L', group.Label);
    }

    [TestMethod]
    public void SetMatchSimulationResult_WithValidData_ShouldApplyResultAndRegisterCards()
    {
        var group = CreateValidGroup();
        var uruguay = TeamTestData.CreateUruguayTeam();
        var argentina = TeamTestData.CreateArgentinaTeam();
        group.AddTeam(uruguay);
        group.AddTeam(argentina);

        var match = CreateMatch(group, uruguay, argentina);
        group.AddMatch(match);

        var result = new SimulationResult(2, 1, 3, 2, 1, 0);
        var random = new Random(42);

        group.SetMatchSimulationResult(match, result, random);

        Assert.AreEqual(2, match.HomeGoals);
        Assert.AreEqual(1, match.AwayGoals);
        Assert.AreEqual(uruguay, match.Winner);
    }

    [TestMethod]
    public void SetMatchSimulationResult_WithNullMatch_ShouldThrowArgumentException()
    {
        var group = CreateValidGroup();

        Assert.ThrowsException<ArgumentException>(() =>
            group.SetMatchSimulationResult(null, new SimulationResult(1, 0), new Random(42)));
    }

    private static void AddFourTeams(Group group)
    {
        group.AddTeam(new Team("TEAM0", "AFC", 1000));
        group.AddTeam(new Team("TEAM1", "CAF", 1001));
        group.AddTeam(new Team("TEAM2", "CONCACAF", 1002));
        group.AddTeam(new Team("TEAM3", "UEFA", 1003));
    }
}