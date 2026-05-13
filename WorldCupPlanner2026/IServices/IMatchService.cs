using DTOs;

namespace IServices;

public interface IMatchService
{
    void AddMatch(MatchRequest matchRequest);
    IEnumerable<MatchResponse> GetMatches();
    MatchResponse GetMatchById(int matchId);
    void SetMatchResult(int matchId, int homeGoals, int awayGoals);
    void UpdateMatchDate(int matchId, DateTime newDate);
    void UpdateMatchStadium(int matchId, StadiumRequest newStadium);
    void FinishMatch(int matchId);
    IEnumerable<StadiumRequest> GetAvailableStadiumsForMatch(int matchId, IEnumerable<StadiumRequest> candidateStadiums);
}