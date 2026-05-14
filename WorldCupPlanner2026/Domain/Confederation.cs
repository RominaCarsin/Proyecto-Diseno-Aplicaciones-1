namespace Domain;

public class Confederation
{
    private const string NameRequiredErrorMessage = "Name cannot be empty.";
    private const string SlotsInvalidErrorMessage = "Slots must be greater than zero.";
    private const string NullTeamErrorMessage = "Team cannot be null.";
    private const string ConfederationFullErrorMessage = "Confederation is full.";

    public string Name { get; private set; }
    public IReadOnlyList<Team> Teams => _teams;
    public int Slots { get; private set; }

    public bool IsFull => _teams.Count >= Slots;
    public int RemainingSlots => Slots - _teams.Count;

    private readonly List<Team> _teams = new();

    public Confederation(string name, int slots)
    {
        ValidateName(name);
        ValidateSlots(slots);

        Name = name;
        Slots = slots;
    }

    public void AddTeam(Team team)
    {
        if (team is null)
            throw new ArgumentException(NullTeamErrorMessage);

        if (IsFull)
            throw new ArgumentException(ConfederationFullErrorMessage);

        _teams.Add(team);
    }

    public void RemoveTeam(Team team)
    {
        if (team is null)
            throw new ArgumentException(NullTeamErrorMessage);

        if (!_teams.Contains(team))
            throw new ArgumentException(
                $"Team '{team.Name}' does not belong to confederation '{Name}'.");

        _teams.Remove(team);
    }
    
    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(NameRequiredErrorMessage);
    }

    private static void ValidateSlots(int slots)
    {
        if (slots <= 0)
            throw new ArgumentException(SlotsInvalidErrorMessage);
    }
}