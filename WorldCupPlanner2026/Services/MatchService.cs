using Domain;
using DTOs;
using IRepository;
using IServices;
using Services.Mappers;

namespace Services;

public class MatchService 
{
    private const string MatchNotFoundErrorMessage = "Match was not found.";
    private const string MatchRequestRequiredErrorMessage = "Match request cannot be null.";
    private const string StadiumRequestRequiredErrorMessage = "Stadium request cannot be null.";

    private readonly IRepository<Match> _repository;

    public MatchService(IRepository<Match> repository)
    {
        _repository = repository;
    }

    public void AddMatch(MatchRequest matchRequest)
    {
        ValidateNotNull(matchRequest, MatchRequestRequiredErrorMessage);

        var match = MatchMapper.ToEntity(matchRequest);

        _repository.Add(match);
    }

    public IEnumerable<MatchResponse> GetAllMatches()
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
    }

    public void UpdateMatchDate(int matchId, DateTime newDate)
    {
        var match = FindMatchByIdOrThrow(matchId);

        match.UpdateDate(newDate);
    }

    public void UpdateMatchStadium(int matchId, StadiumRequest newStadiumRequest)
    {
        ValidateNotNull(newStadiumRequest, StadiumRequestRequiredErrorMessage);

        var match = FindMatchByIdOrThrow(matchId);
        var stadium = StadiumMapper.ToEntity(newStadiumRequest);

        match.UpdateStadium(stadium);
    }

    public void LockMatchEdition(int matchId)
    {
        var match = FindMatchByIdOrThrow(matchId);

        match.LockEdition();
    }
    

    private Match FindMatchByIdOrThrow(int matchId)
    {
        return _repository.GetAll()
            .FirstOrDefault(match => match.Id == matchId)
            ?? throw new ArgumentException(MatchNotFoundErrorMessage);
    }

    private static void ValidateNotNull<T>(T? value, string errorMessage)
    {
        if (value is null)
            throw new ArgumentException(errorMessage);
    }
}