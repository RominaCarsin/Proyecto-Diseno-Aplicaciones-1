namespace Domain;

public class Pot
{
    public const int MaxTeams = 12;
    public const int MinNumber = 1;
    public const int MaxNumber = 4;

    private const string InvalidNumberErrorMessage = "Pot number must be between 1 and 4.";
    private const string NullTeamsListErrorMessage = "Teams list cannot be null.";
    private const string InvalidTeamsCountErrorMessage = "Pot must have exactly 12 teams.";

    public int Number { get; }
    public IReadOnlyList<Team> Teams { get; }

    public bool IsFull() => Teams.Count == MaxTeams;

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