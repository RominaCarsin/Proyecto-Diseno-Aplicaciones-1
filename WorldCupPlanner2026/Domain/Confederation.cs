namespace Domain;

public class Confederation
{
    public string Name { get; private set; }
    public List<Team> Teams { get; private set; }
    public int Slots { get; private set; }

    public Confederation(string name, int slots)
    {
        ValidateName(name);
        ValidateSlots(slots);
        
        Name = name;
        Slots = slots;
        Teams = new List<Team>();
    }
    
    public void AddTeam(Team team)
    {
        if (team == null)
            throw new ArgumentException("Team cannot be null.");
        if (team.ConfederationName == Name && Teams.Count() == Slots)
            throw new ArgumentException("Confederation is full.");
        
        Teams.Add(team);
    }
    
    public void RemoveTeam(Team team)
    {
        if (team == null)
            throw new ArgumentException("Team cannot be null.");
        
        if (team.ConfederationName == Name)
            Teams.Remove(team);
        else
        {
            throw new ArgumentException($"Team '{team.Name}' does not belong to confederation '{Name}'.");
        }
    }

    private void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.");
    }

    private void ValidateSlots(int slots)
    {
        if (slots <= 0)
            throw new ArgumentException("Slots cannot be negative or 0.");
    }
}