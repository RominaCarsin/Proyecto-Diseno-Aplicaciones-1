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
    
        Assert.IsNotNull(_service.GetStadiums().FirstOrDefault(s => s.Name == "Centenario"));
    }
    
    
    [TestMethod]
    public void AddStadium_WithDuplicateName_ShouldThrowArgumentException()
    {
        var stadium1 = new Stadium("Centenario", "Montevideo", null, 20000);
        var stadium2 = new Stadium("Centenario", "Buenos Aires", null, 50000);
        _service.AddStadium(stadium1);

        Assert.ThrowsException<ArgumentException>(() => _service.AddStadium(stadium2));
    }
    
    [TestMethod]
    public void DeleteStadium_WithExistingStadium_ShouldRemoveIt()
    {
        var stadium = new Stadium("Centenario", "Montevideo", null, 20000);
        _service.AddStadium(stadium);

        _service.DeleteStadium(stadium.Name);

        Assert.IsNull(_service.GetStadiums().FirstOrDefault(s => s.Name == "Centenario"));
    }
}