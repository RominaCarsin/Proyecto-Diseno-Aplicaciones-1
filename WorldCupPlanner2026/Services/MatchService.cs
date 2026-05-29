using Domain;
using DTOs;
using IRepository;
using IServices;
using Services.Mappers;

namespace Services;

public class MatchService : IMatchService
{
    private const string MatchNotFoundErrorMessage = "No se encontró el partido.";
    private const string MatchRequestRequiredErrorMessage = "La solicitud de partido no puede ser nula.";
    private const string StadiumRequestRequiredErrorMessage = "La solicitud de estadio no puede ser nula.";
    private const string GroupNotFoundErrorMessage = "No se encontró el grupo.";
    private const string MaxMatchesPerDayErrorMessage = "El estadio no puede tener más de 3 partidos por día.";
    private const string MinHoursBetweenMatchesErrorMessage = "Los partidos en el mismo estadio deben tener al menos 4 horas de diferencia.";

    private readonly IRepository<Match> _repository;
    private readonly IRepository<Group> _groupRepository;
    private readonly IAuditService _auditService;
    private readonly ISessionService _sessionService;

    public MatchService(IRepository<Match> repository, IRepository<Group> groupRepository, IAuditService auditService, ISessionService sessionService)
    {
        _repository = repository;
        _groupRepository = groupRepository;
        _auditService = auditService;
        _sessionService = sessionService;
    }

    public void AddMatch(MatchRequest matchRequest)
    {
        ValidateNotNull(matchRequest, MatchRequestRequiredErrorMessage);

        var match = MatchMapper.ToEntity(matchRequest);
        ValidateStadiumAvailability(null, match.Stadium, match.Date);

        _repository.Add(match);

        if (matchRequest.Group.HasValue)
            AddMatchToGroup(match, matchRequest.Group.Value);

        LogAction("Match created", 
            $"Match added: {match.HomeTeam.Name} vs {match.AwayTeam.Name} at {match.Stadium.Name} on {match.Date}");
    }

    public IEnumerable<MatchResponse> GetMatches()
    {
        return _repository.GetAll().Select(MatchMapper.ToResponse);
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
        
        LogAction("Match result set", 
            $"Result for match {matchId} ({match.HomeTeam.Name} vs {match.AwayTeam.Name}) set to {homeGoals}-{awayGoals}");
    }

    public void UpdateMatchDate(int matchId, DateTime newDate)
    {
        var match = FindMatchByIdOrThrow(matchId);

        ValidateStadiumAvailability(matchId, match.Stadium, newDate);

        var oldDate = match.Date;
        match.UpdateDate(newDate);
        
        LogAction("Match date updated", 
            $"Match {matchId} date changed from {oldDate} to {newDate}");
    }

    public void FinishMatch(int matchId)
    {
        var match = FindMatchByIdOrThrow(matchId);
        match.Finish();
        
        LogAction("Match finished", $"Match {matchId} has been marked as finished");
    }

    public void UpdateMatchStadium(int matchId, StadiumRequest newStadiumRequest)
    {
        ValidateNotNull(newStadiumRequest, StadiumRequestRequiredErrorMessage);

        var match = FindMatchByIdOrThrow(matchId);
        var stadium = StadiumMapper.ToEntity(newStadiumRequest);

        ValidateStadiumAvailability(matchId, stadium, match.Date);

        var oldStadiumName = match.Stadium.Name;
        match.UpdateStadium(stadium);
        
        LogAction("Match stadium updated", 
            $"Match {matchId} stadium changed from {oldStadiumName} to {stadium.Name}");
    }
    

    public IEnumerable<StadiumRequest> GetAvailableStadiumsForMatch(int matchId, IEnumerable<StadiumRequest> candidateStadiums)
    {
        var match = FindMatchByIdOrThrow(matchId);

        return candidateStadiums.Where(stadiumRequest =>
        {
            try
            {
                var stadium = StadiumMapper.ToEntity(stadiumRequest);
                ValidateStadiumAvailability(matchId, stadium, match.Date);
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        });
    }

    private void AddMatchToGroup(Match match, char groupLabel)
    {
        var group = _groupRepository.GetAll()
            .FirstOrDefault(g => g.Label == groupLabel)
            ?? throw new ArgumentException(GroupNotFoundErrorMessage);

        group.AddMatch(match);
    }

    private Match FindMatchByIdOrThrow(int matchId) =>
        _repository.GetAll().FirstOrDefault(match => match.MatchId == matchId)
            ?? throw new ArgumentException(MatchNotFoundErrorMessage);

    private void ValidateStadiumAvailability(int? excludeMatchId, Stadium  stadium, DateTime matchDate)
    {
        var matchesInSameStadiumAndDay = _repository.GetAll()
            .Where(m => excludeMatchId == null || m.MatchId != excludeMatchId.Value)
            .Where(m => m.Stadium.Name == stadium.Name)
            .Where(m => m.Date.Date    == matchDate.Date)
            .ToList();

        if (matchesInSameStadiumAndDay.Count >= 3)
            throw new InvalidOperationException(MaxMatchesPerDayErrorMessage);

        bool tooClose = matchesInSameStadiumAndDay.Any(m =>
            Math.Abs((m.Date - matchDate).TotalHours) < 4);

        if (tooClose)
            throw new InvalidOperationException(MinHoursBetweenMatchesErrorMessage);
    }


    private static void ValidateNotNull<T>(T? value, string errorMessage)
    {
        if (value is null)
            throw new ArgumentException(errorMessage);
    }

    private void LogAction(string action, string details)
    {
        var actorEmail = _sessionService.GetCurrentUserEmail() ?? "system";
        _auditService.Log(actorEmail, action, details);
    }
}