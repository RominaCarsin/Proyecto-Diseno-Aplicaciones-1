namespace Domain.Test;

[TestClass]
public class TeamTests
{
    private const string ValidName = "URUGUAY";
    private const string ValidConfederation = "CONMEBOL";
    private const int ValidRanking = 500;
    
    [TestMethod]
    public void CreateTeam_WithValidData_ShouldCreateTeamWithNormalizedAndDefaultProperties()
    {
        var team = new Team("uruguay", "conmebol", ValidRanking);

        Assert.AreEqual("URUGUAY", team.Name);
        Assert.AreEqual("CONMEBOL", team.ConfederationName);
        Assert.AreEqual(ValidRanking, team.RankingFifa);
        Assert.AreEqual(0, team.Points);
        Assert.AreEqual(0, team.GoalDifference);
        Assert.AreEqual(0, team.GoalsInFavor);
    }
    
    [TestMethod]
    public void CreateTeam_WithEmptyName_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Team("", ValidConfederation, ValidRanking));
    }

    [TestMethod]
    public void CreateTeam_WithWhitespaceOnlyName_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Team("   ", ValidConfederation, ValidRanking));
    }

    [TestMethod]
    public void UpdateName_WithLowercaseValue_ShouldNormalizeToUppercase()
    {
        var team = new Team("SPAIN", "UEFA", ValidRanking);

        team.UpdateName("france");

        Assert.AreEqual("FRANCE", team.Name);
    }
    
    [TestMethod]
    public void CreateTeam_WithInvalidConfederation_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Team(ValidName, "INVALID", ValidRanking));
    }

    [TestMethod]
    public void CreateTeam_WithNullConfederation_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Team(ValidName, null!, ValidRanking));
    }

    [TestMethod]
    public void UpdateConfederationName_WithLowercaseValue_ShouldNormalizeToUppercase()
    {
        var team = new Team("SPAIN", "UEFA", ValidRanking);

        team.UpdateConfederationName("afc");

        Assert.AreEqual("AFC", team.ConfederationName);
    }
    
    [TestMethod]
    public void CreateTeam_WithRankingAtMinimumBoundary_ShouldSucceed()
    {
        var team = new Team(ValidName, ValidConfederation, 300);

        Assert.AreEqual(300, team.RankingFifa);
    }

    [TestMethod]
    public void CreateTeam_WithRankingAtMaximumBoundary_ShouldSucceed()
    {
        var team = new Team(ValidName, ValidConfederation, 2500);

        Assert.AreEqual(2500, team.RankingFifa);
    }

    [TestMethod]
    public void CreateTeam_WithRankingBelowMinimum_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Team(ValidName, ValidConfederation, 299));
    }

    [TestMethod]
    public void CreateTeam_WithRankingAboveMaximum_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Team(ValidName, ValidConfederation, 2501));
    }
    
    [TestMethod]
    public void RecordWin_ShouldAddThreePointsAndUpdateGoalStats()
    {
        var team = CreateValidTeam();

        team.RecordWin(goalsScored: 3, goalsConceded: 1);

        Assert.AreEqual(3, team.Points);
        Assert.AreEqual(3, team.GoalsInFavor);
        Assert.AreEqual(2, team.GoalDifference);
    }

    [TestMethod]
    public void RecordWin_WithNegativeScoredGoals_ShouldThrowArgumentException()
    {
        var team = CreateValidTeam();

        Assert.ThrowsException<ArgumentException>(() =>
            team.RecordWin(goalsScored: -1, goalsConceded: 0));
    }

    [TestMethod]
    public void RecordWin_WithNegativeConcededGoals_ShouldThrowArgumentException()
    {
        var team = CreateValidTeam();

        Assert.ThrowsException<ArgumentException>(() =>
            team.RecordWin(goalsScored: 1, goalsConceded: -1));
    }
    
    [TestMethod]
    public void RecordDraw_ShouldAddOnePointAndKeepZeroGoalDifference()
    {
        var team = CreateValidTeam();

        team.RecordDraw(goalsScored: 2);

        Assert.AreEqual(1, team.Points);
        Assert.AreEqual(2, team.GoalsInFavor);
        Assert.AreEqual(0, team.GoalDifference);
    }

    [TestMethod]
    public void RecordDraw_WithNegativeGoals_ShouldThrowArgumentException()
    {
        var team = CreateValidTeam();

        Assert.ThrowsException<ArgumentException>(() =>
            team.RecordDraw(goalsScored: -1));
    }
    
    [TestMethod]
    public void RecordLoss_ShouldAddZeroPointsAndNegativeGoalDifference()
    {
        var team = CreateValidTeam();

        team.RecordLoss(goalsScored: 0, goalsConceded: 4);

        Assert.AreEqual(0, team.Points);
        Assert.AreEqual(0, team.GoalsInFavor);
        Assert.AreEqual(-4, team.GoalDifference);
    }

    [TestMethod]
    public void RecordLoss_WithNegativeScoredGoals_ShouldThrowArgumentException()
    {
        var team = CreateValidTeam();

        Assert.ThrowsException<ArgumentException>(() =>
            team.RecordLoss(goalsScored: -1, goalsConceded: 0));
    }

    [TestMethod]
    public void RecordLoss_WithNegativeConcededGoals_ShouldThrowArgumentException()
    {
        var team = CreateValidTeam();

        Assert.ThrowsException<ArgumentException>(() =>
            team.RecordLoss(goalsScored: 0, goalsConceded: -1));
    }
    
    [TestMethod]
    public void RevertWin_ShouldRestoreStatsToBeforeTheWin()
    {
        var team = CreateValidTeam();
        team.RecordWin(3, 1);

        team.RevertWin(3, 1);

        Assert.AreEqual(0, team.Points);
        Assert.AreEqual(0, team.GoalsInFavor);
        Assert.AreEqual(0, team.GoalDifference);
    }

    [TestMethod]
    public void RevertWin_WithNegativeGoals_ShouldThrowArgumentException()
    {
        var team = CreateValidTeam();

        Assert.ThrowsException<ArgumentException>(() =>
            team.RevertWin(goalsScored: -1, goalsConceded: 0));
    }
    
    [TestMethod]
    public void RevertDraw_ShouldRestoreStatsToBeforeTheDraw()
    {
        var team = CreateValidTeam();
        team.RecordDraw(2);

        team.RevertDraw(2);

        Assert.AreEqual(0, team.Points);
        Assert.AreEqual(0, team.GoalsInFavor);
        Assert.AreEqual(0, team.GoalDifference);
    }
    
    [TestMethod]
    public void RevertLoss_ShouldRestoreStatsToBeforeTheLoss()
    {
        var team = CreateValidTeam();
        team.RecordLoss(1, 3);

        team.RevertLoss(1, 3);

        Assert.AreEqual(0, team.Points);
        Assert.AreEqual(0, team.GoalsInFavor);
        Assert.AreEqual(0, team.GoalDifference);
    }
    
    [TestMethod]
    public void Normalize_WithLowercaseAndSpaces_ShouldReturnNormalizedString()
    {
        var result = Team.Normalize("  uruguay  ");

        Assert.AreEqual("URUGUAY", result);
    }
    
    [TestMethod]
    public void CreateTeam_WithoutFlag_ShouldHaveEmptyFlag()
    {
        var team = new Team(ValidName, ValidConfederation, ValidRanking);

        Assert.AreEqual(string.Empty, team.FlagBase64);
    }
    
    [TestMethod]
    public void UpdateFlag_WithValidBase64_ShouldAssignFlag()
    {
        var team = CreateValidTeam();
        var validBase64 = Convert.ToBase64String(new byte[] { 1, 2, 3, 4 });

        team.UpdateFlag(validBase64);

        Assert.AreEqual(validBase64, team.FlagBase64);
    }
    
    [TestMethod]
    public void UpdateFlag_WithInvalidBase64_ShouldThrowArgumentException()
    {
        var team = CreateValidTeam();

        Assert.ThrowsException<ArgumentException>(() =>
            team.UpdateFlag("esto no es base64 !!!"));
    }
    
    private static Team CreateValidTeam() =>
        new Team(ValidName, ValidConfederation, ValidRanking);
    
    
}