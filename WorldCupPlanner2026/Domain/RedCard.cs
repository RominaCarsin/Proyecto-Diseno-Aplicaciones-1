namespace Domain;

public class RedCard : MatchIncidence
{
    private RedCard() { }

    public RedCard(Team? team, int minute) : base(team, minute) { }
}