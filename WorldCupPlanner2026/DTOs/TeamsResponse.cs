using Domain;

namespace DTOs;

public class TeamResponse
{    
    public string Name { get; set; } = "";
    public string ConfederationName { get; set; } = "";
    public int RankingFifa { get; set; } = 0;

    public static TeamResponse FromEntity(Team team)
    {
        return new TeamResponse()
        {
            Name = team.Name,
            ConfederationName = team.ConfederationName,
            RankingFifa = team.RankingFifa
        };
    }
}