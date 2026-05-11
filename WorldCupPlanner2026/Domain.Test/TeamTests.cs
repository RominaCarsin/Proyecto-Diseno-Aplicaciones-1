namespace Domain.Test;

[TestClass]
public class TeamTests
{
    
    private const string ValidName = "URUGUAY";
    private const string ValidConf = "CONMEBOL";
    private const int ValidRanking = 500;

    [TestMethod]
    public void CreateTeam_WithValidData_ShouldSucceedAndNormalize()
    {
        
        var team = new Team("uruguay", "conmebol", ValidRanking);
        
        Assert.IsNotNull(team);
        Assert.AreEqual("URUGUAY", team.Name);
        Assert.AreEqual("CONMEBOL", team.ConfederationName);
        Assert.AreEqual(ValidRanking, team.RankingFifa);
    }

    [TestMethod]
    public void CreateTeam_WithSpacesInNameAndConf_ShouldTrimAndUppercase()
    {
        var team = new Team("  argentina  ", "  uefa  ", ValidRanking);
        
        Assert.AreEqual("ARGENTINA", team.Name);
        Assert.AreEqual("UEFA", team.ConfederationName);
    }

    [TestMethod]
    public void CreateTeam_WithInvalidName_ShouldThrowException()
    {
        
        Assert.ThrowsException<ArgumentException>(() => new Team("", ValidConf, ValidRanking));
        Assert.ThrowsException<ArgumentException>(() => new Team("   ", ValidConf, ValidRanking));
    }

    [TestMethod]
    public void CreateTeam_WithNameExactly60Characters_ShouldSucceedInUpper()
    {
        var maxName = new string('a', 60);
        var team = new Team(maxName, ValidConf, ValidRanking);
        
        Assert.AreEqual(maxName.ToUpper(), team.Name);
    }

    [TestMethod]
    public void CreateTeam_WithInvalidConfederation_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Team(ValidName, "INVALID", ValidRanking));
        Assert.ThrowsException<ArgumentException>(() => new Team(ValidName, null, ValidRanking));
    }

    [TestMethod]
    public void UpdateName_ShouldNormalizeToUppercase()
    {
        var team = new Team("Spain", "UEFA", ValidRanking);
        
        team.UpdateName("france");
        
        Assert.AreEqual("FRANCE", team.Name);
    }

    [TestMethod]
    public void UpdateConfederationName_ShouldNormalizeToUppercase()
    {
        var team = new Team("Spain", "UEFA", ValidRanking);
        
        team.UpdateConfederationName("afc");
        
        Assert.AreEqual("AFC", team.ConfederationName);
    }

    [TestMethod]
    public void CreateTeam_WithRankingFifaAtBoundaries_ShouldSucceed()
    {
        var teamMin = new Team(ValidName, ValidConf, 300);
        var teamMax = new Team(ValidName, ValidConf, 2500);

        Assert.AreEqual(300, teamMin.RankingFifa);
        Assert.AreEqual(2500, teamMax.RankingFifa);
    }

    [TestMethod]
    public void CreateTeam_WithRankingFifaOutOfRange_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Team(ValidName, ValidConf, 299));
        Assert.ThrowsException<ArgumentException>(() => new Team(ValidName, ValidConf, 2501));
    }

    [TestMethod]
    public void CreateTeam_ShouldInitializePointsAndGoalsToZero()
    {
        var team = new Team(ValidName, ValidConf, ValidRanking);
        Assert.AreEqual(0, team.Points);
        Assert.AreEqual(0, team.GoalDifference);
        Assert.AreEqual(0, team.GoalsInFavor);
    }

    [TestMethod]
    public void Win_ShouldIncreasePointsBy3_AndUpdateGoals()
    {
        var team = new Team(ValidName, ValidConf, ValidRanking);

        team.RecordWin(3, 1);

        Assert.AreEqual(3, team.Points);
        Assert.AreEqual(3, team.GoalsInFavor);
        Assert.AreEqual(2, team.GoalDifference);
    }

    [TestMethod]
    public void Draw_ShouldIncreasePointsBy1_AndUpdateGoals()
    {
        var team = new Team(ValidName, ValidConf, ValidRanking);

        team.RecordDraw(2);

        Assert.AreEqual(1, team.Points);
        Assert.AreEqual(2, team.GoalsInFavor);
        Assert.AreEqual(0, team.GoalDifference);
    }

    [TestMethod]
    public void Loss_ShouldNotIncreasePoints_AndUpdateGoals()
    {
        var team = new Team(ValidName, ValidConf, ValidRanking);

        team.RecordLoss(0, 4);

        Assert.AreEqual(0, team.Points);
        Assert.AreEqual(0, team.GoalsInFavor);
        Assert.AreEqual(-4, team.GoalDifference);
    }

    [TestMethod]
    public void MatchResults_ShouldThrowException_WhenGoalsAreNegative()
    {
        var team = new Team(ValidName, ValidConf, ValidRanking);

        Assert.ThrowsException<ArgumentException>(() => team.RecordWin(-1, 0));
        Assert.ThrowsException<ArgumentException>(() => team.RecordWin(1, -1));
        Assert.ThrowsException<ArgumentException>(() => team.RecordDraw(-1));
        Assert.ThrowsException<ArgumentException>(() => team.RecordLoss(-1, 0));
        Assert.ThrowsException<ArgumentException>(() => team.RecordLoss(0, -1));
    }
}
