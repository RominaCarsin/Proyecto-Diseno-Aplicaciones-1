using Domain;
using DTOs;
using IRepository;
using Repository;

namespace Services.Tests;

[TestClass]
public class UserServiceTests
{
    private const string ValidEmail = "juan@gmail.com";
    private const string ValidPassword = "Password1!";
    private const string ValidName = "Juan";
    private const string ValidSurname = "Perez";

    private const string OtherEmail = "carlos@gmail.com";
    private const string OtherName = "Carlos";
    private const string OtherSurname = "Lopez";

    private const string AdminEmail = "admin@test.com";

    private static readonly DateTime ValidDateOfBirth = new(2000, 1, 1);

    private UserService _service = null!;
    private IRepository<User> _repository = null!;

    [TestInitialize]
    public void Setup()
    {
        _repository = new FakeRepository<User>();
        var auditRepository = new FakeRepository<AuditLog>();
        var auditService = new AuditService(auditRepository);
        _service = new UserService(_repository, auditService);
    }

    [TestMethod]
    public void AddUser_WithValidData_ShouldPersistUserWithExpectedProperties()
    {
        _service.AddUser(BuildValidUserRequest());

        var user = FindUserByEmail(ValidEmail);
        Assert.IsNotNull(user);
        Assert.AreEqual(ValidName, user.Name);
        Assert.AreEqual(ValidEmail, user.Email);
    }

    [TestMethod]
    public void AddUser_WithValidData_ShouldReturnMappedResponse()
    {
        var result = _service.AddUser(BuildValidUserRequest());

        Assert.AreEqual(ValidEmail, result.Email);
        Assert.AreEqual(ValidName, result.Name);
    }

    [TestMethod]
    public void AddUser_WithExistingEmail_ShouldThrowArgumentException()
    {
        _service.AddUser(BuildValidUserRequest());

        Assert.ThrowsException<ArgumentException>(() =>
            _service.AddUser(BuildValidUserRequest()));
    }
    
    [TestMethod]
    public void UpdateUser_WithValidData_ShouldUpdateUserProperties()
    {
        _service.AddUser(BuildValidUserRequest());
        var updatedRequest = BuildUserRequest(
            name: "Nuevo",
            surname: "Apellido",
            email: ValidEmail);

        _service.UpdateUser(ValidEmail, updatedRequest);

        var user = FindUserByEmail(ValidEmail);
        Assert.IsNotNull(user);
        Assert.AreEqual("Nuevo", user.Name);
        Assert.AreEqual("Apellido", user.Surname);
    }

    [TestMethod]
    public void UpdateUser_WithNonExistingEmail_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _service.UpdateUser("notfound@gmail.com", BuildValidUserRequest()));
    }

    [TestMethod]
    public void UpdateUser_WithEmailAlreadyTakenByAnotherUser_ShouldThrowArgumentException()
    {
        _service.AddUser(BuildValidUserRequest());
        _service.AddUser(BuildUserRequest(OtherName, OtherSurname, OtherEmail));
        var requestWithTakenEmail = BuildUserRequest(
            name: ValidName,
            surname: ValidSurname,
            email: OtherEmail);

        Assert.ThrowsException<ArgumentException>(() =>
            _service.UpdateUser(ValidEmail, requestWithTakenEmail));
    }
    
    [TestMethod]
    public void DeleteUser_WithExistingUser_ShouldRemoveUserFromRepository()
    {
        _service.AddUser(BuildValidUserRequest());

        _service.DeleteUser(ValidEmail);

        Assert.IsNull(FindUserByEmail(ValidEmail));
    }

    [TestMethod]
    public void DeleteUser_WithNonExistingEmail_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _service.DeleteUser("notfound@gmail.com"));
    }
    
    [TestMethod]
    public void GetUserByEmail_WithExistingEmail_ShouldReturnUserWithExpectedProperties()
    {
        _service.AddUser(BuildValidUserRequest());

        var result = _service.GetUserByEmail(ValidEmail);

        Assert.AreEqual(ValidName, result.Name);
        Assert.AreEqual(ValidEmail, result.Email);
    }
    
    [TestMethod]
    public void GetUsers_WithMultipleUsers_ShouldReturnAllUsers()
    {
        _service.AddUser(BuildValidUserRequest());
        _service.AddUser(BuildUserRequest(OtherName, OtherSurname, OtherEmail));

        var result = _service.GetUsers().ToList();

        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.Any(u => u.Email == ValidEmail && u.Name == ValidName));
        Assert.IsTrue(result.Any(u => u.Email == OtherEmail && u.Name == OtherName));
    }
    
    [TestMethod]
    public void Login_WithCorrectCredentials_ShouldReturnUserResponse()
    {
        _service.AddUser(BuildValidUserRequest());

        var result = _service.Login(ValidEmail, ValidPassword);

        Assert.AreEqual(ValidEmail, result.Email);
    }

    [TestMethod]
    public void Login_WithWrongPassword_ShouldThrowArgumentException()
    {
        _service.AddUser(BuildValidUserRequest());

        Assert.ThrowsException<ArgumentException>(() =>
            _service.Login(ValidEmail, "WrongPassword"));
    }
    
    [TestMethod]
    public void AddRoleToUser_WithAdminPerformingAction_ShouldAssignRoleToTargetUser()
    {
        CreateAdminUser();
        _service.AddUser(BuildValidUserRequest());

        _service.AddRoleToUser(AdminEmail, ValidEmail, RoleDto.Admin);

        Assert.IsTrue(_service.UserHasRole(ValidEmail, RoleDto.Admin));
    }

    [TestMethod]
    public void AddRoleToUser_WithNonAdminPerformingAction_ShouldThrowArgumentException()
    {
        _service.AddUser(BuildValidUserRequest());
        _service.AddUser(BuildUserRequest(OtherName, OtherSurname, OtherEmail));

        Assert.ThrowsException<ArgumentException>(() =>
            _service.AddRoleToUser(OtherEmail, ValidEmail, RoleDto.Admin));
    }
    
    [TestMethod]
    public void RemoveRoleFromUser_WithAdminPerformingAction_ShouldRemoveRoleFromTargetUser()
    {
        CreateAdminUser();
        _service.AddUser(BuildValidUserRequest());
        _service.AddRoleToUser(AdminEmail, ValidEmail, RoleDto.Admin);

        _service.RemoveRoleFromUser(AdminEmail, ValidEmail, RoleDto.Admin);

        Assert.IsFalse(_service.UserHasRole(ValidEmail, RoleDto.Admin));
    }

    [TestMethod]
    public void RemoveRoleFromUser_WithNonAdminPerformingAction_ShouldThrowArgumentException()
    {
        CreateAdminUser();
        _service.AddUser(BuildValidUserRequest());
        _service.AddUser(BuildUserRequest(OtherName, OtherSurname, OtherEmail));
        _service.AddRoleToUser(AdminEmail, ValidEmail, RoleDto.Admin);

        Assert.ThrowsException<ArgumentException>(() =>
            _service.RemoveRoleFromUser(OtherEmail, ValidEmail, RoleDto.Admin));
    }
    
    [TestMethod]
    public void ChangePassword_WithCorrectCurrentPassword_ShouldUpdatePassword()
    {
        _service.AddUser(BuildValidUserRequest());

        _service.ChangePassword(ValidEmail, ValidPassword, "Newpass1!");

        var user = _repository.GetAll().First();
        Assert.IsTrue(user.VerifyPassword("Newpass1!"));
    }

    [TestMethod]
    public void ChangePassword_WithWrongCurrentPassword_ShouldThrowArgumentException()
    {
        _service.AddUser(BuildValidUserRequest());

        Assert.ThrowsException<ArgumentException>(() =>
            _service.ChangePassword(ValidEmail, "WrongPassword", "Newpass1!"));
    }
    
    [TestMethod]
    public void ResetPassword_WithAdminPerformingAction_ShouldReplaceTargetUserPassword()
    {
        CreateAdminUser();
        _service.AddUser(BuildValidUserRequest());

        _service.ResetPassword(AdminEmail, ValidEmail, "NewPass1!");

        var user = FindUserByEmail(ValidEmail);
        Assert.IsFalse(user!.VerifyPassword(ValidPassword));
        Assert.IsTrue(user.VerifyPassword("NewPass1!"));
    }

    [TestMethod]
    public void ResetPassword_WithNonAdminPerformingAction_ShouldThrowArgumentException()
    {
        _service.AddUser(BuildValidUserRequest());
        _service.AddUser(BuildUserRequest(OtherName, OtherSurname, OtherEmail));

        Assert.ThrowsException<ArgumentException>(() =>
            _service.ResetPassword(OtherEmail, ValidEmail, "NewPass1!"));
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

    private void CreateAdminUser()
    {
        var adminRequest = BuildUserRequest(
            name: "Admin",
            surname: "User",
            email: AdminEmail);

        _service.AddUser(adminRequest);

        var admin = FindUserByEmail(AdminEmail);
        admin!.AddRole(Role.Admin);
    }

    private User? FindUserByEmail(string email) =>
        _repository.GetAll()
            .FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
}