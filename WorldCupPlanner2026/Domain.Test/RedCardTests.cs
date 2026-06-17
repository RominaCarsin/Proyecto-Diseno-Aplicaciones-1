namespace Domain.Test;

[TestClass]
public class RedCardTests
{
    private static Team CreateValidTeam() => new("Uruguay", "CONMEBOL", 1500);

    [TestMethod]
    public void CreateRedCard_WithValidTeamAndMinute_ShouldSetProperties()
    {
        var team = CreateValidTeam();

        var card = new RedCard(team, minute: 75);

        Assert.AreEqual(team, card.Team);
        Assert.AreEqual(75, card.Minute);
    }

    [TestMethod]
    public void CreateRedCard_WithNullTeam_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new RedCard(null, minute: 30));
    }

    [TestMethod]
    public void CreateRedCard_WithMinuteBelowZero_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new RedCard(CreateValidTeam(), minute: -1));
    }

    [TestMethod]
    public void CreateRedCard_WithMinuteAboveMax_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new RedCard(CreateValidTeam(), minute: 131));
    }

    [TestMethod]
    public void CreateRedCard_WithMinuteAtMinBoundary_ShouldSetMinute()
    {
        var card = new RedCard(CreateValidTeam(), minute: 0);

        Assert.AreEqual(0, card.Minute);
    }

    [TestMethod]
    public void CreateRedCard_WithMinuteAtMaxBoundary_ShouldSetMinute()
    {
        var card = new RedCard(CreateValidTeam(), minute: 130);

        Assert.AreEqual(130, card.Minute);
    }
}
