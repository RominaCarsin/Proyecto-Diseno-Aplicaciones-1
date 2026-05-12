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
    private IRepository<Group> _groupRepository = null!;

    [TestInitialize]
    public void Setup()
    {
        IRepository<Match> repository = new Repository<Match>();
        _groupRepository = new Repository<Group>();
        _matchService = new MatchService(repository, _groupRepository);
    }

    // ─── AddMatch ─────────────────────────────────────────────────────────────

    [TestMethod]
    public void AddMatch_WithValidRequest_ShouldPersistOneMatch()
    {
        var request = BuildValidMatchRequest();

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
    public void AddMatch_WithGroupStageAndExistingGroup_ShouldAddMatchToGroup()
    {
        var group = new Group('A');
        _groupRepository.Add(group);

        var request = BuildValidMatchRequest(group: 'A');
        _matchService.AddMatch(request);

        Assert.AreEqual(1, _matchService.GetMatches().Count());
    }

    [TestMethod]
    public void AddMatch_WithGroupStageAndNonExistingGroup_ShouldThrowArgumentException()
    {
        var request = BuildValidMatchRequest(group: 'Z');

        Assert.ThrowsException<ArgumentException>(() =>
            _matchService.AddMatch(request));
    }

    [TestMethod]
    public void AddMatch_WithGroupAndPhase_ShouldPersistGroupAndPhase()
    {
        var group = new Group('B');
        _groupRepository.Add(group);

        var request = BuildValidMatchRequest(group: 'B', phase: "GroupStage");
        _matchService.AddMatch(request);

        var result = _matchService.GetMatches().First();
        Assert.AreEqual('B', result.Group);
        Assert.AreEqual("GroupStage", result.Phase);
    }

    // ─── GetMatches ───────────────────────────────────────────────────────────

    [TestMethod]
    public void GetMatches_WithNoMatches_ShouldReturnEmptyList()
    {
        var result = _matchService.GetMatches();

        Assert.AreEqual(0, result.Count());
    }

    [TestMethod]
    public void GetMatches_WithMultipleMatches_ShouldReturnAll()
    {
        _matchService.AddMatch(BuildValidMatchRequest());
        _matchService.AddMatch(BuildValidMatchRequest(
            homeTeamName: "FRANCE",
            awayTeamName: "GERMANY"));

        Assert.AreEqual(2, _matchService.GetMatches().Count());
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
    public void SetMatchResult_WithAwayTeamScoringMore_ShouldSetAwayTeamAsWinner()
    {
        var savedMatch = AddValidMatchAndGetResponse();

        _matchService.SetMatchResult(savedMatch.Id, 0, 2);

        var result = _matchService.GetMatchById(savedMatch.Id);
        Assert.AreEqual(AwayTeamName, result.Winner!.Name);
    }

    [TestMethod]
    public void SetMatchResult_WithDraw_ShouldSetNullWinner()
    {
        var savedMatch = AddValidMatchAndGetResponse();

        _matchService.SetMatchResult(savedMatch.Id, 1, 1);

        var result = _matchService.GetMatchById(savedMatch.Id);
        Assert.IsNull(result.Winner);
    }

    [TestMethod]
    public void SetMatchResult_WhenMatchIsLocked_ShouldThrowInvalidOperationException()
    {
        var savedMatch = AddValidMatchAndGetResponse();
        _matchService.LockMatchEdition(savedMatch.Id);

        Assert.ThrowsException<InvalidOperationException>(() =>
            _matchService.SetMatchResult(savedMatch.Id, 1, 0));
    }

    [TestMethod]
    public void SetMatchResult_WithNonExistingId_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _matchService.SetMatchResult(999, 1, 0));
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

    [TestMethod]
    public void UpdateMatchDate_WithNonExistingId_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _matchService.UpdateMatchDate(999, UpdatedMatchDate));
    }

    // ─── UpdateMatchStadium ───────────────────────────────────────────────────

    [TestMethod]
    public void UpdateMatchStadium_WithValidStadium_ShouldReplaceMatchStadium()
    {
        var savedMatch = AddValidMatchAndGetResponse();
        var alternativeStadium = BuildStadiumRequest(AlternativeStadiumName, AlternativeStadiumCity, capacity: 78000);

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
        var alternativeStadium = BuildStadiumRequest(AlternativeStadiumName, AlternativeStadiumCity);

        Assert.ThrowsException<ArgumentException>(() =>
            _matchService.UpdateMatchStadium(999, alternativeStadium));
    }

    [TestMethod]
    public void UpdateMatchStadium_WhenMatchIsLocked_ShouldThrowInvalidOperationException()
    {
        var savedMatch = AddValidMatchAndGetResponse();
        _matchService.LockMatchEdition(savedMatch.Id);
        var alternativeStadium = BuildStadiumRequest(AlternativeStadiumName, AlternativeStadiumCity);

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

    [TestMethod]
    public void LockMatchEdition_WithNonExistingId_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _matchService.LockMatchEdition(999));
    }

    // ─── LockMatchesByPhase ───────────────────────────────────────────────────

    [TestMethod]
    public void LockMatchesByPhase_ShouldLockAllMatches()
    {
        _matchService.AddMatch(BuildValidMatchRequest());
        _matchService.AddMatch(BuildValidMatchRequest(homeTeamName: "FRANCE", awayTeamName: "GERMANY"));

        _matchService.LockMatchesByPhase("GroupStage");

        Assert.IsTrue(_matchService.GetMatches().All(m => m.IsLocked));
    }

    // ─── Helpers ─────────────────────────────────────────────────────────────

    private MatchResponse AddValidMatchAndGetResponse()
    {
        _matchService.AddMatch(BuildValidMatchRequest());
        return _matchService.GetMatches().First();
    }

    private static MatchRequest BuildValidMatchRequest(
        string homeTeamName = HomeTeamName,
        string awayTeamName = AwayTeamName,
        char? group = null,
        string phase = "GroupStage") =>
        new MatchRequest
        {
            Date = ValidMatchDate,
            Stadium = BuildStadiumRequest(MainStadiumName, MainStadiumCity),
            HomeTeam = BuildTeamRequest(homeTeamName),
            AwayTeam = BuildTeamRequest(awayTeamName),
            Group = group,
            Phase = phase
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