namespace Domain;

public class KnockoutMatchFactory : IKnockoutMatchFactory
{
    private readonly List<Stadium> _stadiums;
    private readonly DateTime _defaultDate;
    private int _matchCounter;

    public KnockoutMatchFactory(IEnumerable<Stadium> stadiums, DateTime defaultDate)
    {
        if (stadiums is null || !stadiums.Any())
            throw new ArgumentException("Se requiere al menos un estadio para crear partidos de knockout.");

        if (defaultDate == default(DateTime))
            throw new ArgumentException("La fecha del partido no puede ser la fecha por defecto.");

        _stadiums = new List<Stadium>(stadiums);
        _defaultDate = defaultDate;
        _matchCounter = 0;
    }

    public Match Create(
        Team homeTeam,
        Team awayTeam,
        TournamentPhase phase,
        string matchCode)
    {
        var stadium = _stadiums[_matchCounter % _stadiums.Count];
        _matchCounter++;

        return new Match(
            _defaultDate,
            stadium,
            homeTeam,
            awayTeam,
            group: null,
            phase: phase,
            matchCode: matchCode);
    }
}
