namespace Domain;

public interface IKnockoutMatchFactory
{
    Match Create(
        Team homeTeam,
        Team awayTeam,
        TournamentPhase phase,
        string matchCode);
}
