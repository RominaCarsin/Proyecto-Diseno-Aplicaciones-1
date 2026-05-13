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

    
    // Helpers ──────────────────────────────────────────────────────────────
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