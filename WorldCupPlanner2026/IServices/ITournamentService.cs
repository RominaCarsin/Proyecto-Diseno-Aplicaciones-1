using DTOs;

namespace IServices;

public interface ITournamentService
{
    void GenerateFixture(int seed, DateTime startDate);
    bool CanGenerateFixture();
    void GenerateFixture(int seed);
    int GetStadiumCount();

    IEnumerable<GroupResponse> GetGroups();
    GroupResponse GetGroupByLabel(char label);
    IEnumerable<TeamResponse> GetGroupStandings(char groupLabel);
    IEnumerable<TeamResponse> GetQualifiedTeams();

    IEnumerable<MatchResponse> GetMatches();
    MatchResponse GetMatchById(int matchId);
    void SetGroupMatchResult(char groupLabel, int matchId, int homeGoals, int awayGoals);
    void FinishGroupMatch(char groupLabel, int matchId);

    void SimulateGroupStage(int seed);
    void SimulateGroupStage(int seed, SimulationEngineTypeDto simulationEngineType);

    void AddMatch(MatchRequest matchRequest);
    void SetMatchResult(int matchId, int homeGoals, int awayGoals);
    void UpdateMatchDate(int matchId, DateTime newDate);
    void UpdateMatchStadium(int matchId, StadiumRequest newStadium);
    void FinishMatch(int matchId);
    void FinishKnockoutMatch(string matchCode);
    void FinishAllKnockoutMatches();

    IEnumerable<StadiumRequest> GetAvailableStadiumsForMatch(
        int matchId,
        IEnumerable<StadiumRequest> candidateStadiums);

    void InitializeKnockoutStage(int simulationSeed, int semillaCrucesFase = 0);
    void SetKnockoutMatchResult(string matchCode, int homeGoals, int awayGoals);
    IEnumerable<MatchResponse> GetKnockoutMatches();

    void InitializeRoundOf16();
    void InitializeQuarterFinals();
    void InitializeSemiFinals();
    void InitializeThirdPlace();
    void InitializeFinal();

    void SimulateKnockoutStage(int seed);
    //void SimulateKnockoutStage(int seed, SimulationEngineTypeDto simulationEngineType);

    void FinishAllMatches();

    void RegisterMatchIncidence(int matchId, string incidenceType, bool isHomeTeam, int minute);
}