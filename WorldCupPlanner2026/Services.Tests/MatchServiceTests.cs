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