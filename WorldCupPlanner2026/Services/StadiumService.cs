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
        LogAction("Estadio creado",
            $"Estadio '{stadium.Name}' creado en {stadium.City} con capacidad {stadium.Capacity}");
    }

    public void DeleteStadium(string stadiumName)
    {
        var existingStadium = FindStadiumByNameOrThrow(stadiumName);

        _repository.Delete(existingStadium);
        LogAction("Estadio eliminado",
            $"Estadio '{stadiumName}' eliminado del sistema.");
    }

    public void UpdateStadium(string originalStadiumName, StadiumRequest updatedStadiumData)
    {
        var existingStadium = FindStadiumByNameOrThrow(originalStadiumName);
        var updatedStadium = StadiumMapper.ToEntity(updatedStadiumData);

        if (IsRenamingToDifferentName(existingStadium, updatedStadium))
            EnsureStadiumNameIsNotTaken(updatedStadium.Name);

        ApplyStadiumUpdates(existingStadium, updatedStadium);
        LogAction("Estadio actualizado",
            $"Estadio '{originalStadiumName}' actualizado. Nuevo nombre: '{updatedStadiumData.Name}', ciudad: {updatedStadiumData.City}, capacidad: {updatedStadiumData.Capacity}");
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
            ?? throw new ArgumentException($"No existe un estadio con el nombre '{stadiumName}'.");
    }

    private void EnsureStadiumNameIsNotTaken(string stadiumName)
    {
        bool nameAlreadyExists = _repository.GetAll()
            .Any(s => s.Name.Equals(stadiumName, StringComparison.OrdinalIgnoreCase));

        if (nameAlreadyExists)
            throw new ArgumentException($"Ya existe un estadio con el nombre '{stadiumName}'.");
    }

    private static bool IsRenamingToDifferentName(Stadium existing, Stadium updated)
    {
        return !updated.Name.Equals(existing.Name, StringComparison.OrdinalIgnoreCase);
    }

    private void ApplyStadiumUpdates(Stadium stadiumToUpdate, Stadium stadiumToAdd)
    {
        stadiumToUpdate.UpdateName(stadiumToAdd.Name);
        stadiumToUpdate.UpdateCity(stadiumToAdd.City);
        stadiumToUpdate.UpdateDescription(stadiumToAdd.Description);
        stadiumToUpdate.UpdateCapacity(stadiumToAdd.Capacity);
        
    }
}
