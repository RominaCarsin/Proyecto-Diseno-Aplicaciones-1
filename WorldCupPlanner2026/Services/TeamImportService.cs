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

    private static readonly string[] ExpectedHeaders =
    [
        NameHeader,
        ConfederationHeader,
        RankingHeader
    ];

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
        return Process(csvContent, shouldPersistRows: false);
    }

    public TeamImportResult Import(string csvContent)
    {
        return Process(csvContent, shouldPersistRows: true);
    }

    private TeamImportResult Process(string csvContent, bool shouldPersistRows)
    {
        var result = new TeamImportResult();
        var actorEmail = GetActorEmail();

        if (CsvContentIsEmpty(csvContent))
            return BuildGlobalErrorResult(result, EmptyCsvErrorMessage());

        var lines = GetCsvLines(csvContent);

        if (CsvHasNoLines(lines))
            return BuildGlobalErrorResult(result, EmptyCsvErrorMessage());

        var headerErrorMessage = ValidateHeaders(lines[0]);

        if (HeaderIsInvalid(headerErrorMessage))
            return HandleInvalidHeader(result, headerErrorMessage, actorEmail, shouldPersistRows);

        ProcessRows(lines, result, actorEmail, shouldPersistRows);
        UpdateResultSummary(result);

        if (shouldPersistRows)
            AuditImportSummary(actorEmail, result);

        return result;
    }

    private string GetActorEmail()
    {
        return _sessionService.GetCurrentUserEmail() ?? SystemActorEmail;
    }

    private static bool CsvContentIsEmpty(string csvContent)
    {
        return string.IsNullOrWhiteSpace(csvContent);
    }

    private static List<string> GetCsvLines(string csvContent)
    {
        return csvContent
            .Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.TrimEnd('\r'))
            .ToList();
    }

    private static bool CsvHasNoLines(List<string> lines)
    {
        return lines.Count == 0;
    }

    private static TeamImportResult BuildGlobalErrorResult(
        TeamImportResult result,
        string errorMessage)
    {
        AddGlobalError(result, errorMessage);

        return result;
    }

    private static bool HeaderIsInvalid(string headerErrorMessage)
    {
        return !string.IsNullOrWhiteSpace(headerErrorMessage);
    }

    private TeamImportResult HandleInvalidHeader(
        TeamImportResult result,
        string headerErrorMessage,
        string actorEmail,
        bool shouldPersistRows)
    {
        AddGlobalError(result, headerErrorMessage);

        if (shouldPersistRows)
            AuditImportError(actorEmail, headerErrorMessage);

        return result;
    }

    private void ProcessRows(
        List<string> lines,
        TeamImportResult result,
        string actorEmail,
        bool shouldPersistRows)
    {
        for (var lineIndex = 1; lineIndex < lines.Count; lineIndex++)
        {
            var rowNumber = lineIndex + 1;
            var row = ParseRow(lines[lineIndex], rowNumber);

            ProcessRow(row, result, actorEmail, shouldPersistRows);
        }
    }

    private void ProcessRow(
        TeamImportRowResult row,
        TeamImportResult result,
        string actorEmail,
        bool shouldPersistRows)
    {
        if (!row.Success)
        {
            RegisterFailedRow(row, result, actorEmail, shouldPersistRows);
            return;
        }

        if (shouldPersistRows)
            PersistRow(row, actorEmail);

        result.Rows.Add(row);
    }

    private void RegisterFailedRow(
        TeamImportRowResult row,
        TeamImportResult result,
        string actorEmail,
        bool shouldPersistRows)
    {
        result.Rows.Add(row);

        if (shouldPersistRows)
            AuditImportError(actorEmail, $"Fila {row.RowNumber}: {row.ErrorMessage}");
    }

    private void PersistRow(TeamImportRowResult row, string actorEmail)
    {
        try
        {
            var team = row.Team!;

            _teamService.AddTeam(ToTeamRequest(team));

            AuditImportSuccess(actorEmail, row.RowNumber, team);
        }
        catch (Exception exception)
        {
            MarkRowAsFailed(row, exception.Message);
            AuditImportError(actorEmail, $"Fila {row.RowNumber}: {exception.Message}");
        }
    }

    private static TeamRequest ToTeamRequest(TeamResponse team)
    {
        return new TeamRequest
        {
            Name = team.Name,
            ConfederationName = team.ConfederationName,
            RankingFifa = team.RankingFifa
        };
    }

    private static void MarkRowAsFailed(TeamImportRowResult row, string errorMessage)
    {
        row.Success = false;
        row.ErrorMessage = errorMessage;
        row.Team = null;
    }

    private static TeamImportRowResult ParseRow(string line, int rowNumber)
    {
        var columns = SplitCsvLine(line);

        if (ColumnCountIsInvalid(columns))
            return Failure(rowNumber, InvalidColumnCountErrorMessage());

        var name = columns[0].Trim();
        var confederation = columns[1].Trim();
        var rankingText = columns[2].Trim();

        if (!RankingIsValid(rankingText, out var ranking))
            return Failure(rowNumber, InvalidRankingErrorMessage());

        return SuccessfulRow(rowNumber, name, confederation, ranking);
    }

    private static bool ColumnCountIsInvalid(string[] columns)
    {
        return columns.Length != ExpectedHeaders.Length;
    }

    private static bool RankingIsValid(string rankingText, out int ranking)
    {
        return int.TryParse(rankingText, out ranking);
    }

    private static TeamImportRowResult SuccessfulRow(
        int rowNumber,
        string name,
        string confederation,
        int ranking)
    {
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
        var headers = SplitCsvLine(headerLine)
            .Select(header => header.Trim())
            .ToArray();

        if (ColumnCountIsInvalid(headers))
            return InvalidHeaderErrorMessage();

        return HeadersAreValid(headers)
            ? string.Empty
            : InvalidHeaderErrorMessage();
    }

    private static bool HeadersAreValid(string[] headers)
    {
        for (var index = 0; index < ExpectedHeaders.Length; index++)
        {
            if (!string.Equals(headers[index], ExpectedHeaders[index], StringComparison.Ordinal))
                return false;
        }

        return true;
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

    private static void UpdateResultSummary(TeamImportResult result)
    {
        result.TotalRows = result.Rows.Count;
        result.SuccessfulRows = result.Rows.Count(row => row.Success);
        result.FailedRows = result.Rows.Count(row => !row.Success);
    }

    private void AuditImportSuccess(
        string actorEmail,
        int rowNumber,
        TeamResponse team)
    {
        _auditService.Log(
            actorEmail,
            ImportSuccessAction,
            $"Fila {rowNumber}: {team.Name} ({team.ConfederationName}) ranking {team.RankingFifa}");
    }

    private void AuditImportError(string actorEmail, string errorMessage)
    {
        _auditService.Log(actorEmail, ImportErrorAction, errorMessage);
    }

    private void AuditImportSummary(string actorEmail, TeamImportResult result)
    {
        _auditService.Log(
            actorEmail,
            ImportExecutedAction,
            $"Filas: {result.TotalRows}; exitosas: {result.SuccessfulRows}; errores: {result.FailedRows}");
    }

    private static string EmptyCsvErrorMessage()
    {
        return "El archivo CSV esta vacio.";
    }

    private static string InvalidHeaderErrorMessage()
    {
        return $"El encabezado debe ser exactamente: {NameHeader},{ConfederationHeader},{RankingHeader}";
    }

    private static string InvalidColumnCountErrorMessage()
    {
        return "La fila debe tener exactamente 3 columnas.";
    }

    private static string InvalidRankingErrorMessage()
    {
        return "RankingFIFA debe ser numerico.";
    }
}