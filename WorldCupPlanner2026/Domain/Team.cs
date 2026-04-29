namespace Domain;

public class Team
{
    private const int MinRanking = 300;
    private const int MaxRanking = 2500;
    private const int MaxNameLength = 60;

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
        Name              = ValidateName(NormalizeName(name));
        ConfederationName = ValidateConfederation(NormalizeConfederation(confederationName));
        RankingFifa       = ValidateRankingFifa(rankingFifa);
        Points            = 0;
        GoalDifference    = 0;
        GoalsInFavor      = 0;
    }

    public void UpdateName(string name)
    {
        Name = ValidateName(NormalizeName(name));
    }

    public void UpdateRankingFifa(int rankingFifa)
    {
        RankingFifa = ValidateRankingFifa(rankingFifa);
    }

    public void UpdateConfederationName(string confederationName)
    {
        ConfederationName = ValidateConfederation(NormalizeConfederation(confederationName));
    }

    private static string NormalizeName(string name) => name?.Trim() ?? string.Empty;

    private static string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length > MaxNameLength)
            throw new ArgumentException($"Name must be between 1 and {MaxNameLength} characters.");
        return name;
    }

    private static string NormalizeConfederation(string confederationName) =>
        confederationName?.Trim().ToUpper() ?? string.Empty;

    private static string ValidateConfederation(string confederationName)
    {
        if (string.IsNullOrWhiteSpace(confederationName))
            throw new ArgumentException("Confederation is required.");
        if (!ValidConfederations.Contains(confederationName))
            throw new ArgumentException("Invalid confederation.");
        return confederationName;
    }

    private static int ValidateRankingFifa(int rankingFifa)
    {
        if (rankingFifa < MinRanking || rankingFifa > MaxRanking)
            throw new ArgumentException($"Ranking fifa must be between {MinRanking} and {MaxRanking}.");
        return rankingFifa;
    }
}