namespace Domain.Test;

[TestClass]
public class TeamTests
{
    [TestMethod]
    public void CreateTeam_WithValidData_ShouldSucceed()
    {
        var team = new Team("Uruguay", "CONMEBOL", 500);
        Assert.IsNotNull(team);
        Assert.AreEqual("Uruguay", team.Name);
        Assert.AreEqual("CONMEBOL", team.ConfederationName);
        Assert.AreEqual(500, team.RankingFifa);
    }

    [TestMethod]
    public void CreateTeam_WithInvalidName_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Team("", "CONMEBOL", 500));
    }

    [TestMethod]
    public void CreateTeam_WithNameAbove60Characters_ShouldThrowException()
    {
        var longName = new string('A', 61);
        Assert.ThrowsException<ArgumentException>(() => new Team(longName, "CONMEBOL", 500));
    }

    [TestMethod]
    public void CreateTeam_WithNameExactly60Characters_ShouldSucceed()
    {
        var maxName = new string('A', 60);
        var team = new Team(maxName, "CONMEBOL", 500);
        Assert.AreEqual(maxName, team.Name);
    }

    [TestMethod]
    public void CreateTeam_WithNullConfederation_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Team("Uruguay", null, 500));
    }

    [TestMethod]
    public void CreateTeam_WithInvalidConfederation_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Team("Uruguay", "INVALID", 500));
    }

    [TestMethod]
    public void CreateTeam_WithLowercaseConfederation_ShouldNormalizeToUppercase()
    {
        var team = new Team("Uruguay", "conmebol", 500);
        Assert.AreEqual("CONMEBOL", team.ConfederationName);
    }

    [TestMethod]
    public void CreateTeam_WithRankingFifaBelow300_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Team("Uruguay", "CONMEBOL", 299));
    }

    [TestMethod]
    public void CreateTeam_WithRankingFifaAbove2500_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Team("Uruguay", "CONMEBOL", 2501));
    }

    [TestMethod]
    public void CreateTeam_WithRankingFifaAt300_ShouldSucceed()
    {
        var team = new Team("Uruguay", "CONMEBOL", 300);
        Assert.AreEqual(300, team.RankingFifa);
    }

    [TestMethod]
    public void CreateTeam_WithRankingFifaAt2500_ShouldSucceed()
    {
        var team = new Team("Uruguay", "CONMEBOL", 2500);
        Assert.AreEqual(2500, team.RankingFifa);
    }

    [TestMethod]
    public void CreateTeam_ShouldInitializePointsAndGoalsToZero()
    {
        var team = new Team("Uruguay", "CONMEBOL", 500);
        Assert.AreEqual(0, team.Points);
        Assert.AreEqual(0, team.GoalDifference);
        Assert.AreEqual(0, team.GoalsInFavor);
    }
    

    [TestMethod]
    public void UpdateName_WithValidData_ShouldSucceed()
    {
        var team = new Team("Spain", "UEFA", 500);
        team.UpdateName("France");
        Assert.AreEqual("France", team.Name);
    }

    [TestMethod]
    public void UpdateName_WithEmptyName_ShouldThrowArgumentException()
    {
        var team = new Team("Spain", "UEFA", 500);
        Assert.ThrowsException<ArgumentException>(() => team.UpdateName(""));
    }

    [TestMethod]
    public void UpdateName_WithWhitespaceName_ShouldThrowArgumentException()
    {
        var team = new Team("Spain", "UEFA", 500);
        Assert.ThrowsException<ArgumentException>(() => team.UpdateName("   "));
    }

    [TestMethod]
    public void UpdateName_WithNameExceeding60Chars_ShouldThrowArgumentException()
    {
        var team = new Team("Spain", "UEFA", 500);
        var longName = new string('A', 61);
        Assert.ThrowsException<ArgumentException>(() => team.UpdateName(longName));
    }

    [TestMethod]
    public void UpdateName_WithNameExactly60Chars_ShouldSucceed()
    {
        var team = new Team("Spain", "UEFA", 500);
        var maxName = new string('A', 60);
        team.UpdateName(maxName);
        Assert.AreEqual(maxName, team.Name);
    }
    

    [TestMethod]
    public void UpdateRankingFifa_WithValidData_ShouldSucceed()
    {
        var team = new Team("Spain", "UEFA", 500);
        team.UpdateRankingFifa(1200);
        Assert.AreEqual(1200, team.RankingFifa);
    }

    [TestMethod]
    public void UpdateRankingFifa_BelowMinimum_ShouldThrowArgumentException()
    {
        var team = new Team("Spain", "UEFA", 500);
        Assert.ThrowsException<ArgumentException>(() => team.UpdateRankingFifa(299));
    }

    [TestMethod]
    public void UpdateRankingFifa_AboveMaximum_ShouldThrowArgumentException()
    {
        var team = new Team("Spain", "UEFA", 500);
        Assert.ThrowsException<ArgumentException>(() => team.UpdateRankingFifa(2501));
    }

    [TestMethod]
    public void UpdateRankingFifa_AtMinimumBoundary_ShouldSucceed()
    {
        var team = new Team("Spain", "UEFA", 500);
        team.UpdateRankingFifa(300);
        Assert.AreEqual(300, team.RankingFifa);
    }

    [TestMethod]
    public void UpdateRankingFifa_AtMaximumBoundary_ShouldSucceed()
    {
        var team = new Team("Spain", "UEFA", 500);
        team.UpdateRankingFifa(2500);
        Assert.AreEqual(2500, team.RankingFifa);
    }

    [TestMethod]
    public void UpdateConfederationName_WithValidData_ShouldSucceed()
    {
        var team = new Team("Spain", "UEFA", 500);
        team.UpdateConfederationName("CONMEBOL");
        Assert.AreEqual("CONMEBOL", team.ConfederationName);
    }

    [TestMethod]
    public void UpdateConfederationName_WithLowercase_ShouldNormalizeToUppercase()
    {
        var team = new Team("Spain", "UEFA", 500);
        team.UpdateConfederationName("conmebol");
        Assert.AreEqual("CONMEBOL", team.ConfederationName);
    }
}