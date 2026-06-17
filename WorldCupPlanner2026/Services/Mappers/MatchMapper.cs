using Domain;
using DTOs;
using IRepository;

namespace Services.Mappers;

public static class MatchMapper
{
    public static Match ToEntity(MatchRequest matchRequest) =>
        new(
            date: matchRequest.Date,
            stadium: StadiumMapper.ToEntity(matchRequest.Stadium),
            homeTeam: TeamMapper.ToEntity(matchRequest.HomeTeam),
            awayTeam: TeamMapper.ToEntity(matchRequest.AwayTeam),
            group: matchRequest.Group.HasValue ? char.ToUpperInvariant(matchRequest.Group.Value) : null,
            phase: ResolvePhase(matchRequest.Phase)
        );

    public static MatchResponse ToResponse(Match match, bool isLocked = false) =>
        new()
        {
            Id = match.MatchId,
            Date = match.Date,
            Stadium = StadiumMapper.FromEntity(match.Stadium),
            HomeTeam = TeamMapper.FromEntity(match.HomeTeam),
            AwayTeam = TeamMapper.FromEntity(match.AwayTeam),
            Group = match.Group,
            Phase = match.Phase.ToString(),
            HomeGoals = match.HomeGoals,
            AwayGoals = match.AwayGoals,
            Winner = match.Winner is null ? null : TeamMapper.FromEntity(match.Winner),
            MatchCode = match.MatchCode,
            Status = match.Status.ToString(),
            HasResult = match.HasResult,
            IsFinished = match.IsFinished,
            IsLocked = isLocked,
            HomeYellowCards = match.YellowCardsOf(match.HomeTeam),
            AwayYellowCards = match.YellowCardsOf(match.AwayTeam),
            HomeRedCards = match.RedCardsOf(match.HomeTeam),
            AwayRedCards = match.RedCardsOf(match.AwayTeam)
        };
    

    private static TournamentPhase ResolvePhase(string phase)
    {
        if (string.IsNullOrWhiteSpace(phase))
            return TournamentPhase.GroupStage;

        if (Enum.TryParse<TournamentPhase>(phase, ignoreCase: true, out var result))
            return result;

        throw new ArgumentException($"Fase del torneo inválida: '{phase}'.");
    }
}
