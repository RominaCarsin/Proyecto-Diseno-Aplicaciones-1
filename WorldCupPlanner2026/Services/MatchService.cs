using Domain;
using DTOs;
using IRepository;
using IServices;
using Services.Mappers;

namespace Services;

public class MatchService : IMatchService
{
    private const string MatchNotFoundError = "Match was not found.";
    private const string MatchRequestRequiredError = "Match request cannot be null.";
    private const string StadiumRequestRequiredError = "Stadium request cannot be null.";
    private const string GroupNotFoundError = "Group was not found.";

    private readonly IRepository<Match> _repository;
    private readonly IRepository<Group> _groupRepository;
    private readonly IAuditService _auditService;
    private readonly ISessionService _sessionService;

    public MatchService(
        IRepository<Match> repository,
        IRepository<Group> groupRepository,
        IAuditService auditService,
        ISessionService sessionService)
    {
        _repository = repository;
        _groupRepository = groupRepository;
        _auditService = auditService;
        _sessionService = sessionService;
    }

    public void AddMatch(MatchRequest matchRequest)
    {
        ValidateNotNull(matchRequest, MatchRequestRequiredError);

        var match = MatchMapper.ToEntity(matchRequest, _groupRepository);

        _repository.Add(match);

        LogAction("Match created",
            $"Match {match.Id} created: {match.HomeTeam.Name} vs {match.AwayTeam.Name} on {match.Date:O} at {match.Stadium.Name}");
    }

    public IEnumerable<MatchResponse> GetMatches()
    {
        return _repository.GetAll()
            .Select(MatchMapper.ToResponse);
    }

    public MatchResponse GetMatchById(int matchId)
    {
        var match = FindMatchByIdOrThrow(matchId);
        return MatchMapper.ToResponse(match);
    }

    public void SetMatchResult(int matchId, int homeGoals, int awayGoals)
    {
        var match = FindMatchByIdOrThrow(matchId);

        match.SetResult(homeGoals, awayGoals);
        LogAction("Match result updated",
            $"Match {match.Id} result set to {match.HomeTeam.Name} {homeGoals}-{awayGoals} {match.AwayTeam.Name}");
    }

    public void UpdateMatchDate(int matchId, DateTime newDate)
    {
        var match = FindMatchByIdOrThrow(matchId);

        match.UpdateDate(newDate);
        LogAction("Match date updated",
            $"Match {match.Id} date updated to {newDate:O}");
    }

    public void UpdateMatchStadium(int matchId, StadiumRequest newStadiumRequest)
    {
        ValidateNotNull(newStadiumRequest, StadiumRequestRequiredError);

        var match = FindMatchByIdOrThrow(matchId);
        var stadium = StadiumMapper.ToEntity(newStadiumRequest);

        match.UpdateStadium(stadium);
        LogAction("Match stadium updated",
            $"Match {match.Id} stadium updated to {stadium.Name} ({stadium.City})");
    }

    public void LockMatchEdition(int matchId)
    {
        var match = FindMatchByIdOrThrow(matchId);
        match.LockEdition();
        LogAction("Match locked", $"Match {match.Id} edition was locked");
    }

    public void LockMatchesByPhase(string phase)
    {
        var tournamentPhase = ParseTournamentPhase(phase);

        foreach (var match in GetMatchesByPhase(tournamentPhase))
        {
            match.LockEdition();
        }

        LogAction("Matches locked by phase", $"Matches in phase {tournamentPhase} were locked");
    }

    private void LogAction(string action, string details)
    {
        var actorEmail = _sessionService.GetCurrentUserEmail() ?? "system";
        _auditService.Log(actorEmail, action, details);
    }

    private static TournamentPhase ParseTournamentPhase(string phase)
    {
        if (!Enum.TryParse<TournamentPhase>(phase, ignoreCase: true, out var tournamentPhase))
        {
            throw new ArgumentException($"Invalid tournament phase: '{phase}'.");
        }

        return tournamentPhase;
    }

    private IEnumerable<Match> GetMatchesByPhase(TournamentPhase phase)
    {
        return _repository.GetAll().Where(m => m.Phase == phase);
    }

    private Match FindMatchByIdOrThrow(int matchId)
    {
        return _repository.GetAll()
            .FirstOrDefault(match => match.Id == matchId)
            ?? throw new ArgumentException(MatchNotFoundError);
    }

    private static void ValidateNotNull<T>(T? value, string errorMessage)
    {
        if (value is null)
            throw new ArgumentException(errorMessage);
    }
}
