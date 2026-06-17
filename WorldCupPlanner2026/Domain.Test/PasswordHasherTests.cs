namespace Domain.Test;

[TestClass]
public class PasswordHasherTests
{
    private static readonly IPasswordHasher Hasher = new PasswordHasher();

    [TestMethod]
    public void Hash_WithValidPassword_ShouldReturnCaesarCipher()
    {
        var hash = Hasher.Hash("Password1!");

        Assert.AreEqual("Sdvvzrug4$", hash);
    }

    [TestMethod]
    public void Verify_WithCorrectPassword_ShouldReturnTrue()
    {
        var hash = Hasher.Hash("Password1!");

        Assert.IsTrue(Hasher.Verify("Password1!", hash));
    }

    [TestMethod]
    public void Verify_WithWrongPassword_ShouldReturnFalse()
    {
        var hash = Hasher.Hash("Password1!");

        Assert.IsFalse(Hasher.Verify("WrongPassword", hash));
    }

    [TestMethod]
    public void Verify_WhenPasswordHashIsEmpty_ShouldReturnFalse()
    {
        Assert.IsFalse(Hasher.Verify("Password1!", ""));
    }

    [TestMethod]
    public void Hash_WithDifferentInputs_ShouldProduceDifferentHashes()
    {
        var hash1 = Hasher.Hash("Password1!");
        var hash2 = Hasher.Hash("Password2!");

        Assert.AreNotEqual(hash1, hash2);
    }
}
