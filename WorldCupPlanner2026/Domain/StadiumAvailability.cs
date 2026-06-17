namespace Domain;

public static class StadiumAvailability
{
    public const string MaxMatchesPerDayErrorMessage = "El estadio no puede tener más de 3 partidos por día.";
    public const string MinHoursBetweenMatchesErrorMessage = "Los partidos en el mismo estadio deben tener al menos 4 horas de diferencia.";

    public static void Validate(IEnumerable<Match> scheduledMatches, Stadium stadium, DateTime matchDate)
    {
        var matchesInSameStadiumAndDay = GetMatchesInSameStadiumAndDay(scheduledMatches, stadium, matchDate);

        if (ExceedsMaxMatchesPerDay(matchesInSameStadiumAndDay))
            throw new InvalidOperationException(MaxMatchesPerDayErrorMessage);

        if (HasMatchTooClose(matchesInSameStadiumAndDay, matchDate))
            throw new InvalidOperationException(MinHoursBetweenMatchesErrorMessage);
    }

    public static bool IsAvailable(IEnumerable<Match> scheduledMatches, Stadium stadium, DateTime matchDate)
    {
        var matchesInSameStadiumAndDay = GetMatchesInSameStadiumAndDay(scheduledMatches, stadium, matchDate);

        return !ExceedsMaxMatchesPerDay(matchesInSameStadiumAndDay)
               && !HasMatchTooClose(matchesInSameStadiumAndDay, matchDate);
    }

    private static List<Match> GetMatchesInSameStadiumAndDay(
        IEnumerable<Match> scheduledMatches, Stadium stadium, DateTime matchDate) =>
        scheduledMatches
            .Where(m => m.Stadium.Name == stadium.Name)
            .Where(m => m.Date.Date == matchDate.Date)
            .ToList();

    private static bool ExceedsMaxMatchesPerDay(List<Match> matchesInSameStadiumAndDay) =>
        matchesInSameStadiumAndDay.Count >= 3;

    private static bool HasMatchTooClose(List<Match> matchesInSameStadiumAndDay, DateTime matchDate) =>
        matchesInSameStadiumAndDay.Any(m => Math.Abs((m.Date - matchDate).TotalHours) < 4);
}