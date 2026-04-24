using Domain;
using IRepository;

namespace IServices;

public interface ITeamService
{
    void AddTeam(Team team);
    IEnumerable<Team> GetTeams();
}