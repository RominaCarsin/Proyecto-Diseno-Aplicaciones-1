using Domain;
using DTOs;
using IRepository;
using IServices;
using Repository;
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

    private static readonly DateTime ValidMatchDate = new(2026, 6, 1);
    private static readonly DateTime UpdatedMatchDate = new(2026, 6, 10);

    private IMatchService _matchService = null!;

    [TestInitialize]
    public void Setup()
    {
        IRepository<Match> repository = new Repository<Match>();
        _matchService = new MatchService(repository);
    }

    // ─── AddMatch ─────────────────────────────────────────────────────────────

    [TestMethod]
    public void AddMatch_WithValidRequest_ShouldPersistOneMatch()
    {
        var request = BuildValidMatchRequest();

        _matchService.AddMatch(request);

        Assert.AreEqual(1, _matchService.GetAllMatches().Count());
    }

    [TestMethod]
    public void AddMatch_WithNullRequest_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _matchService.AddMatch(null!));
    }

    // ─── GetMatchById ─────────────────────────────────────────────────────────

    [TestMethod]
    public void GetMatchById_WithExistingId_ShouldReturnMatchWithExpectedProperties()
    {
        var savedMatch = AddValidMatchAndGetResponse();

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

    // ─── SetMatchResult ───────────────────────────────────────────────────────

    [TestMethod]
    public void SetMatchResult_WithHomeTeamScoringMore_ShouldSetHomeTeamAsWinner()
    {
        var savedMatch = AddValidMatchAndGetResponse();
        int homeGoals = 3, awayGoals = 1;

        _matchService.SetMatchResult(savedMatch.Id, homeGoals, awayGoals);

        var result = _matchService.GetMatchById(savedMatch.Id);
        Assert.AreEqual(homeGoals, result.HomeGoals);
        Assert.AreEqual(awayGoals, result.AwayGoals);
        Assert.AreEqual(HomeTeamName, result.Winner!.Name);
    }

    [TestMethod]
    public void SetMatchResult_WhenMatchIsLocked_ShouldThrowInvalidOperationException()
    {
        var savedMatch = AddValidMatchAndGetResponse();
        _matchService.LockMatchEdition(savedMatch.Id);

        Assert.ThrowsException<InvalidOperationException>(() =>
            _matchService.SetMatchResult(savedMatch.Id, 1, 0));
    }

    // ─── UpdateMatchDate ──────────────────────────────────────────────────────

    [TestMethod]
    public void UpdateMatchDate_WithValidDate_ShouldReplaceMatchDate()
    {
        var savedMatch = AddValidMatchAndGetResponse();

        _matchService.UpdateMatchDate(savedMatch.Id, UpdatedMatchDate);

        var result = _matchService.GetMatchById(savedMatch.Id);
        Assert.AreEqual(UpdatedMatchDate, result.Date);
    }

    [TestMethod]
    public void UpdateMatchDate_WithMinValueDate_ShouldThrowArgumentException()
    {
        var savedMatch = AddValidMatchAndGetResponse();

        Assert.ThrowsException<ArgumentException>(() =>
            _matchService.UpdateMatchDate(savedMatch.Id, DateTime.MinValue));
    }

    [TestMethod]
    public void UpdateMatchDate_WhenMatchIsLocked_ShouldThrowInvalidOperationException()
    {
        var savedMatch = AddValidMatchAndGetResponse();
        _matchService.LockMatchEdition(savedMatch.Id);

        Assert.ThrowsException<InvalidOperationException>(() =>
            _matchService.UpdateMatchDate(savedMatch.Id, UpdatedMatchDate));
    }

    // ─── UpdateMatchStadium ───────────────────────────────────────────────────

    [TestMethod]
    public void UpdateMatchStadium_WithValidStadium_ShouldReplaceMatchStadium()
    {
        var savedMatch = AddValidMatchAndGetResponse();
        var alternativeStadium = BuildStadiumRequest(
            name: AlternativeStadiumName,
            city: AlternativeStadiumCity,
            capacity: 78000);

        _matchService.UpdateMatchStadium(savedMatch.Id, alternativeStadium);

        var result = _matchService.GetMatchById(savedMatch.Id);
        Assert.AreEqual(AlternativeStadiumName, result.Stadium.Name);
        Assert.AreEqual(AlternativeStadiumCity, result.Stadium.City);
    }

    [TestMethod]
    public void UpdateMatchStadium_WithNullStadiumRequest_ShouldThrowArgumentException()
    {
        var savedMatch = AddValidMatchAndGetResponse();

        Assert.ThrowsException<ArgumentException>(() =>
            _matchService.UpdateMatchStadium(savedMatch.Id, null!));
    }

    [TestMethod]
    public void UpdateMatchStadium_WithNonExistingMatchId_ShouldThrowArgumentException()
    {
        var alternativeStadium = BuildStadiumRequest(
            name: AlternativeStadiumName,
            city: AlternativeStadiumCity);

        Assert.ThrowsException<ArgumentException>(() =>
            _matchService.UpdateMatchStadium(999, alternativeStadium));
    }

    [TestMethod]
    public void UpdateMatchStadium_WhenMatchIsLocked_ShouldThrowInvalidOperationException()
    {
        var savedMatch = AddValidMatchAndGetResponse();
        _matchService.LockMatchEdition(savedMatch.Id);
        var alternativeStadium = BuildStadiumRequest(
            name: AlternativeStadiumName,
            city: AlternativeStadiumCity);

        Assert.ThrowsException<InvalidOperationException>(() =>
            _matchService.UpdateMatchStadium(savedMatch.Id, alternativeStadium));
    }

    // ─── LockMatchEdition ─────────────────────────────────────────────────────

    [TestMethod]
    public void LockMatchEdition_WithExistingMatch_ShouldSetIsLockedToTrue()
    {
        var savedMatch = AddValidMatchAndGetResponse();

        _matchService.LockMatchEdition(savedMatch.Id);

        var result = _matchService.GetMatchById(savedMatch.Id);
        Assert.IsTrue(result.IsLocked);
    }

    // ─── Helpers ─────────────────────────────────────────────────────────────

    private MatchResponse AddValidMatchAndGetResponse()
    {
        _matchService.AddMatch(BuildValidMatchRequest());
        return _matchService.GetAllMatches().First();
    }

    private static MatchRequest BuildValidMatchRequest() =>
        new MatchRequest
        {
            Date = ValidMatchDate,
            Stadium = BuildStadiumRequest(MainStadiumName, MainStadiumCity),
            HomeTeam = BuildTeamRequest(HomeTeamName),
            AwayTeam = BuildTeamRequest(AwayTeamName)
        };

    private static StadiumRequest BuildStadiumRequest(
        string name,
        string city,
        string description = "Historic stadium",
        int capacity = 60000) =>
        new StadiumRequest
        {
            Name = name,
            City = city,
            Description = description,
            Capacity = capacity
        };

    private static TeamRequest BuildTeamRequest(string name) =>
        new TeamRequest
        {
            Name = name,
            ConfederationName = "CONMEBOL",
            RankingFifa = 1800
        };
}