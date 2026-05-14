using Domain;
using DTOs;
using IRepository;
using IServices;
using Services.Mappers;

namespace Services;

public class TeamService : ITeamService
{
    private const int RequiredTeamsCount = 48;
    private const int FirstFifaRanking = 300;
    private const int LastFifaRanking = 2500;

    private const string TeamNotFoundErrorMessage = "No se encontró el equipo.";
    private const string TeamNameAlreadyExistsErrorMessage = "Ya existe un equipo con este nombre.";
    
    private readonly IRepository<Team> _repository;
    private readonly IConfederationService _confederationService;
    private readonly IAuditService _auditService;
    private readonly ISessionService _sessionService;

    public TeamService(
        IRepository<Team> repository,
        IConfederationService confederationService,
        IAuditService auditService,
        ISessionService sessionService)
    {
        _repository = repository;
        _confederationService = confederationService;
        _auditService = auditService;
        _sessionService = sessionService;
    }

    public void AddTeam(TeamRequest teamRequest)
    {
        EnsureTeamNameIsNotTaken(teamRequest.Name);

        var team = TeamMapper.ToEntity(teamRequest);
        _confederationService.AddTeamToConfederation(team);
        _repository.Add(team);

        LogAction("Team created",
            $"Team '{teamRequest.Name}' added to {teamRequest.ConfederationName} " +
            $"with FIFA ranking {teamRequest.RankingFifa}");
    }

    public void UpdateTeam(string currentName, TeamRequest updatedTeamData)
    {
        var team = FindTeamByNameOrThrow(currentName);

        EnsureTeamNameIsNotTakenByAnotherTeam(currentName, updatedTeamData.Name);

        team.UpdateName(updatedTeamData.Name);
        team.UpdateRankingFifa(updatedTeamData.RankingFifa);

        bool isChangingConfederation = team.ConfederationName != updatedTeamData.ConfederationName;
        if (isChangingConfederation)
        {
            team.UpdateConfederationName(updatedTeamData.ConfederationName);
            _confederationService.UpdateTeamConfederation(team, updatedTeamData.ConfederationName);
        }

        LogAction("Team updated",
            $"Team '{currentName}' updated. New name: '{updatedTeamData.Name}', " +
            $"confederation: {updatedTeamData.ConfederationName}, ranking: {updatedTeamData.RankingFifa}");
    }

    public void DeleteTeam(string name)
    {
        var team = FindTeamByNameOrThrow(name);
        _confederationService.RemoveTeamFromConfederation(team);
        _repository.Delete(team);

        LogAction("Team deleted", $"Team '{name}' was removed from the system");
    }

    public void AutoCompleteTeams(int seed)
    {
        var random = new Random(seed);

        foreach (var confederation in _confederationService.GetAll())
        {
            int remainingGlobalSlots = RequiredTeamsCount - _repository.GetAll().Count();
            if (remainingGlobalSlots <= 0)
                return;

            int teamsToCreate = Math.Min(confederation.RemainingSlots, remainingGlobalSlots);
            AddFictionalTeamsToConfederation(confederation.Name, teamsToCreate, random);
        }
    }

    public IEnumerable<TeamResponse> GetTeams() =>
        _repository.GetAll().Select(TeamMapper.FromEntity);

    public IEnumerable<TeamResponse> GetTeamsOrdered(int seed)
    {
        var orderedTeams = _repository.GetAll()
            .OrderByDescending(team => team.RankingFifa)
            .ToList();

        ShuffleTiedRankingGroups(orderedTeams, seed);

        return orderedTeams.Select(TeamMapper.FromEntity);
    }

    private Team? FindByName(string name) =>
        _repository.GetAll()
            .FirstOrDefault(t => t.Name == Team.Normalize(name));

    private Team FindTeamByNameOrThrow(string name) =>
        FindByName(name) ?? throw new ArgumentException(TeamNotFoundErrorMessage);

    private void EnsureTeamNameIsNotTaken(string name)
    {
        if (FindByName(name) is not null)
            throw new ArgumentException(TeamNameAlreadyExistsErrorMessage);
    }

    private void EnsureTeamNameIsNotTakenByAnotherTeam(string currentName, string newName)
    {
        var teamWithNewName = FindByName(newName);
        if (teamWithNewName is not null && teamWithNewName.Name != currentName)
            throw new ArgumentException(TeamNameAlreadyExistsErrorMessage);
    }

    private void AddFictionalTeamsToConfederation(
        string confederationName,
        int teamsToCreate,
        Random random)
    {
        for (int index = 1; index <= teamsToCreate; index++)
        {
            var request = new TeamRequest
            {
                Name = $"{confederationName}_{index:00}",
                ConfederationName = confederationName,
                RankingFifa = random.Next(FirstFifaRanking, LastFifaRanking + 1)
            };
            AddTeam(request);
        }
    }

    private void ShuffleTiedRankingGroups(IList<Team> teams, int seed)
    {
        var shuffleService = new ShuffleService(seed);
        int start = 0;

        while (start < teams.Count)
        {
            int end = FindEndOfTiedGroup(teams, start);

            if (end - start > 1)
                shuffleService.ShuffleRange(teams, start, end);

            start = end;
        }
    }

    private static int FindEndOfTiedGroup(IList<Team> teams, int start)
    {
        int end = start + 1;
        while (end < teams.Count && teams[end].RankingFifa == teams[start].RankingFifa)
            end++;
        return end;
    }

    private void LogAction(string action, string details)
    {
        var actorEmail = _sessionService.GetCurrentUserEmail() ?? "system";
        _auditService.Log(actorEmail, action, details);
    }
}