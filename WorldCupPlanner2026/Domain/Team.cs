namespace Domain;

public class Team
{
    public string Name { get; private set; }
    public string ConfederationName { get; private set; }
    public int RankingFifa { get; private set; }
    public int Points { get; private set; }
    public int GoalDiference { get; private set; }
    public int GoalsInFavor { get; private set; }

    public Team(string name, string confederationName, int rankingFifa, int points, int goalDiference, int goalsInFavor)
    {
        ValidateName(name);
        ValidateConfederation(confederationName);
        ValidateRankingFifa(rankingFifa);
        
        Name = name;
        ConfederationName = confederationName;
        RankingFifa = rankingFifa;
        Points = points;
        GoalDiference = goalDiference;
        GoalsInFavor = goalsInFavor;
    }
    
    public void UpdateName(string name)
    {
        ValidateName(name);
        Name = name;
    }

    public void UpdateRankingFifa(int rankingFifa)
    {
        ValidateRankingFifa(rankingFifa);
        RankingFifa = rankingFifa;
    }

    private void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.");
        if(name.Length > 60)
            throw new ArgumentException("Name cannot be longer than 60 characters.");
    }

    private void ValidateConfederation(string confederationName)
    {
        if (confederationName is null)
            throw new ArgumentException("Confederation cannot be empty.");
    }

    private void ValidateRankingFifa(int rankingFifa)
    {
        if (rankingFifa < 300 ||  rankingFifa > 2500)
            throw new ArgumentException("Ranking fifa must be between 300 and 2500.");
    }
}