using Domain;
using IRepository;
using IServices;
using Repository;
using Services;

namespace Services.Tests;

[TestClass]
public class MatchServiceTests
{
    private IMatchService _matchService = null!;

    [TestInitialize]
    public void Setup()
    {
        IRepository<Match> repository = new Repository<Match>();
        _matchService = new MatchService(repository);
        
    }
}