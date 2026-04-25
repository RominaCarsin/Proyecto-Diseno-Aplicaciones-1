// TeamTests.cs
namespace Domain.Test;

[TestClass]
public class TeamTests
{
    private static Confederation _conmebol = new Confederation("CONMEBOL", 6);

    [TestMethod]
    public void CreateTeam_WithValidData_ShouldSucceed()
    {
        var team = new Team("Uruguay", _conmebol, 500, 0, 0, 0);
        Assert.IsNotNull(team);
        Assert.AreEqual("Uruguay", team.Name);
        Assert.AreEqual(_conmebol, team.Confederation);
        Assert.AreEqual(500, team.RankingFifa);
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
    public void CreateTeam_WithRankingFifaBelow300_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Team("Uruguay", _conmebol, 299, 0, 0, 0));
    }
    
    [TestMethod]
    public void CreateTeam_WithRankingFifaAbove2500_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Team("Uruguay", _conmebol, 2501, 0, 0, 0));
    }
    
    [TestMethod]
    public void CreateTeam_WithRankingFifaAt300_ShouldSucceed()
    {
        Assert.ThrowsException<ArgumentException>(() => new Team("Uruguay", _conmebol, 300, 0, 0, 0));
    }
    
    [TestMethod]
    public void CreateTeam_WithRankingFifaAt2500_ShouldSucced()
    {
        Assert.ThrowsException<ArgumentException>(() => new Team("Uruguay", _conmebol, 2500, 0, 0, 0));
    }
}