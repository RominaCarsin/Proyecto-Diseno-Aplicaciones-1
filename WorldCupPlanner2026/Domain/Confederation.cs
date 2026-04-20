namespace Domain;

public class Confederation
{
    public string Name { get; private set; }
    public List<Team> Teams { get; private set; }
    public int Slots { get; private set; }

    public Confederation(string name, List<Team> teams, int slots)
    {
        ValidateName(name);
        ValidateTeams(teams);
        ValidateSlots(slots);
        
        Name = name;
        Teams = teams;
        Slots = slots;
    }

    private void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.");
    }

    private void ValidateTeams(List<Team> teams)
    {
        if (teams == null || teams.Count == 0)
            throw new ArgumentException("Teams cannot be null or empty.");
    }

    private void ValidateSlots(int slots)
    {
        if (slots <= 0)
            throw new ArgumentException("Slots cannot be negative or 0.");
    }
}