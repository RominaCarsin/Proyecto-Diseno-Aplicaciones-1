namespace Domain;

public class RedCard : MatchIncidence
{
    [ExcludeFromCodeCoverage]
    protected RedCard() { }

    public RedCard(Team? team, int minute) : base(team, minute) { }
}