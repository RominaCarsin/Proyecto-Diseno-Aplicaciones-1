using Domain;
using DTOs;

namespace Services.Mappers;

public static class MatchMapper
{
    public static Match ToEntity(MatchRequest matchRequest) =>
        new Match(
            date: matchRequest.Date,
            stadium: StadiumMapper.ToEntity(matchRequest.Stadium),
            homeTeam: TeamMapper.ToEntity(matchRequest.HomeTeam),
            awayTeam: TeamMapper.ToEntity(matchRequest.AwayTeam)
        );

    public static MatchResponse ToResponse(Match match) =>
        new MatchResponse
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