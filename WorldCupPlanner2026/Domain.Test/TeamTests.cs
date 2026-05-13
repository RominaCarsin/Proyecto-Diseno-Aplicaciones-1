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

        Assert.AreEqual("URUGUAY", team.Name);
        Assert.AreEqual("CONMEBOL", team.ConfederationName);
        Assert.AreEqual(ValidRanking, team.RankingFifa);
        Assert.AreEqual(0, team.Points);
        Assert.AreEqual(0, team.GoalDifference);
        Assert.AreEqual(0, team.GoalsInFavor);
    }

    [TestMethod]
    public void CreateTeam_WithInvalidData_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Team("", ValidConf, ValidRanking));
        Assert.ThrowsException<ArgumentException>(() => new Team("   ", ValidConf, ValidRanking));
        Assert.ThrowsException<ArgumentException>(() => new Team(ValidName, "INVALID", ValidRanking));
        Assert.ThrowsException<ArgumentException>(() => new Team(ValidName, null!, ValidRanking));
        Assert.ThrowsException<ArgumentException>(() => new Team(ValidName, ValidConf, 299));
        Assert.ThrowsException<ArgumentException>(() => new Team(ValidName, ValidConf, 2501));
    }

    [TestMethod]
    public void UpdateNameAndConfederation_ShouldNormalizeToUppercase()
    {
        var team = new Team("Spain", "UEFA", ValidRanking);

        team.UpdateName("france");
        team.UpdateConfederationName("afc");

        Assert.AreEqual("FRANCE", team.Name);
        Assert.AreEqual("AFC", team.ConfederationName);
    }

    [TestMethod]
    public void CreateTeam_WithRankingFifaAtBoundaries_ShouldSucceed()
    {
        Assert.AreEqual(300, new Team(ValidName, ValidConf, 300).RankingFifa);
        Assert.AreEqual(2500, new Team(ValidName, ValidConf, 2500).RankingFifa);
    }

    [TestMethod]
    public void RecordResults_ShouldUpdateStats()
    {
        var winTeam = new Team(ValidName, ValidConf, ValidRanking);
        winTeam.RecordWin(3, 1);
        Assert.AreEqual(3, winTeam.Points);
        Assert.AreEqual(3, winTeam.GoalsInFavor);
        Assert.AreEqual(2, winTeam.GoalDifference);

        var drawTeam = new Team(ValidName, ValidConf, ValidRanking);
        drawTeam.RecordDraw(2);
        Assert.AreEqual(1, drawTeam.Points);
        Assert.AreEqual(2, drawTeam.GoalsInFavor);
        Assert.AreEqual(0, drawTeam.GoalDifference);

        var lossTeam = new Team(ValidName, ValidConf, ValidRanking);
        lossTeam.RecordLoss(0, 4);
        Assert.AreEqual(0, lossTeam.Points);
        Assert.AreEqual(0, lossTeam.GoalsInFavor);
        Assert.AreEqual(-4, lossTeam.GoalDifference);
    }

    [TestMethod]
    public void RevertResults_ShouldRemovePreviousStats()
    {
        var team = new Team(ValidName, ValidConf, ValidRanking);

        team.RecordWin(3, 1);
        team.RevertWin(3, 1);
        Assert.AreEqual(0, team.Points);
        Assert.AreEqual(0, team.GoalsInFavor);
        Assert.AreEqual(0, team.GoalDifference);

        team.RecordDraw(2);
        team.RevertDraw(2);
        Assert.AreEqual(0, team.Points);
        Assert.AreEqual(0, team.GoalsInFavor);
        Assert.AreEqual(0, team.GoalDifference);

        team.RecordLoss(1, 3);
        team.RevertLoss(1, 3);
        Assert.AreEqual(0, team.Points);
        Assert.AreEqual(0, team.GoalsInFavor);
        Assert.AreEqual(0, team.GoalDifference);
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
        Assert.ThrowsException<ArgumentException>(() => team.RevertWin(-1, 0));
    }
}
