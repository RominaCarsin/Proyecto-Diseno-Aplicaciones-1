using Domain;
using DTOs;
using IRepository;
using Repository;

namespace Services.Tests;

[TestClass]
public class StadiumServiceTests
{
    private StadiumService _service = null!;
    private IRepository<Stadium> _repository = null!;
    private StadiumRequest _validStadiumRequest = new StadiumRequest{Name = "CENTENARIO", City = "MONTEVIDEO", Description = null, Capacity = 20000};
    private StadiumRequest _otherValidStadiumRequest = new StadiumRequest{Name = "CENTENARIO", City = "buenos aires", Description = null, Capacity = 20000};
    private StadiumRequest _otherValidStadiumRequestWithOtherName = new StadiumRequest{Name = "CAMPION", City = "buenos aires", Description = null, Capacity = 20000};
    private AuditService          _auditService;
    private IRepository<AuditLog> _auditRepository;
    private SessionService        _sessionService;

    [TestInitialize]
    public void SetUp()
    {
        _repository = new Repository<Stadium>();
        _auditRepository = new Repository<AuditLog>();
        _auditService = new AuditService(_auditRepository);
        var authState = new AuthStateService();
        _sessionService = new SessionService(null!, authState);
        _service = new StadiumService(_repository, _auditService, _sessionService);
    }
    

    [TestMethod]
    public void AddStadium_WithValidRequest_ShouldPersistStadiumWithExpectedName()
    {
        var request = BuildStadiumRequest(name: "CENTENARIO", city: "MONTEVIDEO");

        _service.AddStadium(request);

        Assert.IsNotNull(_service.GetAllStadiums().FirstOrDefault(s => s.Name == "CENTENARIO"));
    }

    [TestMethod]
    public void AddStadium_WithDuplicateName_ShouldThrowArgumentException()
    {
        var firstRequest = BuildStadiumRequest(name: "CENTENARIO", city: "MONTEVIDEO");
        var duplicateRequest = BuildStadiumRequest(name: "CENTENARIO", city: "BUENOS AIRES");
        _service.AddStadium(firstRequest);

        Assert.ThrowsException<ArgumentException>(() =>
            _service.AddStadium(duplicateRequest));
    }

    [TestMethod]
    public void AddStadium_WithDuplicateNameInDifferentCase_ShouldThrowArgumentException()
    {
        var firstRequest = BuildStadiumRequest(name: "CENTENARIO", city: "MONTEVIDEO");
        var duplicateWithDifferentCase = BuildStadiumRequest(name: "centenario", city: "BUENOS AIRES");
        _service.AddStadium(firstRequest);

        Assert.ThrowsException<ArgumentException>(() =>
            _service.AddStadium(duplicateWithDifferentCase));
    }
    

    [TestMethod]
    public void DeleteStadium_WithExistingStadium_ShouldRemoveStadiumFromRepository()
    {
        var request = BuildStadiumRequest(name: "CENTENARIO", city: "MONTEVIDEO");
        _service.AddStadium(request);

        _service.DeleteStadium("CENTENARIO");

        Assert.IsNull(_service.GetAllStadiums().FirstOrDefault(s => s.Name == "CENTENARIO"));
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

        Assert.IsNotNull(_service.GetAllStadiums().FirstOrDefault(s => s.Name == "CAMPION"));
    }

    [TestMethod]
    public void UpdateStadium_WithDuplicateName_ShouldThrowArgumentException()
    {
        _service.AddStadium(_validStadiumRequest);
        _service.AddStadium(_otherValidStadiumRequestWithOtherName);

        Assert.ThrowsException<ArgumentException>(() => _service.UpdateStadium(_otherValidStadiumRequestWithOtherName.Name, _validStadiumRequest));
    }

    [TestMethod]
    public void UpdateStadium_WithValidData_ShouldReplaceOldStadiumWithUpdatedOne()
    {
        var originalRequest = BuildStadiumRequest(name: "CENTENARIO", city: "MONTEVIDEO");
        var updatedRequest = BuildStadiumRequest(name: "CAMPEON", city: "BUENOS AIRES");
        _service.AddStadium(originalRequest);

        _service.UpdateStadium("CENTENARIO", updatedRequest);

        Assert.IsNull(GetStadiumByName("CENTENARIO"));
        Assert.IsNotNull(GetStadiumByName("CAMPEON"));
    }

    [TestMethod]
    public void UpdateStadium_WithNameAlreadyTakenByAnotherStadium_ShouldThrowArgumentException()
    {
        var centenarioRequest = BuildStadiumRequest(name: "CENTENARIO", city: "MONTEVIDEO");
        var campeonRequest = BuildStadiumRequest(name: "CAMPEON", city: "BUENOS AIRES");
        _service.AddStadium(centenarioRequest);
        _service.AddStadium(campeonRequest);

        Assert.ThrowsException<ArgumentException>(() =>
            _service.UpdateStadium("CAMPEON", centenarioRequest));
    }

    [TestMethod]
    public void UpdateStadium_WithSameNameAsOriginal_ShouldUpdateRemainingDataSuccessfully()
    {
        var originalRequest = BuildStadiumRequest(name: "CENTENARIO", city: "MONTEVIDEO");
        var sameNameUpdatedRequest = BuildStadiumRequest(name: "CENTENARIO", city: "BUENOS AIRES");
        _service.AddStadium(originalRequest);

        _service.UpdateStadium("CENTENARIO", sameNameUpdatedRequest);

        var updatedStadium = GetStadiumByName("CENTENARIO");
        Assert.IsNotNull(updatedStadium);
        Assert.AreEqual("BUENOS AIRES", updatedStadium.City);
    }

    [TestMethod]
    public void UpdateStadium_WithNonExistingOriginalName_ShouldThrowArgumentException()
    {
        var request = BuildStadiumRequest(name: "CENTENARIO", city: "MONTEVIDEO");

        Assert.ThrowsException<ArgumentException>(() =>
            _service.UpdateStadium("NON EXISTING STADIUM", request));
    }
    

    private static StadiumRequest BuildStadiumRequest(
        string name,
        string city,
        string? description = null,
        int capacity = 60000)
    {
        return new StadiumRequest
        {
            Name = name,
            City = city,
            Description = description,
            Capacity = capacity
        };
    }

    private StadiumResponse? GetStadiumByName(string name) =>
        _service.GetAllStadiums().FirstOrDefault(s => s.Name == name);
}
