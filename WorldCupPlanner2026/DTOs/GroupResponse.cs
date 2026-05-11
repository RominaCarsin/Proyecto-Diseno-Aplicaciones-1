namespace DTOs;

public class GroupResponse
{
    public char Label { get; set; }
    public List<TeamResponse> Teams { get; set; } = new();
    public List<MatchResponse> Matches { get; set; } = new();
}