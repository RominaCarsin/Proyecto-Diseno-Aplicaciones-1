using Domain;
using IRepository;
using Repository;

namespace Services.Tests;

[TestClass]
public class AuditServiceTests
{
    private const string ValidActorEmail = "admin@test.com";
    private const string ValidAction = "User created";
    private const string ValidDetails = "juan@test.com";

    private AuditService _service = null!;
    private IRepository<AuditLog> _repository = null!;

    [TestInitialize]
    public void Setup()
    {
        _repository = new Repository<AuditLog>();
        _service = new AuditService(_repository);
    }
    
    [TestMethod]
    public void Log_WithValidData_ShouldPersistAuditLogWithExpectedProperties()
    {
        _service.Log(ValidActorEmail, ValidAction, ValidDetails);

        var logs = _repository.GetAll().ToList();
        Assert.AreEqual(1, logs.Count);
        Assert.AreEqual(ValidActorEmail, logs[0].ActorEmail);
        Assert.AreEqual(ValidAction, logs[0].Action);
        Assert.AreEqual(ValidDetails, logs[0].Details);
    }

    [TestMethod]
    public void Log_CalledMultipleTimes_ShouldPersistAllLogs()
    {
        _service.Log(ValidActorEmail, "User created", "juan@test.com");
        _service.Log(ValidActorEmail, "User deleted", "pedro@test.com");
        _service.Log(ValidActorEmail, "Stadium added", "Estadio Centenario");

        Assert.AreEqual(3, _repository.GetAll().Count());
    }

    [TestMethod]
    public void Log_ShouldSetTimestampCloseToUtcNow()
    {
        var before = DateTime.UtcNow;

        _service.Log(ValidActorEmail, ValidAction, ValidDetails);

        var after = DateTime.UtcNow;
        var log = _repository.GetAll().First();
        Assert.IsTrue(log.Timestamp >= before && log.Timestamp <= after);
    }

    [TestMethod]
    public void Log_WithEmptyActorEmail_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _service.Log("", ValidAction, ValidDetails));
    }

    [TestMethod]
    public void Log_WithNullActorEmail_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _service.Log(null!, ValidAction, ValidDetails));
    }

    [TestMethod]
    public void Log_WithWhitespaceActorEmail_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _service.Log("   ", ValidAction, ValidDetails));
    }

    [TestMethod]
    public void Log_WithEmptyAction_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _service.Log(ValidActorEmail, "", ValidDetails));
    }

    [TestMethod]
    public void Log_WithNullAction_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _service.Log(ValidActorEmail, null!, ValidDetails));
    }

    [TestMethod]
    public void Log_WithWhitespaceAction_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _service.Log(ValidActorEmail, "   ", ValidDetails));
    }
    
    [TestMethod]
    public void GetAll_WithNoLogs_ShouldReturnEmptyCollection()
    {
        var logs = _service.GetAll().ToList();

        Assert.AreEqual(0, logs.Count);
    }

    [TestMethod]
    public void GetAll_WithMultipleLogs_ShouldReturnAllPersistedLogs()
    {
        _service.Log(ValidActorEmail, "User created", "juan@test.com");
        _service.Log(ValidActorEmail, "User deleted", "pedro@test.com");

        var logs = _service.GetAll().ToList();

        Assert.AreEqual(2, logs.Count);
        Assert.IsTrue(logs.Any(l => l.Action == "User created"));
        Assert.IsTrue(logs.Any(l => l.Action == "User deleted"));
    }
}