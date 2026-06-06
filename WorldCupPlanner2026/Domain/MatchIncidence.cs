namespace Domain;

public abstract class MatchIncidence
{
    private const string RequiredTeamErrorMessage = "La incidencia debe estar asociada a un equipo.";
    private const string InvalidMinuteErrorMessage = "El minuto de la incidencia debe estar entre 0 y 130.";

    private const int MinMinute = 0;
    private const int MaxMinute = 130;

    public int IncidenceId { get; private set; } 
    public Team Team { get; private set; }
    public int Minute { get; private set; }

    protected MatchIncidence() { }

    protected MatchIncidence(Team? team, int minute)
    {
        if (team is null)
            throw new ArgumentException(RequiredTeamErrorMessage);

        if (minute < MinMinute || minute > MaxMinute)
            throw new ArgumentException(InvalidMinuteErrorMessage);

        Team = team;
        Minute = minute;
    }
}