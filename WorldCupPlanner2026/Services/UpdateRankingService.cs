using Domain;
using IServices;

namespace Services;

public class UpdateRankingService : IUpdateRankingService
{
    private readonly IAuditService _auditService;
    private const string MatchNotFinishedErrorMessage = "El ranking solo se recalcula para partidos finalizados.";
    private const string SystemActor = "SYSTEM";
    private const string RankingAction = "RANKING_UPDATE";
    
    public UpdateRankingService(IAuditService auditService)
    {
        _auditService = auditService;
    }

    public void UpdateAfterMatch(Match match)
    {
        if (match.Status != MatchStatus.Finished)
            throw new InvalidOperationException(MatchNotFinishedErrorMessage);

        if (match.RankingApplied)
            return;

        var homeTeam = match.HomeTeam;
        var awayTeam = match.AwayTeam;

        var homeRankingBefore = homeTeam.RankingFifa;
        var awayRankingBefore = awayTeam.RankingFifa;

        var homeResult = ResultFor(homeTeam, match);
        var awayResult = ResultFor(awayTeam, match);

        var homeDelta = RankingCalculator.CalculateDelta(homeRankingBefore, awayRankingBefore, homeResult, match.Phase);
        var awayDelta = RankingCalculator.CalculateDelta(awayRankingBefore, homeRankingBefore, awayResult, match.Phase);

        homeTeam.ApplyRankingChange(homeDelta);
        awayTeam.ApplyRankingChange(awayDelta);

        AuditRankingChange(homeTeam, homeRankingBefore);
        AuditRankingChange(awayTeam, awayRankingBefore);

        match.MarkRankingApplied();
    }

    private static double ResultFor(Team team, Match match)
    {
        if (match.Winner is null) return 0.5;
        return ReferenceEquals(match.Winner, team) ? 1.0 : 0.0;
    }
    
    private void AuditRankingChange(Team team, int previousRanking)
    {
        var details = $"{team.Name}: {previousRanking} -> {team.RankingFifa}";
        _auditService.Log(SystemActor, RankingAction, details);
    }
}