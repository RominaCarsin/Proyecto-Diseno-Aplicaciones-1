namespace DTOs;

public class MatchRequest
{
    public DateTime Date { get; init; }
    public StadiumRequest Stadium { get; init; } = new();
    public TeamRequest HomeTeam { get; init; } = new();
    public TeamRequest AwayTeam { get; init; } = new();
}