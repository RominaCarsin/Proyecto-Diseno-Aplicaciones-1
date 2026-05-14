using Domain;
using DTOs;
using IServices;
using IRepository;
using Services.Mappers;

namespace Services;

public class TeamService : ITeamService
{
    private const int RequiredTeamsCount = 48;
    private const int FirstFifaRanking = 300;
    private const int LastFifaRanking = 2500;

    private readonly IRepository<Team> _repository;
    private readonly IConfederationService _confederationService;
    private readonly IAuditService _auditService;
    private readonly ISessionService _sessionService;

    public TeamService(IRepository<Team> repository, IConfederationService confederationService, IAuditService auditService, ISessionService sessionService)
    {
        _repository = repository;
        _confederationService = confederationService;
        _auditService = auditService;
        _sessionService = sessionService;
    }

    public void AddTeam(TeamRequest request)
    {
        CheckNameIsAvailable(request.Name);
        var team = TeamMapper.ToEntity(request);
        _confederationService.AddTeamToConfederation(team);
        _repository.Add(team);
        LogAction("Team created",
            $"Team '{request.Name}' added to {request.ConfederationName} with FIFA ranking {request.RankingFifa}");

    }

    public void UpdateTeam(string currentName, TeamRequest request)
    {
        var team = GetExistingTeamOrThrowArgumentException(currentName);
        CheckNameIsAvailableForUpdate(currentName, request.Name);

        team.UpdateName(request.Name);
        team.UpdateRankingFifa(request.RankingFifa);

        if (team.ConfederationName != request.ConfederationName)
        {
            team.UpdateConfederationName(request.ConfederationName);
            _confederationService.UpdateTeamConfederation(team, request.ConfederationName);
        }
        LogAction("Team updated",
            $"Team '{currentName}' updated. New name: '{request.Name}', confederation: {request.ConfederationName}, ranking: {request.RankingFifa}");
    }

    public void DeleteTeam(string name)
    {
        var team = GetExistingTeamOrThrowArgumentException(name);
        _confederationService.RemoveTeamFromConfederation(team);
        _repository.Delete(team);
        LogAction("Team deleted",
            $"Team '{name}' was removed from the system");
    }

    public void AutoCompleteTeams(int seed)
    {
        var random = new Random(seed);

        foreach (var confederation in _confederationService.GetAll())
        {
            var remainingGlobalSlots = RequiredTeamsCount - _repository.GetAll().Count();
            if (remainingGlobalSlots <= 0) return;

            var teamsToCreate = Math.Min(confederation.RemainingSlots, remainingGlobalSlots);
            AddFictionalTeams(confederation.Name, teamsToCreate, random);
        }
    }

    public IEnumerable<TeamResponse> GetTeams() =>
        _repository.GetAll().Select(TeamMapper.FromEntity);

    public IEnumerable<TeamResponse> GetTeamsOrdered(int seed)
    {
        var orderedTeams = _repository.GetAll()
            .OrderByDescending(team => team.RankingFifa)
            .ToList();

        ResolveRankingTies(orderedTeams, seed);

        return orderedTeams.Select(TeamMapper.FromEntity);
    }

    private void ResolveRankingTies(IList<Team> teams, int seed)
    {
        var shuffleService = CreateShuffleService(seed);
        var tieStartIndex = 0;

        while (tieStartIndex < teams.Count)
        {
            var tieEndIndex = FindTieEndIndex(teams, tieStartIndex);
            if (tieEndIndex - tieStartIndex > 1)
                shuffleService.ShuffleRange(teams, tieStartIndex, tieEndIndex);

            tieStartIndex = tieEndIndex;
        }
    }

    private int FindTieEndIndex(IList<Team> teams, int tieStartIndex)
    {
        var tieRanking = teams[tieStartIndex].RankingFifa;
        var tieEndIndex = tieStartIndex + 1;

        while (tieEndIndex < teams.Count && teams[tieEndIndex].RankingFifa == tieRanking)
            tieEndIndex++;

        return tieEndIndex;
    }

    private IServices.IShuffleService CreateShuffleService(int seed)
    {
        return new ShuffleService(seed);
    }

    private void AddFictionalTeams(string confederationName, int teamsToCreate, Random random)
    {
        for (var index = 1; index <= teamsToCreate; index++)
            AddTeam(BuildFictionalTeamRequest(confederationName, index, random));
    }

    private TeamRequest BuildFictionalTeamRequest(string confederationName, int index, Random random) => new()
    {
        Name = GenerateFictionalName(confederationName, index),
        ConfederationName = confederationName,
        RankingFifa = GenerateRandomRanking(random)
    };

    private string GenerateFictionalName(string confederationName, int index)
    {
        return $"{confederationName}_{index:00}";
    }

    private int GenerateRandomRanking(Random random)
    {
        return random.Next(FirstFifaRanking, LastFifaRanking + 1);
    }

    private Team? FindByName(string name) =>
        _repository.GetAll().FirstOrDefault(t => t.Name == Team.Normalized(name));

    private Team GetExistingTeamOrThrowArgumentException(string name) =>
        FindByName(name) ?? throw new ArgumentException("Team not found.");

    private void CheckNameIsAvailable(string name)
    {
        if (FindByName(name) != null)
            throw new ArgumentException("A team with this name already exists.");
    }

    private void CheckNameIsAvailableForUpdate(string currentName, string newName)
    {
        var teamWithNewName = FindByName(newName);
        if (teamWithNewName != null && teamWithNewName.Name != currentName)
            throw new ArgumentException("A team with this name already exists.");
    }

    private void LogAction(string action, string details)
    {
        var actorEmail = _sessionService.GetCurrentUserEmail() ?? "system";
        _auditService.Log(actorEmail, action, details);
    }
}
