using Domain;

namespace IServices;

public interface ITeamManagementService
{
    void AddTeam(Team team);
    IEnumerable<Team> GetTeams();
}