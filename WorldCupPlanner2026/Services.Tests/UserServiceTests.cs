using Domain;
using DTOs;
using IRepository;
using Repository;

namespace Services.Tests;

[TestClass]
public class UserServiceTests
{
    private UserService _service;
    private IRepository<User> _repository;
    private UserRequest _validUser = new UserRequest {
        Name = "Juan",
        Surname = "Perez",
        Email = "juan@gmail.com",
        DateOfBirth = new DateTime(2000, 1, 1),
        Password = "Password1!"
    };
    private UserRequest _otherUser = new UserRequest
    {
        Name = "Carlos",
        Surname = "Lopez",
        Email = "carlos@gmail.com",
        DateOfBirth = new DateTime(1999, 1, 1),
        Password = "Password1!"
    };

    [TestInitialize]
    public void Setup()
    {
        _repository = new Repository<User>();
        _service = new UserService(_repository);
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
        _service.AddUser(adminRequest);
        
        var admin = _repository.GetAll().FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        admin!.AddRole(Role.Admin);
    }

    [TestMethod]
    public void AddUser_WithValidData_ShouldAddUser()
    {
        var result = _service.AddUser(_validUser);

        var user = _repository.GetAll().FirstOrDefault(u => u.Email == "juan@gmail.com");

        Assert.IsNotNull(user);
        Assert.AreEqual("Juan", user.Name);
        Assert.AreEqual("juan@gmail.com", result.Email);
        Assert.AreEqual("Juan", result.Name);
    }

    [TestMethod]
    public void AddUser_WithExistingEmail_ShouldThrowException()
    {
        _service.AddUser(_validUser);

        Assert.ThrowsException<ArgumentException>(() => _service.AddUser(_validUser));
    }

    [TestMethod]
    public void UpdateUser_WithValidData_ShouldUpdateUser()
    {
        _service.AddUser(_validUser);

        var updated = new UserRequest
        {
            Name = "Nuevo",
            Surname = "Apellido",
            Email = "juan@gmail.com",
            DateOfBirth = new DateTime(1998, 1, 1),
            Password = "Password1!"
        };

        _service.UpdateUser("juan@gmail.com", updated);

        var user = _repository.GetAll().FirstOrDefault(u => u.Email == "juan@gmail.com");

        Assert.IsNotNull(user);
        Assert.AreEqual("Nuevo", user.Name);
        Assert.AreEqual("Apellido", user.Surname);
    }

    [TestMethod]
    public void UpdateUser_WhenUserNotFound_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _service.UpdateUser("notfound@gmail.com", _validUser));
    }

    [TestMethod]
    public void UpdateUser_WhenEmailAlreadyInUse_ShouldThrowException()
    {
        _service.AddUser(_validUser);
        _service.AddUser(_otherUser);

        var update = new UserRequest
        {
            Name = "Juan",
            Surname = "Perez",
            Email = "carlos@gmail.com",
            DateOfBirth = DateTime.Now,
            Password = "Password1!"
        };

        Assert.ThrowsException<ArgumentException>(() =>
            _service.UpdateUser("juan@gmail.com", update));
    }

    [TestMethod]
    public void DeleteUser_WhenUserExists_ShouldRemoveUser()
    {
        _service.AddUser(_validUser);

        _service.DeleteUser("juan@gmail.com");

        var user = _repository.GetAll().FirstOrDefault(u => u.Email == "juan@gmail.com");

        Assert.IsNull(user);
    }

    [TestMethod]
    public void DeleteUser_WhenUserNotFound_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _service.DeleteUser("notfound@gmail.com"));
    }

    [TestMethod]
    public void GetUserByEmail_ShouldReturnUser()
    {
        _service.AddUser(_validUser);

        var result = _service.GetUserByEmail("juan@gmail.com");

        Assert.AreEqual("Juan", result.Name);
        Assert.AreEqual("juan@gmail.com", result.Email);
    }

    [TestMethod]
    public void GetUsers_ShouldReturnAllUsers()
    {
        _service.AddUser(_validUser);
        _service.AddUser(_otherUser);

        var result = _service.GetUsers().ToList();

        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.Any(u => u.Email == "juan@gmail.com" && u.Name == "Juan"));
        Assert.IsTrue(result.Any(u => u.Email == "carlos@gmail.com" && u.Name == "Carlos"));
    }

    [TestMethod]
    public void AddRoleToUser_ShouldAssignRole()
    {
        CreateAdminUser("admin@test.com", "Admin", "User", new DateTime(2000, 1, 1), "Password1!");
        _service.AddUser(_validUser);

        _service.AddRoleToUser("admin@test.com", "juan@gmail.com", RoleDto.Admin);
        var hasRole = _service.UserHasRole("juan@gmail.com", RoleDto.Admin);

        Assert.IsTrue(hasRole);
    }

    [TestMethod]
    public void AddRoleToUser_WhenAdminNotAuthenticated_ShouldThrowException()
    {
        _service.AddUser(_validUser);
        _service.AddUser(_otherUser);

        Assert.ThrowsException<ArgumentException>(() =>
            _service.AddRoleToUser("carlos@gmail.com", "juan@gmail.com", RoleDto.Admin));
    }

    [TestMethod]
    public void RemoveRoleFromUser_ShouldRemoveRole()
    {
        CreateAdminUser("admin@test.com", "Admin", "User", new DateTime(2000, 1, 1), "Password1!");
        _service.AddUser(_validUser);

        _service.AddRoleToUser("admin@test.com", "juan@gmail.com", RoleDto.Admin);
        _service.RemoveRoleFromUser("admin@test.com", "juan@gmail.com", RoleDto.Admin);

        var hasRole = _service.UserHasRole("juan@gmail.com", RoleDto.Admin);

        Assert.IsFalse(hasRole);
    }

    [TestMethod]
    public void RemoveRoleFromUser_WhenAdminNotAuthenticated_ShouldThrowException()
    {
        CreateAdminUser("admin@test.com", "Admin", "User", new DateTime(2000, 1, 1), "Password1!");
        _service.AddUser(_validUser);
        _service.AddUser(_otherUser);

        _service.AddRoleToUser("admin@test.com", "juan@gmail.com", RoleDto.Admin);

        Assert.ThrowsException<ArgumentException>(() =>
            _service.RemoveRoleFromUser("carlos@gmail.com", "juan@gmail.com", RoleDto.Admin));
    }

    [TestMethod]
    public void Login_WithCorrectCredentials_ShouldReturnUser()
    {
        _service.AddUser(_validUser);

        var result = _service.Login("juan@gmail.com", "Password1!");

        Assert.AreEqual("juan@gmail.com", result.Email);
    }

    [TestMethod]
    public void Login_WithWrongPassword_ShouldThrowException()
    {
        _service.AddUser(_validUser);

        Assert.ThrowsException<ArgumentException>(() =>
            _service.Login("juan@gmail.com", "WrongPassword"));
    }

    [TestMethod]
    public void ChangePassword_ShouldUpdatePassword()
    {
        _service.AddUser(_validUser);

        _service.ChangePassword("juan@gmail.com", "Password1!", "Newpass1!");

        var user = _repository.GetAll().First();

        Assert.IsTrue(user.VerifyPassword("Newpass1!"));
    }

    [TestMethod]
    public void ChangePassword_WithWrongCurrentPassword_ShouldThrowException()
    {
        _service.AddUser(_validUser);

        Assert.ThrowsException<ArgumentException>(() =>
            _service.ChangePassword("juan@gmail.com", "WrongPassword", "Newpass1!"));
    }

    [TestMethod]
    public void ResetPassword_ShouldChangePassword()
    {
        CreateAdminUser("admin@test.com", "Admin", "User", new DateTime(2000, 1, 1), "Password1!");
        _service.AddUser(_validUser);

        _service.ResetPassword("admin@test.com", "juan@gmail.com", "NewPass1!");

        var user = _repository.GetAll().First(u => u.Email == "juan@gmail.com");

        Assert.IsFalse(user.VerifyPassword("Password1!"));
        Assert.IsTrue(user.VerifyPassword("NewPass1!"));
    }

    [TestMethod]
    public void ResetPassword_WhenAdminNotAuthenticated_ShouldThrowException()
    {
        _service.AddUser(_validUser);
        _service.AddUser(_otherUser);

        Assert.ThrowsException<ArgumentException>(() =>
            _service.ResetPassword("carlos@gmail.com", "juan@gmail.com", "NewPass1!"));
    }
}

