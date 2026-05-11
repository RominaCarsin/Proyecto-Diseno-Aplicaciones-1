using Domain;
using DTOs;
using IRepository;
using IServices;
using Services.Mappers;

namespace Services;

public class StadiumService : IStadiumService
{
    private readonly IRepository<Stadium> _repository;
    private readonly IAuditService _auditService;
    private readonly ISessionService _sessionService;

    public StadiumService(IRepository<Stadium> repository, IAuditService auditService, ISessionService sessionService)
    {
        _repository = repository;
        _auditService = auditService;
        _sessionService = sessionService;
    }

    public void AddStadium(StadiumRequest request)
    {
        var stadium = StadiumMapper.ToEntity(request);

        bool nameExists = _repository.GetAll()
            .Any(s => s.Name.Equals(stadium.Name, StringComparison.OrdinalIgnoreCase));

        if (nameExists)
            throw new ArgumentException($"A stadium with the name '{stadium.Name}' already exists");

        _repository.Add(stadium);
        _auditService.Log(_sessionService.GetCurrentUserEmail() ?? "system", "Stadium created", 
            $"Stadium '{stadium.Name}' created in {request.City} with capacity {request.Capacity}");
    }

    public void DeleteStadium(string stadiumName)
    {

        var stadium = _repository.GetAll()
            .FirstOrDefault(s => s.Name == stadiumName);

        if (stadium == null)
            throw new ArgumentException($"A stadium with the name '{stadiumName}' does not exist");

        _repository.Delete(stadium);
        _auditService.Log(_sessionService.GetCurrentUserEmail() ?? "system", "Stadium deleted", 
            $"Stadium '{stadiumName}' was removed from the system");
    }

    public void UpdateStadium(string originalName, StadiumRequest request)
    {
        originalName = originalName?.Trim() ?? string.Empty;

        var existingStadium = _repository.GetAll()
            .FirstOrDefault(s => s.Name.Equals(originalName, StringComparison.OrdinalIgnoreCase));

        if (existingStadium == null)
            throw new ArgumentException($"A stadium with the name '{originalName}' does not exist");

        var updatedStadium = StadiumMapper.ToEntity(request);

        bool nameExists = _repository.GetAll()
            .Any(s =>
                s.Name.Equals(updatedStadium.Name, StringComparison.OrdinalIgnoreCase)
                && !s.Name.Equals(originalName, StringComparison.OrdinalIgnoreCase));

        if (nameExists)
            throw new ArgumentException($"A stadium with the name '{updatedStadium.Name}' already exists");

        _repository.Delete(existingStadium);
        _repository.Add(updatedStadium);
        _auditService.Log(_sessionService.GetCurrentUserEmail() ?? "system", "Stadium updated", 
            $"Stadium '{originalName}' updated. New name: '{request.Name}', city: {request.City}, capacity: {request.Capacity}");
    }

    public IEnumerable<StadiumResponse> GetStadiums()
    {
        return _repository.GetAll()
            .Select(StadiumMapper.FromEntity);
    }
}