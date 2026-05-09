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

    [TestInitialize]
    public void SetUp()
    {
        _repository = new Repository<Stadium>();
        _service = new StadiumService(_repository);
    }
    

    [TestMethod]
    public void AddStadium_WithValidRequest_ShouldPersistStadiumWithExpectedName()
    {
        var request = BuildStadiumRequest(name: "CENTENARIO", city: "MONTEVIDEO");

        _service.AddStadium(request);

        var addedStadium = GetStadiumByName("CENTENARIO");
        Assert.IsNotNull(addedStadium);
        Assert.AreEqual("CENTENARIO", addedStadium.Name);
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