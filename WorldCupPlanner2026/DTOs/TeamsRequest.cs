using Domain;

namespace DTOs;

public class TeamRequest
{
    public string Name { get; set; } = "";
    public string ConfederationName { get; set; } = "";
    public int RankingFifa { get; set; } = 0;

    public static Team ToEntity(TeamRequest dto)
    {
        return new Team(dto.Name, dto.ConfederationName, dto.RankingFifa, 0, 0, 0);
    }
}