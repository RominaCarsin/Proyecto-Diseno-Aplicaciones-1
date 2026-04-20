namespace Domain.Test;

[TestClass]
public class ConfederationTests
{
    [TestMethod]
    public void CreateConfederation_WithValidData_ShouldSucceed()
    {
        var teamList = new List<Team>
        {
            new Team("Argentina", "CONMEBOL", 1, 25, 10, 15),
            new Team("Brazil", "CONMEBOL", 2, 24, 8, 12)
        };
        
        var confederation = new Confederation("CONMEBOL", teamList, 5);
        Assert.IsNotNull(confederation);
    }
}