using DTOs;

namespace IServices;

public interface IMatchService
{
    void AddMatch(MatchRequest request);

    IEnumerable<MatchResponse> GetMatches();

    MatchResponse GetMatchById(int id);

    void SetMatchResult(int id, int homeGoals, int awayGoals);

    void UpdateMatchDate(int id, DateTime newDate);

    void UpdateMatchStadium(int id, StadiumRequest newStadium);

    void LockMatchEdition(int id);
    
}