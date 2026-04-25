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
        var team = new Team("Uruguay", confederation, 500, 0, 0, 0);

        confederation.AddTeam(team);

        Assert.AreEqual(1, confederation.Teams.Count);
    }

    [TestMethod]
    public void AddTeam_WithNullTeam_ShouldThrowException()
    {
        var confederation = new Confederation("CONMEBOL", 6);

        Assert.ThrowsException<ArgumentException>(() => confederation.AddTeam(null));
    }
}