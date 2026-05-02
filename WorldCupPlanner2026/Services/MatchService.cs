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
}