namespace Domain;

public class ConfederationBalancedStrategy : IGroupAssignmentStrategy
{
    public int FindTargetGroupIndex(Team team, IReadOnlyList<Group> groups, int maxTeamsPerGroup)
    {
        return groups
            .Select((group, index) => (group, index))
            .Where(x => x.group.Teams.Count < maxTeamsPerGroup)
            .OrderBy(x => x.group.Teams.Count(t =>
                t.ConfederationName.Equals(team.ConfederationName, StringComparison.OrdinalIgnoreCase)))
            .ThenBy(x => x.group.Teams.Count)
            .Select(x => x.index)
            .FirstOrDefault(0);
    }
}
