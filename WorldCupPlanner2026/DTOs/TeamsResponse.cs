namespace DTOs;

public class TeamResponse
{
    public string Name { get; set; } = string.Empty;
    public string ConfederationName { get; set; } = string.Empty;
    public int RankingFifa { get; set; }
    public int Points { get; set; }
    public int GoalDifference { get; set; }
    public int GoalsInFavor { get; set; }
}