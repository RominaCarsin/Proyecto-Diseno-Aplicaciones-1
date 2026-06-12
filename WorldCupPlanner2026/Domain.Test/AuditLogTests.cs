namespace Domain.Test;

[TestClass]
public class AuditLogTests
{
    private const string ValidActorEmail = "admin@test.com";
    private const string ValidAction = "User created";
    private const string ValidDetails = "juan@test.com";
    
    [TestMethod]
    public void CreateAuditLog_WithValidData_ShouldCreateLogWithExpectedProperties()
    {
        var log = new AuditLog(ValidActorEmail, ValidAction, ValidDetails);

        Assert.AreEqual(ValidActorEmail, log.ActorEmail);
        Assert.AreEqual(ValidAction, log.Action);
        Assert.AreEqual(ValidDetails, log.Details);
    }

    [TestMethod]
    public void CreateAuditLog_ShouldSetTimestampCloseToUtcNow()
    {
        var before = DateTime.UtcNow;

        var log = new AuditLog(ValidActorEmail, ValidAction, ValidDetails);

        var after = DateTime.UtcNow;
        Assert.IsTrue(log.Timestamp >= before && log.Timestamp <= after);
    }

    [TestMethod]
    public void CreateAuditLog_WithEmptyActorEmail_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new AuditLog("", ValidAction, ValidDetails));
    }

    [TestMethod]
    public void CreateAuditLog_WithNullActorEmail_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new AuditLog(null!, ValidAction, ValidDetails));
    }

    [TestMethod]
    public void CreateAuditLog_WithWhitespaceOnlyActorEmail_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new AuditLog("   ", ValidAction, ValidDetails));
    }
    
    [TestMethod]
    public void CreateAuditLog_WithEmptyAction_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new AuditLog(ValidActorEmail, "", ValidDetails));
    }

    [TestMethod]
    public void CreateAuditLog_WithNullAction_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new AuditLog(ValidActorEmail, null!, ValidDetails));
    }

    [TestMethod]
    public void CreateAuditLog_WithWhitespaceOnlyAction_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new AuditLog(ValidActorEmail, "   ", ValidDetails));
    }
}