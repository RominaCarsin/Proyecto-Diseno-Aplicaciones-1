using Domain;
using DTOs;

namespace Services.Mappers;

public static class NotificationMapper
{
    public static NotificationResponse FromEntity(Notification notification)
    {
        return new NotificationResponse
        {
            Id = notification.NotificationId,
            UserId = notification.UserId,
            Message = notification.Message,
            CreatedAt = notification.CreatedAt,
            IsRead = notification.IsRead,
            MatchId = notification.MatchId
        };
    }
}
