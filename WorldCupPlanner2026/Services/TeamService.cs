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
        
        _repository.Add(team);
        
        _confederationService.AddTeamToConfederation(team);
    }

    public IEnumerable<Team> GetTeams()
    {
        return _repository.GetAll();
    }
    
}