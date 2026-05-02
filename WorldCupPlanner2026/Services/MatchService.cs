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

    public MatchService(IRepository<Match> repository)
    {
        _repository = repository;
    }

    public void AddMatch(MatchRequest request)
    {
        ValidateMatchRequest(request);

        var match = MatchMapper.ToEntity(request);

        _repository.Add(match);
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
    }

    public void UpdateMatchDate(int id, DateTime newDate)
    {
        var match = FindMatchById(id);

        ExecuteMatchUpdate(() => match.UpdateDate(newDate));
    }

    public void UpdateMatchStadium(int id, StadiumRequest newStadium)
    {
        ValidateStadiumRequest(newStadium);

        var match = FindMatchById(id);
        var stadium = StadiumMapper.ToEntity(newStadium);

        ExecuteMatchUpdate(() => match.UpdateStadium(stadium));
    }

    public void LockMatchEdition(int id)
    {
        var match = FindMatchById(id);

        match.LockEdition();
    }

    public void LockMatchesByPhase(string phase)
    {
        var tournamentPhase = ParseTournamentPhase(phase);

        foreach (var match in GetMatchesByPhase(tournamentPhase))
        {
            match.LockEdition();
        }
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