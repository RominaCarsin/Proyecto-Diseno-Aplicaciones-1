namespace Domain.Test;

[TestClass]
public class ConfederationTests
{
    private const string ValidName = "CONMEBOL";
    private const int ValidSlots = 6;
    
    [TestMethod]
    public void CreateConfederation_WithValidData_ShouldCreateWithExpectedProperties()
    {
        var confederation = new Confederation(ValidName, ValidSlots);

        Assert.AreEqual(ValidName, confederation.Name);
        Assert.AreEqual(ValidSlots, confederation.Slots);
        Assert.AreEqual(0, confederation.Teams.Count);
    }

    [TestMethod]
    public void CreateConfederation_WithEmptyName_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Confederation("", ValidSlots));
    }

    [TestMethod]
    public void CreateConfederation_WithZeroSlots_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Confederation(ValidName, 0));
    }

    [TestMethod]
    public void CreateConfederation_WithNegativeSlots_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Confederation(ValidName, -1));
    }
    
    [TestMethod]
    public void AddTeam_WithValidTeam_ShouldAddTeamToConfederation()
    {
        var confederation = new Confederation(ValidName, ValidSlots);
        var team = BuildTeam("URUGUAY");

        confederation.AddTeam(team);

        Assert.AreEqual(1, confederation.Teams.Count);
    }

    [TestMethod]
    public void AddTeam_WithNullTeam_ShouldThrowArgumentException()
    {
        var confederation = new Confederation(ValidName, ValidSlots);

        Assert.ThrowsException<ArgumentException>(() =>
            confederation.AddTeam(null!));
    }

    [TestMethod]
    public void AddTeam_WhenConfederationIsFull_ShouldThrowArgumentException()
    {
        var confederation = new Confederation("OFC", slots: 1);
        confederation.AddTeam(BuildTeam("URUGUAY", "OFC"));

        Assert.ThrowsException<ArgumentException>(() =>
            confederation.AddTeam(BuildTeam("FRANCE", "OFC")));
    }

    [TestMethod]
    public void RemoveTeam_WithExistingTeam_ShouldRemoveTeamFromConfederation()
    {
        var confederation = new Confederation("OFC", slots: 1);
        var team = BuildTeam("URUGUAY", "OFC");
        confederation.AddTeam(team);

        confederation.RemoveTeam(team);

        Assert.AreEqual(0, confederation.Teams.Count);
    }

    [TestMethod]
    public void RemoveTeam_WithTeamNotInConfederation_ShouldThrowArgumentException()
    {
        var confederation = new Confederation("OFC", slots: 2);
        confederation.AddTeam(BuildTeam("URUGUAY", "OFC"));
        var teamNotInConfederation = BuildTeam("BRAZIL", "OFC");

        Assert.ThrowsException<ArgumentException>(() =>
            confederation.RemoveTeam(teamNotInConfederation));
    }

    [TestMethod]
    public void RemoveTeam_WithNullTeam_ShouldThrowArgumentException()
    {
        var confederation = new Confederation(ValidName, ValidSlots);

        Assert.ThrowsException<ArgumentException>(() =>
            confederation.RemoveTeam(null!));
    }
    
    [TestMethod]
    public void RemainingSlots_AfterAddingOneTeam_ShouldDecrementByOne()
    {
        var confederation = new Confederation(ValidName, ValidSlots);
        confederation.AddTeam(BuildTeam("URUGUAY"));

        Assert.AreEqual(ValidSlots - 1, confederation.RemainingSlots);
    }
    
    private static Team BuildTeam(string name, string confederation = "CONMEBOL") =>
        new Team(name, confederation, 1200);
}