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
        Assert.ThrowsException<ArgumentException>(() => new Team("", _conmebol, 500, 0, 0, 0));
    }
    
    [TestMethod]
    public void CreateTeam_WithNameAbove60Characters_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Team("(..)Comouruguayoquieroserpartedeti.Recorrertodastusplayas(..)", _conmebol, 500, 0, 0, 0));
    }
    
    [TestMethod]
    public void CreateTeam_WithName60Characters_ShouldSucceed()
    {
        var team = new Team("(..)ComouruguayoquieroserpartedetiRecorrertodastusplayas(..)", _conmebol, 500, 0, 0, 0);
        Assert.AreEqual("(..)ComouruguayoquieroserpartedetiRecorrertodastusplayas(..)", team.Name);
    }

    [TestMethod]
    public void CreateTeam_WithNullConfederation_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Team("Uruguay", null, 500, 0, 0, 0));
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
        var team = new Team("Uruguay", _conmebol, 300, 0, 0, 0);
        Assert.AreEqual(300, team.RankingFifa);
    }
    
    [TestMethod]
    public void CreateTeam_WithRankingFifaAt2500_ShouldSucced()
    {
        var team = new Team("Uruguay", _conmebol, 2500, 0, 0, 0);
        Assert.AreEqual(2500, team.RankingFifa);
    }

    [TestMethod]
    public void CreateTeam_WithAlreadyExistingName_ShouldThrowException()
    {
        var team  = new Team("Uruguay", _conmebol, 500, 0, 0, 0);
        Assert.ThrowsException<ArgumentException>(() => new Team("Uruguay", _conmebol, 600, 0, 0, 0));
    }
}