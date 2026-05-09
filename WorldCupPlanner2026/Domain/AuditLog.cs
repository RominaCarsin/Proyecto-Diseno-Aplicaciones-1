namespace Domain;

public class AuditLog
{
    public string ActorEmail { get; }
    public string Action { get; }
    public string Details { get; }
    public DateTime Timestamp { get; }

    public AuditLog(string actorEmail, string action, string details)
    {
        ValidateActorEmail(actorEmail);
        
        ActorEmail = actorEmail;
        Action = action;
        Details = details;
        Timestamp = DateTime.UtcNow;
    }

    private static void ValidateActorEmail(string actorEmail)
    {
        if (string.IsNullOrWhiteSpace(actorEmail))
        {
            throw new ArgumentException("Actor email cannot be empty.");
        }
    }
}