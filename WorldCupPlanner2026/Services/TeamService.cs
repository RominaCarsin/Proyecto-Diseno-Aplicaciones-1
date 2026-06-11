using System.Globalization;
using System.Text;
using Domain;
using DTOs;
using IRepository;
using IServices;
using Services.Mappers;

namespace Services;

public class TeamService : ITeamService
{
    private const int RequiredTeamsCount = 48;
    private const int FirstFifaRanking = 300;
    private const int LastFifaRanking = 2500;
    private const string SystemActorEmail = "system";
    private const string TeamNotFoundErrorMessage = "No se encontro el equipo.";
    private const string TeamNameAlreadyExistsErrorMessage = "Ya existe un equipo con este nombre.";
    private const string NameHeader = "Nombre";
    private const string ConfederationHeader = "Confederacion";
    private const string RankingHeader = "RankingFIFA";
    private const string ImportErrorAction = "Error al importar equipos";
    private const string ImportSuccessAction = "Importacion de equipos exitosa";
    private const string ImportExecutedAction = "Importacion de equipos ejecutada";
    private const string EmptyCsvErrorMessage = "El archivo CSV esta vacio.";
    private const string InvalidColumnCountErrorMessage = "La fila debe tener exactamente 3 columnas.";
    private const string InvalidRankingErrorMessage = "RankingFIFA debe ser numerico.";

    private static readonly string[] ExpectedHeaders = [NameHeader, ConfederationHeader, RankingHeader];

    private readonly IRepository<Team> _repository;
    private readonly IAuditService _auditService;
    private readonly ISessionService _sessionService;
    private readonly List<Confederation> _confederations =
    [
        new("UEFA", 16),
        new("CAF", 9),
        new("AFC", 8),
        new("CONCACAF", 7),
        new("CONMEBOL", 7),
        new("OFC", 1),
    ];

    public TeamService(
        IRepository<Team> repository,
        IAuditService auditService,
        ISessionService sessionService)
    {
        _repository = repository;
        _auditService = auditService;
        _sessionService = sessionService;
    }

    public void AddTeam(TeamRequest teamRequest)
    {
        EnsureTeamNameIsNotTaken(teamRequest.Name);

        var team = TeamMapper.ToEntity(teamRequest);
        AddTeamToConfederation(team);
        _repository.Add(team);

        LogAction("Team created",
            $"Team '{teamRequest.Name}' added to {teamRequest.ConfederationName} with FIFA ranking {teamRequest.RankingFifa}");
    }

    public IEnumerable<TeamResponse> GetTeams() =>
        _repository.GetAll().Select(TeamMapper.FromEntity);

    public IEnumerable<TeamResponse> GetTeamsOrdered(int seed)
    {
        var orderedTeams = _repository.GetAll()
            .OrderByDescending(team => team.RankingFifa)
            .ToList();

        new ShuffleService(seed).ShuffleByRanking(orderedTeams);

        return orderedTeams.Select(TeamMapper.FromEntity);
    }

    public void UpdateTeam(string currentName, TeamRequest updatedTeamData)
    {
        var team = FindTeamByNameOrThrow(currentName);
        EnsureTeamNameIsNotTakenByAnotherTeam(currentName, updatedTeamData.Name);

        var previousConfederationName = team.ConfederationName;
        var isChangingConfederation = !string.Equals(
            previousConfederationName,
            updatedTeamData.ConfederationName,
            StringComparison.OrdinalIgnoreCase);

        if (isChangingConfederation)
            MoveTeamToConfederation(team, updatedTeamData.ConfederationName);

        team.UpdateName(updatedTeamData.Name);
        team.UpdateRankingFifa(updatedTeamData.RankingFifa);
        team.UpdateFlag(updatedTeamData.FlagBase64);
        _repository.Update(team);

        LogAction("Team updated",
            $"Team '{currentName}' updated. New name: '{updatedTeamData.Name}', confederation: {updatedTeamData.ConfederationName}, ranking: {updatedTeamData.RankingFifa}");
    }

    public void DeleteTeam(string name)
    {
        var team = FindTeamByNameOrThrow(name);
        RemoveTeamFromConfederation(team);
        _repository.Delete(team);

        LogAction("Team deleted", $"Team '{name}' was removed from the system");
    }

    public void AutoCompleteTeams(int seed)
    {
        var random = new Random(seed);

        foreach (var confederation in _confederations)
        {
            var remainingGlobalSlots = RequiredTeamsCount - _repository.GetAll().Count();
            if (remainingGlobalSlots <= 0)
                return;

            var teamsToCreate = Math.Min(confederation.RemainingSlots, remainingGlobalSlots);
            AddFictionalTeamsToConfederation(confederation.Name, teamsToCreate, random);
        }
    }

    public IEnumerable<ConfederationResponse> GetConfederations() =>
        _confederations.Select(ConfederationMapper.FromEntity);

    public TeamImportResult PreviewImport(string csvContent) =>
        ProcessImport(csvContent, shouldPersistRows: false);

    public TeamImportResult Import(string csvContent) =>
        ProcessImport(csvContent, shouldPersistRows: true);

    private TeamImportResult ProcessImport(string csvContent, bool shouldPersistRows)
    {
        var result = new TeamImportResult();
        var actorEmail = GetActorEmail();

        if (string.IsNullOrWhiteSpace(csvContent))
            return BuildGlobalErrorResult(result, EmptyCsvErrorMessage);

        var lines = csvContent.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.TrimEnd('\r'))
            .ToList();

        if (lines.Count == 0)
            return BuildGlobalErrorResult(result, EmptyCsvErrorMessage);

        var headers = SplitCsvLine(lines[0]).Select(header => NormalizeHeader(header.Trim())).ToArray();
        if (headers.Length != ExpectedHeaders.Length || !HeadersAreValid(headers))
            return HandleInvalidHeader(result, actorEmail, shouldPersistRows);

        for (var lineIndex = 1; lineIndex < lines.Count; lineIndex++)
        {
            var rowNumber = lineIndex + 1;
            var row = BuildImportRow(rowNumber, SplitCsvLine(lines[lineIndex]));

            if (row.Success && shouldPersistRows)
                PersistImportRow(row, actorEmail);
            else if (!row.Success && shouldPersistRows)
                AuditImportError(actorEmail, $"Fila {row.RowNumber}: {row.ErrorMessage}");

            result.Rows.Add(row);
        }

        result.TotalRows = result.Rows.Count;
        result.SuccessfulRows = result.Rows.Count(row => row.Success);
        result.FailedRows = result.Rows.Count(row => !row.Success);

        if (shouldPersistRows)
            AuditImportSummary(actorEmail, result);

        return result;
    }

    private TeamImportRowResult BuildImportRow(int rowNumber, string[] columns)
    {
        if (columns.Length != ExpectedHeaders.Length)
            return Failure(rowNumber, InvalidColumnCountErrorMessage);

        if (!int.TryParse(columns[2].Trim(), out var ranking))
            return Failure(rowNumber, InvalidRankingErrorMessage);

        return new TeamImportRowResult
        {
            RowNumber = rowNumber,
            Success = true,
            Team = new TeamResponse
            {
                Name = columns[0].Trim(),
                ConfederationName = columns[1].Trim(),
                RankingFifa = ranking
            }
        };
    }

    private void PersistImportRow(TeamImportRowResult row, string actorEmail)
    {
        try
        {
            var team = row.Team!;
            AddTeam(new TeamRequest
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

    private Team? FindByName(string name) =>
        _repository.GetAll().FirstOrDefault(team => team.Name == Team.Normalize(name));

    private Team FindTeamByNameOrThrow(string name) =>
        FindByName(name) ?? throw new ArgumentException(TeamNotFoundErrorMessage);

    private void EnsureTeamNameIsNotTaken(string name)
    {
        if (FindByName(name) is not null)
            throw new ArgumentException(TeamNameAlreadyExistsErrorMessage);
    }

    private void EnsureTeamNameIsNotTakenByAnotherTeam(string currentName, string newName)
    {
        var teamWithNewName = FindByName(newName);
        if (teamWithNewName is not null && teamWithNewName.Name != Team.Normalize(currentName))
            throw new ArgumentException(TeamNameAlreadyExistsErrorMessage);
    }

    private void AddTeamToConfederation(Team team)
    {
        var confederation = FindConfederationByNameOrThrow(team.ConfederationName);
        confederation.AddTeam(team);
    }

    private void RemoveTeamFromConfederation(Team team)
    {
        var confederation = FindConfederationByNameOrThrow(team.ConfederationName);
        confederation.RemoveTeam(team);
    }

    private void MoveTeamToConfederation(Team team, string newConfederationName)
    {
        var oldConfederation = FindConfederationByNameOrThrow(team.ConfederationName);
        var newConfederation = FindConfederationByNameOrThrow(newConfederationName);

        oldConfederation.RemoveTeam(team);
        try
        {
            team.UpdateConfederationName(newConfederationName);
            newConfederation.AddTeam(team);
        }
        catch
        {
            team.UpdateConfederationName(oldConfederation.Name);
            oldConfederation.AddTeam(team);
            throw;
        }
    }

    private Confederation FindConfederationByNameOrThrow(string name) =>
        _confederations.FirstOrDefault(confederation =>
            confederation.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
        ?? throw new ArgumentException($"No se encontro la confederacion '{name}'.");

    public IEnumerable<TeamResponse> GetTeamsByRanking() =>
        _repository.GetAll()
            .OrderByDescending(team => team.RankingFifa)
            .Select(TeamMapper.FromEntity);
    
    private void AddFictionalTeamsToConfederation(string confederationName, int teamsToCreate, Random random)
    {
        var existingNames = _repository.GetAll()
            .Where(t => t.Name.StartsWith($"{confederationName}_", StringComparison.OrdinalIgnoreCase))
            .Select(t => t.Name)
            .ToHashSet();

        int nextIndex = 1;
        for (int i = 0; i < teamsToCreate; i++)
        {
            while (existingNames.Contains($"{confederationName}_{nextIndex:00}"))
                nextIndex++;

            AddTeam(new TeamRequest
            {
                Name = $"{confederationName}_{nextIndex:00}",
                ConfederationName = confederationName,
                RankingFifa = random.Next(FirstFifaRanking, LastFifaRanking + 1)
            });

            existingNames.Add($"{confederationName}_{nextIndex:00}");
            nextIndex++;
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
        TeamImportResult result,
        string actorEmail,
        bool shouldPersistRows)
    {
        var errorMessage = "El encabezado debe ser exactamente: Nombre,Confederacion,RankingFIFA";
        result.Rows.Add(Failure(1, errorMessage));
        result.TotalRows = 0;
        result.SuccessfulRows = 0;
        result.FailedRows = 1;

        if (shouldPersistRows)
            AuditImportError(actorEmail, errorMessage);

        return result;
    }

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

    private static string NormalizeHeader(string header)
    {
        var normalized = header.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(capacity: normalized.Length);

        foreach (var character in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
                builder.Append(character);
        }

        return builder.ToString().Normalize(NormalizationForm.FormC);
    }

    private static string[] SplitCsvLine(string line) => line.Split(',');

    private string GetActorEmail() =>
        _sessionService.GetCurrentUserEmail() ?? SystemActorEmail;

    private void LogAction(string action, string details) =>
        _auditService.Log(GetActorEmail(), action, details);

    private void AuditImportSuccess(string actorEmail, int rowNumber, TeamResponse team) =>
        _auditService.Log(actorEmail, ImportSuccessAction,
            $"Fila {rowNumber}: {team.Name} ({team.ConfederationName}) ranking {team.RankingFifa}");

    private void AuditImportError(string actorEmail, string errorMessage) =>
        _auditService.Log(actorEmail, ImportErrorAction, errorMessage);

    private void AuditImportSummary(string actorEmail, TeamImportResult result) =>
        _auditService.Log(actorEmail, ImportExecutedAction,
            $"Filas: {result.TotalRows}; exitosas: {result.SuccessfulRows}; errores: {result.FailedRows}");
}
