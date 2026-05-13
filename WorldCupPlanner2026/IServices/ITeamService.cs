using DTOs;

namespace IServices;

public interface ITeamService
{
    void AddTeam(TeamRequest team);
    IEnumerable<TeamResponse> GetTeams();
    IEnumerable<TeamResponse> GetTeamsOrdered(int seed);
    void UpdateTeam(string currentName, TeamRequest request);
    void DeleteTeam(string name);
    void AutoCompleteTeams(int seed);
}
