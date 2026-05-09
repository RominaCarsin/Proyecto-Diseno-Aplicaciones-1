using Domain;
using DTOs;
using IRepository;
using IServices;
using Services.Mappers;

namespace Services;

public class StadiumService : IStadiumService
{
    private readonly IRepository<Stadium> _repository;

    public StadiumService(IRepository<Stadium> repository)
    {
        _repository = repository;
    }

    public void AddStadium(StadiumRequest stadiumRequest)
    {
        var stadium = StadiumMapper.ToEntity(stadiumRequest);

        EnsureStadiumNameIsNotTaken(stadium.Name);

        _repository.Add(stadium);
    }

    public void DeleteStadium(string stadiumName)
    {
        var existingStadium = FindStadiumByNameOrThrow(stadiumName);

        _repository.Delete(existingStadium);
    }

    public void UpdateStadium(string originalStadiumName, StadiumRequest updatedStadiumData)
    {
        var existingStadium = FindStadiumByNameOrThrow(originalStadiumName);

        var updatedStadium = StadiumMapper.ToEntity(updatedStadiumData);

        bool isRenamingToADifferentName = !updatedStadium.Name.Equals(
            existingStadium.Name, 
            StringComparison.OrdinalIgnoreCase);
        
        if (isRenamingToADifferentName)
            EnsureStadiumNameIsNotTaken(updatedStadium.Name);

        ReplaceStadium(existingStadium, updatedStadium);
    }

    public IEnumerable<StadiumResponse> GetAllStadiums()
    {
        return _repository.GetAll()
            .Select(StadiumMapper.FromEntity);
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

    private void ReplaceStadium(Stadium stadiumToRemove, Stadium stadiumToAdd)
    {
        _repository.Delete(stadiumToRemove);
        _repository.Add(stadiumToAdd);
    }
}