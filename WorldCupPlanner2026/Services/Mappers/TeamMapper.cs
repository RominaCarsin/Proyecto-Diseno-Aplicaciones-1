using Domain;
using DTOs;

namespace Services.Mappers;

public static class TeamMapper
{
    public static Team ToEntity(TeamRequest request)
    {
        return new Team(request.Name, request.ConfederationName, request.RankingFifa);
    }
    
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