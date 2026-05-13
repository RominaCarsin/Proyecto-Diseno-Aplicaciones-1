using Domain;
using DTOs;
using IRepository;
using Repository;

namespace Services.Tests;

[TestClass]
public class StadiumServiceTests
{
    private StadiumService _service = null!;

    [TestInitialize]
    public void SetUp()
    {
        IRepository<Stadium> repository = new Repository<Stadium>();
        IRepository<AuditLog> auditRepository = new Repository<AuditLog>();
        var auditService = new AuditService(auditRepository);
        var authState = new AuthStateService();
        var sessionService = new SessionService(null!, authState);

        _service = new StadiumService(repository, auditService, sessionService);
    }

    [TestMethod]
    public void AddStadium_WithValidRequest_ShouldPersistStadiumWithExpectedName()
    {
        _service.AddStadium(BuildStadiumRequest("CENTENARIO", "MONTEVIDEO"));

        var addedStadium = GetStadiumByName("CENTENARIO");
        Assert.IsNotNull(addedStadium);
        Assert.AreEqual("CENTENARIO", addedStadium.Name);
    }

    [TestMethod]
    public void AddStadium_WithDuplicateName_ShouldThrowArgumentException()
    {
        _service.AddStadium(BuildStadiumRequest("CENTENARIO", "MONTEVIDEO"));

        Assert.ThrowsException<ArgumentException>(() =>
            _service.AddStadium(BuildStadiumRequest("centenario", "BUENOS AIRES")));
    }

    [TestMethod]
    public void DeleteStadium_WithExistingStadium_ShouldRemoveStadiumFromRepository()
    {
        _service.AddStadium(BuildStadiumRequest("CENTENARIO", "MONTEVIDEO"));

        _service.DeleteStadium("CENTENARIO");

        Assert.IsNull(GetStadiumByName("CENTENARIO"));
    }

    [TestMethod]
    public void DeleteStadium_WithNonExistingName_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _service.DeleteStadium("NON EXISTING STADIUM"));
    }

    [TestMethod]
    public void UpdateStadium_WithValidData_ShouldReplaceOldStadiumWithUpdatedOne()
    {
        _service.AddStadium(BuildStadiumRequest("CENTENARIO", "MONTEVIDEO"));

        _service.UpdateStadium("CENTENARIO", BuildStadiumRequest("CAMPEON", "BUENOS AIRES"));

        Assert.IsNull(GetStadiumByName("CENTENARIO"));
        Assert.IsNotNull(GetStadiumByName("CAMPEON"));
    }

    [TestMethod]
    public void UpdateStadium_WithNameAlreadyTakenByAnotherStadium_ShouldThrowArgumentException()
    {
        _service.AddStadium(BuildStadiumRequest("CENTENARIO", "MONTEVIDEO"));
        _service.AddStadium(BuildStadiumRequest("CAMPEON", "BUENOS AIRES"));

        Assert.ThrowsException<ArgumentException>(() =>
            _service.UpdateStadium("CAMPEON", BuildStadiumRequest("CENTENARIO", "MONTEVIDEO")));
    }

    [TestMethod]
    public void UpdateStadium_WithSameNameAsOriginal_ShouldUpdateRemainingDataSuccessfully()
    {
        _service.AddStadium(BuildStadiumRequest("CENTENARIO", "MONTEVIDEO"));

        _service.UpdateStadium("CENTENARIO", BuildStadiumRequest("CENTENARIO", "BUENOS AIRES"));

        var updatedStadium = GetStadiumByName("CENTENARIO");
        Assert.IsNotNull(updatedStadium);
        Assert.AreEqual("BUENOS AIRES", updatedStadium.City);
    }

    [TestMethod]
    public void UpdateStadium_WithNonExistingOriginalName_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _service.UpdateStadium("NON EXISTING STADIUM", BuildStadiumRequest("CENTENARIO", "MONTEVIDEO")));
    }

    private static StadiumRequest BuildStadiumRequest(
        string name,
        string city,
        string? description = null,
        int capacity = 60000) =>
        new()
        {
            Name = name,
            City = city,
            Description = description,
            Capacity = capacity
        };

    private StadiumResponse? GetStadiumByName(string name) =>
        _service.GetAllStadiums().FirstOrDefault(s => s.Name == name);
}
