
using DTOs;
namespace IServices;

public interface IGroupService
{
    void AddGroup(GroupRequest request);
    IEnumerable<GroupResponse> GetGroups();
    GroupResponse GetGroupByLabel(char label);
    void DeleteGroup(char label);
    void AddTeamToGroup(char groupLabel, TeamRequest teamRequest);
    IEnumerable<TeamResponse> GetGroupStandings(char groupLabel);
    TeamResponse GetTeamByPosition(char groupLabel, int position);
    IEnumerable<MatchResponse> GetGroupMatches(char groupLabel);
    IEnumerable<MatchResponse> GetPendingGroupMatches(char groupLabel);
}