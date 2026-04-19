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
    
    private static int _nextId = 1;
    
    public int Id { get; private set; }

    public Match(DateTime date, Stadium stadium, Team homeTeam, Team awayTeam, string group, TournamentPhase phase)
    {
        if (stadium == null) throw new DomainException("Stadium cannot be null");
        if (homeTeam == null)
        {
            throw new DomainException("Home team cannot be null.");

        }

        if (awayTeam == null)
        {
            throw new DomainException("Away team cannot be null.");
        }

        if (homeTeam.Name == awayTeam.Name)
        {
            throw new DomainException("Home team and away team must be different.");
        }

        if (string.IsNullOrWhiteSpace(group) || group.Length != 1 || group[0] < 'A' || group[0] > 'L'){
            throw new DomainException("Group must be a single uppercase letter between A and L.");
        }
        
        Id = _nextId++;

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
    
    public void SetResult(int homeGoals, int awayGoals)
    {
        if (homeGoals < 0)
            throw new DomainException("Home goals cannot be negative.");
        if (awayGoals < 0)
            throw new DomainException("Away goals cannot be negative.");

        HomeGoals = homeGoals;
        AwayGoals = awayGoals;
        Winner = homeGoals > awayGoals ? HomeTeam : awayGoals > homeGoals ? AwayTeam : null;
    }
    
    public void UpdateDate(DateTime newDate)
    {
        Date = newDate;
    }
}