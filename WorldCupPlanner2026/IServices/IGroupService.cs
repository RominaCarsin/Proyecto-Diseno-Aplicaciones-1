
using DTOs;
namespace IServices;

public interface IGroupService
{
    void AddGroup(GroupRequest request);
    IEnumerable<GroupResponse> GetGroups();
    GroupResponse GetGroupByLabel(char label);
    void DeleteGroup(char label);
    void AddTeamToGroup(char groupLabel, TeamRequest teamRequest);
    void RemoveTeamFromGroup(char groupLabel, TeamRequest teamRequest);
    void AddMatchToGroup(char groupLabel, MatchRequest matchRequest);
    void SetMatchResult(char groupLabel, int matchId, int homeGoals, int awayGoals);
    IEnumerable<TeamResponse> GetGroupStandings(char groupLabel);
    TeamResponse GetTeamByPosition(char groupLabel, int position);
}