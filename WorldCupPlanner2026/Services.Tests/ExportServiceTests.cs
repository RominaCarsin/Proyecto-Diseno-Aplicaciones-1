using System.Globalization;
using System.Text;
using ClosedXML.Excel;
using Domain;
using IServices;

namespace Services.Tests;

[TestClass]
public class ExportServiceTests
{
    private ExportService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _service = new ExportService();
    }

    [TestMethod]
    public void ExportAuditLogToCsv_WithEmptyLogs_ShouldReturnOnlyHeaderRow()
    {
        var result = _service.ExportAuditLogToCsv(Enumerable.Empty<AuditLog>());

        var csv = Encoding.UTF8.GetString(result);
        var lines = csv.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        Assert.AreEqual(1, lines.Length, "Solo debe existir la fila de encabezado.");
        Assert.IsTrue(lines[0].Contains("Timestamp"));
        Assert.IsTrue(lines[0].Contains("ActorEmail"));
        Assert.IsTrue(lines[0].Contains("Action"));
        Assert.IsTrue(lines[0].Contains("Details"));
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

        Assert.IsTrue(csv.Contains("/"), "El timestamp debe contener separadores de fecha.");
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
        var ws = workbook.Worksheet("Audit Log");

        Assert.IsNotNull(ws);
        Assert.AreEqual("Timestamp", ws.Cell(1, 1).GetString());
        Assert.AreEqual("ActorEmail", ws.Cell(1, 2).GetString());
        Assert.AreEqual("Action", ws.Cell(1, 3).GetString());
        Assert.AreEqual("Details", ws.Cell(1, 4).GetString());
        Assert.AreEqual("", ws.Cell(2, 1).GetString(), "No debe haber datos en la fila 2.");
    }

    [TestMethod]
    public void ExportAuditLogToXlsx_ShouldHaveBoldHeader()
    {
        var result = _service.ExportAuditLogToXlsx(Enumerable.Empty<AuditLog>());

        using var stream = new MemoryStream(result);
        using var workbook = new XLWorkbook(stream);
        var ws = workbook.Worksheet("Audit Log");

        Assert.IsTrue(ws.Cell(1, 1).Style.Font.Bold, "El header debe estar en negrita.");
    }

    [TestMethod]
    public void ExportAuditLogToXlsx_ShouldHaveLightGrayHeaderBackground()
    {
        var result = _service.ExportAuditLogToXlsx(Enumerable.Empty<AuditLog>());

        using var stream = new MemoryStream(result);
        using var workbook = new XLWorkbook(stream);
        var ws = workbook.Worksheet("Audit Log");

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
        var ws = workbook.Worksheet("Audit Log");

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
        var ws = workbook.Worksheet("Audit Log");

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
        Assert.IsTrue(lines[0].Contains("MatchCode"));
        Assert.IsTrue(lines[0].Contains("Date"));
        Assert.IsTrue(lines[0].Contains("Phase"));
        Assert.IsTrue(lines[0].Contains("Group"));
        Assert.IsTrue(lines[0].Contains("HomeTeam"));
        Assert.IsTrue(lines[0].Contains("AwayTeam"));
        Assert.IsTrue(lines[0].Contains("HomeGoals"));
        Assert.IsTrue(lines[0].Contains("AwayGoals"));
        Assert.IsTrue(lines[0].Contains("Stadium"));
        Assert.IsTrue(lines[0].Contains("Status"));
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
        Assert.AreEqual("MatchCode", ws.Cell(1, 1).GetString());
        Assert.AreEqual("Date", ws.Cell(1, 2).GetString());
        Assert.AreEqual("Phase", ws.Cell(1, 3).GetString());
        Assert.AreEqual("Group", ws.Cell(1, 4).GetString());
        Assert.AreEqual("HomeTeam", ws.Cell(1, 5).GetString());
        Assert.AreEqual("AwayTeam", ws.Cell(1, 6).GetString());
        Assert.AreEqual("HomeGoals", ws.Cell(1, 7).GetString());
        Assert.AreEqual("AwayGoals", ws.Cell(1, 8).GetString());
        Assert.AreEqual("Stadium", ws.Cell(1, 9).GetString());
        Assert.AreEqual("Status", ws.Cell(1, 10).GetString());
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

        Assert.IsTrue(workbook.Worksheets.Any(ws => ws.Name == "Audit Log"));
    }
}
