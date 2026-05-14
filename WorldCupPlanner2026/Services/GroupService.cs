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

    // RED: Casos de prueba pendientes para GetQualifiedTeams:
    //
    // GetQualifiedTeams_With12FullGroups_ShouldReturn32Teams
    //   - 12 grupos, cada uno con >=3 equipos y partidos jugados
    //   - Assert.AreEqual(32, result.Count)
    //
    // GetQualifiedTeams_WithLessThan12Groups_ShouldThrowInvalidOperationException
    //   - Solo se crean 11 grupos
    //   - Assert.ThrowsException<InvalidOperationException>(() => ...)
    //
    // GetQualifiedTeams_WhenGroupHasLessThan3Teams_ShouldThrowInvalidOperationException
    //   - 12 grupos, pero alguno tiene solo 2 equipos (no se puede obtener 3ra posicion)
    //   - Assert.ThrowsException<InvalidOperationException>(() => ...)
    //
    // GetQualifiedTeams_ShouldPlaceBest4WinnersFirst
    //   - 12 grupos con resultados deterministas
    //   - Los 4 ganadores con mas puntos/GD/GF deben estar en indices [0..3]
    //
    // GetQualifiedTeams_ShouldPlaceWinnersBeforeRunnersUp
    //   - Indices [0..11] deben ser equipos que terminaron 1ros en su grupo
    //
    // GetQualifiedTeams_ShouldPlaceRunnersUpBeforeThirds
    //   - Indices [12..23] deben ser segundos, indices [24..31] deben ser terceros
    //
    // GetQualifiedTeams_ShouldSelectOnly8BestThirds
    //   - De 12 terceros, solo los 8 mejores deben estar en la lista final
    //   - Los 4 peores terceros NO deben aparecer
    //
    // GetQualifiedTeams_WhenTiedOnPoints_ShouldOrderByGoalDifference
    //   - 2 ganadores con 9pts, uno con GD+10 y otro con GD+5
    //   - El de GD+10 debe aparecer primero
    //
    // GetQualifiedTeams_WhenTiedOnPointsAndGD_ShouldOrderByGoalsFor
    //   - 2 ganadores con 9pts y GD+5, uno con GF 10 y otro con GF 6
    //   - El de GF 10 debe aparecer primero
    //
    public IEnumerable<TeamResponse> GetQualifiedTeams()
    {
        throw new NotImplementedException("GetQualifiedTeams - RED phase: implementar en GREEN.");
    }
}
