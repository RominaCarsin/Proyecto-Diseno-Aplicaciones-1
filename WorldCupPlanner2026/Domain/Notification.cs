namespace Domain;

public class Notification
{
    private const string InvalidUserIdErrorMessage = "El ID de usuario debe ser mayor a cero.";
    private const string MessageRequiredErrorMessage = "El mensaje no puede estar vacío.";

    public int NotificationId { get; private set; }
    public int UserId { get; private set; }
    public User User { get; private set; } = null!;
    public string Message { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public bool IsRead { get; private set; }
    public int MatchId { get; private set; }

    private Notification() { }

    public Notification(int userId, string message, int matchId = 0)
    {
        ValidateUserId(userId);
        ValidateMessage(message);

        UserId = userId;
        Message = message;
        MatchId = matchId;
        CreatedAt = DateTime.UtcNow;
        IsRead = false;
    }

    public void MarkAsRead()
    {
        IsRead = true;
    }

    private static void ValidateUserId(int userId)
    {
        if (userId <= 0)
            throw new ArgumentException(InvalidUserIdErrorMessage, nameof(userId));
    }

    private static void ValidateMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException(MessageRequiredErrorMessage, nameof(message));
    }
}
