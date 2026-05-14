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
    private const int RequiredGroupCount = 12;

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
        Group group = FindGroupByLabel(label);
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
        Team team = FindTeamInGroup(group, request.Name);
        group.RemoveTeam(team);
    }

    public IEnumerable<TeamResponse> GetGroupStandings(char groupLabel)
    {
        Group group = FindGroupByLabel(groupLabel);
        return group.GetStandings().Select(TeamMapper.FromEntity);
    }
    
    public void FinishMatch(char groupLabel, int matchId)
    {
        Group group = FindGroupByLabel(groupLabel);
        Match match = FindMatchById(group, matchId);
        group.FinishMatch(match);
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

        Team homeTeam = FindTeamInGroup(group, request.HomeTeam.Name);
        Team awayTeam = FindTeamInGroup(group, request.AwayTeam.Name);

        Match match = new Match(
            date: request.Date,
            stadium: StadiumMapper.ToEntity(request.Stadium),
            homeTeam: homeTeam,
            awayTeam: awayTeam,
            group: groupLabel,
            phase: TournamentPhase.GroupStage
        );

        group.AddMatch(match);
    }

    private static Team FindTeamInGroup(Group group, string teamName)
    {
        return group.Teams.FirstOrDefault(t => 
                   t.Name.Equals(Team.Normalized(teamName), StringComparison.OrdinalIgnoreCase))
               ?? throw new ArgumentException($"Team {teamName} was not found in group.");
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

    // ──────────────────────────────────────────────
    // GREEN: Implementación de GetQualifiedTeams
    // ──────────────────────────────────────────────

    public IEnumerable<TeamResponse> GetQualifiedTeams()
    {
        var groups = _repository.GetAll().ToList();

        if (groups.Count != RequiredGroupCount)
        {
            throw new InvalidOperationException("Deben existir exactamente 12 grupos para determinar clasificados.");
        }

        if (groups.Any(g => g.Teams.Count < 3))
        {
            throw new InvalidOperationException("Cada grupo debe tener al menos 3 equipos para determinar 1ro, 2do y 3ro.");
        }

        var winners = groups.Select(g => g.GetTeamByPosition(1));
        var runnersUp = groups.Select(g => g.GetTeamByPosition(2));
        var thirds = groups.Select(g => g.GetTeamByPosition(3));

        var sortedWinners = SortTeams(winners);
        var sortedRunnersUp = SortTeams(runnersUp);
        var bestThirds = SortTeams(thirds).Take(8); // Solo los mejores 8 terceros

        var qualifiedTeams = sortedWinners
            .Concat(sortedRunnersUp)
            .Concat(bestThirds);

        return qualifiedTeams.Select(TeamMapper.FromEntity);
    }

    /// <summary>
    /// Ordena una colección de equipos basándose en las reglas de desempate
    /// (Puntos > Diferencia de Goles > Goles a Favor).
    /// </summary>
    private static IEnumerable<Team> SortTeams(IEnumerable<Team> teams)
    {
        return teams
            .OrderByDescending(t => t.Points)
            .ThenByDescending(t => t.GoalDifference)
            .ThenByDescending(t => t.GoalsInFavor);
    }
}