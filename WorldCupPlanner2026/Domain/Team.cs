namespace Domain;

public class Team
{
    public string Name { get; private set; }
    public string Confederation { get; private set; }
    public int RankingFifa { get; private set; }
    public int Points { get; private set; }
    public int GoalDiference { get; private set; }
    public int GoalsInFavor { get; private set; }

    public Team(string name, string confederation, int rankingFifa, int points, int goalDiference, int goalsInFavor)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.");
        
        if (string.IsNullOrWhiteSpace(confederation))
            throw new ArgumentException("Confederation cannot be empty.");
        
        Name = name;
        Confederation = confederation;
        RankingFifa = rankingFifa;
        Points = points;
        GoalDiference = goalDiference;
        GoalsInFavor = goalsInFavor;
    }
}