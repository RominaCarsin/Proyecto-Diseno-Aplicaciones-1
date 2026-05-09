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
}