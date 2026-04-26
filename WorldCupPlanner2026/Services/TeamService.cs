using Domain;
using IServices;
using IRepository;

namespace Services;

public class TeamService : ITeamService
{
    private readonly IRepository<Team> _repository;
    
    public TeamService(IRepository<Team> repository)
    {
        _repository = repository;
    }

    public void AddTeam(Team team)
    {
        var existing = _repository.GetAll().FirstOrDefault(t => t.Name == team.Name);
        if (existing != null)
            throw new ArgumentException("A team with this name already exists.");
        
        _repository.Add(team);
        team.Confederation.AddTeam(team);
    }

    public IEnumerable<Team> GetTeams()
    {
        return _repository.GetAll();
    }
    
}