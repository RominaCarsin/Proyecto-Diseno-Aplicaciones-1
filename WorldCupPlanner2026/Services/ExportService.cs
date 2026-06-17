using System.Globalization;
using ClosedXML.Excel;
using CsvHelper;
using CsvHelper.Configuration;
using Domain;
using IServices;

namespace Services;

public class ExportService : IExportService
{
    private const string ExportAction = "EXPORT";

    private readonly IAuditService _auditService;
    private readonly ISessionService _sessionService;

    public ExportService(IAuditService auditService, ISessionService sessionService)
    {
        _auditService = auditService;
        _sessionService = sessionService;
    }
    public byte[] ExportAuditLogToCsv(IEnumerable<AuditLog> logs)
    {
        using var memoryStream = new MemoryStream();
        using var writer = new StreamWriter(memoryStream, leaveOpen: true);
        using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ","
        });

        csv.WriteField("Marca de Tiempo");
        csv.WriteField("Email del Actor");
        csv.WriteField("Acción");
        csv.WriteField("Detalles");
        csv.NextRecord();

        foreach (var log in logs)
        {
            csv.WriteField(log.Timestamp.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
            csv.WriteField(log.ActorEmail);
            csv.WriteField(log.Action);
            csv.WriteField(log.Details);
            csv.NextRecord();
        }

        writer.Flush();
        var result = memoryStream.ToArray();
        LogExport("AuditLog", "CSV");
        return result;
    }

    public byte[] ExportAuditLogToXlsx(IEnumerable<AuditLog> logs)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.AddWorksheet("Log de Auditoría");

        ws.Cell(1, 1).Value = "Marca de Tiempo";
        ws.Cell(1, 2).Value = "Email del Actor";
        ws.Cell(1, 3).Value = "Acción";
        ws.Cell(1, 4).Value = "Detalles";

        var headerRange = ws.Range(1, 1, 1, 4);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

        var row = 2;
        foreach (var log in logs)
        {
            ws.Cell(row, 1).Value = log.Timestamp.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
            ws.Cell(row, 2).Value = log.ActorEmail;
            ws.Cell(row, 3).Value = log.Action;
            ws.Cell(row, 4).Value = log.Details;
            row++;
        }

        ws.Columns().AdjustToContents();

        using var memoryStream = new MemoryStream();
        workbook.SaveAs(memoryStream);
        var result = memoryStream.ToArray();
        LogExport("AuditLog", "XLSX");
        return result;
    }

    public byte[] ExportFixtureToCsv(IEnumerable<MatchResponseRow> matches)
    {
        using var memoryStream = new MemoryStream();
        using var writer = new StreamWriter(memoryStream, leaveOpen: true);
        using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ","
        });

        csv.WriteField("Código");
        csv.WriteField("Fecha");
        csv.WriteField("Fase");
        csv.WriteField("Grupo");
        csv.WriteField("Equipo Local");
        csv.WriteField("Equipo Visitante");
        csv.WriteField("Goles Local");
        csv.WriteField("Goles Visitante");
        csv.WriteField("Estadio");
        csv.WriteField("Estado");
        csv.NextRecord();

        foreach (var match in matches)
        {
            csv.WriteField(match.MatchCode);
            csv.WriteField(match.Date);
            csv.WriteField(match.Phase);
            csv.WriteField(match.Group);
            csv.WriteField(match.HomeTeam);
            csv.WriteField(match.AwayTeam);
            csv.WriteField(match.HomeGoals);
            csv.WriteField(match.AwayGoals);
            csv.WriteField(match.Stadium);
            csv.WriteField(match.Status);
            csv.NextRecord();
        }

        writer.Flush();
        var fixtureCsvResult = memoryStream.ToArray();
        LogExport("Fixture", "CSV");
        return fixtureCsvResult;
    }

    public byte[] ExportFixtureToXlsx(IEnumerable<MatchResponseRow> matches)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.AddWorksheet("Fixture");

        ws.Cell(1, 1).Value = "Código";
        ws.Cell(1, 2).Value = "Fecha";
        ws.Cell(1, 3).Value = "Fase";
        ws.Cell(1, 4).Value = "Grupo";
        ws.Cell(1, 5).Value = "Equipo Local";
        ws.Cell(1, 6).Value = "Equipo Visitante";
        ws.Cell(1, 7).Value = "Goles Local";
        ws.Cell(1, 8).Value = "Goles Visitante";
        ws.Cell(1, 9).Value = "Estadio";
        ws.Cell(1, 10).Value = "Estado";

        var headerRange = ws.Range(1, 1, 1, 10);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

        var row = 2;
        foreach (var match in matches)
        {
            ws.Cell(row, 1).Value = match.MatchCode;
            ws.Cell(row, 2).Value = match.Date;
            ws.Cell(row, 3).Value = match.Phase;
            ws.Cell(row, 4).Value = match.Group;
            ws.Cell(row, 5).Value = match.HomeTeam;
            ws.Cell(row, 6).Value = match.AwayTeam;
            ws.Cell(row, 7).Value = match.HomeGoals;
            ws.Cell(row, 8).Value = match.AwayGoals;
            ws.Cell(row, 9).Value = match.Stadium;
            ws.Cell(row, 10).Value = match.Status;
            row++;
        }

        ws.Columns().AdjustToContents();

        using var memoryStream2 = new MemoryStream();
        workbook.SaveAs(memoryStream2);
        var fixtureXlsxResult = memoryStream2.ToArray();
        LogExport("Fixture", "XLSX");
        return fixtureXlsxResult;
    }

    private void LogExport(string fileName, string format)
    {
        var actor = _sessionService.GetCurrentUserEmail() ?? "system";
        _auditService.Log(actor, ExportAction, $"{fileName} exportado como {format}");
    }
}
