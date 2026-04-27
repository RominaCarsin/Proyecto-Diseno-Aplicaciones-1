using Domain;
using IRepository;

namespace IServices;

public interface ITeamService
{
    void AddTeam(Team team);
    IEnumerable<Team> GetTeams();
    void UpdateTeam(string currentName, string newName, int rankingFifa, string confederationName);
    void DeleteTeam(string name);
}