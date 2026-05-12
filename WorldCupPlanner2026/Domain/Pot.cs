namespace Domain;

public class Pot
{
    public const int MaxTeams = 12;
    public const int MinNumber = 1;
    public const int MaxNumber = 4;

    public int Number { get; }
    public List<Team> Teams { get; }

    public Pot(int number, List<Team> teams)
    {
        if (number < MinNumber || number > MaxNumber)
            throw new ArgumentException($"El número de bombo debe estar entre {MinNumber} y {MaxNumber}.");

        if (teams == null)
            throw new ArgumentException("La lista de equipos no puede ser nula.");

        if (teams.Count != MaxTeams)
            throw new ArgumentException($"El bombo debe tener exactamente {MaxTeams} equipos.");

        Number = number;
        Teams  = teams;
    }

    public bool IsFull() => Teams.Count == MaxTeams;
}