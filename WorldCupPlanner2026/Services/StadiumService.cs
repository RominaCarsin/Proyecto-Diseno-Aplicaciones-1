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

    public void AddStadium(StadiumRequest stadiumRequest)
    {
        var stadium = StadiumMapper.ToEntity(stadiumRequest);

        EnsureStadiumNameIsNotTaken(stadium.Name);

        _repository.Add(stadium);
        LogAction("Stadium created",
            $"Stadium '{stadium.Name}' created in {stadium.City} with capacity {stadium.Capacity}");
    }

    public void DeleteStadium(string stadiumName)
    {
        var existingStadium = FindStadiumByNameOrThrow(stadiumName);

        _repository.Delete(existingStadium);
        LogAction("Stadium deleted",
            $"Stadium '{stadiumName}' was removed from the system");
    }

    public void UpdateStadium(string originalStadiumName, StadiumRequest updatedStadiumData)
    {
        var existingStadium = FindStadiumByNameOrThrow(originalStadiumName);

        var updatedStadium = StadiumMapper.ToEntity(updatedStadiumData);

        if (IsRenamingToDifferentName(existingStadium, updatedStadium))
            EnsureStadiumNameIsNotTaken(updatedStadium.Name);

        ReplaceStadium(existingStadium, updatedStadium);
        LogAction("Stadium updated",
            $"Stadium '{originalStadiumName}' updated. New name: '{updatedStadiumData.Name}', city: {updatedStadiumData.City}, capacity: {updatedStadiumData.Capacity}");
    }

    public IEnumerable<StadiumResponse> GetAllStadiums()
    {
        return _repository.GetAll()
            .Select(StadiumMapper.FromEntity);
    }

    private void LogAction(string action, string details)
    {
        var actorEmail = _sessionService.GetCurrentUserEmail() ?? "system";
        _auditService.Log(actorEmail, action, details);
    }

    private Stadium FindStadiumByNameOrThrow(string stadiumName)
    {
        return _repository.GetAll()
            .FirstOrDefault(s => s.Name.Equals(stadiumName, StringComparison.OrdinalIgnoreCase))
            ?? throw new ArgumentException($"A stadium with the name '{stadiumName}' does not exist");
    }

    private void EnsureStadiumNameIsNotTaken(string stadiumName)
    {
        bool nameAlreadyExists = _repository.GetAll()
            .Any(s => s.Name.Equals(stadiumName, StringComparison.OrdinalIgnoreCase));

        if (nameAlreadyExists)
            throw new ArgumentException($"A stadium with the name '{stadiumName}' already exists");
    }

    private static bool IsRenamingToDifferentName(Stadium existing, Stadium updated)
    {
        return !updated.Name.Equals(existing.Name, StringComparison.OrdinalIgnoreCase);
    }

    private void ReplaceStadium(Stadium stadiumToRemove, Stadium stadiumToAdd)
    {
        _repository.Delete(stadiumToRemove);
        _repository.Add(stadiumToAdd);
    }
}
