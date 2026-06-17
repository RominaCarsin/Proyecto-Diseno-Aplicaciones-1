namespace Domain;

public class YellowCard : MatchIncidence
{
    [ExcludeFromCodeCoverage]
    protected YellowCard() { }

    public YellowCard(Team? team, int minute) : base(team, minute) { }
}