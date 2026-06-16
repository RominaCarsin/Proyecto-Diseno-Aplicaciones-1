namespace Domain;

public class TournamentRules
{
    public int RequiredTeamCount { get; init; } = 48;
    public int MinStadiumCount { get; init; } = 4;
    public int GroupCount { get; init; } = 12;
    public int MaxTeamsPerGroup { get; init; } = 4;
    public int RoundsPerGroup { get; init; } = 3;
    public int MatchesPerRound { get; init; } = 2;
    public int FirstMatchHour { get; init; } = 14;
    public int HoursBetweenMatches { get; init; } = 4;
    public int DaysBetweenRounds { get; init; } = 3;
    public int DaysBetweenGroupStarts { get; init; } = 3;
}
