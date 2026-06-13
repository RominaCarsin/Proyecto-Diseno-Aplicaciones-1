using DTOs;

namespace IServices;

public interface INotificationService
{
    void NotifyMatchResult(int matchId, string homeTeam, string awayTeam, int homeGoals, int awayGoals);
    void NotifySimulationCompleted(string phase, int matchCount);
    IEnumerable<NotificationResponse> GetUnreadNotifications(int userId);
    IEnumerable<NotificationResponse> GetAllNotifications(int userId);
    void MarkAsRead(int notificationId);
    void MarkAllAsRead(int userId);
    int GetUnreadCount(int userId);
}
