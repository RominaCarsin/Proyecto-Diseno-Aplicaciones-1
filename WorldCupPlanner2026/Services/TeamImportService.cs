using DTOs;
using IServices;

namespace Services;

public class TeamImportService : ITeamImportService
{
    private const string SystemActorEmail = "sistema";
    private const string NameHeader = "Nombre";
    private const string ConfederationHeader = "Confederación";
    private const string RankingHeader = "RankingFIFA";
    private const string ImportErrorAction = "Error al importar equipos";
    private const string ImportSuccessAction = "Importación de equipos exitosa";
    private const string ImportExecutedAction = "Importación de equipos ejecutada";
    private const string EmptyCsvErrorMessage = "El archivo CSV esta vacio.";
    private const string ExpectedHeader = "Nombre,Confederación,RankingFIFA";
    private const string InvalidHeaderErrorMessage = $"El encabezado debe ser exactamente: {ExpectedHeader}";
    private const string InvalidColumnCountErrorMessage = "La fila debe tener exactamente 3 columnas.";
    private const string InvalidRankingErrorMessage = "RankingFIFA debe ser numerico.";

    private static readonly string[] ExpectedHeaders = [NameHeader, ConfederationHeader, RankingHeader];

    private readonly ITeamService _teamService;
    private readonly IAuditService _auditService;
    private readonly ISessionService _sessionService;

    public TeamImportService(ITeamService teamService, IAuditService auditService, ISessionService sessionService)
    {
        _teamService = teamService;
        _auditService = auditService;
        _sessionService = sessionService;
    }

    public TeamImportResult Preview(string csvContent) =>
        Process(csvContent, shouldPersistRows: false);

    public TeamImportResult Import(string csvContent) =>
        Process(csvContent, shouldPersistRows: true);

    private TeamImportResult Process(string csvContent, bool shouldPersistRows)
    {
        var result = new TeamImportResult();
        var actorEmail = _sessionService.GetCurrentUserEmail() ?? SystemActorEmail;

        if (string.IsNullOrWhiteSpace(csvContent))
            return BuildGlobalErrorResult(result, EmptyCsvErrorMessage);

        var lines = csvContent.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.TrimEnd('\r')).ToList();

        if (lines.Count == 0)
            return BuildGlobalErrorResult(result, EmptyCsvErrorMessage);

        var headers = SplitCsvLine(lines[0]).Select(h => h.Trim()).ToArray();

        if (headers.Length != ExpectedHeaders.Length || !HeadersAreValid(headers))
            return HandleInvalidHeader(result, InvalidHeaderErrorMessage, actorEmail, shouldPersistRows);

        for (var lineIndex = 1; lineIndex < lines.Count; lineIndex++)
        {
            var rowNumber = lineIndex + 1;
            var columns = SplitCsvLine(lines[lineIndex]);

            TeamImportRowResult row;

            if (columns.Length != ExpectedHeaders.Length)
                row = Failure(rowNumber, InvalidColumnCountErrorMessage);
            else if (!int.TryParse(columns[2].Trim(), out var ranking))
                row = Failure(rowNumber, InvalidRankingErrorMessage);
            else
                row = SuccessfulRow(rowNumber, columns[0].Trim(), columns[1].Trim(), ranking);

            if (!row.Success)
            {
                result.Rows.Add(row);
                if (shouldPersistRows)
                    AuditImportError(actorEmail, $"Fila {row.RowNumber}: {row.ErrorMessage}");
                continue;
            }

            if (shouldPersistRows)
                PersistRow(row, actorEmail);

            result.Rows.Add(row);
        }

        result.TotalRows = result.Rows.Count;
        result.SuccessfulRows = result.Rows.Count(row => row.Success);
        result.FailedRows = result.Rows.Count(row => !row.Success);

        if (shouldPersistRows)
            AuditImportSummary(actorEmail, result);

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
            AuditImportSuccess(actorEmail, row.RowNumber, team);
        }
        catch (Exception exception)
        {
            row.Success = false;
            row.ErrorMessage = exception.Message;
            row.Team = null;
            AuditImportError(actorEmail, $"Fila {row.RowNumber}: {exception.Message}");
        }
    }

    private static TeamImportResult BuildGlobalErrorResult(TeamImportResult result, string errorMessage)
    {
        result.Rows.Add(Failure(1, errorMessage));
        result.TotalRows = 0;
        result.SuccessfulRows = 0;
        result.FailedRows = 1;
        return result;
    }

    private TeamImportResult HandleInvalidHeader(
        TeamImportResult result, string errorMessage, string actorEmail, bool shouldPersistRows)
    {
        result.Rows.Add(Failure(1, errorMessage));
        result.TotalRows = 0;
        result.SuccessfulRows = 0;
        result.FailedRows = 1;

        if (shouldPersistRows)
            AuditImportError(actorEmail, errorMessage);

        return result;
    }

    private static TeamImportRowResult SuccessfulRow(int rowNumber, string name, string confederation, int ranking) =>
        new()
        {
            RowNumber = rowNumber,
            Success = true,
            Team = new TeamResponse { Name = name, ConfederationName = confederation, RankingFifa = ranking }
        };

    private static TeamImportRowResult Failure(int rowNumber, string message) =>
        new() { RowNumber = rowNumber, Success = false, ErrorMessage = message };

    private static bool HeadersAreValid(string[] headers)
    {
        for (var index = 0; index < ExpectedHeaders.Length; index++)
        {
            if (!string.Equals(headers[index], ExpectedHeaders[index], StringComparison.Ordinal))
                return false;
        }
        return true;
    }

    private static string[] SplitCsvLine(string line) => line.Split(',');

    private void AuditImportSuccess(string actorEmail, int rowNumber, TeamResponse team) =>
        _auditService.Log(actorEmail, ImportSuccessAction,
            $"Fila {rowNumber}: {team.Name} ({team.ConfederationName}) ranking {team.RankingFifa}");

    private void AuditImportError(string actorEmail, string errorMessage) =>
        _auditService.Log(actorEmail, ImportErrorAction, errorMessage);

    private void AuditImportSummary(string actorEmail, TeamImportResult result) =>
        _auditService.Log(actorEmail, ImportExecutedAction,
            $"Filas: {result.TotalRows}; exitosas: {result.SuccessfulRows}; errores: {result.FailedRows}");
}
