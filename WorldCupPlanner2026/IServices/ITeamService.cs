using DTOs;


namespace IServices;

public interface ITeamService
{
    void AddTeam(TeamRequest team);
    IEnumerable<TeamResponse> GetTeams();
    void UpdateTeam(string currentName, TeamRequest request);
    void DeleteTeam(string name);
}