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
}