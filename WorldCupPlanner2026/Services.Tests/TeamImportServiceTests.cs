using Domain;
using DTOs;
using IRepository;
using Repository;

namespace Services.Tests;

[TestClass]
public class TeamImportServiceTests
{
    private const string Header = "Nombre,Confederación,RankingFIFA";
    private const string ValidCsv = $"{Header}\r\nUruguay,CONMEBOL,1900";
    private const string InvalidHeaderCsv = "name,confederation,ranking\r\nUruguay,CONMEBOL,1900";
    private const string HeaderWithWrongColumnCountCsv = "Nombre,RankingFIFA\r\nUruguay,1900";
    private const string MissingColumnCsv = $"{Header}\r\nUruguay,CONMEBOL";
    private const string NonNumericRankingCsv = $"{Header}\r\nUruguay,CONMEBOL,abc";

    private TeamImportService _service = null!;
    private TeamService _teamService = null!;
    private IRepository<AuditLog> _auditRepository = null!;

    [TestInitialize]
    public void Setup()
    {
        CreateServiceWithDefaultSession();
    }

    [TestMethod]
    public void Preview_WithValidCsv_ShouldReportSuccessfulRowsWithoutPersisting()
    {
        var result = _service.Preview(ValidCsv);

        Assert.AreEqual(1, result.TotalRows);
        Assert.AreEqual(1, result.SuccessfulRows);
        Assert.AreEqual(0, _teamService.GetTeams().Count());
    }

    [TestMethod]
    public void Import_WithValidCsv_ShouldPersistTeam()
    {
        var result = _service.Import(ValidCsv);

        Assert.AreEqual(1, result.SuccessfulRows);
        Assert.AreEqual("URUGUAY", _teamService.GetTeams().Single().Name);
    }

    [TestMethod]
    public void Import_WithInvalidHeader_ShouldFailAndAuditError()
    {
        var result = _service.Import(InvalidHeaderCsv);

        AssertFailedImport(result, "encabezado");
        AssertAuditActionExists("Error al importar equipos");
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
        var result = _service.Preview(NonNumericRankingCsv);

        AssertFailedImport(result, "RankingFIFA");
    }

    [TestMethod]
    public void Preview_WithEmptyCsv_ShouldReportGlobalError()
    {
        var result = _service.Preview(string.Empty);

        AssertGlobalError(result, "vacio");
    }

    [TestMethod]
    public void Preview_WithWhitespaceCsv_ShouldReportGlobalError()
    {
        var result = _service.Preview("   ");

        AssertGlobalError(result, "vacio");
    }

    [TestMethod]
    public void Import_WithEmptyCsv_ShouldReportGlobalError()
    {
        var result = _service.Import(string.Empty);

        AssertGlobalError(result, "vacio");
    }

    [TestMethod]
    public void Import_WithInvalidHeaderColumnCount_ShouldFailAndAuditError()
    {
        var result = _service.Import(HeaderWithWrongColumnCountCsv);

        AssertFailedImport(result, "encabezado");
        AssertAuditActionExists("Error al importar equipos");
    }

    [TestMethod]
    public void Preview_WithInvalidHeader_ShouldReportErrorWithoutAuditing()
    {
        var result = _service.Preview(InvalidHeaderCsv);

        AssertFailedImport(result, "encabezado");
        AssertNoAuditLogs();
    }

    [TestMethod]
    public void Import_WithRowMissingColumns_ShouldReportRowErrorAndAudit()
    {
        var result = _service.Import(MissingColumnCsv);

        AssertFailedImport(result, "3 columnas");
        AssertAuditActionExists("Error al importar equipos");
    }

    [TestMethod]
    public void Import_WithOnlyNewline_ShouldReportGlobalError()
    {
        var result = _service.Import("\n");

        AssertGlobalError(result, "vacio");
    }
    
    [TestMethod]
    public void Preview_WithMultipleValidRows_ShouldNotAuditSummary()
    {
        var csv = $"{Header}\r\nUruguay,CONMEBOL,1900\r\nArgentina,CONMEBOL,1800";

        _service.Preview(csv);

        AssertNoAuditLogs();
    }
    
    [TestMethod]
    public void Preview_WithRowMissingColumns_ShouldReportRowError()
    {
        var result = _service.Preview(MissingColumnCsv);

        AssertFailedImport(result, "3 columnas");
    }

    [TestMethod]
    public void Preview_WithHeaderWithWrongColumnCount_ShouldReportGlobalError()
    {
        var result = _service.Preview(HeaderWithWrongColumnCountCsv);

        AssertFailedImport(result, "encabezado");
    }

    [TestMethod]
    public void Preview_WithValidRow_ShouldNotPersistNorAudit()
    {
        var result = _service.Preview(ValidCsv);

        Assert.AreEqual(1, result.SuccessfulRows);
        Assert.AreEqual(0, _teamService.GetTeams().Count());
        AssertNoAuditLogs();
    }

    [TestMethod]
    public void Import_WithNullSession_ShouldUseSystemAsActorEmail()
    {
        CreateServiceWithDefaultSession();

        var result = _service.Import(ValidCsv);

        Assert.AreEqual(1, result.SuccessfulRows);
        AssertAuditActorExists("system");
    }

    [TestMethod]
    public void Import_WithNonNumericRanking_ShouldReportRowErrorAndAudit()
    {
        var result = _service.Import(NonNumericRankingCsv);

        AssertFailedImport(result, "RankingFIFA");
        AssertAuditActionExists("Error al importar equipos");
    }

    private void CreateServiceWithDefaultSession()
    {
        var teamRepository = new FakeRepository<Team>();
        _auditRepository = new FakeRepository<AuditLog>();

        var auditService = new AuditService(_auditRepository);
        var sessionService = new SessionService(null!, new AuthStateService());

        _teamService = new TeamService(
            teamRepository,
            new ConfederationService(),
            auditService,
            sessionService);

        _service = new TeamImportService(
            _teamService,
            auditService,
            sessionService);
    }

    private static void AssertFailedImport(TeamImportResult result, string expectedErrorText)
    {
        Assert.AreEqual(0, result.SuccessfulRows);
        Assert.AreEqual(1, result.FailedRows);
        Assert.IsTrue(result.Rows.Single().ErrorMessage.Contains(expectedErrorText));
    }

    private static void AssertGlobalError(TeamImportResult result, string expectedErrorText)
    {
        Assert.AreEqual(0, result.TotalRows);
        Assert.AreEqual(1, result.FailedRows);
        Assert.IsTrue(result.Rows.Single().ErrorMessage.Contains(expectedErrorText));
    }

    private void AssertAuditActionExists(string expectedAction)
    {
        Assert.IsTrue(_auditRepository
            .GetAll()
            .Any(log => log.Action == expectedAction));
    }

    private void AssertAuditActorExists(string expectedActorEmail)
    {
        Assert.IsTrue(_auditRepository
            .GetAll()
            .Any(log => log.ActorEmail == expectedActorEmail));
    }

    private void AssertNoAuditLogs()
    {
        Assert.IsFalse(_auditRepository.GetAll().Any());
    }
    
    [TestMethod]
    public void Import_WithHeaderOnly_ShouldSucceedWithZeroRowsNoneSaved()
    {
        var result = _service.Import(Header);

        Assert.AreEqual(0, result.TotalRows);
        Assert.AreEqual(0, result.SuccessfulRows);
        Assert.AreEqual(0, _teamService.GetTeams().Count());
    }

    [TestMethod]
    public void Preview_WithHeaderOnly_ShouldSucceedWithZeroRowsNoneSaved()
    {
        var result = _service.Preview(Header);

        Assert.AreEqual(0, result.TotalRows);
        Assert.AreEqual(0, result.SuccessfulRows);
        Assert.AreEqual(0, _teamService.GetTeams().Count());
        AssertNoAuditLogs();
    }

    [TestMethod]
    public void Preview_WithOnlyNewlines_ShouldReportGlobalError()
    {
        var result = _service.Preview("\r\n");

        AssertGlobalError(result, "vacio");
    }
    
    [TestMethod]
    public void Import_WithOnlyNewlines_ShouldReportGlobalError()
    {
        var result = _service.Import("\r\n\r\n");

        Assert.AreEqual(0, result.TotalRows);
        Assert.AreEqual(1, result.FailedRows);
        Assert.IsTrue(result.Rows.Single().ErrorMessage.Contains("vacio"));
    }
}