using Domain;
using DTOs;
using IRepository;
using IServices;
using Services.Mappers;

namespace Services;

public class NotificationService : INotificationService
{
    private const string NotificationNotFoundErrorMessage = "No se encontró la notificación.";

    private readonly IRepository<Notification> _notificationRepository;
    private readonly IRepository<User> _userRepository;

    public NotificationService(
        IRepository<Notification> notificationRepository,
        IRepository<User> userRepository)
    {
        _notificationRepository = notificationRepository;
        _userRepository = userRepository;
    }

    public void NotifyMatchResult(int matchId, string homeTeam, string awayTeam, int homeGoals, int awayGoals)
    {
        var message = $"Resultado: {homeTeam} {homeGoals} - {awayGoals} {awayTeam}";
        CreateNotificationsForJournalists(message, matchId);
    }

    public void NotifySimulationCompleted(string phase, int matchCount)
    {
        var message = $"Simulación completada: {phase} ({matchCount} partidos)";
        CreateNotificationsForJournalists(message, matchId: 0);
    }

    public IEnumerable<NotificationResponse> GetUnreadNotifications(int userId)
    {
        return _notificationRepository.GetAll()
            .Where(n => n.UserId == userId && !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .Select(NotificationMapper.FromEntity);
    }

    public IEnumerable<NotificationResponse> GetAllNotifications(int userId)
    {
        return _notificationRepository.GetAll()
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Select(NotificationMapper.FromEntity);
    }

    public void MarkAsRead(int notificationId)
    {
        var notification = FindNotificationByIdOrThrow(notificationId);
        notification.MarkAsRead();
        _notificationRepository.Update(notification);
    }

    public void MarkAllAsRead(int userId)
    {
        var unreadNotifications = _notificationRepository.GetAll()
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToList();

        foreach (var notification in unreadNotifications)
        {
            notification.MarkAsRead();
            _notificationRepository.Update(notification);
        }
    }

    public int GetUnreadCount(int userId)
    {
        return _notificationRepository.GetAll()
            .Count(n => n.UserId == userId && !n.IsRead);
    }

    private void CreateNotificationsForJournalists(string message, int matchId)
    {
        var journalists = _userRepository.GetAll()
            .Where(user => user.HasRole(Role.Journalist))
            .ToList();

        foreach (var journalist in journalists)
        {
            var notification = new Notification(journalist.UserId, message, matchId);
            _notificationRepository.Add(notification);
        }
    }

    private Notification FindNotificationByIdOrThrow(int notificationId) =>
        _notificationRepository.GetAll()
            .FirstOrDefault(n => n.NotificationId == notificationId)
            ?? throw new ArgumentException(NotificationNotFoundErrorMessage);
}
