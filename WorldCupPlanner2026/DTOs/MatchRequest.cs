namespace DTOs;

public class MatchRequest
{
    public DateTime Date { get; init; }
    
    public StadiumRequest Stadium { get; set; } = new();

    public TeamRequest HomeTeam { get; set; } = new();

    public TeamRequest AwayTeam { get; set; } = new();
    
}