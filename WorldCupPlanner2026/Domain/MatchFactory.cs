namespace Domain;

public class MatchFactory : IMatchFactory
{
    public Match CreateGroupMatch(
        DateTime date,
        Stadium stadium,
        Team home,
        Team away,
        char groupLabel)
    {
        return new Match(
            date,
            stadium,
            home,
            away,
            group: groupLabel,
            phase: TournamentPhase.GroupStage
        );
    }
}
