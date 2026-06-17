using Domain;

namespace IServices;

public interface IExportService
{
    byte[] ExportAuditLogToCsv(IEnumerable<AuditLog> logs);
    byte[] ExportAuditLogToXlsx(IEnumerable<AuditLog> logs);
    byte[] ExportFixtureToCsv(IEnumerable<MatchResponseRow> matches);
    byte[] ExportFixtureToXlsx(IEnumerable<MatchResponseRow> matches);
}

public record MatchResponseRow(
    string MatchCode,
    string Date,
    string Phase,
    string Group,
    string HomeTeam,
    string AwayTeam,
    string HomeGoals,
    string AwayGoals,
    string Stadium,
    string Status
);
