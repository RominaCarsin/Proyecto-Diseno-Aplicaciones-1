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
        _repository.Add(team);
    }

    public IEnumerable<Team> GetTeams()
    {
        return _repository.GetAll();
    }
    
}