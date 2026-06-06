namespace Domain.Test;

[TestClass]
public class MatchIncidenceTests
{
    private static Team BuildTeam(string name) =>
        new(name, "CONMEBOL", 1800);

    [TestMethod]
    public void CreateYellowCard_ShouldExposeTeamAndMinute()
    {
        var team = BuildTeam("URUGUAY");

        var incidence = new YellowCard(team, minute: 23);

        Assert.AreEqual(team, incidence.Team);
        Assert.AreEqual(23, incidence.Minute);
    }
    
    [TestMethod]
    public void CreateYellowCard_WithNullTeam_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new YellowCard(null, minute: 10));
    }
    
    [TestMethod]
    public void CreateYellowCard_WithNegativeMinute_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new YellowCard(BuildTeam("URUGUAY"), minute: -1));
    }
    
    [TestMethod]
    public void CreateRedCard_ShouldExposeTeamAndMinute()
    {
        var team = BuildTeam("BRAZIL");

        var incidence = new RedCard(team, minute: 67);

        Assert.AreEqual(team, incidence.Team);
        Assert.AreEqual(67, incidence.Minute);
    }
}