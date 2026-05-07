using Domain;
using DTOs;
using IRepository;
using IServices;
using Repository;
using IServices;
using Services;

namespace Services.Tests;

[TestClass]
public class MatchServiceTests
{
    private const string HomeTeamName = "ARGENTINA";
    private const string AwayTeamName = "BRAZIL";
      
    private const string MainStadiumName = "CENTENARIO";
    private const string MainStadiumCity = "MONTEVIDEO";

    private const string AlternativeStadiumName = "MARACANA";
    private const string AlternativeStadiumCity = "RIO DE JANEIRO";

    private static readonly DateTime GroupStageDate = new(2026, 6, 1);
    private static readonly DateTime UpdatedDate = new(2026, 6, 10);
    private static readonly DateTime LockedUpdatedDate = new(2026, 6, 20);

    private IMatchService _matchService = null!;

    [TestInitialize]
    public void Setup()
    {
        IRepository<Match> repository = new Repository<Match>();
        _matchService = new MatchService(repository);
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

    [TestMethod]
    public void GetMatchById_WithExistingId_ShouldReturnMatch()
    {
        var savedMatch = AddValidGroupStageMatch();

        var result = _matchService.GetMatchById(savedMatch.Id);

        Assert.AreEqual(savedMatch.Id, result.Id);
        Assert.AreEqual(HomeTeamName, result.HomeTeam.Name);
        Assert.AreEqual(AwayTeamName, result.AwayTeam.Name);
    }

    [TestMethod]
    public void GetMatchById_WithNonExistingId_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _matchService.GetMatchById(999));
    }

    [TestMethod]
    public void SetMatchResult_WithExistingMatch_ShouldUpdateResult()
    {
        var savedMatch = AddValidGroupStageMatch();

        _matchService.SetMatchResult(savedMatch.Id, 3, 1);

        var result = _matchService.GetMatchById(savedMatch.Id);

        Assert.AreEqual(3, result.HomeGoals);
        Assert.AreEqual(1, result.AwayGoals);
        Assert.AreEqual(HomeTeamName, result.Winner!.Name);
    }

    [TestMethod]
    public void UpdateMatchDate_WithExistingMatch_ShouldUpdateDate()
    {
        var savedMatch = AddValidGroupStageMatch();

        _matchService.UpdateMatchDate(savedMatch.Id, UpdatedDate);

        var result = _matchService.GetMatchById(savedMatch.Id);

        Assert.AreEqual(UpdatedDate, result.Date);
    }

    [TestMethod]
    public void UpdateMatchStadium_WithExistingMatch_ShouldUpdateStadium()
    {
        var savedMatch = AddValidGroupStageMatch();
        var newStadium = CreateAlternativeStadiumRequest();

        _matchService.UpdateMatchStadium(savedMatch.Id, newStadium);

        var result = _matchService.GetMatchById(savedMatch.Id);

        Assert.AreEqual(AlternativeStadiumName, result.Stadium.Name);
        Assert.AreEqual(AlternativeStadiumCity, result.Stadium.City);
    }

    [TestMethod]
    public void LockMatchEdition_WithExistingMatch_ShouldLockMatch()
    {
        var savedMatch = AddValidGroupStageMatch();

        _matchService.LockMatchEdition(savedMatch.Id);

        var result = _matchService.GetMatchById(savedMatch.Id);

        Assert.IsTrue(result.IsLocked);
    }

    private MatchResponse AddValidGroupStageMatch()
    {
        var request = CreateValidGroupStageMatchRequest();

        _matchService.AddMatch(request);

        return _matchService.GetMatches().First();
    }

    private static StadiumRequest CreateValidStadiumRequest()
    {
        return new StadiumRequest
        {
            Name = MainStadiumName,
            City = MainStadiumCity,
            Description = "Historic stadium",
            Capacity = 60000
        };
    }

    private static StadiumRequest CreateAlternativeStadiumRequest()
    {
        return new StadiumRequest
        {
            Name = AlternativeStadiumName,
            City = AlternativeStadiumCity,
            Description = "Historic stadium",
            Capacity = 78000
        };
    }

    private static TeamRequest CreateValidHomeTeamRequest()
    {
        return new TeamRequest
        {
            Name = HomeTeamName,
            ConfederationName = "CONMEBOL",
            RankingFifa = 1800
        };
    }

    private static TeamRequest CreateValidAwayTeamRequest()
    {
        return new TeamRequest
        {
            Name = AwayTeamName,
            ConfederationName = "CONMEBOL",
            RankingFifa = 1750
        };
    }

    private static MatchRequest CreateValidGroupStageMatchRequest()
    {
        return new MatchRequest
        {
            Date = GroupStageDate,
            Stadium = CreateValidStadiumRequest(),
            HomeTeam = CreateValidHomeTeamRequest(),
            AwayTeam = CreateValidAwayTeamRequest()
        };
    }


    
    [TestMethod]
    public void UpdateMatchStadium_WithNullStadiumRequest_ShouldThrowArgumentException()
    {
        var savedMatch = AddValidGroupStageMatch();

        Assert.ThrowsException<ArgumentException>(() =>
            _matchService.UpdateMatchStadium(savedMatch.Id, null!));
    }
}