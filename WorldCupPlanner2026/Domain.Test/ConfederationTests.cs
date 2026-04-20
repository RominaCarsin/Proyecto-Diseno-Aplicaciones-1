namespace Domain.Test;

[TestClass]
public class ConfederationTests
{
    [TestMethod]
    public void CreateConfederation_WithValidData_ShouldSucceed()
    {
        var confederation = new Confederation("CONMEBOL", teamList, 5);
        Assert.IsNotNull(confederation);
    }
}