using Domain;
using DTOs;
using IServices;
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

    [TestMethod]
    public void AddUser_WithValidData_ShouldAddUser()
    {
        _service.AddUser(_validUser);

        var user = _repository.GetAll().FirstOrDefault(u => u.Email == "juan@gmail.com");

        Assert.IsNotNull(user);
        Assert.AreEqual("Juan", user.Name);
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

    
    [Ignore("UserService methods not implemented yet")]
    [TestMethod]
    public void GetUserByEmail_ShouldReturnUser()
    {
        _service.AddUser(_validUser);

        var result = _service.GetUserByEmail("juan@gmail.com");

        Assert.AreEqual("Juan", result.Name);
        Assert.AreEqual("juan@gmail.com", result.Email);
    }

    [Ignore("UserService methods not implemented yet")]
    [TestMethod]
    public void GetUsers_ShouldReturnAllUsers()
    {
        _service.AddUser(_validUser);
        _service.AddUser(_otherUser);

        var result = _service.GetUsers().ToList();

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void AddRoleToUser_ShouldAssignRole()
    {
        _service.AddUser(_validUser);

        _service.AddRoleToUser("juan@gmail.com", RoleDto.Admin);

        var hasRole = _service.UserHasRole("juan@gmail.com", RoleDto.Admin);

        Assert.IsTrue(hasRole);
    }

    [TestMethod]
    public void RemoveRoleFromUser_ShouldRemoveRole()
    {
        _service.AddUser(_validUser);

        _service.AddRoleToUser("juan@gmail.com", RoleDto.Admin);
        _service.RemoveRoleFromUser("juan@gmail.com", RoleDto.Admin);

        var hasRole = _service.UserHasRole("juan@gmail.com", RoleDto.Admin);

        Assert.IsFalse(hasRole);
    }

    [Ignore("UserService methods not implemented yet")]
    [TestMethod]
    public void Login_WithCorrectCredentials_ShouldReturnUser()
    {
        _service.AddUser(_validUser);

        var result = _service.Login("juan@gmail.com", "Password1!");

        Assert.AreEqual("juan@gmail.com", result.Email);
    }

    [Ignore("UserService methods not implemented yet")]
    [TestMethod]
    public void Login_WithWrongPassword_ShouldThrowException()
    {
        _service.AddUser(_validUser);

        Assert.ThrowsException<ArgumentException>(() =>
            _service.Login("juan@gmail.com", "WrongPassword"));
    }

    [Ignore("UserService methods not implemented yet")]
    [TestMethod]
    public void ChangePassword_ShouldUpdatePassword()
    {
        _service.AddUser(_validUser);

        _service.ChangePassword("juan@gmail.com", "Newpass1!");

        var user = _repository.GetAll().First();

        Assert.IsTrue(user.VerifyPassword("Newpass1!"));
    }

    [Ignore("UserService methods not implemented yet")]
    [TestMethod]
    public void ResetPassword_ShouldChangePassword()
    {
        _service.AddUser(_validUser);

        _service.ResetPassword("juan@gmail.com");

        var user = _repository.GetAll().First();

        Assert.IsFalse(user.VerifyPassword("Password1!"));
    }   

}
