namespace Domain.Test;

[TestClass]
public class AuditLogTests
{
    [TestMethod]
    public void CreateAuditLog_WithValidData_ShouldSucceed()
    {
        var log = new AuditLog("admin@test.com", "user created", "juan@test.com");
        
        Assert.IsNotNull(log);
        Assert.AreEqual("admin@test.com",log.ActorEmail);
        Assert.AreEqual("user created",log.Action);
        Assert.AreEqual("juan@test.com", log.Details);
    }

    [TestMethod]
    public void CreateAuditLog_WithEmptyActorEmail_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => new AuditLog("", "user created", "juan@test.com"));
    }
    
    [TestMethod]
    public void CreateAuditLog_WithEmptyAction_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => new AuditLog("admin@test.com", "", "juan@test.com"));
    }

    [TestMethod]
    public void CreateAuditLog_Timestamp_ShouldBeCloseToNow()
    {
        var before = DateTime.UtcNow;
        var log    = new AuditLog("admin@test.com", "User created", "juan@test.com");
        var after  = DateTime.UtcNow;

        Assert.IsTrue(log.Timestamp >= before && log.Timestamp <= after);
    }
    
    
}