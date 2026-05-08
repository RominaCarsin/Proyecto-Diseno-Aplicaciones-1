namespace Domain;

public class Group
{
    private const char MinLabel = 'A';
    private const char MaxLabel = 'L';
    private const int MaxTeams = 4;

    private const string LabelOutOfRangeErrorMessage = "Label debe ser una letra mayúscula entre 'A' y 'L'.";
    private const string NullTeamErrorMessage = "Team cannot be null.";
    private const string DuplicatedTeamErrorMessage = "Team already exists in group.";
    private const string FullGroupErrorMessage = "Group cannot have more than 4 teams.";
    private const string TeamNotFoundErrorMessage = "Team does not exist in group.";
    private const string InvalidPositionErrorMessage = "Position is invalid.";

    private readonly List<Team> _teams = new();

    public char Label { get; private set; }

    public IReadOnlyList<Team> Teams => _teams;

    public Group(char label)
    {
        UpdateLabel(label);
    }

    public void UpdateLabel(char label)
    {
        char normalizedLabel = NormalizeLabel(label);

        ValidateLabel(normalizedLabel);

        Label = normalizedLabel;
    }

    public void AddTeam(Team? team)
    {
        ValidateRequiredTeam(team);
        ValidateTeamIsNotDuplicated(team);
        ValidateGroupHasAvailableSlot();

        _teams.Add(team);
    }

    public void RemoveTeam(Team? team)
    {
        ValidateRequiredTeam(team);

        if (!_teams.Remove(team))
        {
            throw new ArgumentException(TeamNotFoundErrorMessage);
        }
    }

    public Team GetTeamByPosition(int position)
    {
        ValidatePosition(position);

        return _teams[position - 1];
    }

    private static char NormalizeLabel(char label)
    {
        return char.ToUpperInvariant(label);
    }

    private static void ValidateLabel(char label)
    {
        if (label < MinLabel || label > MaxLabel)
        {
            throw new ArgumentException(LabelOutOfRangeErrorMessage, nameof(label));
        }
    }

    private static void ValidateRequiredTeam(Team? team)
    {
        if (team is null)
        {
            throw new ArgumentException(NullTeamErrorMessage);
        }
    }

    private void ValidateTeamIsNotDuplicated(Team team)
    {
        if (ContainsTeam(team))
        {
            throw new ArgumentException(DuplicatedTeamErrorMessage);
        }
    }

    private bool ContainsTeam(Team team)
    {
        return _teams.Any(existingTeam =>
            existingTeam.Name.Equals(team.Name, StringComparison.OrdinalIgnoreCase));
    }

    private void ValidateGroupHasAvailableSlot()
    {
        if (_teams.Count >= MaxTeams)
        {
            throw new ArgumentException(FullGroupErrorMessage);
        }
    }

    private void ValidatePosition(int position)
    {
        if (position < 1 || position > _teams.Count)
        {
            throw new ArgumentException(InvalidPositionErrorMessage);
        }
    }
    
    private readonly List<Match> _matches = new();

    public IReadOnlyList<Match> Matches => _matches;

    public void AddMatch(Match match)
    {
        _matches.Add(match);
    }
}