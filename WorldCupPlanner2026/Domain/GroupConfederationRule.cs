namespace Domain;

public class GroupConfederationRule : IGroupRules
{
    private const string UefaConfederationName = "UEFA";
    private const int MaxUefaTeamsPerGroup = 2;
    private const int MaxOtherConfederationTeamsPerGroup = 1;

    private const string TooManyTeamsFromSameConfederationErrorMessage =
        "El grupo no puede tener más de 2 equipos de la misma confederación.";

    public bool CanAdd(IReadOnlyList<Team> currentTeams, Team candidate)
    {
        int teamsFromSameConfederation = currentTeams.Count(t =>
            t.ConfederationName == candidate.ConfederationName);

        bool isUefa = candidate.ConfederationName == UefaConfederationName;

        int limit = isUefa
            ? MaxUefaTeamsPerGroup
            : MaxOtherConfederationTeamsPerGroup;

        return teamsFromSameConfederation < limit;
    }

    public void EnsureCanAdd(IReadOnlyList<Team> currentTeams, Team candidate)
    {
        if (!CanAdd(currentTeams, candidate))
            throw new ArgumentException(TooManyTeamsFromSameConfederationErrorMessage);
    }
}