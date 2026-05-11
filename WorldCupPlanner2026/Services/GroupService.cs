using Domain;
using DTOs;
using IRepository;
using IServices;
using Services.Mappers;

namespace Services;

public class GroupService : IGroupService
{
    private readonly IRepository<Group> _repository;
    private const string GroupNotFoundErrorMessage = "Group was not found.";

    public GroupService(IRepository<Group> repository)
    {
        _repository = repository;
    }

    public void AddGroup(GroupRequest request)
    {
        Group group = GroupMapper.ToEntity(request);

        _repository.Add(group);
    }

    public IEnumerable<GroupResponse> GetGroups()
    {
        return _repository
            .GetAll()
            .Select(GroupMapper.FromEntity);
    }

    public void DeleteGroup(char label)
    {
        Domain.Group group = _repository
            .GetAll()
            .First(g => g.Label == label);

        _repository.Delete(group);
    }
    
    public GroupResponse GetGroupByLabel(char label)
    {
        return GroupMapper.FromEntity(FindGroupByLabel(label));
    }

    private Group FindGroupByLabel(char label)
    {
        return _repository.GetAll().FirstOrDefault(g => g.Label == label)
               ?? throw new ArgumentException(GroupNotFoundErrorMessage);
    }

    public void AddTeamToGroup(char groupLabel, TeamRequest request)
    {
        Group group = FindGroupByLabel(groupLabel);
        Team team = TeamMapper.ToEntity(request);
        group.AddTeam(team);
    }

    public void RemoveTeamFromGroup(char groupLabel, TeamRequest request)
    {
        Group group = FindGroupByLabel(groupLabel);
        Team team = TeamMapper.ToEntity(request);
        group.RemoveTeam(team);
    }

    public IEnumerable<TeamResponse> GetGroupStandings(char groupLabel)
    {
        throw new NotImplementedException();
    }

    public TeamResponse GetTeamByPosition(char groupLabel, int position)
    {
        Group group = FindGroupByLabel(groupLabel);
        Team team = group.GetTeamByPosition(position);
        return TeamMapper.FromEntity(team);
    }

    public void AddMatchToGroup(char groupLabel, MatchRequest request)
    {
        Group group = FindGroupByLabel(groupLabel);
        Match match = MatchMapper.ToEntity(request);
        group.AddMatch(match);
    }

    public void SetMatchResult(char groupLabel, int matchId, int homeGoals, int awayGoals)
    {
        Group group = FindGroupByLabel(groupLabel);
        Match match = FindMatchById(group, matchId);
        group.SetMatchResult(match, homeGoals, awayGoals);
    }
    private static Match FindMatchById(Group group, int matchId)
    {
        return group.Matches.FirstOrDefault(m => m.Id == matchId)
               ?? throw new ArgumentException("Match was not found in group.");
    }
}