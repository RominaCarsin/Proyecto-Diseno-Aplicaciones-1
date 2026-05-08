namespace DTOs;

public class GroupResponse
{
    public char Label { get; set; }
    public List<TeamResponse> Teams { get; set; } = new();
}