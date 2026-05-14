namespace Domain.Test;

[TestClass]
public class PotTests
{

    [TestMethod]
    public void CreatePot_WithValidData_ShouldCreatePotWithExpectedProperties()
    {
        var pot = new Pot(1, BuildTeams(12));

        Assert.AreEqual(1, pot.Number);
        Assert.AreEqual(12, pot.Teams.Count);
    }

    [TestMethod]
    public void CreatePot_WithMinValidNumber_ShouldSucceed()
    {
        var pot = new Pot(Pot.MinNumber, BuildTeams(12));

        Assert.AreEqual(Pot.MinNumber, pot.Number);
    }

    [TestMethod]
    public void CreatePot_WithMaxValidNumber_ShouldSucceed()
    {
        var pot = new Pot(Pot.MaxNumber, BuildTeams(12));

        Assert.AreEqual(Pot.MaxNumber, pot.Number);
    }

    [TestMethod]
    public void CreatePot_WithNumberBelowMinimum_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Pot(0, BuildTeams(12)));
    }

    [TestMethod]
    public void CreatePot_WithNegativeNumber_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Pot(-1, BuildTeams(12)));
    }

    [TestMethod]
    public void CreatePot_WithNumberAboveMaximum_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Pot(5, BuildTeams(12)));
    }

    [TestMethod]
    public void CreatePot_WithMoreThanMaxTeams_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Pot(1, BuildTeams(13)));
    }

    [TestMethod]
    public void CreatePot_WithLessThanMaxTeams_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Pot(1, BuildTeams(11)));
    }

    [TestMethod]
    public void CreatePot_WithEmptyTeamsList_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Pot(1, new List<Team>()));
    }

    [TestMethod]
    public void CreatePot_WithNullTeamsList_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Pot(1, null!));
    }

    [TestMethod]
    public void IsFull_WithExactlyMaxTeams_ShouldReturnTrue()
    {
        var pot = new Pot(1, BuildTeams(12));

        Assert.IsTrue(pot.IsFull());
    }
    
    private static List<Team> BuildTeams(int count, int rankingStart = 500)
    {
        var teams = new List<Team>();
        for (int i = 1; i <= count; i++)
            teams.Add(new Team($"TEAM_{i}", "UEFA", rankingStart + i));
        return teams;
    }
}