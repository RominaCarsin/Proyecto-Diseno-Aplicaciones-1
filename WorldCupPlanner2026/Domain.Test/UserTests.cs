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
    public void CreateUser_WithEmptyPassword_ShouldThrowException()
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

    
    [TestMethod]
    public void CreateUser_ShouldStorePasswordAsHash()
    {
        var plainPassword = "Password1!";
        var user = new User("Juan", "Chaves", "jauncito@gmail.com", new DateTime(2026, 4, 12), plainPassword);

        Assert.AreNotEqual(plainPassword, user.PasswordHash);
        Assert.IsFalse(string.IsNullOrWhiteSpace(user.PasswordHash));
        Assert.IsFalse(user.PasswordHash.Contains(plainPassword));
    }


    [TestMethod]
    public void CreateUser_ShouldEncryptPasswordWithCaesarCipher()
    {
        var user = new User("Juan", "Chaves", "jauncito@gmail.com", new DateTime(2026, 4, 12), "Password1!");

        Assert.AreEqual("Sdvvzrug4$", user.PasswordHash);
    }

    [TestMethod]
    public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
    {
        var user = new User("Juan", "Chaves", "jauncito@gmail.com", new DateTime(2026, 4, 12), "Password1!");

        Assert.IsTrue(user.VerifyPassword("Password1!"));
    }

    [TestMethod]
    public void VerifyPassword_WithWrongPassword_ShouldReturnFalse()
    {
        var user = new User("Juan", "Chaves", "jauncito@gmail.com", new DateTime(2026, 4, 12), "Password1!");

        Assert.IsFalse(user.VerifyPassword("Password2!"));
    }

    [TestMethod]
    public void SetPassword_ShouldChangeHash()
    {
        var user = new User("Juan", "Chaves", "jauncito@gmail.com", new DateTime(2026, 4, 12), "Password1!");
        var oldHash = user.PasswordHash;

        user.SetPassword("Newpass1!");

        Assert.AreNotEqual(oldHash, user.PasswordHash);
        Assert.IsTrue(user.VerifyPassword("Newpass1!"));
        Assert.IsFalse(user.VerifyPassword("Password1!"));
    }
    
    [TestMethod]
    public void SetPassword_WithInvalidPassword_ShouldThrow()
    {
        var user = new User("Juan", "Chaves", "a@gmail.com", new DateTime(2000,1,1), "Password1!");

        Assert.ThrowsException<ArgumentException>(() =>
            user.SetPassword("short"));
    }

    [TestMethod]
    public void AddRole_ShouldAddRoleToUser()
    {
        var user = new User("Juan", "Chaves", "jauncito@gmail.com", new DateTime(2026, 4, 12), "Password1!");

        user.AddRole(Role.Admin);

        Assert.IsTrue(user.HasRole(Role.Admin));
    }

    [TestMethod]
    public void AddRole_ShouldAllowMultipleRoles()
    {
        var user = new User("Juan", "Chaves", "jauncito@gmail.com", new DateTime(2026, 4, 12), "Password1!");

        user.AddRole(Role.Admin);
        user.AddRole(Role.Editor);

        Assert.IsTrue(user.HasRole(Role.Admin));
        Assert.IsTrue(user.HasRole(Role.Editor));
    }

    [TestMethod]
    public void RemoveRole_ShouldRemoveRoleFromUser()
    {
        var user = new User("Juan", "Chaves", "jauncito@gmail.com", new DateTime(2026, 4, 12), "Password1!");

        user.AddRole(Role.Admin);
        user.RemoveRole(Role.Admin);

        Assert.IsFalse(user.HasRole(Role.Admin));
    }

    [TestMethod]
    public void RemoveRole_ShouldNotAffectOtherRoles()
    {
        var user = new User("Juan", "Chaves", "jauncito@gmail.com", new DateTime(2026, 4, 12), "Password1!");

        user.AddRole(Role.Admin);
        user.AddRole(Role.Editor);
        user.RemoveRole(Role.Admin);

        Assert.IsFalse(user.HasRole(Role.Admin));
        Assert.IsTrue(user.HasRole(Role.Editor));
    }

    [TestMethod]
    public void HasRole_ShouldReturnFalseWhenRoleNotAdded()
    {
        var user = new User("Juan", "Chaves", "jauncito@gmail.com", new DateTime(2026, 4, 12), "Password1!");

        Assert.IsFalse(user.HasRole(Role.Admin));
    }
    
    [TestMethod]
    public void HasRole_WithCombinedRoles_ShouldWorkCorrectly()
    {
        var user = new User("Juan", "Chaves", "a@gmail.com", new DateTime(2000,1,1), "Password1!");

        user.AddRole(Role.Admin | Role.Editor);

        Assert.IsTrue(user.HasRole(Role.Admin));
        Assert.IsTrue(user.HasRole(Role.Editor));
        Assert.IsTrue(user.HasRole(Role.Admin | Role.Editor));
    }
    
    [TestMethod]
    public void RemoveRole_WhenRoleNotPresent_ShouldDoNothing()
    {
        var user = new User("Juan", "Chaves", "a@gmail.com", new DateTime(2000,1,1), "Password1!");

        user.RemoveRole(Role.Admin);

        Assert.IsFalse(user.HasRole(Role.Admin));
    }
    
    [TestMethod]
    public void CreateUser_WithInitialRoles_ShouldAssignRoles()
    {
        var user = new User("Juan", "Chaves", "a@gmail.com", new DateTime(2000,1,1), "Password1!", Role.Admin);

        Assert.IsTrue(user.HasRole(Role.Admin));
    }
    
    [TestMethod]
    public void VerifyPassword_WithEmptyHash_ShouldReturnFalse()
    {
        var user = new User("Juan", "Chaves", "jauncito@gmail.com", new DateTime(2000, 1, 1), "Password1!");

        typeof(User)
            .GetProperty("PasswordHash")
            .SetValue(user, "");

        Assert.IsFalse(user.VerifyPassword("Password1!"));
    }
    
    [TestMethod]
    public void UpdatePersonalInfo_ShouldUpdateFields()
    {
        var user = new User("Juan", "Chaves", "a@gmail.com", new DateTime(2000,1,1), "Password1!");

        user.UpdatePersonalInfo("Pedro", "Gomez", "b@gmail.com", new DateTime(1995,1,1));

        Assert.AreEqual("Pedro", user.Name);
        Assert.AreEqual("Gomez", user.Surname);
        Assert.AreEqual("b@gmail.com", user.Email);
    }
    
    [TestMethod]
    public void UpdatePersonalInfo_WithInvalidName_ShouldThrow()
    {
        var user = new User("Juan", "Chaves", "a@gmail.com", new DateTime(2000,1,1), "Password1!");

        Assert.ThrowsException<ArgumentException>(() =>
            user.UpdatePersonalInfo("", "Gomez", "b@gmail.com", new DateTime(1995,1,1)));
    }
    
}