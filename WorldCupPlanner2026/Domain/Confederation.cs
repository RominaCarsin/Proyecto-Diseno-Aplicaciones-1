using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Confederation
{
    private const string NameRequiredErrorMessage = "El nombre no puede estar vacío.";
    private const string SlotsInvalidErrorMessage = "Los cupos deben ser mayores a cero.";
    private const string NullTeamErrorMessage = "El equipo no puede ser nulo.";
    private const string ConfederationFullErrorMessage = "La confederación está completa.";
    
    public string Name { get; private set; }
    public IReadOnlyList<Team> Teams => _teams;
    public int Slots { get; private set; }

    public bool IsFull => _teams.Count >= Slots;
    public int RemainingSlots => Slots - _teams.Count;

    private readonly List<Team> _teams = new();

    private Confederation() { }
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

        if (!_teams.Remove(team))
            throw new ArgumentException($"El equipo '{team.Name}' no pertenece a la confederación '{Name}'.");
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