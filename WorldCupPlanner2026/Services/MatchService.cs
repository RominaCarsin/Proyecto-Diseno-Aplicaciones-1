using System.Text.RegularExpressions;
using IServices;
using Domain;
using DTOs;
using Services.Mappers;


namespace Services;

using Domain;
using IRepository;

public class MatchService
{
    private readonly IRepository<Match> _repository;
    
    private const string RequiredMatchError = "Match cannot be null";

    private const string MatchNotFoundError = "Match was not found.";
    
    public MatchService(IRepository<Match> repository)
    {
        _repository = repository;
    }
    
    public void AddMatch(MatchRequest request)
    {
        if (request is null)
        {
            throw new ArgumentException(RequiredMatchError);
        }

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

    private Match FindMatchById(int id)
    {
        var match = _repository.GetAll()
            .FirstOrDefault(match => match.Id == id);

        if (match is null)
        {
            throw new ArgumentException(MatchNotFoundError);
        }

        return match;
    }
    
    public void SetMatchResult(int id, int homeGoals, int awayGoals)
    {
        var match = FindMatchById(id);

        match.SetResult(homeGoals, awayGoals);
    }
    
    public void UpdateMatchDate(int id, DateTime newDate)
    {
        var match = FindMatchById(id);

        match.UpdateDate(newDate);
    }
}