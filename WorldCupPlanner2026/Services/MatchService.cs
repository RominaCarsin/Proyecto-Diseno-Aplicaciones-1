using Domain;
using DTOs;
using IRepository;
using IServices;
using Services.Mappers;

namespace Services;

public class MatchService : IMatchService
{
    private const string RequiredMatchError = "Match cannot be null.";
    private const string RequiredStadiumError = "Stadium cannot be null.";
    private const string MatchNotFoundError = "Match was not found.";

    private readonly IRepository<Match> _repository;
    private readonly IAuditService _auditService;
    private readonly ISessionService _sessionService;

    public MatchService(IRepository<Match> repository, IAuditService auditService, ISessionService sessionService)
    {
        _repository = repository;
        _auditService = auditService;
        _sessionService = sessionService;
    }

    public void AddMatch(MatchRequest request)
    {
        ValidateMatchRequest(request);

        var match = MatchMapper.ToEntity(request);

        _repository.Add(match);
        LogAction("Match created",
            $"Match {match.Id} created: {match.HomeTeam.Name} vs {match.AwayTeam.Name} on {match.Date:O} at {match.Stadium.Name}");
    }

    public IEnumerable<MatchResponse> GetMatches()
    {
        return _repository.GetAll()
            .Select(MatchMapper.ToResponse);
    }

    public MatchResponse GetMatchById(int id)
    {
        var match = FindMatchById(id);

        return MatchMapper.ToResponse(match);
    }

    public void SetMatchResult(int id, int homeGoals, int awayGoals)
    {
        var match = FindMatchById(id);

        ExecuteMatchUpdate(() => match.SetResult(homeGoals, awayGoals));
        LogAction("Match result updated",
            $"Match {match.Id} result set to {match.HomeTeam.Name} {homeGoals}-{awayGoals} {match.AwayTeam.Name}");
    }

    public void UpdateMatchDate(int id, DateTime newDate)
    {
        var match = FindMatchById(id);

        ExecuteMatchUpdate(() => match.UpdateDate(newDate));
        LogAction("Match date updated",
            $"Match {match.Id} date updated to {newDate:O}");
    }

    public void UpdateMatchStadium(int id, StadiumRequest newStadium)
    {
        ValidateStadiumRequest(newStadium);

        var match = FindMatchById(id);
        var stadium = StadiumMapper.ToEntity(newStadium);

        ExecuteMatchUpdate(() => match.UpdateStadium(stadium));
        LogAction("Match stadium updated",
            $"Match {match.Id} stadium updated to {stadium.Name} ({stadium.City})");
    }

    public void LockMatchEdition(int id)
    {
        var match = FindMatchById(id);

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

    private Match FindMatchById(int id)
    {
        var match = _repository.GetAll()
            .FirstOrDefault(currentMatch => currentMatch.Id == id);

        if (match is null)
        {
            throw new ArgumentException(MatchNotFoundError);
        }

        return match;
    }

    private IEnumerable<Match> GetMatchesByPhase(TournamentPhase phase)
    {
        return _repository.GetAll()
            .Where(match => match.Phase == phase);
    }

    private static void ValidateMatchRequest(MatchRequest request)
    {
        if (request is null)
        {
            throw new ArgumentException(RequiredMatchError);
        }
    }

    private static void ValidateStadiumRequest(StadiumRequest stadium)
    {
        if (stadium is null)
        {
            throw new ArgumentException(RequiredStadiumError);
        }
    }

    private static TournamentPhase ParseTournamentPhase(string phase)
    {
        return Enum.Parse<TournamentPhase>(phase);
    }

    private static void ExecuteMatchUpdate(Action updateAction)
    {
        try
        {
            updateAction();
        }
        catch (Exception exception)
        {
            throw new ArgumentException(exception.Message);
        }
    }
}