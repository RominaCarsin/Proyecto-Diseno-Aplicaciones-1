namespace Domain;

public class Match
{
    public DateTime Date { get; private set; }
    public Stadium Stadium { get; private set; }
    public Team HomeTeam { get; private set; }
    public Team AwayTeam { get; private set; }
    public Group Group { get; private set; }
    public TournamentPhase Phase { get; private set; }

    public Match(DateTime date, Stadium stadium, Team homeTeam, Team awayTeam, Group group, TournamentPhase phase)
    {
        
        if (stadium == null)
        {
            throw new Exception("Match stadium cannot be null.");
        }
        if (homeTeam == null)
        {
            throw new Exception("Match home team cannot be null.");
        }
        
        if (awayTeam == null)
        {
            throw new Exception("Match away team cannot be null.");
        }
        
        if (homeTeam == awayTeam)
        {
            throw new Exception("Home team and away team must be different.");
        }
        
        if (group == null)
        {
            throw new Exception("Match group cannot be null.");
        }
        
        
        Date = date;
        Stadium = stadium;
        HomeTeam = homeTeam;
        AwayTeam = awayTeam;
        Group = group;
        Phase = phase;
    }
}