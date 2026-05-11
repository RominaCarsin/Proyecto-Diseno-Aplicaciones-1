using Domain;
using DTOs;
using IRepository;
using Repository;

namespace Services.Tests;

[TestClass]
public class AuthStateServiceTests
{
    private AuthStateService _authStateService;
    private int _notifyCount;

    private string validEmail = "juan@gmail.com";
    private string validPassword = "Password1!";

    private UserRequest _validUser = new UserRequest
    {
        Name = "Juan",
        Surname = "Perez",
        Email = "juan@gmail.com",
        DateOfBirth = new DateTime(2000, 1, 1),
        Password = "Password1!"
    };

    [TestInitialize]
    public void Setup()
    {
        _authStateService = new AuthStateService();
        _notifyCount = 0;
    }

    private void AddValidUser(UserService userService)
    {
        userService.AddUser(_validUser);
    }

    [TestMethod]
    public void NotifyStateChanged_WhenNoSubscribers_ShouldNotThrow()
    {
        _authStateService.NotifyStateChanged();
    }

    [TestMethod]
    public void NotifyStateChanged_WithOneSubscriber_ShouldInvokeOnce()
    {
        _authStateService.OnChange += () => _notifyCount++;

        _authStateService.NotifyStateChanged();

        Assert.AreEqual(1, _notifyCount);
    }

    [TestMethod]
    public void NotifyStateChanged_WithMultipleSubscribers_ShouldInvokeAll()
    {
        int secondCount = 0;
        _authStateService.OnChange += () => _notifyCount++;
        _authStateService.OnChange += () => secondCount++;

        _authStateService.NotifyStateChanged();

        Assert.AreEqual(1, _notifyCount);
        Assert.AreEqual(1, secondCount);
    }

    [TestMethod]
    public void NotifyStateChanged_CalledMultipleTimes_ShouldInvokeEachTime()
    {
        _authStateService.OnChange += () => _notifyCount++;

        _authStateService.NotifyStateChanged();
        _authStateService.NotifyStateChanged();
        _authStateService.NotifyStateChanged();

        Assert.AreEqual(3, _notifyCount);
    }

    [TestMethod]
    public void NotifyStateChanged_AfterUnsubscribe_ShouldNotInvoke()
    {
        Action handler = () => _notifyCount++;
        _authStateService.OnChange += handler;
        _authStateService.OnChange -= handler;

        _authStateService.NotifyStateChanged();

        Assert.AreEqual(0, _notifyCount);
    }

    [TestMethod]
    public void Login_ShouldTriggerOnChangeEvent()
    {
        var auditRepo    = new Repository<AuditLog>();
        var auditService = new AuditService(auditRepo);
        var tempSession  = new SessionService(null!, _authStateService);
        var repository = new Repository<User>();
        var userService  = new UserService(repository, auditService, tempSession);
        var sessionService = new SessionService(userService, _authStateService);

        AddValidUser(userService);

        _authStateService.OnChange += () => _notifyCount++;
        sessionService.Login(validEmail, validPassword);

        Assert.AreEqual(1, _notifyCount);
    }

    [TestMethod]
    public void Logout_ShouldTriggerOnChangeEvent()
    {
        var auditRepo    = new Repository<AuditLog>();
        var auditService = new AuditService(auditRepo);
        var tempSession  = new SessionService(null!, _authStateService);
        var repository = new Repository<User>();
        var userService  = new UserService(repository, auditService, tempSession);
        var sessionService = new SessionService(userService, _authStateService);

        AddValidUser(userService);

        sessionService.Login(validEmail, validPassword);
        _authStateService.OnChange += () => _notifyCount++;
        sessionService.Logout();

        Assert.AreEqual(1, _notifyCount);
    }

    [TestMethod]
    public void Login_WithInvalidCredentials_ShouldNotTriggerEvent()
    {
        var auditRepo    = new Repository<AuditLog>();
        var auditService = new AuditService(auditRepo);
        var tempSession  = new SessionService(null!, _authStateService);
        var repository = new Repository<User>();
        var userService  = new UserService(repository, auditService, tempSession);
        var sessionService = new SessionService(userService, _authStateService);

        AddValidUser(userService);

        _authStateService.OnChange += () => _notifyCount++;
        try { sessionService.Login(validEmail, "WrongPass1!"); } catch { }

        Assert.AreEqual(0, _notifyCount);
    }
}