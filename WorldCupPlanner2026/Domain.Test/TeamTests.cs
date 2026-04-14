namespace Domain.Test;

[TestClass]
public class TeamTests
{
    [TestMethod]
    public void CreateTeam_WithValidData_ShouldSucceed()
    {
        var team = new Team("Uruguay", "CONMEBOL", 8, 0, 0, 0);
        Assert.IsNotNull(team);
        Assert.AreEqual("Uruguay", team.Name);
        Assert.AreEqual("CONMEBOL", team.Confederation);
        Assert.AreEqual(8, team.RankingFifa);
        Assert.AreEqual(0, team.Points);
        Assert.AreEqual(0, team.GoalDiference);
        Assert.AreEqual(0, team.GoalsInFavor);
    }

    [TestMethod]
    public void CreateTeam_WithInvalidName_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Team("", "CONMEBOL", 8, 0, 0, 0));
    }

    [TestMethod]
    public void CreateTeam_WithInvalidConfederation_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Team("Uruguay", "", 8, 0, 0, 0));
    }
}