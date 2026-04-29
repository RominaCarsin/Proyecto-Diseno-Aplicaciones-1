using Domain;
using DTOs;
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

    public void AddTeam(TeamRequest request)
    {
        
        var name = request.Name;
        
        var existing = _repository.GetAll().FirstOrDefault(t => t.Name == name);
        if (existing != null)
            throw new ArgumentException("A team with this name already exists.");

        var team = TeamRequest.ToEntity(request);
        _confederationService.AddTeamToConfederation(team);
        _repository.Add(team);
    }

    public void UpdateTeam(string currentName, TeamRequest request)
    {
        var current = currentName;
        var newName = request.Name;

        var existing = _repository.GetAll().FirstOrDefault(t => t.Name == current);
        if (existing == null)
            throw new ArgumentException("Team not found.");
        
        var nameConflict = _repository.GetAll()
            .FirstOrDefault(t => t.Name == newName && t.Name != current);
            
        if (nameConflict != null)
            throw new ArgumentException("A team with this name already exists.");

        existing.UpdateName(request.Name);
        existing.UpdateRankingFifa(request.RankingFifa);
        
        var newConfederationName = request.ConfederationName;
        
        if (existing.ConfederationName != newConfederationName)
        {
            existing.UpdateConfederationName(request.ConfederationName);
            _confederationService.UpdateTeamConfederation(existing, newConfederationName);
        }
        
    }

    public void DeleteTeam(string name)
    {
        var existing = _repository.GetAll().FirstOrDefault(t => t.Name == name);
        
        if (existing == null)
            throw new ArgumentException("Team not found.");

        _confederationService.RemoveTeamFromConfederation(existing);
        _repository.Delete(existing);
    }

    public IEnumerable<TeamResponse> GetTeams()
    {
        return _repository.GetAll().Select(TeamResponse.FromEntity);
    }
}