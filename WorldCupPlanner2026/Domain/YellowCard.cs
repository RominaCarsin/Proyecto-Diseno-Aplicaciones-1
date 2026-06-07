namespace Domain;

public class YellowCard : MatchIncidence
{
    private YellowCard() { }

    public YellowCard(Team? team, int minute) : base(team, minute) { }
}