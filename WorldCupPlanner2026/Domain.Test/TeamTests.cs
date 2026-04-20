namespace Domain.Test;

[TestClass]
public class TeamTests
{
    private static Confederation _conmebol = new Confederation("CONMEBOL", new List<Team>(), 6);
    
    [TestMethod]
    public void CreateTeam_WithValidData_ShouldSucceed()
    {
        var team = new Team("Uruguay", -_conmebol, 8, 0, 0, 0);
        Assert.IsNotNull(team);
        Assert.AreEqual("Uruguay", team.Name);
        Assert.AreEqual(_conmebol, team.Confederation);
        Assert.AreEqual(8, team.RankingFifa);
        Assert.AreEqual(0, team.Points);
        Assert.AreEqual(0, team.GoalDiference);
        Assert.AreEqual(0, team.GoalsInFavor);
    }

    [TestMethod]
    public void CreateTeam_WithInvalidName_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Team("", _conmebol, 8, 0, 0, 0));
    }

    [TestMethod]
    public void CreateTeam_WithNullConfederation_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Team("Uruguay", null, 8, 0, 0, 0));
    }

    [TestMethod]
    public void CreateTeam_WithInvalidRankingFifa_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Team("Uruguay", _conmebol, -1, 0, 0, 0));
    }
}