using Domain;
using DTOs;

namespace Services.Mappers;

public class MatchMapper
{
    public static Match ToEntity(MatchRequest request)
    {
        return new Match(
            date: request.Date,
            stadium: StadiumMapper.ToEntity(request.Stadium),
            homeTeam: TeamMapper.ToEntity(request.HomeTeam),
            awayTeam: TeamMapper.ToEntity(request.AwayTeam)
        );
    }

    public static MatchResponse ToResponse(Match match)
    {
        return new MatchResponse
        {
            Id = match.Id,
            Date = match.Date,
            Stadium = StadiumMapper.FromEntity(match.Stadium),
            HomeTeam = TeamMapper.FromEntity(match.HomeTeam),
            AwayTeam = TeamMapper.FromEntity(match.AwayTeam),
            HomeGoals = match.HomeGoals,
            AwayGoals = match.AwayGoals,
            Winner = match.Winner is null ? null : TeamMapper.FromEntity(match.Winner),
            IsLocked = match.IsLocked
        };
    }
}