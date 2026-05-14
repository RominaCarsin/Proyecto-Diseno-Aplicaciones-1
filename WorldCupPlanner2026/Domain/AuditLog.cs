namespace Domain;

public class AuditLog
{
    private const string ActorEmailRequiredErrorMessage = "El email del actor no puede estar vacío.";
    private const string ActionRequiredErrorMessage = "La acción no puede estar vacía.";

    private static int _nextId = 1;

    public int Id { get; private set; }
    public DateTime Timestamp { get; private set; }
    public string ActorEmail { get; private set; }
    public string Action { get; private set; }
    public string Details { get; private set; }

    public AuditLog(string actorEmail, string action, string details)
    {
        ValidateActorEmail(actorEmail);
        ValidateAction(action);

        Id = _nextId++;
        Timestamp = DateTime.UtcNow;
        ActorEmail = actorEmail;
        Action = action;
        Details = details;
    }

    private static void ValidateActorEmail(string actorEmail)
    {
        if (string.IsNullOrWhiteSpace(actorEmail))
            throw new ArgumentException(ActorEmailRequiredErrorMessage);
    }

    private static void ValidateAction(string action)
    {
        if (string.IsNullOrWhiteSpace(action))
            throw new ArgumentException(ActionRequiredErrorMessage);
    }
}