namespace Domain.Test;

[TestClass]
public class UserTests
{
    private const string ValidName = "Juan";
    private const string ValidSurname = "Chaves";
    private const string ValidEmail = "juan@gmail.com";
    private const string ValidPassword = "Password1!";

    private static readonly DateTime ValidDateOfBirth = new(2000, 1, 1);

    [TestMethod]
    public void CreateUser_WithValidData_ShouldCreateUserWithExpectedProperties()
    {
        var user = CreateValidUser();

        Assert.AreEqual(ValidName, user.Name);
        Assert.AreEqual(ValidSurname, user.Surname);
        Assert.AreEqual(ValidEmail, user.Email);
        Assert.AreEqual(ValidDateOfBirth, user.DateOfBirth);
    }

    [TestMethod]
    public void CreateUser_WithInitialRoles_ShouldAssignRoles()
    {
        var user = new User(ValidName, ValidSurname, ValidEmail, ValidDateOfBirth, ValidPassword, Role.Admin);

        Assert.IsTrue(user.HasRole(Role.Admin));
    }

    [TestMethod]
    public void CreateUser_ShouldStorePasswordEncryptedNotAsPlainText()
    {
        var user = CreateValidUser();

        Assert.AreNotEqual(ValidPassword, user.PasswordHash);
        Assert.IsFalse(string.IsNullOrWhiteSpace(user.PasswordHash));
        Assert.IsFalse(user.PasswordHash.Contains(ValidPassword));
    }

    [TestMethod]
    public void CreateUser_ShouldEncryptPasswordWithCaesarCipher()
    {
        var user = CreateValidUser();

        Assert.AreEqual("Sdvvzrug4$", user.PasswordHash);
    }
    
    [TestMethod]
    public void CreateUser_WithEmptyName_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new User("", ValidSurname, ValidEmail, ValidDateOfBirth, ValidPassword));
    }

    [TestMethod]
    public void CreateUser_WithWhitespaceOnlyName_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new User("   ", ValidSurname, ValidEmail, ValidDateOfBirth, ValidPassword));
    }
    
    [TestMethod]
    public void CreateUser_WithEmptySurname_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new User(ValidName, "", ValidEmail, ValidDateOfBirth, ValidPassword));
    }

    [TestMethod]
    public void CreateUser_WithWhitespaceOnlySurname_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new User(ValidName, "   ", ValidEmail, ValidDateOfBirth, ValidPassword));
    }
    
    [TestMethod]
    public void CreateUser_WithEmptyEmail_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new User(ValidName, ValidSurname, "", ValidDateOfBirth, ValidPassword));
    }

    [TestMethod]
    public void CreateUser_WithEmailMissingAtSign_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new User(ValidName, ValidSurname, "juancitoemail.com", ValidDateOfBirth, ValidPassword));
    }

    [TestMethod]
    public void CreateUser_WithEmailMissingTextBeforeAtSign_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new User(ValidName, ValidSurname, "@gmail.com", ValidDateOfBirth, ValidPassword));
    }

    [TestMethod]
    public void CreateUser_WithEmailMissingTextAfterAtSign_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new User(ValidName, ValidSurname, "juancito@", ValidDateOfBirth, ValidPassword));
    }
    
    [TestMethod]
    public void CreateUser_WithMinValueBirthDate_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new User(ValidName, ValidSurname, ValidEmail, DateTime.MinValue, ValidPassword));
    }

    [TestMethod]
    public void CreateUser_WithFutureBirthDate_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new User(ValidName, ValidSurname, ValidEmail, DateTime.Now.AddDays(1), ValidPassword));
    }

    [TestMethod]
    public void CreateUser_WithBirthDateOlderThan120Years_ShouldThrowArgumentException()
    {
        var tooOldDate = DateTime.Now.AddYears(-121);

        Assert.ThrowsException<ArgumentException>(() =>
            new User(ValidName, ValidSurname, ValidEmail, tooOldDate, ValidPassword));
    }
    
    [TestMethod]
    public void CreateUser_WithEmptyPassword_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new User(ValidName, ValidSurname, ValidEmail, ValidDateOfBirth, ""));
    }

    [TestMethod]
    public void CreateUser_WithPasswordShorterThan8Characters_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new User(ValidName, ValidSurname, ValidEmail, ValidDateOfBirth, "Pass1!"));
    }

    [TestMethod]
    public void CreateUser_WithPasswordMissingLowercaseLetter_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new User(ValidName, ValidSurname, ValidEmail, ValidDateOfBirth, "PASSWORD1!"));
    }

    [TestMethod]
    public void CreateUser_WithPasswordMissingUppercaseLetter_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new User(ValidName, ValidSurname, ValidEmail, ValidDateOfBirth, "password1!"));
    }

    [TestMethod]
    public void CreateUser_WithPasswordMissingDigit_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new User(ValidName, ValidSurname, ValidEmail, ValidDateOfBirth, "Password!"));
    }

    [TestMethod]
    public void CreateUser_WithPasswordMissingSpecialCharacter_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new User(ValidName, ValidSurname, ValidEmail, ValidDateOfBirth, "Password1"));
    }
    
    [TestMethod]
    public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
    {
        var user = CreateValidUser();

        Assert.IsTrue(user.VerifyPassword(ValidPassword));
    }

    [TestMethod]
    public void VerifyPassword_WithWrongPassword_ShouldReturnFalse()
    {
        var user = CreateValidUser();

        Assert.IsFalse(user.VerifyPassword("Password2!"));
    }

    [TestMethod]
    public void VerifyPassword_WhenPasswordHashIsEmpty_ShouldReturnFalse()
    {
        var user = CreateValidUser();
        typeof(User)
            .GetProperty(nameof(user.PasswordHash))!
            .SetValue(user, "");

        Assert.IsFalse(user.VerifyPassword(ValidPassword));
    }
    
    [TestMethod]
    public void SetPassword_WithValidPassword_ShouldReplacePasswordHash()
    {
        var user = CreateValidUser();
        var oldHash = user.PasswordHash;

        user.SetPassword("Newpass1!");

        Assert.AreNotEqual(oldHash, user.PasswordHash);
        Assert.IsTrue(user.VerifyPassword("Newpass1!"));
        Assert.IsFalse(user.VerifyPassword(ValidPassword));
    }

    [TestMethod]
    public void SetPassword_WithInvalidPassword_ShouldThrowArgumentException()
    {
        var user = CreateValidUser();

        Assert.ThrowsException<ArgumentException>(() =>
            user.SetPassword("short"));
    }
    
    [TestMethod]
    public void UpdatePersonalInfo_WithValidData_ShouldUpdateAllFields()
    {
        var user = CreateValidUser();

        user.UpdatePersonalInfo("Pedro", "Gomez", "pedro@gmail.com", new DateTime(1995, 1, 1));

        Assert.AreEqual("Pedro", user.Name);
        Assert.AreEqual("Gomez", user.Surname);
        Assert.AreEqual("pedro@gmail.com", user.Email);
        Assert.AreEqual(new DateTime(1995, 1, 1), user.DateOfBirth);
    }

    [TestMethod]
    public void UpdatePersonalInfo_WithEmptyName_ShouldThrowArgumentException()
    {
        var user = CreateValidUser();

        Assert.ThrowsException<ArgumentException>(() =>
            user.UpdatePersonalInfo("", "Gomez", "pedro@gmail.com", new DateTime(1995, 1, 1)));
    }
    
    [TestMethod]
    public void AddRole_WithSingleRole_ShouldAssignRoleToUser()
    {
        var user = CreateValidUser();

        user.AddRole(Role.Admin);

        Assert.IsTrue(user.HasRole(Role.Admin));
    }

    [TestMethod]
    public void AddRole_WithJournalistRole_ShouldAssignRoleToUser()
    {
        var user = CreateValidUser();

        user.AddRole(Role.Journalist);

        Assert.IsTrue(user.HasRole(Role.Journalist));
    }

    [TestMethod]
    public void AddRole_WithMultipleRoles_ShouldAssignAllRoles()
    {
        var user = CreateValidUser();

        user.AddRole(Role.Admin);
        user.AddRole(Role.Editor);

        Assert.IsTrue(user.HasRole(Role.Admin));
        Assert.IsTrue(user.HasRole(Role.Editor));
    }

    [TestMethod]
    public void AddRole_WithCombinedFlags_ShouldAssignAllCombinedRoles()
    {
        var user = CreateValidUser();

        user.AddRole(Role.Admin | Role.Editor);

        Assert.IsTrue(user.HasRole(Role.Admin));
        Assert.IsTrue(user.HasRole(Role.Editor));
        Assert.IsTrue(user.HasRole(Role.Admin | Role.Editor));
    }
    
    [TestMethod]
    public void RemoveRole_WithExistingRole_ShouldRemoveRoleFromUser()
    {
        var user = CreateValidUser();
        user.AddRole(Role.Admin);

        user.RemoveRole(Role.Admin);

        Assert.IsFalse(user.HasRole(Role.Admin));
    }

    [TestMethod]
    public void RemoveRole_ShouldNotAffectOtherRoles()
    {
        var user = CreateValidUser();
        user.AddRole(Role.Admin);
        user.AddRole(Role.Editor);

        user.RemoveRole(Role.Admin);

        Assert.IsFalse(user.HasRole(Role.Admin));
        Assert.IsTrue(user.HasRole(Role.Editor));
    }

    [TestMethod]
    public void RemoveRole_WhenRoleNotPresent_ShouldDoNothing()
    {
        var user = CreateValidUser();

        user.RemoveRole(Role.Admin);

        Assert.IsFalse(user.HasRole(Role.Admin));
    }
    
    [TestMethod]
    public void HasRole_WhenRoleNotAssigned_ShouldReturnFalse()
    {
        var user = CreateValidUser();

        Assert.IsFalse(user.HasRole(Role.Admin));
    }
    
    private static User CreateValidUser() =>
        new User(ValidName, ValidSurname, ValidEmail, ValidDateOfBirth, ValidPassword);
}