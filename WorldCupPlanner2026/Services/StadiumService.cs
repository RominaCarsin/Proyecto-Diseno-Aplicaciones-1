using Domain;
using DTOs;
using IRepository;
using Services.Mappers;

namespace Services;

public class StadiumService
{
    private readonly IRepository<Stadium> _repository;

    public StadiumService(IRepository<Stadium> repository)
    {
        _repository = repository;
    }

    public void AddStadium(StadiumRequest request)
    {
        var stadium = StadiumMapper.ToEntity(request);

        bool nameExists = _repository.GetAll()
            .Any(s => s.Name.Equals(stadium.Name, StringComparison.OrdinalIgnoreCase));

        if (nameExists)
            throw new ArgumentException($"A stadium with the name '{stadium.Name}' already exists");

        _repository.Add(stadium);
    }

    public void DeleteStadium(string stadiumName)
    {

        var stadium = _repository.GetAll()
            .FirstOrDefault(s => s.Name == stadiumName);

        if (stadium == null)
            throw new ArgumentException($"A stadium with the name '{stadiumName}' does not exist");

        _repository.Delete(stadium);
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
    }

    public IEnumerable<StadiumResponse> GetStadiums()
    {
        return _repository.GetAll()
            .Select(StadiumMapper.FromEntity);
    }
}