using Domain;
using DTOs;

namespace Services.Mappers;

public static class TeamMapper
{
    public static Team ToEntity(TeamRequest request)
    {
        var team = new Team(request.Name, request.ConfederationName, request.RankingFifa);
        team.UpdateFlag(request.FlagBase64);   
        return team;
    }
    
    public static TeamResponse FromEntity(Team team)
    {
        return new TeamResponse
        {
            Name = team.Name,
            ConfederationName = team.ConfederationName,
            RankingFifa = team.RankingFifa,
            Points = team.Points,
            GoalDifference = team.GoalDifference,
            GoalsInFavor = team.GoalsInFavor,
            GroupLabel = team.GroupLabel,
            FlagBase64 = team.FlagBase64
        };
    }

    public static Team ToEntity(TeamResponse response)
    {
        var team = new Team(response.Name, response.ConfederationName, response.RankingFifa);
        team.UpdateGroupLabel(response.GroupLabel);
        team.UpdateFlag(response.FlagBase64);
        return team;
    }
    
    
}