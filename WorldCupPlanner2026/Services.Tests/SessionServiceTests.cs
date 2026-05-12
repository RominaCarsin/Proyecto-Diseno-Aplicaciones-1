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
        _repository = new Repository<User>();
        var auditRepo    = new Repository<AuditLog>();
        var auditService = new AuditService(auditRepo);
        var authState = new AuthStateService();
        _userService = new UserService(_repository, auditService);
        _sessionService = new SessionService(_userService, authState);
    }

    private void CreateAdminUser(string email, string name, string surname, DateTime dob, string password)
    {
        var adminRequest = new UserRequest
        {
            Name = name,
            Surname = surname,
            Email = email,
            DateOfBirth = dob,
            Password = password
        };
        _userService.AddUser(adminRequest);

        var admin = _repository.GetAll().FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        admin!.AddRole(Role.Admin);
    }

    private void CreateEditorUser(string email, string name, string surname, DateTime dob, string password)
    {
        var editorRequest = new UserRequest
        {
            Name = name,
            Surname = surname,
            Email = email,
            DateOfBirth = dob,
            Password = password
        };
        _userService.AddUser(editorRequest);

        var editor = _repository.GetAll().FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        editor!.AddRole(Role.Editor);
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
    public void Login_WhenAlreadyLoggedIn_ShouldOverwriteCurrentUser()
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

    [TestMethod]
    public void CurrentUserHasRole_WhenNotLoggedIn_ReturnsFalse()
    {
        Assert.IsFalse(_sessionService.CurrentUserHasRole(RoleDto.Admin));
    }

    [TestMethod]
    public void CurrentUserHasRole_WhenUserHasRole_ReturnsTrue()
    {
        CreateAdminUser("adminrole@test.com", "Admin", "User", new DateTime(2000, 1, 1), "Password1!");
        _sessionService.Login("adminrole@test.com", "Password1!");

        Assert.IsTrue(_sessionService.CurrentUserHasRole(RoleDto.Admin));
    }

    [TestMethod]
    public void CurrentUserHasRole_WhenUserDoesNotHaveRole_ReturnsFalse()
    {
        CreateEditorUser("editorrole@test.com", "Editor", "User", new DateTime(2000, 1, 1), "Password1!");
        _sessionService.Login("editorrole@test.com", "Password1!");

        Assert.IsFalse(_sessionService.CurrentUserHasRole(RoleDto.Admin));
    }

    [TestMethod]
    public void CurrentUserHasRole_WhenUserHasMultipleRoles_ReturnsTrueForEach()
    {
        CreateAdminUser("bothroles@test.com", "Admin", "Editor", new DateTime(2000, 1, 1), "Password1!");
        var user = _repository.GetAll().FirstOrDefault(u => u.Email.Equals("bothroles@test.com", StringComparison.OrdinalIgnoreCase));
        user!.AddRole(Role.Editor);

        _sessionService.Login("bothroles@test.com", "Password1!");

        Assert.IsTrue(_sessionService.CurrentUserHasRole(RoleDto.Admin));
        Assert.IsTrue(_sessionService.CurrentUserHasRole(RoleDto.Editor));
    }
}