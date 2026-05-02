using Domain;

namespace IServices;

public interface IMatchService
{
    void AddMatch(Match match);

    IEnumerable<Match> GetMatches();

    Match GetMatchById(int id);

    void SetMatchResult(int id, int homeGoals, int awayGoals);

    void UpdateMatchDate(int id, DateTime newDate);

    void UpdateMatchStadium(int id, Stadium newStadium);

    void LockMatchEdition(int id);

    void LockMatchesByPhase(TournamentPhase phase);
}