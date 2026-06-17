namespace Domain;

public class SimulationResult
{
    public int HomeGoals { get; }
    public int AwayGoals { get; }
    public int HomeYellowCards { get; }
    public int AwayYellowCards { get; }
    public int HomeRedCards { get; }
    public int AwayRedCards { get; }

    public SimulationResult(int homeGoals, int awayGoals,
        int homeYellowCards = 0, int awayYellowCards = 0,
        int homeRedCards = 0, int awayRedCards = 0)
    {
        HomeGoals = homeGoals;
        AwayGoals = awayGoals;
        HomeYellowCards = homeYellowCards;
        AwayYellowCards = awayYellowCards;
        HomeRedCards = homeRedCards;
        AwayRedCards = awayRedCards;
    }

    public override bool Equals(object? obj) =>
        obj is SimulationResult other
        && HomeGoals == other.HomeGoals
        && AwayGoals == other.AwayGoals
        && HomeYellowCards == other.HomeYellowCards
        && AwayYellowCards == other.AwayYellowCards
        && HomeRedCards == other.HomeRedCards
        && AwayRedCards == other.AwayRedCards;

    public override int GetHashCode() =>
        HashCode.Combine(HomeGoals, AwayGoals, HomeYellowCards, AwayYellowCards, HomeRedCards, AwayRedCards);
}
