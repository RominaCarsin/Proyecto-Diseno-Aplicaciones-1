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
        if ()
            throw new ArgumentException("Confederation is full.");
        
        this.--;
        Teams.Add(team);
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