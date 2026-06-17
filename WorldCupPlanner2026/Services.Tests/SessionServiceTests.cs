using Domain;
using DTOs;
using IRepository;
using Repository;

namespace Services.Tests;

[TestClass]
public class SessionServiceTests
{
    private const string ValidEmail = "juan@gmail.com";
    private const string ValidPassword = "Password1!";
    private const string ValidName = "Juan";
    private const string ValidSurname = "Perez";

    private static readonly DateTime ValidDateOfBirth = new(2000, 1, 1);

    private static readonly IPasswordHasher Hasher = new PasswordHasher();

    private UserService _userService = null!;
    private SessionService _sessionService = null!;
    private IRepository<User> _repository = null!;

    [TestInitialize]
    public void Setup()
    {
        _repository = new FakeRepository<User>();
        var auditRepository = new FakeRepository<AuditLog>();
        var auditService = new AuditService(auditRepository);
        var authState = new AuthStateService();

        _userService = new UserService(_repository, auditService, Hasher);
        _sessionService = new SessionService(_userService, authState);
    }

    [TestMethod]
    public void IsLoggedIn_WhenNoLoginHasOccurred_ShouldReturnFalse()
    {
        Assert.IsFalse(_sessionService.IsLoggedIn());
    }

    [TestMethod]
    public void IsLoggedIn_AfterSuccessfulLogin_ShouldReturnTrue()
    {
        _userService.AddUser(BuildValidUserRequest());
        _sessionService.Login(ValidEmail, ValidPassword);

        Assert.IsTrue(_sessionService.IsLoggedIn());
    }

    [TestMethod]
    public void IsLoggedIn_AfterLogout_ShouldReturnFalse()
    {
        _userService.AddUser(BuildValidUserRequest());
        _sessionService.Login(ValidEmail, ValidPassword);

        _sessionService.Logout();

        Assert.IsFalse(_sessionService.IsLoggedIn());
    }
    
    [TestMethod]
    public void Login_WithNonExistingEmail_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _sessionService.Login("notfound@gmail.com", ValidPassword));
    }

    [TestMethod]
    public void Login_WithWrongPassword_ShouldThrowArgumentException()
    {
        _userService.AddUser(BuildValidUserRequest());

        Assert.ThrowsException<ArgumentException>(() =>
            _sessionService.Login(ValidEmail, "WrongPass1!"));
    }

    [TestMethod]
    public void Login_WithWrongPassword_ShouldNotSetCurrentUser()
    {
        _userService.AddUser(BuildValidUserRequest());
        try { _sessionService.Login(ValidEmail, "WrongPass1!"); } catch { }

        Assert.IsFalse(_sessionService.IsLoggedIn());
    }

    [TestMethod]
    public void Login_WhenAlreadyLoggedIn_ShouldOverwriteCurrentUser()
    {
        var otherUserRequest = BuildUserRequest(
            name: "Carlos",
            surname: "Lopez",
            email: "carlos@gmail.com");

        _userService.AddUser(BuildValidUserRequest());
        _userService.AddUser(otherUserRequest);

        _sessionService.Login(ValidEmail, ValidPassword);
        _sessionService.Login("carlos@gmail.com", ValidPassword);

        Assert.AreEqual("carlos@gmail.com", _sessionService.GetCurrentUserEmail());
    }
    
    [TestMethod]
    public void Logout_WhenNotLoggedIn_ShouldNotThrow()
    {
        _sessionService.Logout();

        Assert.IsFalse(_sessionService.IsLoggedIn());
    }

    [TestMethod]
    public void Logout_ShouldClearCurrentUser()
    {
        _userService.AddUser(BuildValidUserRequest());
        _sessionService.Login(ValidEmail, ValidPassword);

        _sessionService.Logout();

        Assert.IsNull(_sessionService.GetCurrentUser());
    }
    
    [TestMethod]
    public void GetCurrentUserEmail_WhenNotLoggedIn_ShouldReturnNull()
    {
        Assert.IsNull(_sessionService.GetCurrentUserEmail());
    }

    [TestMethod]
    public void GetCurrentUserEmail_AfterSuccessfulLogin_ShouldReturnEmail()
    {
        _userService.AddUser(BuildValidUserRequest());
        _sessionService.Login(ValidEmail, ValidPassword);

        Assert.AreEqual(ValidEmail, _sessionService.GetCurrentUserEmail());
    }

    [TestMethod]
    public void GetCurrentUserEmail_AfterLogout_ShouldReturnNull()
    {
        _userService.AddUser(BuildValidUserRequest());
        _sessionService.Login(ValidEmail, ValidPassword);
        _sessionService.Logout();

        Assert.IsNull(_sessionService.GetCurrentUserEmail());
    }
    
    [TestMethod]
    public void GetCurrentUser_WhenNotLoggedIn_ShouldReturnNull()
    {
        Assert.IsNull(_sessionService.GetCurrentUser());
    }

    [TestMethod]
    public void GetCurrentUser_AfterSuccessfulLogin_ShouldReturnUserWithExpectedProperties()
    {
        _userService.AddUser(BuildValidUserRequest());
        _sessionService.Login(ValidEmail, ValidPassword);

        var result = _sessionService.GetCurrentUser();

        Assert.IsNotNull(result);
        Assert.AreEqual(ValidEmail, result.Email);
        Assert.AreEqual(ValidName, result.Name);
    }

    [TestMethod]
    public void GetCurrentUser_AfterLogout_ShouldReturnNull()
    {
        _userService.AddUser(BuildValidUserRequest());
        _sessionService.Login(ValidEmail, ValidPassword);
        _sessionService.Logout();

        Assert.IsNull(_sessionService.GetCurrentUser());
    }
    
    [TestMethod]
    public void CurrentUserHasRole_WhenNotLoggedIn_ShouldReturnFalse()
    {
        Assert.IsFalse(_sessionService.CurrentUserHasRole(RoleDto.Admin));
    }

    [TestMethod]
    public void CurrentUserHasRole_WhenUserHasRole_ShouldReturnTrue()
    {
        CreateUserWithRole("admin@test.com", Role.Admin);
        _sessionService.Login("admin@test.com", ValidPassword);

        Assert.IsTrue(_sessionService.CurrentUserHasRole(RoleDto.Admin));
    }

    [TestMethod]
    public void CurrentUserHasRole_WhenUserIsJournalist_ShouldReturnTrueOnlyForJournalist()
    {
        CreateUserWithRole("Journalist@test.com", Role.Journalist);
        _sessionService.Login("Journalist@test.com", ValidPassword);

        Assert.IsTrue(_sessionService.CurrentUserHasRole(RoleDto.Journalist));
        Assert.IsFalse(_sessionService.CurrentUserHasRole(RoleDto.Admin));
        Assert.IsFalse(_sessionService.CurrentUserHasRole(RoleDto.Editor));
    }

    [TestMethod]
    public void CurrentUserHasRole_WhenUserDoesNotHaveRole_ShouldReturnFalse()
    {
        CreateUserWithRole("editor@test.com", Role.Editor);
        _sessionService.Login("editor@test.com", ValidPassword);

        Assert.IsFalse(_sessionService.CurrentUserHasRole(RoleDto.Admin));
    }

    [TestMethod]
    public void CurrentUserHasRole_WhenUserHasMultipleRoles_ShouldReturnTrueForEach()
    {
        CreateUserWithRole("both@test.com", Role.Admin);
        var user = FindUserByEmail("both@test.com");
        user!.AddRole(Role.Editor);
        _sessionService.Login("both@test.com", ValidPassword);

        Assert.IsTrue(_sessionService.CurrentUserHasRole(RoleDto.Admin));
        Assert.IsTrue(_sessionService.CurrentUserHasRole(RoleDto.Editor));
    }

    [TestMethod]
    public void Login_AsJournalist_ShouldSetCurrentUserWithJournalistRole()
    {
        CreateUserWithRole("journalist@test.com", Role.Journalist);

        _sessionService.Login("journalist@test.com", ValidPassword);

        Assert.IsTrue(_sessionService.IsLoggedIn());
        Assert.AreEqual("journalist@test.com", _sessionService.GetCurrentUserEmail());
        Assert.IsTrue(_sessionService.CurrentUserHasRole(RoleDto.Journalist));
        Assert.IsFalse(_sessionService.CurrentUserHasRole(RoleDto.Editor));
        Assert.IsFalse(_sessionService.CurrentUserHasRole(RoleDto.Admin));
    }

    [TestMethod]
    public void GetCurrentUser_AsJournalist_ShouldReturnUserWithJournalistRole()
    {
        CreateUserWithRole("journalist@test.com", Role.Journalist);
        _sessionService.Login("journalist@test.com", ValidPassword);

        var current = _sessionService.GetCurrentUser();

        Assert.IsNotNull(current);
        Assert.IsTrue(current.Roles.HasFlag(RoleDto.Journalist));
        Assert.IsFalse(current.Roles.HasFlag(RoleDto.Editor));
        Assert.IsFalse(current.Roles.HasFlag(RoleDto.Admin));
    }

    [TestMethod]
    public void CurrentUserHasRole_WhenJournalistTriesEditorAction_ShouldReturnFalse()
    {
        CreateUserWithRole("journalist@test.com", Role.Journalist);
        _sessionService.Login("journalist@test.com", ValidPassword);

        Assert.IsFalse(_sessionService.CurrentUserHasRole(RoleDto.Editor));
    }
    
    private static UserRequest BuildValidUserRequest() =>
        BuildUserRequest(ValidName, ValidSurname, ValidEmail);

    private static UserRequest BuildUserRequest(
        string name,
        string surname,
        string email,
        string password = ValidPassword) =>
        new UserRequest
        {
            Name = name,
            Surname = surname,
            Email = email,
            DateOfBirth = ValidDateOfBirth,
            Password = password
        };

    private void CreateUserWithRole(string email, Role role)
    {
        var request = BuildUserRequest(name: "Test", surname: "User", email: email);
        _userService.AddUser(request);

        var user = FindUserByEmail(email);
        user!.AddRole(role);
    }

    private User? FindUserByEmail(string email) =>
        _repository.GetAll()
            .FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
}