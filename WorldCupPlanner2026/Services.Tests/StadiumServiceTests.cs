using Domain;
using DTOs;
using Services.Mappers;
using Services;
using Repository;
using IRepository;

namespace Services.Tests;

[TestClass]
public class StadiumServiceTests
{
    private StadiumService _service = null!;
    private IRepository<Stadium> _repository = null!;
    private StadiumRequest _validStadiumRequest = new StadiumRequest{Name = "CENTENARIO", City = "MONTEVIDEO", Description = null, Capacity = 20000};
    private StadiumRequest _otherValidStadiumRequest = new StadiumRequest{Name = "CENTENARIO", City = "buenos aires", Description = null, Capacity = 20000};
    private StadiumRequest _otherValidStadiumRequestWithOtherName = new StadiumRequest{Name = "CAMPION", City = "buenos aires", Description = null, Capacity = 20000};

    

    [TestInitialize]
    public void SetUp()
    {
        _repository = new Repository<Stadium>();
        _service = new StadiumService(_repository);
    }

    [TestMethod]
    public void AddStadium_WithValidStadium_ShouldAddStadium()
    {

        _service.AddStadium(_validStadiumRequest);

        Assert.IsNotNull(_service.GetStadiums().FirstOrDefault(s => s.Name == "CENTENARIO"));
    }

    [TestMethod]
    public void AddStadium_WithDuplicateName_ShouldThrowArgumentException()
    {
        _service.AddStadium(_validStadiumRequest);

        Assert.ThrowsException<ArgumentException>(() => _service.AddStadium(_validStadiumRequest));
    }

    [TestMethod]
    public void AddStadium_WithDuplicateNameIgnoringCase_ShouldThrowArgumentException()
    {
        _service.AddStadium(_validStadiumRequest);

        Assert.ThrowsException<ArgumentException>(() => _service.AddStadium(_otherValidStadiumRequest));
    }

    [TestMethod]
    public void DeleteStadium_WithExistingStadium_ShouldRemoveIt()
    {
        _service.AddStadium(_validStadiumRequest);
        _service.DeleteStadium(_validStadiumRequest.Name);

        Assert.IsNull(_service.GetStadiums().FirstOrDefault(s => s.Name == "CENTENARIO"));
    }

    [TestMethod]
    public void DeleteStadium_WithNonExistingName_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() => _service.DeleteStadium("NombreQueNoExiste"));
    }

    [TestMethod]
    public void UpdateStadium_WithValidData_ShouldUpdateStadium()
    {
      
        _service.AddStadium(_validStadiumRequest);
        _service.UpdateStadium("CENTENARIO", _otherValidStadiumRequestWithOtherName);

        Assert.IsNotNull(_service.GetStadiums().FirstOrDefault(s => s.Name == "CAMPION"));
    }

    [TestMethod]
    public void UpdateStadium_WithDuplicateName_ShouldThrowArgumentException()
    {
        _service.AddStadium(_validStadiumRequest);
        _service.AddStadium(_otherValidStadiumRequestWithOtherName);

        Assert.ThrowsException<ArgumentException>(() => _service.UpdateStadium(_otherValidStadiumRequestWithOtherName.Name, _validStadiumRequest));
    }
    

    [TestMethod]
    public void UpdateStadium_WithNonExistingOriginalName_ShouldThrowArgumentException()
    {
        _service.AddStadium(_validStadiumRequest);
        
        Assert.ThrowsException<ArgumentException>(() => _service.UpdateStadium("NombreQueNoExiste", _validStadiumRequest));
    }


}