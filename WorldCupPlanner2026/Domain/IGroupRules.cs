namespace Domain;

public interface IGroupRules
{
    void EnsureCanAdd(IReadOnlyList<Team> currentTeams, Team candidate);
    bool CanAdd(IReadOnlyList<Team> currentTeams, Team candidate);
}