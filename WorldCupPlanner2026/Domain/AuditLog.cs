namespace Domain;

public class AuditLog
{
    public int Id { get; private set; }
    public DateTime Timestamp { get; private set; }
    public string ActorEmail { get; private set; }
    public string Action { get; private set; }
    public string Details { get; private set; }
    private static int _nextId = 1;

    public AuditLog(string actorEmail, string action, string details)
    {
        ValidateActorEmail(actorEmail);
        ValidateAction(action);
        
        Id = _nextId++;
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
    
    private static void ValidateAction(string action)
    {
        if (string.IsNullOrWhiteSpace(action))
            throw new ArgumentException("Action cannot be empty");
    }
}