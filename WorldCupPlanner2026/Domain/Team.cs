namespace Domain;

public class Team
{
    private const int MinRanking = 300;
    private const int MaxRanking = 2500;
    private const int MaxNameLength = 60;

    private const string NameInvalidErrorMessage = "Name must be between 1 and 60 characters.";
    private const string RankingInvalidErrorMessage = "Ranking FIFA must be between 300 and 2500.";
    private const string ConfederationRequiredErrorMessage = "Confederation is required.";
    private const string ConfederationInvalidErrorMessage = "Invalid confederation.";
    private const string NegativeGoalsErrorMessage = "Goals cannot be negative.";

    private static readonly HashSet<string> ValidConfederations = new()
    {
        "AFC", "CAF", "CONCACAF", "CONMEBOL", "OFC", "UEFA"
    };

    public string Name { get; private set; }
    public string ConfederationName { get; private set; }
    public int RankingFifa { get; private set; }
    public int Points { get; private set; }
    public int GoalDifference { get; private set; }
    public int GoalsInFavor { get; private set; }

    public Team(string name, string confederationName, int rankingFifa)
    {
        UpdateName(name);
        UpdateConfederationName(confederationName);
        UpdateRankingFifa(rankingFifa);
    }

    public void UpdateName(string name)
    {
        var normalized = Normalize(name);

        if (string.IsNullOrWhiteSpace(normalized) || normalized.Length > MaxNameLength)
            throw new ArgumentException(NameInvalidErrorMessage);

        Name = normalized;
    }

    public void UpdateConfederationName(string confederationName)
    {
        var normalized = Normalize(confederationName);

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

    public void RecordWin(int goalsScored, int goalsConceded) =>
        ApplyStatsChange(points: 3, goalsScored, goalsConceded);

    public void RecordDraw(int goalsScored) =>
        ApplyStatsChange(points: 1, goalsScored, goalsConceded: goalsScored);

    public void RecordLoss(int goalsScored, int goalsConceded) =>
        ApplyStatsChange(points: 0, goalsScored, goalsConceded);

    public void RevertWin(int goalsScored, int goalsConceded) =>
        RevertStatsChange(points: 3, goalsScored, goalsConceded);

    public void RevertDraw(int goalsScored) =>
        RevertStatsChange(points: 1, goalsScored, goalsConceded: goalsScored);

    public void RevertLoss(int goalsScored, int goalsConceded) =>
        RevertStatsChange(points: 0, goalsScored, goalsConceded);
    
    public static string Normalize(string value) =>
            value?.Trim().ToUpper() ?? string.Empty;
    
    public static string Normalized(string value) => Normalize(value);
        
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