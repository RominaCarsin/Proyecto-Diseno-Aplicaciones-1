using System.Text.RegularExpressions;
using IServices;

namespace Services;

using Domain;
using IRepository;

public class MatchService
{
    private readonly IRepository<Match> _repository;

    public MatchService(IRepository<Match> repository)
    {
        _repository = repository;
    }
}