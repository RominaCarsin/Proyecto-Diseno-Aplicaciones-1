using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Team
{
    private const int MinRanking = 300;
    private const int MaxRanking = 2500;
    private const int MaxNameLength = 60;

    private const string NameInvalidErrorMessage = "El nombre debe tener entre 1 y 60 caracteres.";
    private const string RankingInvalidErrorMessage = "El ranking FIFA debe estar entre 300 y 2500.";
    private const string ConfederationRequiredErrorMessage = "La confederación es obligatoria.";
    private const string ConfederationInvalidErrorMessage = "La confederación es inválida.";
    private const string NegativeGoalsErrorMessage = "Los goles no pueden ser negativos.";
    private const string FlagInvalidErrorMessage = "La bandera debe estar en formato base64 válido.";
    
    private static readonly HashSet<string> ValidConfederations = new()
    {
        "AFC", "CAF", "CONCACAF", "CONMEBOL", "OFC", "UEFA"
    };
    
    public int TeamId { get; private set; }
    public string Name { get; private set; } = null!;
    public string ConfederationName { get; private set; } = null!;
    public int RankingFifa { get; private set; }
    public int Points { get; private set; }
    public int GoalDifference { get; private set; }
    public int GoalsInFavor { get; private set; }
    public string GroupLabel { get; private set; } = string.Empty;
    public string FlagBase64 { get; private set; } = string.Empty;

    public Team (){}
    public Team(string name, string confederationName, int rankingFifa, string groupLabel = "", string flagBase64 = "")
    {
        UpdateName(name);
        UpdateConfederationName(confederationName);
        UpdateRankingFifa(rankingFifa);
        UpdateGroupLabel(groupLabel);
        UpdateFlag(flagBase64);
        
        Points = 0;
        GoalDifference = 0;
        GoalsInFavor = 0;
    }

    public void UpdateName(string name)
    {
        var normalized = TextNormalizer. Normalize(name);

        if (string.IsNullOrWhiteSpace(normalized) || normalized.Length > MaxNameLength)
            throw new ArgumentException(NameInvalidErrorMessage);

        Name = normalized;
    }

    public void UpdateConfederationName(string confederationName)
    {
        var normalized = TextNormalizer.Normalize(confederationName);

        if (string.IsNullOrWhiteSpace(normalized))
            throw new ArgumentException(ConfederationRequiredErrorMessage);

        if (!ValidConfederations.Contains(normalized))
            throw new ArgumentException(ConfederationInvalidErrorMessage);

        ConfederationName = normalized;
    }

    public void UpdateRankingFifa(int rankingFifa)
    {
        if (rankingFifa < MinRanking || rankingFifa > MaxRanking)
            throw new ArgumentException(RankingInvalidErrorMessage);

        RankingFifa = rankingFifa;
    }
    
    public void UpdateGroupLabel(string groupLabel)
    {
        GroupLabel = TextNormalizer.Normalize(groupLabel);
    }

    public void RecordWin(int goalsScored, int goalsConceded) =>
        ApplyStatsChange(3, goalsScored, goalsConceded);

    public void RecordDraw(int goalsScored) =>
        ApplyStatsChange(1, goalsScored, goalsScored);

    public void RecordLoss(int goalsScored, int goalsConceded) =>
        ApplyStatsChange(0, goalsScored, goalsConceded);

    public void RevertWin(int goalsScored, int goalsConceded) =>
        RevertStatsChange(3, goalsScored, goalsConceded);

    public void RevertDraw(int goalsScored) =>
        RevertStatsChange(1, goalsScored, goalsScored);

    public void RevertLoss(int goalsScored, int goalsConceded) =>
        RevertStatsChange(0, goalsScored, goalsConceded);

    public void UpdateFlag(string flagBase64)
    {
        FlagBase64 = NormalizeFlag(flagBase64);
    }

    private static string NormalizeFlag(string flagBase64)
    {
        if (string.IsNullOrWhiteSpace(flagBase64))
            return string.Empty;

        var trimmed = flagBase64.Trim();

        if (!IsValidBase64(trimmed))
            throw new ArgumentException(FlagInvalidErrorMessage);

        return trimmed;
    }

    private static bool IsValidBase64(string value)
    {
        var buffer = new Span<byte>(new byte[value.Length]);
        return Convert.TryFromBase64String(value, buffer, out _);
    }

    private void ApplyStatsChange(int points, int goalsScored, int goalsConceded)
    {
        ValidateGoalsAreNotNegative(goalsScored, goalsConceded);

        Points += points;
        GoalsInFavor += goalsScored;
        GoalDifference += goalsScored - goalsConceded;
    }

    private void RevertStatsChange(int points, int goalsScored, int goalsConceded)
    {
        ValidateGoalsAreNotNegative(goalsScored, goalsConceded);

        Points -= points;
        GoalsInFavor -= goalsScored;
        GoalDifference -= goalsScored - goalsConceded;
    }

    private static void ValidateGoalsAreNotNegative(params int[] goals)
    {
        if (goals.Any(goal => goal < 0))
            throw new ArgumentException(NegativeGoalsErrorMessage);
    }
}