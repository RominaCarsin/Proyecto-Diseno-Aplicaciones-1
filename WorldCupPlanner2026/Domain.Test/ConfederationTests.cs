namespace Domain.Test;

[TestClass]
public class ConfederationTests
{
    
    [TestMethod]
    public void CreateConfederation_WithValidData_ShouldSucceed()
    {
        var confederation = new Confederation("CONMEBOL", 6);
        Assert.IsNotNull(confederation);
        Assert.AreEqual("CONMEBOL", confederation.Name);
        Assert.AreEqual(6, confederation.Slots);
        Assert.AreEqual(0, confederation.Teams.Count);
    }

    [TestMethod]
    public void CreateConfederation_WithEmptyName_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Confederation("", 6));
    }

    [TestMethod]
    public void CreateConfederation_WithNegativeSlots_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Confederation("CONMEBOL", -1));
    }

    [TestMethod]
    public void CreateConfederation_WithZeroSlots_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Confederation("CONMEBOL", 0));
    }

    [TestMethod]
    public void AddTeam_WithValidTeam_ShouldAddToList()
    {
        var confederation = new Confederation("CONMEBOL", 6);
        var team = new Team("Uruguay", confederation.Name, 500, 0, 0, 0);

        confederation.AddTeam(team);

        Assert.AreEqual(1, confederation.Teams.Count);
    }

    [TestMethod]
    public void AddTeam_WithNullTeam_ShouldThrowException()
    {
        var confederation = new Confederation("CONMEBOL", 6);

        Assert.ThrowsException<ArgumentException>(() => confederation.AddTeam(null));
    }
    
    [TestMethod]
    public void AddTeam_OnAFullConfederation_ShouldTrowException()
    {
        var confederation = new Confederation("OFC", 1);
        var team = new Team("Uruguay", "OFC", 1200, 0, 0, 0);
        confederation.AddTeam(team);
        Assert.ThrowsException<ArgumentException>(() => confederation.AddTeam(new Team("Francia", "OFC", 1200, 0, 0, 0)));
    }

    [TestMethod]
    public void RemoveTeam_WhenTeamExists_ShouldRemoveTeam()
    {
        var confederation = new Confederation("OFC", 1);
        var team = new Team("Uruguay", "OFC", 1200, 0, 0, 0);
        confederation.AddTeam(team);

        confederation.RemoveTeam(team);
        Assert.AreEqual(0, confederation.Teams.Count);
    }

    [TestMethod]
    public void RemoveTeam_WhenTeamNotInConfederation_ShouldThrowArgumentException()
    {
        var confederation = new Confederation("OFC", 2);
        var team1 = new Team("Uruguay", "OFC", 1200, 0, 0, 0);
        var team2 = new Team("Brazil", "OFC", 1300, 0, 0, 0);
        confederation.AddTeam(team1);
        
        Assert.ThrowsException<ArgumentException>(() => confederation.RemoveTeam(team2));
    }

    [TestMethod]
    public void RemoveTeam_WithNullTeam_ShouldThrowException()
    {
        var confederation = new Confederation("CONMEBOL", 6);

        Assert.ThrowsException<ArgumentException>(() => confederation.RemoveTeam(null));
    }
}