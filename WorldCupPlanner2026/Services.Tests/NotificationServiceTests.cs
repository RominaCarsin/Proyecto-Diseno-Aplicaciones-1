using Domain;
using IRepository;
using IServices;
using Services;

namespace Services.Tests;

[TestClass]
public class NotificationServiceTests
{
    private const string HomeTeamName = "ARGENTINA";
    private const string AwayTeamName = "BRAZIL";

    private INotificationService _service = null!;
    private IRepository<Notification> _notificationRepository = null!;
    private IRepository<User> _userRepository = null!;

    [TestInitialize]
    public void Setup()
    {
        _notificationRepository = new FakeRepository<Notification>();
        _userRepository = new FakeRepository<User>();
        _service = new NotificationService(_notificationRepository, _userRepository);
    }

    // ─── NotifyMatchResult ───

    [TestMethod]
    public void NotifyMatchResult_WithJournalists_ShouldCreateNotificationForEachJournalist()
    {
        AddJournalist("journalist1@test.com");
        AddJournalist("journalist2@test.com");

        _service.NotifyMatchResult(1, HomeTeamName, AwayTeamName, 2, 1);

        var notifications = _notificationRepository.GetAll().ToList();
        Assert.AreEqual(2, notifications.Count);
    }

    [TestMethod]
    public void NotifyMatchResult_WithNoJournalists_ShouldNotCreateNotifications()
    {
        AddEditor("editor@test.com");

        _service.NotifyMatchResult(1, HomeTeamName, AwayTeamName, 2, 1);

        var notifications = _notificationRepository.GetAll().ToList();
        Assert.AreEqual(0, notifications.Count);
    }

    [TestMethod]
    public void NotifyMatchResult_ShouldCreateNotificationWithCorrectMessage()
    {
        AddJournalist("journalist@test.com");

        _service.NotifyMatchResult(5, HomeTeamName, AwayTeamName, 3, 0);

        var notification = _notificationRepository.GetAll().First();
        Assert.IsTrue(notification.Message.Contains(HomeTeamName));
        Assert.IsTrue(notification.Message.Contains(AwayTeamName));
        Assert.IsTrue(notification.Message.Contains("3"));
        Assert.IsTrue(notification.Message.Contains("0"));
    }

    [TestMethod]
    public void NotifyMatchResult_ShouldSetMatchIdOnNotification()
    {
        AddJournalist("journalist@test.com");

        _service.NotifyMatchResult(42, HomeTeamName, AwayTeamName, 1, 0);

        var notification = _notificationRepository.GetAll().First();
        Assert.AreEqual(42, notification.MatchId);
    }

    [TestMethod]
    public void NotifyMatchResult_ShouldAssignCorrectUserIdToEachJournalist()
    {
        var j1 = AddJournalist("j1@test.com");
        var j2 = AddJournalist("j2@test.com");

        _service.NotifyMatchResult(1, HomeTeamName, AwayTeamName, 2, 1);

        var notifications = _notificationRepository.GetAll().ToList();
        Assert.IsTrue(notifications.Any(n => n.UserId == j1.UserId));
        Assert.IsTrue(notifications.Any(n => n.UserId == j2.UserId));
    }

    // ─── NotifySimulationCompleted ───

    [TestMethod]
    public void NotifySimulationCompleted_WithJournalists_ShouldCreateNotificationForEach()
    {
        AddJournalist("journalist1@test.com");
        AddJournalist("journalist2@test.com");

        _service.NotifySimulationCompleted("Fase de grupos", 72);

        var notifications = _notificationRepository.GetAll().ToList();
        Assert.AreEqual(2, notifications.Count);
    }

    [TestMethod]
    public void NotifySimulationCompleted_ShouldCreateNotificationWithCorrectMessage()
    {
        AddJournalist("journalist@test.com");

        _service.NotifySimulationCompleted("Fase de grupos", 72);

        var notification = _notificationRepository.GetAll().First();
        Assert.IsTrue(notification.Message.Contains("Fase de grupos"));
        Assert.IsTrue(notification.Message.Contains("72"));
    }

    [TestMethod]
    public void NotifySimulationCompleted_ShouldNotSetMatchId()
    {
        AddJournalist("journalist@test.com");

        _service.NotifySimulationCompleted("Fase de grupos", 72);

        var notification = _notificationRepository.GetAll().First();
        Assert.AreEqual(0, notification.MatchId);
    }

    // ─── GetUnreadNotifications ───

    [TestMethod]
    public void GetUnreadNotifications_ForUser_ShouldReturnOnlyUnreadNotifications()
    {
        var journalist = AddJournalist("journalist@test.com");
        AddEditor("editor@test.com");

        _service.NotifyMatchResult(1, HomeTeamName, AwayTeamName, 2, 1);
        _service.NotifyMatchResult(2, HomeTeamName, AwayTeamName, 0, 0);

        var unread = _service.GetUnreadNotifications(journalist.UserId).ToList();
        Assert.AreEqual(2, unread.Count);
        Assert.IsTrue(unread.All(n => !n.IsRead));
    }

    [TestMethod]
    public void GetUnreadNotifications_AfterMarkingAsRead_ShouldExcludeReadNotifications()
    {
        var journalist = AddJournalist("journalist@test.com");

        _service.NotifyMatchResult(1, HomeTeamName, AwayTeamName, 2, 1);
        _service.NotifyMatchResult(2, HomeTeamName, AwayTeamName, 0, 0);

        var firstNotification = _notificationRepository.GetAll().First(n => n.UserId == journalist.UserId);
        _service.MarkAsRead(firstNotification.NotificationId);

        var unread = _service.GetUnreadNotifications(journalist.UserId).ToList();
        Assert.AreEqual(1, unread.Count);
    }

    [TestMethod]
    public void GetUnreadNotifications_ShouldOnlyReturnNotificationsForSpecificUser()
    {
        var j1 = AddJournalist("j1@test.com");
        var j2 = AddJournalist("j2@test.com");

        _service.NotifyMatchResult(1, HomeTeamName, AwayTeamName, 2, 1);

        var unreadJ1 = _service.GetUnreadNotifications(j1.UserId).ToList();
        var unreadJ2 = _service.GetUnreadNotifications(j2.UserId).ToList();

        Assert.AreEqual(1, unreadJ1.Count);
        Assert.AreEqual(1, unreadJ2.Count);
        Assert.AreNotEqual(unreadJ1[0].Id, unreadJ2[0].Id);
    }

    // ─── GetAllNotifications ───

    [TestMethod]
    public void GetAllNotifications_ShouldReturnBothReadAndUnread()
    {
        var journalist = AddJournalist("journalist@test.com");

        _service.NotifyMatchResult(1, HomeTeamName, AwayTeamName, 2, 1);
        _service.NotifyMatchResult(2, HomeTeamName, AwayTeamName, 0, 0);

        var firstNotification = _notificationRepository.GetAll().First(n => n.UserId == journalist.UserId);
        _service.MarkAsRead(firstNotification.NotificationId);

        var all = _service.GetAllNotifications(journalist.UserId).ToList();
        Assert.AreEqual(2, all.Count);
    }

    // ─── MarkAsRead ───

    [TestMethod]
    public void MarkAsRead_ShouldSetIsReadToTrue()
    {
        AddJournalist("journalist@test.com");

        _service.NotifyMatchResult(1, HomeTeamName, AwayTeamName, 2, 1);

        var notification = _notificationRepository.GetAll().First();
        Assert.IsFalse(notification.IsRead);

        _service.MarkAsRead(notification.NotificationId);

        var updated = _notificationRepository.GetAll().First(n => n.NotificationId == notification.NotificationId);
        Assert.IsTrue(updated.IsRead);
    }

    // ─── MarkAllAsRead ───

    [TestMethod]
    public void MarkAllAsRead_ShouldMarkAllNotificationsAsReadForUser()
    {
        var j1 = AddJournalist("j1@test.com");
        AddJournalist("j2@test.com");

        _service.NotifyMatchResult(1, HomeTeamName, AwayTeamName, 2, 1);
        _service.NotifyMatchResult(2, HomeTeamName, AwayTeamName, 0, 0);

        _service.MarkAllAsRead(j1.UserId);

        var j1Notifications = _service.GetAllNotifications(j1.UserId).ToList();
        Assert.IsTrue(j1Notifications.All(n => n.IsRead));

        var j2Notifications = _service.GetUnreadNotifications(_userRepository.GetAll()
            .First(u => u.Email == "j2@test.com").UserId).ToList();
        Assert.AreEqual(2, j2Notifications.Count);
    }

    // ─── GetUnreadCount ───

    [TestMethod]
    public void GetUnreadCount_WithNoNotifications_ShouldReturnZero()
    {
        var journalist = AddJournalist("journalist@test.com");

        Assert.AreEqual(0, _service.GetUnreadCount(journalist.UserId));
    }

    [TestMethod]
    public void GetUnreadCount_WithUnreadNotifications_ShouldReturnCorrectCount()
    {
        var journalist = AddJournalist("journalist@test.com");

        _service.NotifyMatchResult(1, HomeTeamName, AwayTeamName, 2, 1);
        _service.NotifyMatchResult(2, HomeTeamName, AwayTeamName, 0, 0);

        Assert.AreEqual(2, _service.GetUnreadCount(journalist.UserId));
    }

    [TestMethod]
    public void GetUnreadCount_AfterMarkingOneAsRead_ShouldDecreaseByOne()
    {
        var journalist = AddJournalist("journalist@test.com");

        _service.NotifyMatchResult(1, HomeTeamName, AwayTeamName, 2, 1);
        _service.NotifyMatchResult(2, HomeTeamName, AwayTeamName, 0, 0);

        var firstNotification = _notificationRepository.GetAll().First(n => n.UserId == journalist.UserId);
        _service.MarkAsRead(firstNotification.NotificationId);

        Assert.AreEqual(1, _service.GetUnreadCount(journalist.UserId));
    }

    // ─── Helpers ───

    private User AddJournalist(string email)
    {
        var user = new User("Periodista", "Prensa", email, new DateTime(1990, 1, 1), "Password1!", Role.Journalist);
        _userRepository.Add(user);
        return user;
    }

    private User AddEditor(string email)
    {
        var user = new User("Editor", "Deportes", email, new DateTime(1990, 1, 1), "Password1!", Role.Editor);
        _userRepository.Add(user);
        return user;
    }
}
