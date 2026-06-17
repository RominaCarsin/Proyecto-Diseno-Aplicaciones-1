namespace Domain;

public interface IGroupAssignmentStrategy
{
    int FindTargetGroupIndex(Team team, IReadOnlyList<Group> groups, int maxTeamsPerGroup);
}
