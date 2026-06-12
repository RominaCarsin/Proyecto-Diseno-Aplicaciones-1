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
        IRepository<Stadium> repository = new FakeRepository<Stadium>();
        IRepository<AuditLog> auditRepository = new FakeRepository<AuditLog>();
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
    public void UpdateStadium_WithValidData_ShouldUpdateStadiumWithExpectedProperties()
    {
        _service.AddStadium(BuildStadiumRequest(
            "CENTENARIO",
            "MONTEVIDEO",
            "Old description",
            60000));

        _service.UpdateStadium("CENTENARIO", BuildStadiumRequest(
            "CAMPEON DEL SIGLO",
            "CANELONES",
            "Updated description",
            70000));

        Assert.IsNull(GetStadiumByName("CENTENARIO"));

        var updatedStadium = GetStadiumByName("CAMPEON DEL SIGLO");

        Assert.IsNotNull(updatedStadium);
        Assert.AreEqual("CAMPEON DEL SIGLO", updatedStadium.Name);
        Assert.AreEqual("CANELONES", updatedStadium.City);
        Assert.AreEqual("Updated description", updatedStadium.Description);
        Assert.AreEqual(70000, updatedStadium.Capacity);
    }

    [TestMethod]
    public void UpdateStadium_WithLowercaseAndSpaces_ShouldNormalizeUpdatedNameAndCity()
    {
        _service.AddStadium(BuildStadiumRequest("CENTENARIO", "MONTEVIDEO"));

        _service.UpdateStadium("CENTENARIO", BuildStadiumRequest(
            "  gran parque central  ",
            "  montevideo  ",
            "Updated description",
            50000));

        var updatedStadium = GetStadiumByName("GRAN PARQUE CENTRAL");

        Assert.IsNotNull(updatedStadium);
        Assert.AreEqual("GRAN PARQUE CENTRAL", updatedStadium.Name);
        Assert.AreEqual("MONTEVIDEO", updatedStadium.City);
    }
    
    [TestMethod]
    public void UpdateStadium_WithNullDescription_ShouldUpdateDescriptionToNull()
    {
        _service.AddStadium(BuildStadiumRequest(
            "CENTENARIO",
            "MONTEVIDEO",
            "Old description",
            60000));

        _service.UpdateStadium("CENTENARIO", BuildStadiumRequest(
            "CENTENARIO",
            "MONTEVIDEO",
            null,
            60000));

        var updatedStadium = GetStadiumByName("CENTENARIO");

        Assert.IsNotNull(updatedStadium);
        Assert.IsNull(updatedStadium.Description);
    }

    [TestMethod]
    public void UpdateStadium_WithEmptyDescription_ShouldUpdateDescriptionToEmpty()
    {
        _service.AddStadium(BuildStadiumRequest(
            "CENTENARIO",
            "MONTEVIDEO",
            "Old description",
            60000));

        _service.UpdateStadium("CENTENARIO", BuildStadiumRequest(
            "CENTENARIO",
            "MONTEVIDEO",
            "",
            60000));

        var updatedStadium = GetStadiumByName("CENTENARIO");

        Assert.IsNotNull(updatedStadium);
        Assert.AreEqual("", updatedStadium.Description);
    }
    
    [TestMethod]
    public void UpdateStadium_WithInvalidName_ShouldThrowArgumentException()
    {
        _service.AddStadium(BuildStadiumRequest("CENTENARIO", "MONTEVIDEO"));

        Assert.ThrowsException<ArgumentException>(() =>
            _service.UpdateStadium("CENTENARIO", BuildStadiumRequest(
                "",
                "MONTEVIDEO",
                "Description",
                60000)));
    }

    [TestMethod]
    public void UpdateStadium_WithInvalidCity_ShouldThrowArgumentException()
    {
        _service.AddStadium(BuildStadiumRequest("CENTENARIO", "MONTEVIDEO"));

        Assert.ThrowsException<ArgumentException>(() =>
            _service.UpdateStadium("CENTENARIO", BuildStadiumRequest(
                "CENTENARIO",
                "",
                "Description",
                60000)));
    }

    [TestMethod]
    public void UpdateStadium_WithDescriptionOneAboveMaxLength_ShouldThrowArgumentException()
    {
        _service.AddStadium(BuildStadiumRequest("CENTENARIO", "MONTEVIDEO"));
        var descriptionOneAboveMaxLength = new string('A', 401);

        Assert.ThrowsException<ArgumentException>(() =>
            _service.UpdateStadium("CENTENARIO", BuildStadiumRequest(
                "CENTENARIO",
                "MONTEVIDEO",
                descriptionOneAboveMaxLength,
                60000)));
    }

    [TestMethod]
    public void UpdateStadium_WithCapacityOneBelowMinimum_ShouldThrowArgumentException()
    {
        _service.AddStadium(BuildStadiumRequest("CENTENARIO", "MONTEVIDEO"));

        Assert.ThrowsException<ArgumentException>(() =>
            _service.UpdateStadium("CENTENARIO", BuildStadiumRequest(
                "CENTENARIO",
                "MONTEVIDEO",
                "Description",
                19999)));
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
        _service.AddStadium(BuildStadiumRequest(
            "CENTENARIO",
            "MONTEVIDEO",
            "Old description",
            60000));

        _service.UpdateStadium("CENTENARIO", BuildStadiumRequest(
            "CENTENARIO",
            "BUENOS AIRES",
            "New description",
            80000));

        var updatedStadium = GetStadiumByName("CENTENARIO");

        Assert.IsNotNull(updatedStadium);
        Assert.AreEqual("CENTENARIO", updatedStadium.Name);
        Assert.AreEqual("BUENOS AIRES", updatedStadium.City);
        Assert.AreEqual("New description", updatedStadium.Description);
        Assert.AreEqual(80000, updatedStadium.Capacity);
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
