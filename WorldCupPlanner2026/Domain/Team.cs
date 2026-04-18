using Domain;

namespace Domain;

public class Team
{
    public string Name { get; private set; }
    public Confederation Confederation { get; private set; }
    public int FifaRanking { get; private set; }

    public Team(string name, Confederation confederation, int fifaRanking)
    {
        Name = name;
        Confederation = confederation;
        FifaRanking = fifaRanking;
    }
}