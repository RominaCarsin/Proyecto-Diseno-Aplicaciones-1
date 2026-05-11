namespace DTOs;

public class StadiumRequest
{
    public string Name { get; init; } = "";
    public string City { get; init; } = "";
    public string? Description { get; init; }
    public int Capacity { get; init; }
}