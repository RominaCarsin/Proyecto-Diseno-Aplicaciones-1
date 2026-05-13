namespace DTOs;

public class StadiumRequest
{
    public string Name { get; set; } = "";
    public string City { get; set; } = "";
    public string? Description { get; set; }
    public int Capacity { get; set; }
}
