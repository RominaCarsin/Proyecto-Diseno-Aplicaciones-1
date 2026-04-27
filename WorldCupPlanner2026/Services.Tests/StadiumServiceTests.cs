using Domain;
using Services;
using Repository;
using IRepository;


namespace Services.Tests;

[TestClass]
public class StadiumServiceTests
{
    private StadiumService _service = null!;
    private IRepository<Stadium> _repository = null!;

    [TestInitialize]
    public void SetUp()
    {
        _repository = new Repository<Stadium>();
        _service = new StadiumService(_repository);
    }

    [TestMethod]
    public void AddStadium_WithValidStadium_ShouldAddStadium()
    {
        var stadium = new Stadium("Centenario", "Montevideo", null, 20000);

        _service.AddStadium(stadium);

        Assert.IsTrue(_service.GetStadiums().Any(s => s.Name == "Centenario"));
    }
}