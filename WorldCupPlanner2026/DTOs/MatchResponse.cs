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
    
    public string? MatchCode { get; set; }
    
    public string Status { get; set; } = "NotFinished";
    public bool IsFinished { get; set; }
    
    public bool HasResult { get; set; }
    
    public int HomeYellowCards { get; set; }
    
    public int AwayYellowCards { get; set; }
    
    public int HomeRedCards { get; set; }
    
    public int AwayRedCards { get; set; }
}