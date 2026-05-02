using Domain;
using Domain.Exceptions;
using DTOs;
using IRepository;
using IServices;
using Repository;
using Services;

namespace Services.Tests;

[TestClass]
public class MatchServiceTests
{
    private MatchService _matchService = null!;

    [TestInitialize]
    public void Setup()
    {
        IRepository<Match> repository = new Repository<Match>();
        _matchService = new MatchService(repository);
        
    }
    
    private StadiumRequest CreateValidStadiumRequest()
    {
        return new StadiumRequest
        {
            Name = "CENTENARIO",
            City = "MONTEVIDEO",
            Description = "Historic stadium",
            Capacity = 60000
        };
    }

    private StadiumRequest CreateAnotherValidStadiumRequest()
    {
        return new StadiumRequest
        {
            Name = "MARACANA",
            City = "RIO DE JANEIRO",
            Description = "Historic stadium",
            Capacity = 78000
        };
    }

    private TeamRequest CreateValidHomeTeamRequest()
    {
        return new TeamRequest
        {
            Name = "ARGENTINA",
            ConfederationName = "CONMEBOL",
            RankingFifa = 1800
        };
    }

    private TeamRequest CreateValidAwayTeamRequest()
    {
        return new TeamRequest
        {
            Name = "BRAZIL",
            ConfederationName = "CONMEBOL",
            RankingFifa = 1750
        };
    }

    private MatchRequest CreateValidGroupStageMatchRequest()
    {
        return new MatchRequest
        {
            Date = new DateTime(2026, 6, 1),
            Stadium = CreateValidStadiumRequest(),
            HomeTeam = CreateValidHomeTeamRequest(),
            AwayTeam = CreateValidAwayTeamRequest(),
            Group = 'A',
            Phase = "GroupStage"
        };
    }

    private MatchRequest CreateValidKnockoutMatchRequest()
    {
        return new MatchRequest
        {
            Date = new DateTime(2026, 7, 1),
            Stadium = CreateValidStadiumRequest(),
            HomeTeam = CreateValidHomeTeamRequest(),
            AwayTeam = CreateValidAwayTeamRequest(),
            Group = null,
            Phase = "RoundOf32"
        };
    }
    
    [TestMethod]
    public void AddMatch_WithValidRequest_ShouldAddMatch()
    {
        var request = CreateValidGroupStageMatchRequest();

        _matchService.AddMatch(request);

        Assert.AreEqual(1, _matchService.GetMatches().Count());
    }
    
    [TestMethod]
    public void AddMatch_WithNullRequest_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _matchService.AddMatch(null!));
    }
    
    
}