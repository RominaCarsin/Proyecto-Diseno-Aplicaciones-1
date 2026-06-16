namespace Domain;

public interface IGroupRules
{
    void EnsureCanAdd(IReadOnlyList<Team> currentTeams, Team candidate);
}