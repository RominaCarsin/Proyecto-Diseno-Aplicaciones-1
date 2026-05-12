namespace DTOs;

public class MatchResponse
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public StadiumResponse Stadium { get; set; } = new();

    public TeamResponse HomeTeam { get; set; } = new();

    public TeamResponse AwayTeam { get; set; } = new();

    public char? Group { get; set; }

    public string Phase { get; set; } = "";

    public int? HomeGoals { get; set; }

    public int? AwayGoals { get; set; }

    public TeamResponse? Winner { get; set; }

    public bool IsLocked { get; set; }
}