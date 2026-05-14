using Domain;
using DTOs;
using IRepository;
using Repository;

namespace Services.Tests;

[TestClass]
public class AuthStateServiceTests
{
    private const string ValidEmail = "juan@gmail.com";
    private const string ValidPassword = "Password1!";

    private AuthStateService _authStateService = null!;
    private SessionService _sessionService = null!;
    private UserService _userService = null!;
    private int _notifyCount;

    [TestInitialize]
    public void Setup()
    {
        _authStateService = new AuthStateService();
        _notifyCount = 0;

        var userRepository = new Repository<User>();
        var auditRepository = new Repository<AuditLog>();
        var auditService = new AuditService(auditRepository);

        _userService = new UserService(userRepository, auditService);
        _sessionService = new SessionService(_userService, _authStateService);
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
    public void NotifyStateChanged_CalledMultipleTimes_ShouldInvokeOnEachCall()
    {
        _authStateService.OnChange += () => _notifyCount++;

        _authStateService.NotifyStateChanged();
        _authStateService.NotifyStateChanged();
        _authStateService.NotifyStateChanged();

        Assert.AreEqual(3, _notifyCount);
    }

    [TestMethod]
    public void NotifyStateChanged_AfterUnsubscribe_ShouldNotInvokeHandler()
    {
        Action handler = () => _notifyCount++;
        _authStateService.OnChange += handler;
        _authStateService.OnChange -= handler;

        _authStateService.NotifyStateChanged();

        Assert.AreEqual(0, _notifyCount);
    }
    
    [TestMethod]
    public void Login_WithValidCredentials_ShouldTriggerOnChangeEvent()
    {
        _userService.AddUser(BuildValidUserRequest());
        _authStateService.OnChange += () => _notifyCount++;

        _sessionService.Login(ValidEmail, ValidPassword);

        Assert.AreEqual(1, _notifyCount);
    }

    [TestMethod]
    public void Logout_AfterSuccessfulLogin_ShouldTriggerOnChangeEvent()
    {
        _userService.AddUser(BuildValidUserRequest());
        _sessionService.Login(ValidEmail, ValidPassword);
        _authStateService.OnChange += () => _notifyCount++;

        _sessionService.Logout();

        Assert.AreEqual(1, _notifyCount);
    }

    [TestMethod]
    public void Login_WithInvalidCredentials_ShouldNotTriggerOnChangeEvent()
    {
        _userService.AddUser(BuildValidUserRequest());
        _authStateService.OnChange += () => _notifyCount++;

        try { _sessionService.Login(ValidEmail, "WrongPass1!"); } catch { }

        Assert.AreEqual(0, _notifyCount);
    }
    
    private static UserRequest BuildValidUserRequest() =>
        new UserRequest
        {
            Name = "Juan",
            Surname = "Perez",
            Email = ValidEmail,
            DateOfBirth = new DateTime(2000, 1, 1),
            Password = ValidPassword
        };
}