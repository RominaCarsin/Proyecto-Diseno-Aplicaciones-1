namespace Domain.Test;

[TestClass]
public class PotTests
{
    // ─── Helper sin LINQ ──────────────────────────────────────────────────────

    private List<Team> BuildTeams(int count, int rankingStart = 500)
    {
        var teams = new List<Team>();
        for (int i = 1; i <= count; i++)
            teams.Add(new Team($"Team{i}", "UEFA", rankingStart + i));
        return teams;
    }

    // ─── Constructor válido ───────────────────────────────────────────────────

    [TestMethod]
    public void CreatePot_WithValidNumberAndTwelveTeams_ShouldCreate()
    {
        var pot = new Pot(1, BuildTeams(12));

        Assert.AreEqual(1, pot.Number);
        Assert.AreEqual(12, pot.Teams.Count);
    }

    [TestMethod]
    public void CreatePot_NumberOne_ShouldBeValid()
    {
        var pot = new Pot(1, BuildTeams(12));
        Assert.AreEqual(1, pot.Number);
    }

    [TestMethod]
    public void CreatePot_NumberFour_ShouldBeValid()
    {
        var pot = new Pot(4, BuildTeams(12));
        Assert.AreEqual(4, pot.Number);
    }

    // ─── Número de bombo inválido ─────────────────────────────────────────────

    [TestMethod]
    public void CreatePot_WithNumberZero_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Pot(0, BuildTeams(12)));
    }

    [TestMethod]
    public void CreatePot_WithNumberFive_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Pot(5, BuildTeams(12)));
    }

    [TestMethod]
    public void CreatePot_WithNegativeNumber_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Pot(-1, BuildTeams(12)));
    }

    // ─── Cantidad de equipos inválida ─────────────────────────────────────────

    [TestMethod]
    public void CreatePot_WithMoreThanTwelveTeams_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Pot(1, BuildTeams(13)));
    }

    [TestMethod]
    public void CreatePot_WithLessThanTwelveTeams_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Pot(1, BuildTeams(11)));
    }

    [TestMethod]
    public void CreatePot_WithEmptyList_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Pot(1, new List<Team>()));
    }

    [TestMethod]
    public void CreatePot_WithNullList_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Pot(1, null));
    }

    // ─── IsFull ───────────────────────────────────────────────────────────────

    [TestMethod]
    public void IsFull_WithTwelveTeams_ShouldReturnTrue()
    {
        var pot = new Pot(1, BuildTeams(12));
        Assert.IsTrue(pot.IsFull());
    }
}