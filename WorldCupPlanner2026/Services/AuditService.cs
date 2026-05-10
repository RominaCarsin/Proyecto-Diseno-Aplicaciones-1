using Domain;
using IRepository;
using IServices;

namespace Services;

public class AuditService : IAuditService
{
    private readonly IRepository<AuditLog> _repository;

    public AuditService(IRepository<AuditLog> repository)
    {
        _repository = repository;
    }

    public void Log(string actorEmail, string action, string details)
    {
        var log = new AuditLog(actorEmail, action, details);
        _repository.Add(log);
    }

    public IEnumerable<AuditLog> GetAll()
    {
        return _repository.GetAll();
    }
}