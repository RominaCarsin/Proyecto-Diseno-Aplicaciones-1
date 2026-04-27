namespace Domain.Test;

[TestClass]
public class TeamTests
{
    private static Confederation _conmebol = new Confederation("CONMEBOL", 6);

    [TestMethod]
    public void CreateTeam_WithValidData_ShouldSucceed()
    {
        var team = new Team("Uruguay", _conmebol.Name, 500, 0, 0, 0);
        Assert.IsNotNull(team);
        Assert.AreEqual("Uruguay", team.Name);
        Assert.AreEqual(_conmebol.Name, team.ConfederationName);
        Assert.AreEqual(500, team.RankingFifa);
    }

    [TestMethod]
    public void CreateTeam_WithInvalidName_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Team("", _conmebol.Name, 500, 0, 0, 0));
    }
    
    [TestMethod]
    public void CreateTeam_WithNameAbove60Characters_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Team("(..)Comouruguayoquieroserpartedeti.Recorrertodastusplayas(..)", _conmebol.Name, 500, 0, 0, 0));
    }
    
    [TestMethod]
    public void CreateTeam_WithName60Characters_ShouldSucceed()
    {
        var team = new Team("(..)ComouruguayoquieroserpartedetiRecorrertodastusplayas(..)", _conmebol.Name, 500, 0, 0, 0);
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
        Assert.ThrowsException<ArgumentException>(() => new Team("Uruguay", _conmebol.Name, 299, 0, 0, 0));
    }
    
    [TestMethod]
    public void CreateTeam_WithRankingFifaAbove2500_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Team("Uruguay", _conmebol.Name, 2501, 0, 0, 0));
    }
    
    [TestMethod]
    public void CreateTeam_WithRankingFifaAt300_ShouldSucceed()
    {
        var team = new Team("Uruguay", _conmebol.Name, 300, 0, 0, 0);
        Assert.AreEqual(300, team.RankingFifa);
    }
    
    [TestMethod]
    public void CreateTeam_WithRankingFifaAt2500_ShouldSucced()
    {
        var team = new Team("Uruguay", _conmebol.Name, 2500, 0, 0, 0);
        Assert.AreEqual(2500, team.RankingFifa);
    }

    [TestMethod]
    public void UpdateName_WithValidData_ShouldSucceed()
    {
        var team = new Team("Spain", "UEFA", 500, 0, 0, 0);

        team.UpdateName("France");

        Assert.AreEqual("France", team.Name);
    }

    [TestMethod]
    public void UpdateName_WithEmptyName_ShouldThrowArgumentException()
    {
        var team = new Team("Spain", "UEFA", 500, 0, 0, 0);

        Assert.ThrowsException<ArgumentException>(() => team.UpdateName(""));
    }

    [TestMethod]
    public void UpdateName_WithWhitespaceName_ShouldThrowArgumentException()
    {
        var team = new Team("Spain", "UEFA", 500, 0, 0, 0);

        Assert.ThrowsException<ArgumentException>(() => team.UpdateName("   "));
    }

    [TestMethod]
    public void UpdateName_WithNameExceeding60Chars_ShouldThrowArgumentException()
    {
        var team = new Team("Spain", "UEFA", 500, 0, 0, 0);
        var longName = new string('A', 61);

        Assert.ThrowsException<ArgumentException>(() => team.UpdateName(longName));
    }

    [TestMethod]
    public void UpdateName_WithNameExactly60Chars_ShouldSucceed()
    {
        var team = new Team("Spain", "UEFA", 500, 0, 0, 0);
        var maxName = new string('A', 60);

        team.UpdateName(maxName);

        Assert.AreEqual(maxName, team.Name);
    }

    [TestMethod]
    public void UpdateRankingFifa_WithValidData_ShouldSucceed()
    {
        var team = new Team("Spain", "UEFA", 500, 0, 0, 0);

        team.UpdateRankingFifa(1200);

        Assert.AreEqual(1200, team.RankingFifa);
    }

    [TestMethod]
    public void UpdateRankingFifa_BelowMinimum_ShouldThrowArgumentException()
    {
        var team = new Team("Spain", "UEFA", 500, 0, 0, 0);

        Assert.ThrowsException<ArgumentException>(() => team.UpdateRankingFifa(299));
    }

    [TestMethod]
    public void UpdateRankingFifa_AboveMaximum_ShouldThrowArgumentException()
    {
        var team = new Team("Spain", "UEFA", 500, 0, 0, 0);

        Assert.ThrowsException<ArgumentException>(() => team.UpdateRankingFifa(2501));
    }

    [TestMethod]
    public void UpdateRankingFifa_AtMinimumBoundary_ShouldSucceed()
    {
        var team = new Team("Spain", "UEFA", 500, 0, 0, 0);

        team.UpdateRankingFifa(300);

        Assert.AreEqual(300, team.RankingFifa);
    }

    [TestMethod]
    public void UpdateRankingFifa_AtMaximumBoundary_ShouldSucceed()
    {
        var team = new Team("Spain", "UEFA", 500, 0, 0, 0);

        team.UpdateRankingFifa(2500);

        Assert.AreEqual(2500, team.RankingFifa);
    }
   
}