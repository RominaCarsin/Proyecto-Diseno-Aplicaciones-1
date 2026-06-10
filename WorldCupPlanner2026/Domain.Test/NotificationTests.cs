namespace Domain.Test;

[TestClass]
public class NotificationTests
{
    private const int ValidUserId = 1;
    private const string ValidMessage = "Resultado: ARGENTINA 2 - 1 BRAZIL";

    [TestMethod]
    public void CreateNotification_WithValidData_ShouldCreateNotificationWithExpectedProperties()
    {
        var notification = new Notification(ValidUserId, ValidMessage);

        Assert.AreEqual(ValidUserId, notification.UserId);
        Assert.AreEqual(ValidMessage, notification.Message);
        Assert.IsFalse(notification.IsRead);
        Assert.AreEqual(0, notification.MatchId);
    }

    [TestMethod]
    public void CreateNotification_WithMatchId_ShouldSetMatchId()
    {
        var notification = new Notification(ValidUserId, ValidMessage, matchId: 42);

        Assert.AreEqual(42, notification.MatchId);
    }

    [TestMethod]
    public void CreateNotification_ShouldSetCreatedAtCloseToUtcNow()
    {
        var before = DateTime.UtcNow;

        var notification = new Notification(ValidUserId, ValidMessage);

        var after = DateTime.UtcNow;
        Assert.IsTrue(notification.CreatedAt >= before && notification.CreatedAt <= after);
    }

    [TestMethod]
    public void CreateNotification_WithInvalidUserId_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Notification(0, ValidMessage));
    }

    [TestMethod]
    public void CreateNotification_WithNegativeUserId_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Notification(-1, ValidMessage));
    }

    [TestMethod]
    public void CreateNotification_WithEmptyMessage_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Notification(ValidUserId, ""));
    }

    [TestMethod]
    public void CreateNotification_WithNullMessage_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Notification(ValidUserId, null!));
    }

    [TestMethod]
    public void CreateNotification_WithWhitespaceOnlyMessage_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Notification(ValidUserId, "   "));
    }

    [TestMethod]
    public void MarkAsRead_OnUnreadNotification_ShouldSetIsReadToTrue()
    {
        var notification = new Notification(ValidUserId, ValidMessage);

        Assert.IsFalse(notification.IsRead);

        notification.MarkAsRead();

        Assert.IsTrue(notification.IsRead);
    }

    [TestMethod]
    public void MarkAsRead_OnAlreadyReadNotification_ShouldRemainRead()
    {
        var notification = new Notification(ValidUserId, ValidMessage);
        notification.MarkAsRead();

        notification.MarkAsRead();

        Assert.IsTrue(notification.IsRead);
    }
}
