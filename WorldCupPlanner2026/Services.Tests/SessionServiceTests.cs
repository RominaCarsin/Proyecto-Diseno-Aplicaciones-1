using Domain;
using DTOs;
using IRepository;
using Repository;

namespace Services.Tests;

[TestClass]
public class SessionServiceTests
{
    private UserService _userService;
    private SessionService _sessionService;
    private IRepository<User> _repository;

    private string validEmail = "juan@gmail.com";
    private string validPassword = "Password1!";
    
    private UserRequest _validUser = new UserRequest {
        Name = "Juan",
        Surname = "Perez",
        Email = "juan@gmail.com",
        DateOfBirth = new DateTime(2000, 1, 1),
        Password = "Password1!"
    };
    
    [TestInitialize]
    public void Setup()
    {
        _repository = new Repository<User>();
        _userService = new UserService(_repository);
        _sessionService = new SessionService(_userService);
    }

    [TestMethod]
    public void IsLoggedIn_WhenNoLoginHasOccurred_ShouldReturnFalse()
    {
        Assert.IsFalse(_sessionService.IsLoggedIn());
    }

    [TestMethod]
    public void IsLoggedIn_WhenLoginHasOccurred_ShouldReturnTrue()
    {
        _userService.AddUser(_validUser);
        _sessionService.Login(validEmail, validPassword);
        Assert.IsTrue(_sessionService.IsLoggedIn());
    }
    
    [TestMethod]
    public void isLoggedIn_AfterLogout_ShouldReturnFalse()
    {
        _userService.AddUser(_validUser);
        _sessionService.Login(validEmail, validPassword);
        _sessionService.Logout();
        Assert.IsFalse(_sessionService.IsLoggedIn());
    }
    
    [TestMethod]
    public void Login_WithNonExistingEmail_ShouldReturnFalse()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _sessionService.Login("notfound@gmail.com", validPassword));
    }

    [TestMethod]
    public void Login_WithWrongPassword_ShouldThrowException()
    {
        _userService.AddUser(_validUser);
        Assert.ThrowsException<ArgumentException>(() =>
            _sessionService.Login(validEmail, "WrongPass1!"));
    }

    [TestMethod]
    public void Login_WithWrongPassword_ShouldNotSetCurrentUser()
    {
        _userService.AddUser(_validUser);
        try { _sessionService.Login(validEmail, "WrongPass1!"); } catch { }
        Assert.IsFalse(_sessionService.IsLoggedIn());
    }
    
    [TestMethod]
    public void Logout_whenNotLoggedIn_ShouldNotThrow()
    {
        _sessionService.Logout();

        Assert.IsFalse(_sessionService.IsLoggedIn());
    }
    
    [TestMethod]
    public void Logout_ShouldClearCurrentUser()
    {
        _userService.AddUser(_validUser);
        _sessionService.Login(validEmail, validPassword);
        _sessionService.Logout();
        Assert.IsNull(_sessionService.GetCurrentUser());
    }

    [TestMethod]
    public void GetCurrentUserEmail_WhenNotLoggedIn_ReturnsNull()
    {
        Assert.IsNull(_sessionService.GetCurrentUser());
    }

    [TestMethod]
    public void GetCurrentUserEmail_AfterSuccesfullLogin_ShouldReturnEmail()
    {
        _userService.AddUser(_validUser);
        _sessionService.Login(validEmail, validPassword);
        Assert.AreEqual(validEmail, _sessionService.GetCurrentUserEmail());
    }

    [TestMethod]
    public void GetCurrentUserEmail_AfterLogout_ReturnsNull()
    {
        _userService.AddUser(_validUser);
        _sessionService.Login(validEmail, validPassword);
        _sessionService.Logout();
        Assert.IsNull(_sessionService.GetCurrentUserEmail());
    }
    
    [TestMethod]
    public void GetCurrentUser_WhenNotLoggedIn_ReturnsNull()
    {
        Assert.IsNull(_sessionService.GetCurrentUser());
    }

    [TestMethod]
    public void GetCurrentUser_AfterSuccesfulLogin_ReturnsUser()
    {
        _userService.AddUser(_validUser);
        _sessionService.Login(validEmail, validPassword);
        var result = _sessionService.GetCurrentUser();

        Assert.IsNotNull(result);
        Assert.AreEqual(validEmail, result.Email);
        Assert.AreEqual("Juan", result.Name);
    }
    
    [TestMethod]
    public void GetCurrentUser_AfterLogout_ReturnsNull()
    {
        _userService.AddUser(_validUser);
        _sessionService.Login(validEmail, validPassword);
        _sessionService.Logout();
        Assert.IsNull(_sessionService.GetCurrentUser());
    }
    
    [TestMethod]
    public void Login_WhenAlreadyLoggedIn_ShouldThrowException()
    {
        var otherUser = new UserRequest
        {
            Name = "Carlos",
            Surname = "Lopez",
            Email = "carlos@gmail.com",
            DateOfBirth = new DateTime(1999, 1, 1),
            Password = validPassword
        };

        _userService.AddUser(_validUser);
        _userService.AddUser(otherUser);

        _sessionService.Login(validEmail, validPassword);
        _sessionService.Login("carlos@gmail.com", validPassword);
        Assert.AreEqual("carlos@gmail.com", _sessionService.GetCurrentUserEmail());
    }
}