using Domain;
using DTOs;
using IRepository;
using IServices;
using Services.Mappers;

namespace Services;

public class GroupService : IGroupService
{
    private const int RequiredGroupCount = 12;
    private const int MinTeamsPerGroup = 3;
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

    public IEnumerable<GroupResponse> GetGroups() =>
        _repository.GetAll().Select(GroupMapper.FromEntity);

    public GroupResponse GetGroupByLabel(char label) =>
        GroupMapper.FromEntity(FindGroupByLabelOrThrow(label));

    public void DeleteGroup(char label) =>
        _repository.Delete(FindGroupByLabelOrThrow(label));

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
    
    private Group FindGroupByLabelOrThrow(char label) =>
        _repository.GetAll().FirstOrDefault(g => g.Label == label)
            ?? throw new ArgumentException(GroupNotFoundErrorMessage);

    public IEnumerable<TeamResponse> GetQualifiedTeams()
    {
        var groups = _repository.GetAll().ToList();

        if (groups.Count != RequiredGroupCount)
            throw new InvalidOperationException("Deben existir exactamente 12 grupos para determinar clasificados.");

        if (groups.Any(g => g.Teams.Count < MinTeamsPerGroup))
            throw new InvalidOperationException("Cada grupo debe tener al menos 3 equipos para determinar 1ro, 2do y 3ro.");

        var winners = groups.Select(g => g.GetTeamByPosition(1));
        var runnersUp = groups.Select(g => g.GetTeamByPosition(2));
        var thirds = groups.Select(g => g.GetTeamByPosition(3));

        var sortedWinners = SortTeams(winners);
        var sortedRunnersUp = SortTeams(runnersUp);
        var bestThirds = SortTeams(thirds).Take(8);

        return sortedWinners
            .Concat(sortedRunnersUp)
            .Concat(bestThirds)
            .Select(TeamMapper.FromEntity);
    }

    private static Team FindTeamInGroupOrThrow(Group group, string teamName) =>
        group.Teams.FirstOrDefault(t => t.Name.Equals(Team.Normalize(teamName), StringComparison.OrdinalIgnoreCase))
            ?? throw new ArgumentException($"El equipo '{teamName}' no se encontró en el grupo.");

    private static Match FindMatchInGroupOrThrow(Group group, int matchId) =>
        group.Matches.FirstOrDefault(m => m.Id == matchId)
            ?? throw new ArgumentException(MatchNotFoundErrorMessage);

    private static IEnumerable<Team> SortTeams(IEnumerable<Team> teams) =>
        teams.OrderByDescending(t => t.Points)
            .ThenByDescending(t => t.GoalDifference)
            .ThenByDescending(t => t.GoalsInFavor);
}