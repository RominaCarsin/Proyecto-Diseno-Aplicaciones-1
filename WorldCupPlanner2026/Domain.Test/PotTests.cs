namespace Domain.Test;

[TestClass]
public class PotTests
{
    private List<Team> TwelveValidTeams() => Enumerable.Range(1, 12)
        .Select(i => new Team($"Team{i}", "UEFA", 500 + i))
        .ToList();
    

    [TestMethod]
    public void CreatePot_WithValidNumberAndTwelveTeams_ShouldCreate()
    {
        var pot = new Pot(1, TwelveValidTeams());

        Assert.AreEqual(1, pot.Number);
        Assert.AreEqual(12, pot.Teams.Count);
    }

    [TestMethod]
    public void CreatePot_NumberOne_ShouldBeValid()
    {
        var pot = new Pot(1, TwelveValidTeams());
        Assert.AreEqual(1, pot.Number);
    }

    [TestMethod]
    public void CreatePot_NumberFour_ShouldBeValid()
    {
        var pot = new Pot(4, TwelveValidTeams());
        Assert.AreEqual(4, pot.Number);
    }

    [TestMethod]
    public void CreatePot_WithNumberZero_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Pot(0, TwelveValidTeams()));
    }

    [TestMethod]
    public void CreatePot_WithNumberFive_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Pot(5, TwelveValidTeams()));
    }

    [TestMethod]
    public void CreatePot_WithMoreThanTwelveTeams_ShouldThrowArgumentException()
    {
        var teams = Enumerable.Range(1, 13)
            .Select(i => new Team($"Team{i}", "UEFA", 500 + i))
            .ToList();

        Assert.ThrowsException<ArgumentException>(() => new Pot(1, teams));
    }

    [TestMethod]
    public void CreatePot_WithLessThanTwelveTeams_ShouldThrowArgumentException()
    {
        var teams = Enumerable.Range(1, 11)
            .Select(i => new Team($"Team{i}", "UEFA", 500 + i))
            .ToList();

        Assert.ThrowsException<ArgumentException>(() => new Pot(1, teams));
    }

    [TestMethod]
    public void CreatePot_WithEmptyTeamList_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Pot(1, new List<Team>()));
    }

    [TestMethod]
    public void CreatePot_WithNullTeamList_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Pot(1, null));
    }
    
}