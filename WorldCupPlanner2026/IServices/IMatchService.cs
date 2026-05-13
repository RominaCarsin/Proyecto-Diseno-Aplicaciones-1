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
    void LockMatchEdition(int matchId);
    void LockMatchesByPhase(string phase);
    void FinishMatch(int matchId);
    
}