namespace Domain;

public class Team
{
    public string Name { get; private set; }
    public Confederation Confederation { get; private set; }
    public int RankingFifa { get; private set; }
    public int Points { get; private set; }
    public int GoalDiference { get; private set; }
    public int GoalsInFavor { get; private set; }

    public Team(string name, Confederation confederation, int rankingFifa, int points, int goalDiference, int goalsInFavor)
    {
        ValidateName(name);
        ValidateConfederation(confederation);
        ValidateRankingFifa(rankingFifa);
        
        Name = name;
        Confederation = confederation;
        RankingFifa = rankingFifa;
        Points = points;
        GoalDiference = goalDiference;
        GoalsInFavor = goalsInFavor;
    }

    private void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.");
        if(name.Length > 60)
            throw new ArgumentException("Name cannot be longer than 60 characters.");
    }

    private void ValidateConfederation(Confederation confederation)
    {
        if (confederation is null)
            throw new ArgumentException("Confederation cannot be empty.");
    }

    private void ValidateRankingFifa(int rankingFifa)
    {
        if (rankingFifa < 300 ||  rankingFifa > 2500)
            throw new ArgumentException("Ranking fifa must be between 300 and 2500.");
    }
}