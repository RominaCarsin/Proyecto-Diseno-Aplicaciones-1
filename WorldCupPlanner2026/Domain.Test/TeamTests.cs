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
    public void ApplyRankingChange_WithPositiveDelta_ShouldIncreaseRankingRounded()
    {
        var team = new Team(ValidName, ValidConfederation, 1200);

        team.ApplyRankingChange(20.02);

        // 1200 + 20.02 = 1220.02 → redondea a 1220
        Assert.AreEqual(1220, team.RankingFifa);
    }
    
    [TestMethod]
    public void ApplyRankingChange_WithExactHalfDelta_ShouldRoundUp()
    {
        var team = new Team(ValidName, ValidConfederation, 1500);

        team.ApplyRankingChange(10.5);

        // 1510.5 → con AwayFromZero redondea a 1511 (no a 1510 como haría banker's rounding)
        Assert.AreEqual(1511, team.RankingFifa);
    }
    
    [TestMethod]
    public void ApplyRankingChange_ExceedingMaximum_ShouldClampTo2500()
    {
        var team = new Team(ValidName, ValidConfederation, 2490);

        team.ApplyRankingChange(50);

        // 2540 → clamp a 2500
        Assert.AreEqual(2500, team.RankingFifa);
    }
    
    [TestMethod]
    public void ApplyRankingChange_BelowMinimum_ShouldClampTo300()
    {
        var team = new Team(ValidName, ValidConfederation, 320);

        team.ApplyRankingChange(-50);

        // 270 → clamp a 300
        Assert.AreEqual(300, team.RankingFifa);
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
    
    [TestMethod]
    public void UpdateFlag_WithEmptyOrWhitespace_ShouldResetToEmpty()
    {
        var team = CreateValidTeam();
        team.UpdateFlag(Convert.ToBase64String(new byte[] { 9, 9 }));

        team.UpdateFlag("   ");

        Assert.AreEqual(string.Empty, team.FlagBase64);
    }

    [TestMethod]
    public void RecordMultipleResults_ShouldAccumulateCorrectly()
    {
        var team = CreateValidTeam();

        team.RecordWin(goalsScored: 3, goalsConceded: 1);
        team.RecordDraw(goalsScored: 1);
        team.RecordLoss(goalsScored: 0, goalsConceded: 2);

        Assert.AreEqual(4, team.Points);
        Assert.AreEqual(4, team.GoalsInFavor);
        Assert.AreEqual(0, team.GoalDifference);
    }

    [TestMethod]
    public void RevertLoss_WithNegativeScoredGoals_ShouldThrowArgumentException()
    {
        var team = CreateValidTeam();

        Assert.ThrowsException<ArgumentException>(() =>
            team.RevertLoss(goalsScored: -1, goalsConceded: 0));
    }

    [TestMethod]
    public void RevertLoss_WithNegativeConcededGoals_ShouldThrowArgumentException()
    {
        var team = CreateValidTeam();

        Assert.ThrowsException<ArgumentException>(() =>
            team.RevertLoss(goalsScored: 0, goalsConceded: -1));
    }

    [TestMethod]
    public void RevertDraw_WithNegativeGoals_ShouldThrowArgumentException()
    {
        var team = CreateValidTeam();

        Assert.ThrowsException<ArgumentException>(() =>
            team.RevertDraw(goalsScored: -1));
    }

    [TestMethod]
    public void ApplyRankingChange_WithNegativeDelta_ShouldDecreaseRankingRounded()
    {
        var team = new Team(ValidName, ValidConfederation, 1200);

        team.ApplyRankingChange(-20.6);

        Assert.AreEqual(1179, team.RankingFifa);
    }

    [TestMethod]
    public void ApplyRankingChange_WithZeroDelta_ShouldNotChangeRanking()
    {
        var team = new Team(ValidName, ValidConfederation, 1500);

        team.ApplyRankingChange(0);

        Assert.AreEqual(1500, team.RankingFifa);
    }

    [TestMethod]
    public void CreateTeam_WithMaxLengthName_ShouldSucceed()
    {
        var name = new string('A', 60);

        var team = new Team(name, ValidConfederation, ValidRanking);

        Assert.AreEqual(name, team.Name);
    }

    [TestMethod]
    public void CreateTeam_WithNameExceedingMaxLength_ShouldThrowArgumentException()
    {
        var name = new string('A', 61);

        Assert.ThrowsException<ArgumentException>(() =>
            new Team(name, ValidConfederation, ValidRanking));
    }

    [TestMethod]
    public void UpdateGroupLabel_WithLowercase_ShouldNormalizeToUppercase()
    {
        var team = CreateValidTeam();

        team.UpdateGroupLabel("group a");

        Assert.AreEqual("GROUP A", team.GroupLabel);
    }

    [TestMethod]
    public void CreateTeam_WithGroupLabel_ShouldStoreNormalizedLabel()
    {
        var team = new Team(ValidName, ValidConfederation, ValidRanking, "group b");

        Assert.AreEqual("GROUP B", team.GroupLabel);
    }

    [TestMethod]
    public void Normalize_WithNull_ShouldReturnEmpty()
    {
        var result = Team.Normalize(null!);

        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void UpdateRankingFifa_ShouldUpdateValue()
    {
        var team = CreateValidTeam();

        team.UpdateRankingFifa(800);

        Assert.AreEqual(800, team.RankingFifa);
    }

    [TestMethod]
    public void CreateTeam_WithValidFlag_ShouldStoreFlag()
    {
        var validBase64 = Convert.ToBase64String(new byte[] { 1, 2, 3, 4 });

        var team = new Team(ValidName, ValidConfederation, ValidRanking, flagBase64: validBase64);

        Assert.AreEqual(validBase64, team.FlagBase64);
    }

    [TestMethod]
    public void UpdateFlag_WithNullValue_ShouldResetToEmpty()
    {
        var team = CreateValidTeam();
        team.UpdateFlag(Convert.ToBase64String(new byte[] { 9, 9 }));

        team.UpdateFlag(null!);

        Assert.AreEqual(string.Empty, team.FlagBase64);
    }

    [TestMethod]
    public void RecordDraw_WithZeroGoals_ShouldSucceed()
    {
        var team = CreateValidTeam();

        team.RecordDraw(0);

        Assert.AreEqual(1, team.Points);
        Assert.AreEqual(0, team.GoalsInFavor);
        Assert.AreEqual(0, team.GoalDifference);
    }

    [TestMethod]
    public void UpdateConfederationName_WithInvalidValue_ShouldThrowArgumentException()
    {
        var team = CreateValidTeam();

        Assert.ThrowsException<ArgumentException>(() =>
            team.UpdateConfederationName("INVALID"));
    }
    
    private static Team CreateValidTeam() =>
        new Team(ValidName, ValidConfederation, ValidRanking);
}
