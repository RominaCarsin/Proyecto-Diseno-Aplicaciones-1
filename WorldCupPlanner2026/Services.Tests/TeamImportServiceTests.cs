using Domain;
using DTOs;
using IRepository;
using Repository;

namespace Services.Tests;

[TestClass]
public class TeamImportServiceTests
{
    private const string Header = "Nombre,Confederaci\u00f3n,RankingFIFA";

    private TeamImportService _service = null!;
    private TeamService _teamService = null!;
    private IRepository<AuditLog> _auditRepository = null!;

    [TestInitialize]
    public void Setup()
    {
        var teamRepository = new Repository<Team>();
        _auditRepository = new Repository<AuditLog>();
        var auditService = new AuditService(_auditRepository);
        var sessionService = new SessionService(null!, new AuthStateService());
        _teamService = new TeamService(teamRepository, new ConfederationService(), auditService, sessionService);
        _service = new TeamImportService(_teamService, auditService, sessionService);
    }

    [TestMethod]
    public void Preview_WithValidCsv_ShouldReportSuccessfulRowsWithoutPersisting()
    {
        var csv = $"{Header}\r\nUruguay,CONMEBOL,1900";

        var result = _service.Preview(csv);

        Assert.AreEqual(1, result.TotalRows);
        Assert.AreEqual(1, result.SuccessfulRows);
        Assert.AreEqual(0, _teamService.GetTeams().Count());
    }

    [TestMethod]
    public void Import_WithValidCsv_ShouldPersistTeam()
    {
        var csv = $"{Header}\r\nUruguay,CONMEBOL,1900";

        var result = _service.Import(csv);

        Assert.AreEqual(1, result.SuccessfulRows);
        Assert.AreEqual("URUGUAY", _teamService.GetTeams().Single().Name);
    }

    [TestMethod]
    public void Import_WithInvalidHeader_ShouldFailAndAuditError()
    {
        var csv = "name,confederation,ranking\r\nUruguay,CONMEBOL,1900";

        var result = _service.Import(csv);

        Assert.AreEqual(1, result.FailedRows);
        Assert.IsTrue(result.Rows.Single().ErrorMessage.Contains("encabezado"));
        Assert.IsTrue(_auditRepository.GetAll().Any(log => log.Action == "Teams import error"));
    }

    [TestMethod]
    public void Import_WithDuplicateName_ShouldReportRowError()
    {
        var csv = $"{Header}\r\nUruguay,CONMEBOL,1900\r\nUruguay,CONMEBOL,1800";

        var result = _service.Import(csv);

        Assert.AreEqual(1, result.SuccessfulRows);
        Assert.AreEqual(1, result.FailedRows);
        Assert.AreEqual(1, _teamService.GetTeams().Count());
    }

    [TestMethod]
    public void Preview_WithNonNumericRanking_ShouldReportError()
    {
        var csv = $"{Header}\r\nUruguay,CONMEBOL,abc";

        var result = _service.Preview(csv);

        Assert.AreEqual(0, result.SuccessfulRows);
        Assert.AreEqual(1, result.FailedRows);
        Assert.IsTrue(result.Rows.Single().ErrorMessage.Contains("RankingFIFA"));
    }
}
