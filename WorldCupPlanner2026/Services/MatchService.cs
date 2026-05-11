using Domain;
using DTOs;
using IRepository;
using IServices;
using Services.Mappers;

namespace Services;

public class MatchService : IMatchService
{
    private const string MatchNotFoundErrorMessage = "Match was not found.";
    private const string MatchRequestRequiredErrorMessage = "Match request cannot be null.";
    private const string StadiumRequestRequiredErrorMessage = "Stadium request cannot be null.";
    private const string GroupNotFoundErrorMessage = "Group was not found.";

    private readonly IRepository<Match> _repository;
    private readonly IRepository<Group> _groupRepository;

    public MatchService(IRepository<Match> repository, IRepository<Group> groupRepository)
    {
        _repository = repository;
        _groupRepository = groupRepository;
    }

    public void AddMatch(MatchRequest matchRequest)
    {
        ValidateNotNull(matchRequest, MatchRequestRequiredErrorMessage);

        var match = MatchMapper.ToEntity(matchRequest);

        _repository.Add(match);

        if (matchRequest.Group.HasValue)
            AddMatchToGroup(match, matchRequest.Group.Value);
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
    
    public void LockMatchesByPhase(string phase)
    {
        _repository.GetAll()
            .ToList()
            .ForEach(match => match.LockEdition());
    }

    private void AddMatchToGroup(Match match, char groupLabel)
    {
        var group = _groupRepository.GetAll()
            .FirstOrDefault(g => g.Label == groupLabel)
            ?? throw new ArgumentException(GroupNotFoundErrorMessage);

        group.AddMatch(match);
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