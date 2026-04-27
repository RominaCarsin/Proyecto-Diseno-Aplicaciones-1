using Domain;
using IServices;
using IRepository;

namespace Services;

public class TeamService : ITeamService
{
    private readonly IRepository<Team> _repository;
    private readonly IConfederationService _confederationService;

    public TeamService(IRepository<Team> repository, IConfederationService confederationService)
    {
        _repository = repository;
        _confederationService = confederationService;
    }

    public void AddTeam(Team team)
    {
        var existing = _repository.GetAll().FirstOrDefault(t => t.Name == team.Name);
        if (existing != null)
            throw new ArgumentException("A team with this name already exists.");

        _confederationService.AddTeamToConfederation(team);
        _repository.Add(team);
    }

    public IEnumerable<Team> GetTeams() => _repository.GetAll();

    public void UpdateTeam(string currentName, string newName, int rankingFifa, string confederationName)
    {
        var existing = _repository.GetAll().FirstOrDefault(t => t.Name == currentName);
        if (existing == null)
            throw new ArgumentException("Team not found.");

        var nameConflict = _repository.GetAll()
            .FirstOrDefault(t => t.Name == newName && t.Name != currentName);
        if (nameConflict != null)
            throw new ArgumentException("A team with this name already exists.");

        existing.UpdateName(newName);
        existing.UpdateRankingFifa(rankingFifa);

        if (existing.ConfederationName != confederationName)
            _confederationService.UpdateTeamConfederation(existing, confederationName);
    }

    public void DeleteTeam(string name)
    {
        var existing = _repository.GetAll().FirstOrDefault(t => t.Name == name);
        if (existing == null)
            throw new ArgumentException("Team not found.");

        _confederationService.RemoveTeamFromConfederation(existing);
        _repository.Delete(existing);
    }
}