namespace Domain;

public class GroupDrawRules
{
    private readonly IGroupAssignmentStrategy _strategy;
    private readonly TournamentRules _rules;

    public GroupDrawRules(IGroupAssignmentStrategy strategy, TournamentRules rules)
    {
        _strategy = strategy;
        _rules = rules;
    }

    public void AssignTeams(List<Group> groups, List<Pot> pots)
    {
        foreach (var pot in pots)
        {
            foreach (var team in pot.Teams)
            {
                int preferredGroupIndex = _strategy.FindTargetGroupIndex(team, groups, _rules.MaxTeamsPerGroup);
                AssignTeamToGroupStartingFrom(team, preferredGroupIndex, groups);
            }
        }
    }

    private void AssignTeamToGroupStartingFrom(Team team, int startGroupIndex, List<Group> groups)
    {
        for (int offset = 0; offset < groups.Count; offset++)
        {
            var group = groups[(startGroupIndex + offset) % groups.Count];

            if (group.Teams.Count < _rules.MaxTeamsPerGroup)
            {
                try
                {
                    group.AddTeam(team);
                    return;
                }
                catch (ArgumentException)
                {
                }
            }
        }

        throw new InvalidOperationException(
            $"No se pudo asignar el equipo {team.Name} a ningún grupo.");
    }
}
