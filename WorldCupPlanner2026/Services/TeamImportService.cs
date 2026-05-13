using DTOs;
using IServices;

namespace Services;

public class TeamImportService : ITeamImportService
{
    private const string ConfederationHeader = "Confederaci\u00f3n";
    private static readonly string[] ExpectedHeaders = ["Nombre", ConfederationHeader, "RankingFIFA"];

    private readonly ITeamService _teamService;
    private readonly IAuditService _auditService;
    private readonly ISessionService _sessionService;

    public TeamImportService(
        ITeamService teamService,
        IAuditService auditService,
        ISessionService sessionService)
    {
        _teamService = teamService;
        _auditService = auditService;
        _sessionService = sessionService;
    }

    public TeamImportResult Preview(string csvContent)
    {
        return Process(csvContent, persist: false);
    }

    public TeamImportResult Import(string csvContent)
    {
        return Process(csvContent, persist: true);
    }

    private TeamImportResult Process(string csvContent, bool persist)
    {
        var result = new TeamImportResult();
        var actorEmail = _sessionService.GetCurrentUserEmail() ?? "system";

        if (string.IsNullOrWhiteSpace(csvContent))
        {
            AddGlobalError(result, "El archivo CSV esta vacio.");
            return result;
        }

        var lines = csvContent
            .Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.TrimEnd('\r'))
            .ToList();

        if (lines.Count == 0)
        {
            AddGlobalError(result, "El archivo CSV esta vacio.");
            return result;
        }

        var headerError = ValidateHeaders(lines[0]);
        if (!string.IsNullOrWhiteSpace(headerError))
        {
            AddGlobalError(result, headerError);
            if (persist)
                _auditService.Log(actorEmail, "Teams import error", headerError);

            return result;
        }

        for (var index = 1; index < lines.Count; index++)
        {
            var rowNumber = index + 1;
            var row = ParseRow(lines[index], rowNumber);

            if (!row.Success)
            {
                result.Rows.Add(row);
                if (persist)
                    _auditService.Log(actorEmail, "Teams import error", $"Fila {rowNumber}: {row.ErrorMessage}");
                continue;
            }

            if (persist)
                PersistRow(row, actorEmail);

            result.Rows.Add(row);
        }

        result.TotalRows = result.Rows.Count;
        result.SuccessfulRows = result.Rows.Count(row => row.Success);
        result.FailedRows = result.Rows.Count(row => !row.Success);

        if (persist)
        {
            _auditService.Log(
                actorEmail,
                "Teams import executed",
                $"Filas: {result.TotalRows}; exitosas: {result.SuccessfulRows}; errores: {result.FailedRows}");
        }

        return result;
    }

    private void PersistRow(TeamImportRowResult row, string actorEmail)
    {
        try
        {
            var team = row.Team!;
            _teamService.AddTeam(new TeamRequest
            {
                Name = team.Name,
                ConfederationName = team.ConfederationName,
                RankingFifa = team.RankingFifa
            });

            _auditService.Log(
                actorEmail,
                "Teams import success",
                $"Fila {row.RowNumber}: {team.Name} ({team.ConfederationName}) ranking {team.RankingFifa}");
        }
        catch (Exception ex)
        {
            row.Success = false;
            row.ErrorMessage = ex.Message;
            row.Team = null;
            _auditService.Log(actorEmail, "Teams import error", $"Fila {row.RowNumber}: {ex.Message}");
        }
    }

    private static TeamImportRowResult ParseRow(string line, int rowNumber)
    {
        var columns = SplitCsvLine(line);
        if (columns.Length != ExpectedHeaders.Length)
        {
            return Failure(rowNumber, "La fila debe tener exactamente 3 columnas.");
        }

        var name = columns[0].Trim();
        var confederation = columns[1].Trim();
        var rankingText = columns[2].Trim();

        if (!int.TryParse(rankingText, out var ranking))
            return Failure(rowNumber, "RankingFIFA debe ser numerico.");

        return new TeamImportRowResult
        {
            RowNumber = rowNumber,
            Success = true,
            Team = new TeamResponse
            {
                Name = name,
                ConfederationName = confederation,
                RankingFifa = ranking
            }
        };
    }

    private static string ValidateHeaders(string headerLine)
    {
        var headers = SplitCsvLine(headerLine).Select(header => header.Trim()).ToArray();

        if (headers.Length != ExpectedHeaders.Length)
            return $"El encabezado debe ser exactamente: Nombre,{ConfederationHeader},RankingFIFA";

        for (var index = 0; index < ExpectedHeaders.Length; index++)
        {
            if (!string.Equals(headers[index], ExpectedHeaders[index], StringComparison.Ordinal))
                return $"El encabezado debe ser exactamente: Nombre,{ConfederationHeader},RankingFIFA";
        }

        return string.Empty;
    }

    private static string[] SplitCsvLine(string line)
    {
        return line.Split(',');
    }

    private static void AddGlobalError(TeamImportResult result, string message)
    {
        result.Rows.Add(Failure(1, message));
        result.TotalRows = 0;
        result.SuccessfulRows = 0;
        result.FailedRows = 1;
    }

    private static TeamImportRowResult Failure(int rowNumber, string message)
    {
        return new TeamImportRowResult
        {
            RowNumber = rowNumber,
            Success = false,
            ErrorMessage = message
        };
    }
}
