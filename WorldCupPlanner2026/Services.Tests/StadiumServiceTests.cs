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
    public void AddStadium_WithDuplicateNameIgnoringCase_ShouldThrowArgumentException()
    {
        var stadium1 = new Stadium("Centenario", "Montevideo", null, 20000);
        var stadium2 = new Stadium("CENTENARIO", "Buenos Aires", null, 50000);
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
    
    [TestMethod]
    public void DeleteStadium_WithNonExistingName_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() => _service.DeleteStadium("NombreQueNoExiste"));
    }
    
    [TestMethod]
    public void UpdateStadium_WithValidData_ShouldUpdateStadium()
    {
        var stadium = new Stadium("Centenario", "Montevideo", null, 20000);
        _service.AddStadium(stadium);
        var updated = new Stadium("Centenario Renovado", "Montevideo", null, 25000);

        _service.UpdateStadium("Centenario", updated);

        Assert.IsNotNull(_service.GetStadiums().FirstOrDefault(s => s.Name == "Centenario Renovado"));
    }
    
    [TestMethod]
    public void UpdateStadium_WithDuplicateName_ShouldThrowArgumentException()
    {
        var stadium1 = new Stadium("Centenario", "Montevideo", null, 20000);
        var stadium2 = new Stadium("Estadio Nacional", "Montevideo", null, 30000);
        _service.AddStadium(stadium1);
        _service.AddStadium(stadium2);
        var updated = new Stadium("Centenario", "Otra ciudad", null, 25000);

        Assert.ThrowsException<ArgumentException>(() => _service.UpdateStadium("Estadio Nacional", updated));
    }

    [TestMethod]
    public void UpdateStadium_WithDuplicateNameIgnoringCase_ShouldThrowArgumentException()
    {
        var stadium1 = new Stadium("Centenario", "Montevideo", null, 20000);
        var stadium2 = new Stadium("Estadio Nacional", "Montevideo", null, 30000);
        _service.AddStadium(stadium1);
        _service.AddStadium(stadium2);
        var updated = new Stadium("CENTENARIO", "Otra ciudad", null, 25000);

        Assert.ThrowsException<ArgumentException>(() => _service.UpdateStadium("Estadio Nacional", updated));
    }

    [TestMethod]
    public void UpdateStadium_WithNonExistingOriginalName_ShouldThrowArgumentException()
    {
        var updated = new Stadium("Nuevo Estadio", "Ciudad", null, 25000);

        Assert.ThrowsException<ArgumentException>(() => _service.UpdateStadium("NombreQueNoExiste", updated));
    }

}