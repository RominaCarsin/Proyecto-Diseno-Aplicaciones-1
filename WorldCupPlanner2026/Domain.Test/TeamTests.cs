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
}