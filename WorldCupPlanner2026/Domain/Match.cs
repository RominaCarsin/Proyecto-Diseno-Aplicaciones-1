namespace Domain;

public class Match
{
    private const int MinGoals = 0;

    private const string InvalidDateError = "Match date must be valid.";
    private const string RequiredStadiumError = "Stadium cannot be null.";
    private const string RequiredHomeTeamError = "Home team cannot be null.";
    private const string RequiredAwayTeamError = "Away team cannot be null.";
    private const string SameTeamsError = "Home team and away team must be different.";
    private const string LockedMatchError = "Locked matches cannot be edited.";
    private const string NegativeGoalsError = "Goals cannot be negative.";

    private static int _nextId = 1;

    public int Id { get; private set; }
    public DateTime Date { get; private set; }
    public Stadium Stadium { get; private set; }
    public Team HomeTeam { get; private set; }
    public Team AwayTeam { get; private set; }
    public int? HomeGoals { get; private set; }
    public int? AwayGoals { get; private set; }
    public Team? Winner { get; private set; }
    public bool IsLocked { get; private set; }

    public Match(
        DateTime date,
        Stadium? stadium,
        Team? homeTeam,
        Team? awayTeam)
    {
        ValidateCreationData(date, stadium, homeTeam, awayTeam);

        Id = _nextId++;
        Date = date;
        Stadium = stadium!;
        HomeTeam = homeTeam!;
        AwayTeam = awayTeam!;
    }

    public void SetResult(int homeGoals, int awayGoals)
    {
        ValidateCanEdit();
        ValidateResult(homeGoals, awayGoals);

        HomeGoals = homeGoals;
        AwayGoals = awayGoals;
        Winner = GetWinner(homeGoals, awayGoals);
    }

    public void UpdateDate(DateTime newDate)
    {
        ValidateCanEdit();
        ValidateDate(newDate);

        Date = newDate;
    }

    public void UpdateStadium(Stadium? newStadium)
    {
        ValidateCanEdit();
        ValidateRequired(newStadium, RequiredStadiumError);

        Stadium = newStadium!;
    }

    public void LockEdition()
    {
        IsLocked = true;
    }

    private static void ValidateCreationData(
        DateTime date,
        Stadium? stadium,
        Team? homeTeam,
        Team? awayTeam)
    {
        ValidateDate(date);
        ValidateRequired(stadium, RequiredStadiumError);
        ValidateRequired(homeTeam, RequiredHomeTeamError);
        ValidateRequired(awayTeam, RequiredAwayTeamError);
        ValidateTeamsAreDifferent(homeTeam!, awayTeam!);
    }

    private static void ValidateResult(int homeGoals, int awayGoals)
    {
        ValidateGoals(homeGoals, awayGoals);
    }

    private void ValidateCanEdit()
    {
        if (IsLocked)
        {
            throw new InvalidOperationException(LockedMatchError);
        }
    }

    private static void ValidateDate(DateTime date)
    {
        if (date == DateTime.MinValue)
        {
            throw new ArgumentException(InvalidDateError, nameof(date));
        }
    }

    private static void ValidateRequired<T>(T? value, string errorMessage)
    {
        if (value is null)
        {
            throw new ArgumentException(errorMessage);
        }
    }

    private static void ValidateTeamsAreDifferent(Team homeTeam, Team awayTeam)
    {
        if (AreSameTeam(homeTeam, awayTeam))
        {
            throw new ArgumentException(SameTeamsError);
        }
    }

    private static bool AreSameTeam(Team homeTeam, Team awayTeam)
    {
        return ReferenceEquals(homeTeam, awayTeam)
               || string.Equals(
                   homeTeam.Name,
                   awayTeam.Name,
                   StringComparison.OrdinalIgnoreCase);
    }

    private static void ValidateGoals(int homeGoals, int awayGoals)
    {
        if (homeGoals < MinGoals)
        {
            throw new ArgumentOutOfRangeException(nameof(homeGoals), NegativeGoalsError);
        }

        if (awayGoals < MinGoals)
        {
            throw new ArgumentOutOfRangeException(nameof(awayGoals), NegativeGoalsError);
        }
    }

    private Team? GetWinner(int homeGoals, int awayGoals)
    {
        if (homeGoals > awayGoals)
        {
            return HomeTeam;
        }

        if (awayGoals > homeGoals)
        {
            return AwayTeam;
        }

        return null;
    }
}