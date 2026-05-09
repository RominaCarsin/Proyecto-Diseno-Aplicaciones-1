using Domain;
using IRepository;
using Repository;

namespace Services.Tests;

[TestClass]
public class AuditServicesTests
{
    private AuditService _service;
    private IRepository<AuditLog> _repository;
    
    private const string AdminEmail = "admin@test.com";
    private const string Action = "User created";
    private const string Details = "juan@test.com";

    [TestInitialize]
    public void Setup()
    {
        _repository = new Repository<AuditLog>();
        _service = new AuditService(_repository);
    }

    [TestMethod]
    public void Log_WithValidData_ShouldSaveAuditLog()
    {
        _service.Log(AdminEmail, Action, Details);
        var logs = _repository.GetAll().ToList();
        Assert.AreEqual(1, logs.Count);
        Assert.AreEqual(AdminEmail,logs[0].ActorEmail);
        Assert.AreEqual(Action, logs[0].Action);
        Assert.AreEqual(Details, logs[0].Details);
    }
    
    [TestMethod]
    public void Log_CalledMultipleTimes_ShouldSaveAllLogs()
    {
        _service.Log(AdminEmail, "User created",  "juan@test.com");
        _service.Log(AdminEmail, "User deleted",  "pedro@test.com");
        _service.Log(AdminEmail, "Stadium added", "Estadio Centenario");

        var logs = _repository.GetAll().ToList();

        Assert.AreEqual(3, logs.Count);
    }
    
    [TestMethod]
    public void GetAll_ShouldReturnAllLogs()
    {
        _service.Log(AdminEmail, "User created", "juan@test.com");
        _service.Log(AdminEmail, "User deleted", "pedro@test.com");

        var logs = _service.GetAll().ToList();

        Assert.AreEqual(2, logs.Count);
        Assert.IsTrue(logs.Any(l => l.Action == "User created"));
        Assert.IsTrue(logs.Any(l => l.Action == "User deleted"));
    }
    
    [TestMethod]
    public void GetAll_WhenNoLogs_ShouldReturnEmptyList()
    {
        var logs = _service.GetAll().ToList();

        Assert.AreEqual(0, logs.Count);
    }
}