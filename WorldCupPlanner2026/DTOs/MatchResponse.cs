namespace DTOs;

public class MatchResponse
{
    public int Id { get; init; }
    public DateTime Date { get; init; }
    public StadiumResponse Stadium { get; init; } = new();
    public TeamResponse HomeTeam { get; init; } = new();
    public TeamResponse AwayTeam { get; init; } = new();
    public char? Group { get; init; }
    public string? Phase { get; init; }
    public int? HomeGoals { get; init; }
    public int? AwayGoals { get; init; }
    public TeamResponse? Winner { get; init; }
    public bool IsLocked { get; init; }
}