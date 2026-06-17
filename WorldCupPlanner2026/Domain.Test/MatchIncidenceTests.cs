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

    [TestMethod]
    public void CreateYellowCard_WithMinuteAboveMax_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new YellowCard(BuildTeam("URUGUAY"), minute: 131));
    }

    [TestMethod]
    public void CreateRedCard_WithMinuteAboveMax_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new RedCard(BuildTeam("BRAZIL"), minute: 131));
    }

    [TestMethod]
    public void CreateYellowCard_WithMinuteAtMaxBoundary_ShouldSetMinute()
    {
        var incidence = new YellowCard(BuildTeam("URUGUAY"), minute: 130);

        Assert.AreEqual(130, incidence.Minute);
    }

    [TestMethod]
    public void CreateRedCard_WithMinuteAtMinBoundary_ShouldSetMinute()
    {
        var incidence = new RedCard(BuildTeam("BRAZIL"), minute: 0);

        Assert.AreEqual(0, incidence.Minute);
    }

    [TestMethod]
    public void CreateYellowCard_IncidenceId_ShouldDefaultToZero()
    {
        var incidence = new YellowCard(BuildTeam("URUGUAY"), minute: 10);

        Assert.AreEqual(0, incidence.IncidenceId);
    }

    [TestMethod]
    public void CreateRedCard_IncidenceId_ShouldDefaultToZero()
    {
        var incidence = new RedCard(BuildTeam("BRAZIL"), minute: 20);

        Assert.AreEqual(0, incidence.IncidenceId);
    }
}