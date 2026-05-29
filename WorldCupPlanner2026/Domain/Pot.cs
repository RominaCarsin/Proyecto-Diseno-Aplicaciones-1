using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Pot
{
    public const int MaxTeams = 12;
    public const int MinNumber = 1;
    public const int MaxNumber = 4;

    private const string InvalidNumberErrorMessage = "El número del bombo debe estar entre 1 y 4.";
    private const string NullTeamsListErrorMessage = "La lista de equipos no puede ser nula.";
    private const string InvalidTeamsCountErrorMessage = "El bombo debe tener exactamente 12 equipos.";
    
    public int Number { get; }
    public IReadOnlyList<Team> Teams { get; }

    public bool IsFull() => Teams.Count == MaxTeams;

    private Pot() {}
    public Pot(int number, List<Team> teams)
    {
        ValidatePotNumber(number);
        ValidateTeamsList(teams);

        Number = number;
        Teams = teams;
    }
    

    private static void ValidatePotNumber(int number)
    {
        if (number < MinNumber || number > MaxNumber)
            throw new ArgumentException(InvalidNumberErrorMessage);
    }

    private static void ValidateTeamsList(List<Team> teams)
    {
        if (teams is null)
            throw new ArgumentException(NullTeamsListErrorMessage);

        if (teams.Count != MaxTeams)
            throw new ArgumentException(InvalidTeamsCountErrorMessage);
    }
}