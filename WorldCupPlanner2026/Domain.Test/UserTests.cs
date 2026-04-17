using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Domain.Test;

[TestClass]
public class UserTests
{
    [TestMethod]
    public void CreateUser_withValidData_ShouldSucced()
    {
        var user = new User("Juan", "Chaves", "jauncito@gmail.com", new DateTime(2026, 4, 12), "Password1!");
        Assert.IsNotNull(user);
        Assert.AreEqual("Juan",user.Name);
    }

    [TestMethod]
    public void CreateUser_withInvalidData_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new User("", "Chaves", "jauncito@gmail.com", new DateTime(2026, 4, 12), "Password1!"));
    }

    [TestMethod]
    public void CreateUser_withInvalidSurname_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new User("Juan", "", "jauncito@gmail.com", new DateTime(2026, 4, 12), "Password1!"));

    }
    
    [TestMethod]
    public void CreateUser_withInvalidEmail_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new User("Juan", "Chaves", "", new DateTime(2026, 4, 12), "Password1!"));
    }
    
    [TestMethod]
    public void CreateUser_withEmailWithoutAt_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new User("Juan", "Chaves", "juancitoemail.com",  new DateTime(2026, 4, 12), "Password1!"));
    }

    [TestMethod]
    public void CreateUser_withNothingBeforeAt_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new User("Juan", "Chaves", "@gmail.com",  new DateTime(2026, 4, 12), "Password1!"));
    }

    [TestMethod]
    public void CreateUser_withNothingAfterAt_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new User("Juan", "Chaves", "juancito@", new DateTime(2026, 4, 12), "Password1!"));
    }

    [TestMethod]
    public void CreateUser_withDefaultDate_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new User("Juan", "Chaves", "jauncito@gmail.com", DateTime.MinValue, "Password1!"));
    }

    [TestMethod]
    public void CreateUser_withFutureDate_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new User("Juan", "Chaves", "jauncito@gmail.com", DateTime.Now.AddDays(1), "Password1!"));
    }
    
    [TestMethod]
    public void CreateUser_withTooOldDate_ShouldThrowException()
    {
        var tooOldDate = DateTime.Now.AddYears(-121);

        Assert.ThrowsException<ArgumentException>(() =>
            new User("Juan", "Chaves", "jauncito@gmail.com", tooOldDate, "Password1!"));
    }
    
    [TestMethod]
    public void CreateUser_WithEmptyPassword_ShouldTrowException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new User("Juan", "Chaves", "jauncito@gmail.com", new DateTime(2026, 4, 12), ""));
    }
    
    [TestMethod]
    public void CreateUser_withPasswordShorterThan8Characters_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new User("Juan", "Chaves", "jauncito@gmail.com", new DateTime(2026, 4, 12), "Passwor"));
    }
    [TestMethod]
    public void CreateUser_withPasswordWithoutLowercase_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new User("Juan", "Chaves", "jauncito@gmail.com", new DateTime(2026, 4, 12), "PASSWORD1!"));
    }

    [TestMethod]
    public void CreateUser_withPasswordWithoutUppercase_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new User("Juan", "Chaves", "jauncito@gmail.com", new DateTime(2026, 4, 12), "password1!"));
    }

    [TestMethod]
    public void CreateUser_withPasswordWithoutNumber_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new User("Juan", "Chaves", "jauncito@gmail.com", new DateTime(2026, 4, 12), "Password!"));
    }

    [TestMethod]
    public void CreateUser_withPasswordWithoutSpecialCharacter_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new User("Juan", "Chaves", "jauncito@gmail.com", new DateTime(2026, 4, 12), "Password1"));
    }

    [TestMethod]
    public void CreateUser_withValidPassword_ShouldCreateUser()
    {
        var user = new User("Juan", "Chaves", "jauncito@gmail.com", new DateTime(2026, 4, 12), "Password1!");

        Assert.IsNotNull(user);
    }
}