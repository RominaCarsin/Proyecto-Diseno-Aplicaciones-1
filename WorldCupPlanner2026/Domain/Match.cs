namespace Domain;

using Domain.Exceptions;

public class Match
{
    public DateTime Date { get; private set; }
    public Stadium Stadium { get; private set; }
    public Team HomeTeam { get; private set; }
    public Team AwayTeam { get; private set; }
    public string Group { get; private set; }
    public TournamentPhase Phase { get; private set; }
    public int? HomeGoals { get; private set; }
    public int? AwayGoals { get; private set; }
    public Team? Winner { get; private set; }

    public Match(DateTime date, Stadium stadium, Team homeTeam, Team awayTeam, string group, TournamentPhase phase)
    {
        if (stadium == null) throw new DomainException("Stadium cannot be null");
        Date = date;
        Stadium = stadium;
        HomeTeam = homeTeam;
        AwayTeam = awayTeam;
        Group = group;
        Phase = phase;
    }

    public void LoadResult(int homeGoals, int awayGoals)
    {
        HomeGoals = homeGoals;
        AwayGoals = awayGoals;
    }
}