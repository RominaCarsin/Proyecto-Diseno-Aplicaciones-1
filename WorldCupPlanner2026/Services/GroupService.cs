using Domain;
using DTOs;
using IRepository;
using IServices;
using Services.Mappers;

namespace Services;

public class GroupService : IGroupService
{
    private const string GroupNotFoundErrorMessage = "No se encontró el grupo.";
    private const string MatchNotFoundErrorMessage = "No se encontró el partido en el grupo.";

    private readonly IRepository<Group> _repository;

    public GroupService(IRepository<Group> repository)
    {
        _repository = repository;
    }

    public void AddGroup(GroupRequest request)
    {
        var group = GroupMapper.ToEntity(request);
        _repository.Add(group);
    }

    public IEnumerable<GroupResponse> GetGroups()
    {
        return _repository.GetAll()
            .Select(GroupMapper.FromEntity);
    }

    public GroupResponse GetGroupByLabel(char label)
    {
        return GroupMapper.FromEntity(FindGroupByLabelOrThrow(label));
    }

    public void DeleteGroup(char label)
    {
        var group = FindGroupByLabelOrThrow(label);
        _repository.Delete(group);
    }

    public void AddTeamToGroup(char groupLabel, TeamRequest teamRequest)
    {
        var group = FindGroupByLabelOrThrow(groupLabel);
        var team = TeamMapper.ToEntity(teamRequest);
        group.AddTeam(team);
    }

    public void RemoveTeamFromGroup(char groupLabel, TeamRequest teamRequest)
    {
        var group = FindGroupByLabelOrThrow(groupLabel);
        var team = FindTeamInGroupOrThrow(group, teamRequest.Name);
        group.RemoveTeam(team);
    }

    public IEnumerable<TeamResponse> GetGroupStandings(char groupLabel)
    {
        var group = FindGroupByLabelOrThrow(groupLabel);
        return group.GetStandings().Select(TeamMapper.FromEntity);
    }

    public TeamResponse GetTeamByPosition(char groupLabel, int position)
    {
        var group = FindGroupByLabelOrThrow(groupLabel);
        var team = group.GetTeamByPosition(position);
        return TeamMapper.FromEntity(team);
    }

    public void AddMatchToGroup(char groupLabel, MatchRequest matchRequest)
    {
        var group = FindGroupByLabelOrThrow(groupLabel);
        var homeTeam = FindTeamInGroupOrThrow(group, matchRequest.HomeTeam.Name);
        var awayTeam = FindTeamInGroupOrThrow(group, matchRequest.AwayTeam.Name);

        var match = new Match(
            date: matchRequest.Date,
            stadium: StadiumMapper.ToEntity(matchRequest.Stadium),
            homeTeam: homeTeam,
            awayTeam: awayTeam,
            group: groupLabel,
            phase: TournamentPhase.GroupStage
        );

        group.AddMatch(match);
    }

    public void SetMatchResult(char groupLabel, int matchId, int homeGoals, int awayGoals)
    {
        var group = FindGroupByLabelOrThrow(groupLabel);
        var match = FindMatchInGroupOrThrow(group, matchId);
        group.SetMatchResult(match, homeGoals, awayGoals);
    }

    public void FinishMatch(char groupLabel, int matchId)
    {
        var group = FindGroupByLabelOrThrow(groupLabel);
        var match = FindMatchInGroupOrThrow(group, matchId);
        group.FinishMatch(match);
    }
    
    private Group FindGroupByLabelOrThrow(char label)
    {
        return _repository.GetAll()
            .FirstOrDefault(g => g.Label == label)
            ?? throw new ArgumentException(GroupNotFoundErrorMessage);
    }

    private static Team FindTeamInGroupOrThrow(Group group, string teamName)
    {
        return group.Teams
            .FirstOrDefault(t => t.Name.Equals(
                Team.Normalize(teamName),
                StringComparison.OrdinalIgnoreCase))
            ?? throw new ArgumentException($"Team '{teamName}' was not found in group.");
    }

    private static Match FindMatchInGroupOrThrow(Group group, int matchId)
    {
        return group.Matches
            .FirstOrDefault(m => m.Id == matchId)
            ?? throw new ArgumentException(MatchNotFoundErrorMessage);
    }
}