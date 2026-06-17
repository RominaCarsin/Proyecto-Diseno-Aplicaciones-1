using System.Globalization;
using System.Text;
using ClosedXML.Excel;
using Domain;
using IRepository;
using IServices;
using Repository;
using Services;

namespace Services.Tests;

[TestClass]
public class ExportServiceTests
{
    private ExportService _service = null!;

    private IRepository<AuditLog> _auditRepository = null!;
    private IAuditService _auditService = null!;
    private ISessionService _sessionService = null!;

    [TestInitialize]
    public void Setup()
    {
        _auditRepository = new FakeRepository<AuditLog>();
        _auditService = new AuditService(_auditRepository);
        _sessionService = new SessionService(null!, new AuthStateService());
        _service = new ExportService(_auditService, _sessionService);
    }

    [TestMethod]
    public void ExportAuditLogToCsv_WithEmptyLogs_ShouldReturnOnlyHeaderRow()
    {
        var result = _service.ExportAuditLogToCsv(Enumerable.Empty<AuditLog>());

        var csv = Encoding.UTF8.GetString(result);
        var lines = csv.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        Assert.AreEqual(1, lines.Length, "Solo debe existir la fila de encabezado.");
        Assert.IsTrue(lines[0].Contains("Marca de Tiempo"));
        Assert.IsTrue(lines[0].Contains("Email del Actor"));
        Assert.IsTrue(lines[0].Contains("Acción"));
        Assert.IsTrue(lines[0].Contains("Detalles"));
    }

    [TestMethod]
    public void ExportAuditLogToCsv_WithSingleLog_ShouldContainOneDataRow()
    {
        var logs = new List<AuditLog>
        {
            new AuditLog("admin@test.com", "User created", "juan@test.com")
        };

        var result = _service.ExportAuditLogToCsv(logs);
        var csv = Encoding.UTF8.GetString(result);
        var lines = csv.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        Assert.AreEqual(2, lines.Length, "Header + 1 fila de datos.");
        Assert.IsTrue(lines[1].Contains("admin@test.com"));
        Assert.IsTrue(lines[1].Contains("User created"));
        Assert.IsTrue(lines[1].Contains("juan@test.com"));
    }

    [TestMethod]
    public void ExportAuditLogToCsv_WithMultipleLogs_ShouldReturnAllRows()
    {
        var logs = new List<AuditLog>
        {
            new AuditLog("admin@test.com", "User created", "juan@test.com"),
            new AuditLog("admin@test.com", "User deleted", "pedro@test.com"),
            new AuditLog("editor@test.com", "Stadium added", "Estadio Centenario")
        };

        var result = _service.ExportAuditLogToCsv(logs);
        var csv = Encoding.UTF8.GetString(result);
        var lines = csv.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        Assert.AreEqual(4, lines.Length, "Header + 3 filas de datos.");
        Assert.IsTrue(lines[1].Contains("User created"));
        Assert.IsTrue(lines[2].Contains("User deleted"));
        Assert.IsTrue(lines[3].Contains("Stadium added"));
    }

    [TestMethod]
    public void ExportAuditLogToCsv_ShouldIncludeFormattedTimestamp()
    {
        var logs = new List<AuditLog>
        {
            new AuditLog("admin@test.com", "Login", "ok")
        };

        var result = _service.ExportAuditLogToCsv(logs);
        var csv = Encoding.UTF8.GetString(result);

        Assert.IsTrue(csv.Contains("-"), "El timestamp debe contener separadores de fecha.");
        Assert.IsTrue(csv.Contains(":"), "El timestamp debe contener separadores de hora.");
    }

    [TestMethod]
    public void ExportAuditLogToCsv_ShouldReturnNonEmptyByteArray()
    {
        var result = _service.ExportAuditLogToCsv(Enumerable.Empty<AuditLog>());

        Assert.IsNotNull(result);
        Assert.IsTrue(result.Length > 0, "El resultado no debe estar vacío.");
    }

    [TestMethod]
    public void ExportAuditLogToXlsx_WithEmptyLogs_ShouldCreateWorkbookWithHeaderOnly()
    {
        var result = _service.ExportAuditLogToXlsx(Enumerable.Empty<AuditLog>());

        using var stream = new MemoryStream(result);
        using var workbook = new XLWorkbook(stream);
        var ws = workbook.Worksheet("Log de Auditoría");

        Assert.IsNotNull(ws);
        Assert.AreEqual("Marca de Tiempo", ws.Cell(1, 1).GetString());
        Assert.AreEqual("Email del Actor", ws.Cell(1, 2).GetString());
        Assert.AreEqual("Acción", ws.Cell(1, 3).GetString());
        Assert.AreEqual("Detalles", ws.Cell(1, 4).GetString());
        Assert.AreEqual("", ws.Cell(2, 1).GetString(), "No debe haber datos en la fila 2.");
    }

    [TestMethod]
    public void ExportAuditLogToXlsx_ShouldHaveBoldHeader()
    {
        var result = _service.ExportAuditLogToXlsx(Enumerable.Empty<AuditLog>());

        using var stream = new MemoryStream(result);
        using var workbook = new XLWorkbook(stream);
        var ws = workbook.Worksheet("Log de Auditoría");

        Assert.IsTrue(ws.Cell(1, 1).Style.Font.Bold, "El header debe estar en negrita.");
    }

    [TestMethod]
    public void ExportAuditLogToXlsx_ShouldHaveLightGrayHeaderBackground()
    {
        var result = _service.ExportAuditLogToXlsx(Enumerable.Empty<AuditLog>());

        using var stream = new MemoryStream(result);
        using var workbook = new XLWorkbook(stream);
        var ws = workbook.Worksheet("Log de Auditoría");

        Assert.AreEqual(XLColor.LightGray, ws.Cell(1, 1).Style.Fill.BackgroundColor);
    }

    [TestMethod]
    public void ExportAuditLogToXlsx_WithSingleLog_ShouldContainDataInSecondRow()
    {
        var logs = new List<AuditLog>
        {
            new AuditLog("admin@test.com", "User created", "juan@test.com")
        };

        var result = _service.ExportAuditLogToXlsx(logs);

        using var stream = new MemoryStream(result);
        using var workbook = new XLWorkbook(stream);
        var ws = workbook.Worksheet("Log de Auditoría");

        Assert.AreEqual("admin@test.com", ws.Cell(2, 2).GetString());
        Assert.AreEqual("User created", ws.Cell(2, 3).GetString());
        Assert.AreEqual("juan@test.com", ws.Cell(2, 4).GetString());
    }

    [TestMethod]
    public void ExportAuditLogToXlsx_WithMultipleLogs_ShouldContainAllRows()
    {
        var logs = new List<AuditLog>
        {
            new AuditLog("admin@test.com", "User created", "juan@test.com"),
            new AuditLog("admin@test.com", "User deleted", "pedro@test.com"),
            new AuditLog("editor@test.com", "Stadium added", "Estadio Centenario")
        };

        var result = _service.ExportAuditLogToXlsx(logs);

        using var stream = new MemoryStream(result);
        using var workbook = new XLWorkbook(stream);
        var ws = workbook.Worksheet("Log de Auditoría");

        Assert.AreEqual("User created", ws.Cell(2, 3).GetString());
        Assert.AreEqual("User deleted", ws.Cell(3, 3).GetString());
        Assert.AreEqual("Stadium added", ws.Cell(4, 3).GetString());
    }

    [TestMethod]
    public void ExportAuditLogToXlsx_ShouldReturnValidXlsxBytes()
    {
        var result = _service.ExportAuditLogToXlsx(Enumerable.Empty<AuditLog>());

        Assert.IsNotNull(result);
        Assert.IsTrue(result.Length > 0);

        using var stream = new MemoryStream(result);
        using var workbook = new XLWorkbook(stream);
        Assert.AreEqual(1, workbook.Worksheets.Count);
    }

    [TestMethod]
    public void ExportFixtureToCsv_WithEmptyMatches_ShouldReturnOnlyHeaderRow()
    {
        var result = _service.ExportFixtureToCsv(Enumerable.Empty<MatchResponseRow>());

        var csv = Encoding.UTF8.GetString(result);
        var lines = csv.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        Assert.AreEqual(1, lines.Length, "Solo debe existir la fila de encabezado.");
        Assert.IsTrue(lines[0].Contains("Código"));
        Assert.IsTrue(lines[0].Contains("Fecha"));
        Assert.IsTrue(lines[0].Contains("Fase"));
        Assert.IsTrue(lines[0].Contains("Grupo"));
        Assert.IsTrue(lines[0].Contains("Equipo Local"));
        Assert.IsTrue(lines[0].Contains("Equipo Visitante"));
        Assert.IsTrue(lines[0].Contains("Goles Local"));
        Assert.IsTrue(lines[0].Contains("Goles Visitante"));
        Assert.IsTrue(lines[0].Contains("Estadio"));
        Assert.IsTrue(lines[0].Contains("Estado"));
    }

    [TestMethod]
    public void ExportFixtureToCsv_WithSingleMatch_ShouldContainOneDataRow()
    {
        var matches = new List<MatchResponseRow>
        {
            new MatchResponseRow("M01", "2026-06-11", "Group Stage", "A",
                "Uruguay", "Mexico", "2", "1", "Estadio Azteca", "Finished")
        };

        var result = _service.ExportFixtureToCsv(matches);
        var csv = Encoding.UTF8.GetString(result);
        var lines = csv.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        Assert.AreEqual(2, lines.Length, "Header + 1 fila de datos.");
        Assert.IsTrue(lines[1].Contains("M01"));
        Assert.IsTrue(lines[1].Contains("Uruguay"));
        Assert.IsTrue(lines[1].Contains("Mexico"));
        Assert.IsTrue(lines[1].Contains("2"));
        Assert.IsTrue(lines[1].Contains("1"));
        Assert.IsTrue(lines[1].Contains("Finished"));
    }

    [TestMethod]
    public void ExportFixtureToCsv_WithMultipleMatches_ShouldReturnAllRows()
    {
        var matches = new List<MatchResponseRow>
        {
            new MatchResponseRow("M01", "2026-06-11", "Group Stage", "A",
                "Uruguay", "Mexico", "2", "1", "Estadio Azteca", "Finished"),
            new MatchResponseRow("M02", "2026-06-12", "Group Stage", "B",
                "Argentina", "Nigeria", "3", "0", "Estadio Centenario", "Finished"),
            new MatchResponseRow("M03", "2026-06-13", "Round of 16", "",
                "Brazil", "Germany", "1", "1", "MetLife Stadium", "In Progress")
        };

        var result = _service.ExportFixtureToCsv(matches);
        var csv = Encoding.UTF8.GetString(result);
        var lines = csv.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        Assert.AreEqual(4, lines.Length, "Header + 3 filas de datos.");
    }

    [TestMethod]
    public void ExportFixtureToCsv_ShouldReturnNonEmptyByteArray()
    {
        var result = _service.ExportFixtureToCsv(Enumerable.Empty<MatchResponseRow>());

        Assert.IsNotNull(result);
        Assert.IsTrue(result.Length > 0, "El resultado no debe estar vacío.");
    }

    [TestMethod]
    public void ExportFixtureToXlsx_WithEmptyMatches_ShouldCreateWorkbookWithHeaderOnly()
    {
        var result = _service.ExportFixtureToXlsx(Enumerable.Empty<MatchResponseRow>());

        using var stream = new MemoryStream(result);
        using var workbook = new XLWorkbook(stream);
        var ws = workbook.Worksheet("Fixture");

        Assert.IsNotNull(ws);
        Assert.AreEqual("Código", ws.Cell(1, 1).GetString());
        Assert.AreEqual("Fecha", ws.Cell(1, 2).GetString());
        Assert.AreEqual("Fase", ws.Cell(1, 3).GetString());
        Assert.AreEqual("Grupo", ws.Cell(1, 4).GetString());
        Assert.AreEqual("Equipo Local", ws.Cell(1, 5).GetString());
        Assert.AreEqual("Equipo Visitante", ws.Cell(1, 6).GetString());
        Assert.AreEqual("Goles Local", ws.Cell(1, 7).GetString());
        Assert.AreEqual("Goles Visitante", ws.Cell(1, 8).GetString());
        Assert.AreEqual("Estadio", ws.Cell(1, 9).GetString());
        Assert.AreEqual("Estado", ws.Cell(1, 10).GetString());
        Assert.AreEqual("", ws.Cell(2, 1).GetString(), "No debe haber datos en la fila 2.");
    }

    [TestMethod]
    public void ExportFixtureToXlsx_ShouldHaveBoldHeader()
    {
        var result = _service.ExportFixtureToXlsx(Enumerable.Empty<MatchResponseRow>());

        using var stream = new MemoryStream(result);
        using var workbook = new XLWorkbook(stream);
        var ws = workbook.Worksheet("Fixture");

        Assert.IsTrue(ws.Cell(1, 1).Style.Font.Bold, "El header debe estar en negrita.");
    }

    [TestMethod]
    public void ExportFixtureToXlsx_ShouldHaveLightGrayHeaderBackground()
    {
        var result = _service.ExportFixtureToXlsx(Enumerable.Empty<MatchResponseRow>());

        using var stream = new MemoryStream(result);
        using var workbook = new XLWorkbook(stream);
        var ws = workbook.Worksheet("Fixture");

        Assert.AreEqual(XLColor.LightGray, ws.Cell(1, 1).Style.Fill.BackgroundColor);
    }

    [TestMethod]
    public void ExportFixtureToXlsx_WithSingleMatch_ShouldContainDataInSecondRow()
    {
        var matches = new List<MatchResponseRow>
        {
            new MatchResponseRow("M01", "2026-06-11", "Group Stage", "A",
                "Uruguay", "Mexico", "2", "1", "Estadio Azteca", "Finished")
        };

        var result = _service.ExportFixtureToXlsx(matches);

        using var stream = new MemoryStream(result);
        using var workbook = new XLWorkbook(stream);
        var ws = workbook.Worksheet("Fixture");

        Assert.AreEqual("M01", ws.Cell(2, 1).GetString());
        Assert.AreEqual("2026-06-11", ws.Cell(2, 2).GetString());
        Assert.AreEqual("Group Stage", ws.Cell(2, 3).GetString());
        Assert.AreEqual("A", ws.Cell(2, 4).GetString());
        Assert.AreEqual("Uruguay", ws.Cell(2, 5).GetString());
        Assert.AreEqual("Mexico", ws.Cell(2, 6).GetString());
        Assert.AreEqual("2", ws.Cell(2, 7).GetString());
        Assert.AreEqual("1", ws.Cell(2, 8).GetString());
        Assert.AreEqual("Estadio Azteca", ws.Cell(2, 9).GetString());
        Assert.AreEqual("Finished", ws.Cell(2, 10).GetString());
    }

    [TestMethod]
    public void ExportFixtureToXlsx_WithMultipleMatches_ShouldContainAllRows()
    {
        var matches = new List<MatchResponseRow>
        {
            new MatchResponseRow("M01", "2026-06-11", "Group Stage", "A",
                "Uruguay", "Mexico", "2", "1", "Estadio Azteca", "Finished"),
            new MatchResponseRow("M02", "2026-06-12", "Group Stage", "B",
                "Argentina", "Nigeria", "3", "0", "Estadio Centenario", "Finished"),
            new MatchResponseRow("M03", "2026-06-13", "Round of 16", "",
                "Brazil", "Germany", "1", "1", "MetLife Stadium", "In Progress")
        };

        var result = _service.ExportFixtureToXlsx(matches);

        using var stream = new MemoryStream(result);
        using var workbook = new XLWorkbook(stream);
        var ws = workbook.Worksheet("Fixture");

        Assert.AreEqual("M01", ws.Cell(2, 1).GetString());
        Assert.AreEqual("M02", ws.Cell(3, 1).GetString());
        Assert.AreEqual("M03", ws.Cell(4, 1).GetString());
    }

    [TestMethod]
    public void ExportFixtureToXlsx_ShouldReturnValidXlsxBytes()
    {
        var result = _service.ExportFixtureToXlsx(Enumerable.Empty<MatchResponseRow>());

        Assert.IsNotNull(result);
        Assert.IsTrue(result.Length > 0);

        using var stream = new MemoryStream(result);
        using var workbook = new XLWorkbook(stream);
        Assert.AreEqual(1, workbook.Worksheets.Count);
    }

    [TestMethod]
    public void ExportFixtureToXlsx_ShouldCreateWorksheetNamedFixture()
    {
        var result = _service.ExportFixtureToXlsx(Enumerable.Empty<MatchResponseRow>());

        using var stream = new MemoryStream(result);
        using var workbook = new XLWorkbook(stream);

        Assert.IsTrue(workbook.Worksheets.Any(ws => ws.Name == "Fixture"));
    }

    [TestMethod]
    public void ExportAuditLogToXlsx_ShouldCreateWorksheetNamedAuditLog()
    {
        var result = _service.ExportAuditLogToXlsx(Enumerable.Empty<AuditLog>());

        using var stream = new MemoryStream(result);
        using var workbook = new XLWorkbook(stream);

        Assert.IsTrue(workbook.Worksheets.Any(ws => ws.Name == "Log de Auditoría"));
    }

    // ─── Audit ───

    [TestMethod]
    public void ExportAuditLogToCsv_ShouldLogAuditEntry()
    {
        _service.ExportAuditLogToCsv(Enumerable.Empty<AuditLog>());

        var logs = _auditRepository.GetAll().ToList();
        Assert.IsTrue(logs.Any(l => l.Action == "Exportación"));
        Assert.IsTrue(logs.Any(l => l.Details.Contains("AuditLog") && l.Details.Contains("CSV")));
    }

    [TestMethod]
    public void ExportAuditLogToXlsx_ShouldLogAuditEntry()
    {
        _service.ExportAuditLogToXlsx(Enumerable.Empty<AuditLog>());

        var logs = _auditRepository.GetAll().ToList();
        Assert.IsTrue(logs.Any(l => l.Action == "Exportación"));
        Assert.IsTrue(logs.Any(l => l.Details.Contains("AuditLog") && l.Details.Contains("XLSX")));
    }

    [TestMethod]
    public void ExportFixtureToCsv_ShouldLogAuditEntry()
    {
        _service.ExportFixtureToCsv(Enumerable.Empty<MatchResponseRow>());

        var logs = _auditRepository.GetAll().ToList();
        Assert.IsTrue(logs.Any(l => l.Action == "Exportación"));
        Assert.IsTrue(logs.Any(l => l.Details.Contains("Fixture") && l.Details.Contains("CSV")));
    }

    [TestMethod]
    public void ExportFixtureToXlsx_ShouldLogAuditEntry()
    {
        _service.ExportFixtureToXlsx(Enumerable.Empty<MatchResponseRow>());

        var logs = _auditRepository.GetAll().ToList();
        Assert.IsTrue(logs.Any(l => l.Action == "Exportación"));
        Assert.IsTrue(logs.Any(l => l.Details.Contains("Fixture") && l.Details.Contains("XLSX")));
    }
}
