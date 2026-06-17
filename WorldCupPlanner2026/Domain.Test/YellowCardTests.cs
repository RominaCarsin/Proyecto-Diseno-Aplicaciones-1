namespace Domain.Test;

[TestClass]
public class YellowCardTests
{
    private static Team CreateValidTeam() => new("Uruguay", "CONMEBOL", 1500);

    [TestMethod]
    public void CreateYellowCard_WithValidTeamAndMinute_ShouldSetProperties()
    {
        var team = CreateValidTeam();

        var card = new YellowCard(team, minute: 45);

        Assert.AreEqual(team, card.Team);
        Assert.AreEqual(45, card.Minute);
    }

    [TestMethod]
    public void CreateYellowCard_WithNullTeam_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new YellowCard(null, minute: 30));
    }

    [TestMethod]
    public void CreateYellowCard_WithMinuteBelowZero_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new YellowCard(CreateValidTeam(), minute: -1));
    }

    [TestMethod]
    public void CreateYellowCard_WithMinuteAboveMax_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new YellowCard(CreateValidTeam(), minute: 131));
    }

    [TestMethod]
    public void CreateYellowCard_WithMinuteAtMinBoundary_ShouldSetMinute()
    {
        var card = new YellowCard(CreateValidTeam(), minute: 0);

        Assert.AreEqual(0, card.Minute);
    }

    [TestMethod]
    public void CreateYellowCard_WithMinuteAtMaxBoundary_ShouldSetMinute()
    {
        var card = new YellowCard(CreateValidTeam(), minute: 130);

        Assert.AreEqual(130, card.Minute);
    }
}
