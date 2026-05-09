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
}