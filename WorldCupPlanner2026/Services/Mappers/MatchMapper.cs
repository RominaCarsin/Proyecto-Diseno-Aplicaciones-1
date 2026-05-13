using Domain;
using DTOs;
using IRepository;

namespace Services.Mappers;

public static class MatchMapper
{
    public static Match ToEntity(MatchRequest matchRequest, IRepository<Group>? groupRepository = null) =>
        new(
            date: matchRequest.Date,
            stadium: StadiumMapper.ToEntity(matchRequest.Stadium),
            homeTeam: TeamMapper.ToEntity(matchRequest.HomeTeam),
            awayTeam: TeamMapper.ToEntity(matchRequest.AwayTeam),
            group: matchRequest.Group.HasValue ? char.ToUpperInvariant(matchRequest.Group.Value) : null,
            phase: ResolvePhase(matchRequest.Phase)
        );

    public static MatchResponse ToResponse(Match match) =>
        new()
        {
            Id = match.Id,
            Date = match.Date,
            Stadium = StadiumMapper.FromEntity(match.Stadium),
            HomeTeam = TeamMapper.FromEntity(match.HomeTeam),
            AwayTeam = TeamMapper.FromEntity(match.AwayTeam),
            Group = match.Group,
            Phase = match.Phase.ToString(),
            HomeGoals = match.HomeGoals,
            AwayGoals = match.AwayGoals,
            Winner = match.Winner is null ? null : TeamMapper.FromEntity(match.Winner),
            IsLocked = match.IsLocked,
            Status = match.Status.ToString(),
            HasResult = match.HasResult
        };
    

    private static TournamentPhase ResolvePhase(string phase)
    {
        if (string.IsNullOrWhiteSpace(phase))
            return TournamentPhase.GroupStage;

        if (Enum.TryParse<TournamentPhase>(phase, ignoreCase: true, out var result))
            return result;

        throw new ArgumentException($"Invalid tournament phase: '{phase}'.");
    }
}
