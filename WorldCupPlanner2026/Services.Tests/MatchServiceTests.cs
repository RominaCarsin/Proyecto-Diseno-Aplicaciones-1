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
    private const string AlternativeStadiumCity  = "RIO DE JANEIRO";

    private static readonly DateTime ValidMatchDate   = new(2026, 6, 1, 18, 0, 0);
    private static readonly DateTime UpdatedMatchDate = new(2026, 6, 10, 18, 0, 0);
    

    private IMatchService         _matchService;
    private IRepository<Match>    _matchRepository;
    private IRepository<Group>    _groupRepository;
    private AuditService          _auditService;
    private IRepository<AuditLog> _auditRepository;
    private SessionService        _sessionService;

    [TestInitialize]
    public void Setup()
    {
        _matchRepository = new Repository<Match>();
        _groupRepository = new Repository<Group>();
        _auditRepository = new Repository<AuditLog>();
        _auditService    = new AuditService(_auditRepository);
        var authState    = new AuthStateService();
        _sessionService  = new SessionService(null!, authState);

        _matchService = new MatchService(_matchRepository, _groupRepository, _auditService, _sessionService);
    }
    

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
    public void AddMatch_WithExistingGroup_ShouldAddMatchToGroup()
    {
        var group = new Group('A');
        _groupRepository.Add(group);
 
        _matchService.AddMatch(BuildValidMatchRequest(group: 'A'));
 
        Assert.AreEqual(1, _matchService.GetMatches().Count());
    }
 
    [TestMethod]
    public void AddMatch_WithNonExistingGroup_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _matchService.AddMatch(BuildValidMatchRequest(group: 'Z')));
    }
 
    [TestMethod]
    public void AddMatch_WhenStadiumAlreadyHasThreeMatchesThatDay_ShouldThrowInvalidOperationException()
    {
        _matchService.AddMatch(BuildValidMatchRequest(date: ValidMatchDate.AddHours(-8)));
        _matchService.AddMatch(BuildValidMatchRequest(date: ValidMatchDate.AddHours(-4),
            homeTeamName: "FRANCE", awayTeamName: "GERMANY"));
        _matchService.AddMatch(BuildValidMatchRequest(date: ValidMatchDate.AddHours(4),
            homeTeamName: "SPAIN",  awayTeamName: "PORTUGAL"));
 
        Assert.ThrowsException<InvalidOperationException>(() =>
            _matchService.AddMatch(BuildValidMatchRequest(date: ValidMatchDate,
                homeTeamName: "URUGUAY", awayTeamName: "CHILE")));
    }
 
    [TestMethod]
    public void AddMatch_WhenStadiumHasMatchLessThanFourHoursApart_ShouldThrowInvalidOperationException()
    {
        _matchService.AddMatch(BuildValidMatchRequest(date: ValidMatchDate));
 
        Assert.ThrowsException<InvalidOperationException>(() =>
            _matchService.AddMatch(BuildValidMatchRequest(date: ValidMatchDate.AddHours(2),
                homeTeamName: "FRANCE", awayTeamName: "GERMANY")));
    }
    
    [TestMethod]
    public void AddMatch_WhenStadiumHasMatchExactlyFourHoursApart_ShouldPersistBothMatches()
    {
        _matchService.AddMatch(BuildValidMatchRequest(date: ValidMatchDate));
        _matchService.AddMatch(BuildValidMatchRequest(date: ValidMatchDate.AddHours(4),
            homeTeamName: "FRANCE", awayTeamName: "GERMANY"));
 
        Assert.AreEqual(2, _matchService.GetMatches().Count());
    }
 
    [TestMethod]
    public void AddMatch_ShouldLogCreationInAuditService()
    {
        var initialLogCount = _auditRepository.GetAll().Count();

        _matchService.AddMatch(BuildValidMatchRequest());

        var logs = _auditRepository.GetAll();
        Assert.AreEqual(initialLogCount + 1, logs.Count());
        Assert.AreEqual("Match created", logs.Last().Action);
    }
    
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
            awayTeamName: "GERMANY",
            date: ValidMatchDate.AddHours(5)));

        Assert.AreEqual(2, _matchService.GetMatches().Count());
    }

    // ─── GetMatchById ─────────────────────────────────────────────────────────

    [TestMethod]
    public void GetMatchById_WithExistingId_ShouldReturnMatchWithExpectedProperties()
    {
        var saved = AddValidMatchAndGetResponse();

        var result = _matchService.GetMatchById(saved.Id);

        Assert.AreEqual(saved.Id,    result.Id);
        Assert.AreEqual(HomeTeamName, result.HomeTeam.Name);
        Assert.AreEqual(AwayTeamName, result.AwayTeam.Name);
        Assert.AreEqual(ValidMatchDate, result.Date);
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
        var saved = AddValidMatchAndGetResponse();

        _matchService.SetMatchResult(saved.Id, homeGoals: 3, awayGoals: 1);

        var result = _matchService.GetMatchById(saved.Id);
        Assert.AreEqual(3, result.HomeGoals);
        Assert.AreEqual(1, result.AwayGoals);
        Assert.AreEqual(HomeTeamName, result.Winner!.Name);
    }

    [TestMethod]
    public void SetMatchResult_WithAwayTeamScoringMore_ShouldSetAwayTeamAsWinner()
    {
        var saved = AddValidMatchAndGetResponse();

        _matchService.SetMatchResult(saved.Id, homeGoals: 0, awayGoals: 2);

        var result = _matchService.GetMatchById(saved.Id);
        Assert.AreEqual(AwayTeamName, result.Winner!.Name);
    }

    [TestMethod]
    public void SetMatchResult_WithDraw_ShouldSetNullWinner()
    {
        var saved = AddValidMatchAndGetResponse();

        _matchService.SetMatchResult(saved.Id, homeGoals: 1, awayGoals: 1);

        var result = _matchService.GetMatchById(saved.Id);
        Assert.IsNull(result.Winner);
    }

    [TestMethod]
    public void SetMatchResult_WithNonExistingId_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _matchService.SetMatchResult(999, homeGoals: 1, awayGoals: 0));
    }

    [TestMethod]
    public void SetMatchResult_ShouldLogResultInAuditService()
    {
        var saved = AddValidMatchAndGetResponse();
        var initialLogCount = _auditRepository.GetAll().Count();

        _matchService.SetMatchResult(saved.Id, homeGoals: 2, awayGoals: 0);

        var logs = _auditRepository.GetAll();
        Assert.AreEqual(initialLogCount + 1, logs.Count());
        Assert.AreEqual("Match result set", logs.Last().Action);
    }

    // ─── UpdateMatchDate ──────────────────────────────────────────────────────

    [TestMethod]
    public void UpdateMatchDate_WithValidDate_ShouldReplaceMatchDate()
    {
        var saved = AddValidMatchAndGetResponse();

        _matchService.UpdateMatchDate(saved.Id, UpdatedMatchDate);

        var result = _matchService.GetMatchById(saved.Id);
        Assert.AreEqual(UpdatedMatchDate, result.Date);
    }

    [TestMethod]
    public void UpdateMatchDate_WithNonExistingId_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _matchService.UpdateMatchDate(999, UpdatedMatchDate));
    }

    [TestMethod]
    public void UpdateMatchDate_WhenNewDateConflictsWithSameStadium_ShouldThrowInvalidOperationException()
    {
        _matchService.AddMatch(BuildValidMatchRequest(date: ValidMatchDate));
        var secondMatch = AddMatchAndGetResponse(BuildValidMatchRequest(
            date: ValidMatchDate.AddDays(1),
            homeTeamName: "FRANCE",
            awayTeamName: "GERMANY"));

        Assert.ThrowsException<InvalidOperationException>(() =>
            _matchService.UpdateMatchDate(secondMatch.Id, ValidMatchDate.AddHours(2)));
    }

    [TestMethod]
    public void UpdateMatchDate_ShouldLogUpdateInAuditService()
    {
        var saved = AddValidMatchAndGetResponse();
        var initialLogCount = _auditRepository.GetAll().Count();

        _matchService.UpdateMatchDate(saved.Id, UpdatedMatchDate);

        var logs = _auditRepository.GetAll();
        Assert.AreEqual(initialLogCount + 1, logs.Count());
        Assert.AreEqual("Match date updated", logs.Last().Action);
    }
    
    // --- UpdateMatchStadium ---

[TestMethod]
public void UpdateMatchStadium_WithValidRequest_ShouldUpdateStadiumAndLog()
{
    var saved = AddValidMatchAndGetResponse();
    var newStadium = BuildStadiumRequest("WEMBLEY", "LONDON");
    var initialLogCount = _auditRepository.GetAll().Count();

    _matchService.UpdateMatchStadium(saved.Id, newStadium);

    var result = _matchService.GetMatchById(saved.Id);
    Assert.AreEqual("WEMBLEY", result.Stadium.Name);
    
    var logs = _auditRepository.GetAll();
    Assert.AreEqual(initialLogCount + 1, logs.Count());
    Assert.AreEqual("Match stadium updated", logs.Last().Action);
}

[TestMethod]
public void UpdateMatchStadium_WithNullRequest_ShouldThrowArgumentException()
{
    Assert.ThrowsException<ArgumentException>(() =>
        _matchService.UpdateMatchStadium(1, null!));
}

[TestMethod]
public void UpdateMatchStadium_WhenNewStadiumConflicts_ShouldThrowInvalidOperationException()
{
    // Creamos un partido en el estadio B
    var stadiumB = BuildStadiumRequest("CENTENARIO", "MONTEVIDEO");
    _matchService.AddMatch(BuildValidMatchRequest(date: ValidMatchDate, stadiumRequest: stadiumB));

    // Intentamos mover otro partido al mismo estadio y hora
    var matchToMove = AddMatchAndGetResponse(BuildValidMatchRequest(
        date: ValidMatchDate, 
        stadiumRequest: BuildStadiumRequest("OTRO", "LUGAR")));

    Assert.ThrowsException<InvalidOperationException>(() =>
        _matchService.UpdateMatchStadium(matchToMove.Id, stadiumB));
}

// --- FinishMatch ---

[TestMethod]
public void FinishMatch_WithExistingId_ShouldMarkAsFinishedAndLog()
{
    var saved = AddValidMatchAndGetResponse();
    var initialLogCount = _auditRepository.GetAll().Count();

    _matchService.FinishMatch(saved.Id);

    var result = _matchService.GetMatchById(saved.Id);
    // Nota: Necesitas que MatchResponse exponga la propiedad Status o IsFinished si quieres asertar esto en el Service
    // O verificar que no se puede editar más.
    
    var logs = _auditRepository.GetAll();
    Assert.AreEqual(initialLogCount + 1, logs.Count());
    Assert.AreEqual("Match finished", logs.Last().Action);
}

[TestMethod]
public void FinishMatch_WithNonExistingId_ShouldThrowArgumentException()
{
    Assert.ThrowsException<ArgumentException>(() =>
        _matchService.FinishMatch(999));
}
    
    // GetAvailableStadiumsForMatch ─────────────────────────────────────────────────
    
    [TestMethod]
    public void GetAvailableStadiumsForMatch_WhenAllCandidatesAreFree_ShouldReturnAllCandidates()
    {
        var saved = AddValidMatchAndGetResponse();
        var candidates = new[]
        {
            BuildStadiumRequest(AlternativeStadiumName, AlternativeStadiumCity),
            BuildStadiumRequest("AZTECA", "CIUDAD DE MEXICO")
        };

        var result = _matchService.GetAvailableStadiumsForMatch(saved.Id, candidates).ToList();

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void GetAvailableStadiumsForMatch_WhenSomeCandidatesConflict_ShouldReturnOnlyAvailableOnes()
    {
        var altStadium = BuildStadiumRequest(AlternativeStadiumName, AlternativeStadiumCity);
        _matchService.AddMatch(BuildValidMatchRequest(
            date: ValidMatchDate.AddHours(2),
            stadiumRequest: altStadium,
            homeTeamName: "FRANCE",
            awayTeamName: "GERMANY"));

        var matchToCheck = AddMatchAndGetResponse(BuildValidMatchRequest(
            date: ValidMatchDate,
            homeTeamName: "URUGUAY",
            awayTeamName: "CHILE"));

        var candidates = new[]
        {
            altStadium,
            BuildStadiumRequest("AZTECA", "CIUDAD DE MEXICO")
        };

        var result = _matchService.GetAvailableStadiumsForMatch(matchToCheck.Id, candidates).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("AZTECA", result[0].Name);
    }

    [TestMethod]
    public void GetAvailableStadiumsForMatch_WithNonExistingMatchId_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _matchService.GetAvailableStadiumsForMatch(999, Enumerable.Empty<StadiumRequest>()).ToList());
    }

    
    // Helpers ──────────────────────────────────────────────────────────────
    private MatchResponse AddValidMatchAndGetResponse()
    {
        _matchService.AddMatch(BuildValidMatchRequest());
        return _matchService.GetMatches().Last();
    }

    private MatchResponse AddMatchAndGetResponse(MatchRequest request)
    {
        _matchService.AddMatch(request);
        return _matchService.GetMatches().Last();
    }
    
    private static MatchRequest BuildValidMatchRequest(
        string          homeTeamName   = HomeTeamName,
        string          awayTeamName   = AwayTeamName,
        char?           group          = null,
        DateTime?       date           = null,
        StadiumRequest? stadiumRequest = null) =>
        new()
        {
            HomeTeam = BuildTeamRequest(homeTeamName),
            AwayTeam = BuildTeamRequest(awayTeamName),
            Stadium  = stadiumRequest ?? BuildStadiumRequest(MainStadiumName, MainStadiumCity),
            Date     = date ?? ValidMatchDate,
            Group    = group
        };
    
    private static StadiumRequest BuildStadiumRequest(
        string name,
        string city,
        string description = "Historic stadium",
        int    capacity    = 60_000) =>
        new()
        {
            Name        = name,
            City        = city,
            Description = description,
            Capacity    = capacity
        };
 
    private static TeamRequest BuildTeamRequest(string name) =>
        new()
        {
            Name              = name,
            ConfederationName = "CONMEBOL",
            RankingFifa       = 1800
        };
    


}