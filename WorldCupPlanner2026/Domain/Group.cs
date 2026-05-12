namespace Domain;

public class Group
{
    private const char MinLabel = 'A';
    private const char MaxLabel = 'L';
    private const int MaxTeams = 4;
    private const int MaxUEFATeamsPerGroup = 2;
    private const int MaxOtherConfederationTeamsPerGroup = 1;
    
    private const string TooManyTeamsFromSameConfederationErrorMessage = "Group cannot have more than 2 teams from the same confederation.";
    private const string LabelOutOfRangeErrorMessage = "Label debe ser una letra mayúscula entre 'A' y 'L'.";
    private const string NullTeamErrorMessage = "Team cannot be null.";
    private const string FullGroupErrorMessage = "Group cannot have more than 4 teams.";
    private const string TeamNotFoundErrorMessage = "Team does not exist in group.";
    private const string InvalidPositionErrorMessage = "Position is invalid.";
    private const string NullMatchErrorMessage = "Match cannot be null.";

    private readonly List<Team> _teams = new();
    private readonly List<Match> _matches = new();

    public char Label { get; private set; }
    public IReadOnlyList<Team> Teams => _teams;
    public IReadOnlyList<Match> Matches => _matches;

    public Group(char label)
    {
        UpdateLabel(label);
    }

    public void UpdateLabel(char label)
    {
        var normalizedLabel = NormalizeLabel(label);
        ValidateLabel(normalizedLabel);
        Label = normalizedLabel;
    }

    public void AddTeam(Team? team)
    {
        ValidateRequiredTeam(team);
        ValidateGroupHasAvailableSlot();
        ValidateConfederationLimit(team!);

        _teams.Add(team!);
    }
    
    
    private void ValidateConfederationLimit(Team team)
    {
        int teamsFromSameConfederation = _teams.Count(t =>
            t.ConfederationName == team.ConfederationName);

        bool isUefa = team.ConfederationName == "UEFA";

        int limit = isUefa
            ? MaxUEFATeamsPerGroup
            : MaxOtherConfederationTeamsPerGroup;

        if (teamsFromSameConfederation >= limit)
            throw new ArgumentException(TooManyTeamsFromSameConfederationErrorMessage);
    }

    public void RemoveTeam(Team? team)
    {
        ValidateRequiredTeam(team);

        if (!_teams.Remove(team!))
            throw new ArgumentException(TeamNotFoundErrorMessage);
    }

    public void AddMatch(Match? match)
    {
        if (match is null)
            throw new ArgumentException(NullMatchErrorMessage);

        _matches.Add(match);
    }

    // Group dice "actualizate con este resultado" — Match se encarga del resto
    public void SetMatchResult(Match match, int homeGoals, int awayGoals)
    {
        match.SetResult(homeGoals, awayGoals); // Match notifica a los Teams internamente
    }

    public Team GetTeamByPosition(int position)
    {
        ValidatePosition(position);
        return GetStandings().ElementAt(position - 1);
    }

    // Group ordena por lo que Team ya sabe de sí mismo — sin recalcular
    public IEnumerable<Team> GetStandings()
    {
        return _teams
            .OrderByDescending(t => t.Points)
            .ThenByDescending(t => t.GoalDifference)
            .ThenByDescending(t => t.GoalsInFavor)
            .ToList();
    }

    private static char NormalizeLabel(char label) => char.ToUpperInvariant(label);

    private static void ValidateLabel(char label)
    {
        if (label < MinLabel || label > MaxLabel)
            throw new ArgumentException(LabelOutOfRangeErrorMessage, nameof(label));
    }

    private static void ValidateRequiredTeam(Team? team)
    {
        if (team is null)
            throw new ArgumentException(NullTeamErrorMessage);
    }

    private void ValidateGroupHasAvailableSlot()
    {
        if (_teams.Count >= MaxTeams)
            throw new ArgumentException(FullGroupErrorMessage);
    }

    private void ValidatePosition(int position)
    {
        if (position < 1 || position > _teams.Count)
            throw new ArgumentException(InvalidPositionErrorMessage);
    }
}