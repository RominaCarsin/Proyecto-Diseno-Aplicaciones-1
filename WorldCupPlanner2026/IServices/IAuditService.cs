using Domain;

namespace IServices;

public interface IAuditService
{
    void Log(string actorEmail, string action, string details);
    IEnumerable<AuditLog> GetAll();
}