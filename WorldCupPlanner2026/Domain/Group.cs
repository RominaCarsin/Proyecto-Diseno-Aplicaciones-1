namespace Domain;

public class Group
{
    private const string LabelOutOfRangeErrorMessage = "Label debe ser una letra mayúscula entre 'A' y 'L'.";
    private const string NullTeamErrorMessage = "Team cannot be null.";
    private const string DuplicatedTeamErrorMessage = "Team already exists in group.";
    private const int MaxTeams = 4;
    private const string FullGroupErrorMessage = "Group cannot have more than 4 teams.";
    private const string TeamNotFoundErrorMessage = "Team does not exist in group.";
    private const string InvalidPositionErrorMessage = "Position is invalid.";
    
    public char Label { get; private set; }
    private readonly List<Team> _teams = new();

    public IReadOnlyList<Team> Teams => _teams;

    public Group(char label)
    {
        UpdateLabel(label);
    }

    public void UpdateLabel(char label)
    {
        label = char.ToUpperInvariant(label);
        if (label < 'A' || label > 'L')
            throw new ArgumentException(LabelOutOfRangeErrorMessage, nameof(label));

        Label = char.ToUpperInvariant(label);
    }
    
    
    public void AddTeam(Team team)
    {
        if (team is null)
        {
            throw new ArgumentException(NullTeamErrorMessage);
        }
        bool teamAlreadyExists = Teams.Any(existingTeam =>
            existingTeam.Name.Equals(
                team.Name,
                StringComparison.OrdinalIgnoreCase));
        if (teamAlreadyExists)
        {
            throw new ArgumentException(DuplicatedTeamErrorMessage);
        }
        if (Teams.Count >= MaxTeams)
        {
            throw new ArgumentException(FullGroupErrorMessage);
        }
        _teams.Add(team);
    }
    
    
    public void RemoveTeam(Team? team)
    {
        
        if (team is null)
        {
            throw new ArgumentException(NullTeamErrorMessage);
        }

        bool removed = _teams.Remove(team);

        if (!removed)
        {
            throw new ArgumentException(TeamNotFoundErrorMessage);
        }
        
    }
    
    public Team GetTeamByPosition(int position)
    {
        if (position < 1 || position > Teams.Count)
        {
            throw new ArgumentException(InvalidPositionErrorMessage);
        }
        return Teams[position - 1];
    }
}