namespace Services.Tests;

[TestClass]
public class StadiumServiceTests
{
    private StadiumsService _service = null!;

    [TestInitialize]
    public void SetUp()
    {
        _service = new StadiumsService();
    }

    [TestMethod]
    public void AddStadium_WithValidStadium_ShouldAddStadium()
    {
        var stadium = new Stadium("Centenario", "Montevideo", null, 20000);

        _service.Add(stadium);

        Assert.IsTrue(_service.GetAll().Any(s => s.Name == "Centenario"));
    }
}