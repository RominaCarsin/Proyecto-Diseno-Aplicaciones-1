namespace Domain;

public class GroupOrderPointsGoalDiffCalculator : IGroupPositionsCalculator
{
    public IEnumerable<Team> Order(IReadOnlyList<Team> teams)
    {
        return teams
            .OrderByDescending(t => t.Points)
            .ThenByDescending(t => t.GoalDifference)
            .ThenByDescending(t => t.GoalsInFavor)
            .ToList();
    }

}