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
    private const string FinishedMatchError = "Finished matches cannot be edited.";
    private const string DrawInKnockoutError = "Knockout matches cannot end in a draw.";
    private const string KnockoutGroupError = "Knockout matches cannot have a group assigned.";

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
    public MatchStatus Status { get; private set; }
    public char? Group { get; private set; }
    public TournamentPhase Phase { get; private set; }
    public string? MatchCode { get; private set; }

    public bool HasResult => HomeGoals.HasValue && AwayGoals.HasValue;

    public Match(
        DateTime date,
        Stadium? stadium,
        Team? homeTeam,
        Team? awayTeam,
        char? group = null,
        TournamentPhase phase = TournamentPhase.GroupStage,
        string? matchCode = null)
    {
        ValidateDate(date);
        ValidateRequired(stadium, RequiredStadiumError);
        ValidateRequired(homeTeam, RequiredHomeTeamError);
        ValidateRequired(awayTeam, RequiredAwayTeamError);
        ValidateTeamsAreDifferent(homeTeam!, awayTeam!);
        ValidateGroupForPhase(group, phase);

        Id = _nextId++;
        Date = date;
        Stadium = stadium!;
        HomeTeam = homeTeam!;
        AwayTeam = awayTeam!;
        Group = group;
        Phase = phase;
        MatchCode = NormalizeMatchCode(matchCode);
        Status = MatchStatus.NotFinished;
    }

    public void SetResult(int homeGoals, int awayGoals)
    {
        ValidateIsEditable();
        ValidateGoals(homeGoals, awayGoals);

        if (HasResult)
            RevertPreviousResult();

        HomeGoals = homeGoals;
        AwayGoals = awayGoals;
        Winner = DetermineWinner(homeGoals, awayGoals);

        NotifyTeams();
    }

    public void Finish()
    {
        ValidateIsNotFinished();
        Status = MatchStatus.Finished;
        IsLocked = true;
    }

    public void UpdateDate(DateTime newDate)
    {
        ValidateIsEditable();
        ValidateDate(newDate);

        Date = newDate;
    }

    public void UpdateStadium(Stadium? newStadium)
    {
        ValidateIsEditable();
        ValidateRequired(newStadium, RequiredStadiumError);

        Stadium = newStadium!;
    }

    public void LockEdition()
    {
        IsLocked = true;
    }

    private void RevertPreviousResult()
    {
        if (HomeGoals > AwayGoals)
        {
            HomeTeam.RevertWin(HomeGoals!.Value, AwayGoals!.Value);
            AwayTeam.RevertLoss(AwayGoals.Value, HomeGoals.Value);
        }
        else if (AwayGoals > HomeGoals)
        {
            AwayTeam.RevertWin(AwayGoals!.Value, HomeGoals!.Value);
            HomeTeam.RevertLoss(HomeGoals.Value, AwayGoals.Value);
        }
        else
        {
            HomeTeam.RevertDraw(HomeGoals!.Value);
            AwayTeam.RevertDraw(AwayGoals!.Value);
        }
    }

    private void NotifyTeams()
    {
        if (HomeGoals > AwayGoals)
        {
            HomeTeam.RecordWin(HomeGoals!.Value, AwayGoals!.Value);
            AwayTeam.RecordLoss(AwayGoals.Value, HomeGoals.Value);
        }
        else if (AwayGoals > HomeGoals)
        {
            AwayTeam.RecordWin(AwayGoals!.Value, HomeGoals!.Value);
            HomeTeam.RecordLoss(HomeGoals.Value, AwayGoals.Value);
        }
        else
        {
            HomeTeam.RecordDraw(HomeGoals!.Value);
            AwayTeam.RecordDraw(AwayGoals!.Value);
        }
    }

    private void ValidateIsEditable()
    {
        ValidateIsNotFinished();
        ValidateIsNotLocked();
    }

    private void ValidateIsNotFinished()
    {
        if (Status == MatchStatus.Finished)
            throw new InvalidOperationException(FinishedMatchError);
    }

    private void ValidateIsNotLocked()
    {
        if (IsLocked)
            throw new InvalidOperationException(LockedMatchError);
    }

    private static void ValidateDate(DateTime date)
    {
        if (date == DateTime.MinValue)
            throw new ArgumentException(InvalidDateError, nameof(date));
    }

    private static void ValidateRequired<T>(T? value, string errorMessage)
    {
        if (value is null)
            throw new ArgumentException(errorMessage);
    }

    private static void ValidateTeamsAreDifferent(Team homeTeam, Team awayTeam)
    {
        if (AreSameTeam(homeTeam, awayTeam))
            throw new ArgumentException(SameTeamsError);
    }

    private static void ValidateGroupForPhase(char? group, TournamentPhase phase)
    {
        if (phase != TournamentPhase.GroupStage && group.HasValue)
            throw new InvalidOperationException(KnockoutGroupError);
    }

    private void ValidateGoals(int homeGoals, int awayGoals)
    {
        ValidateSingleGoalValue(homeGoals, nameof(homeGoals));
        ValidateSingleGoalValue(awayGoals, nameof(awayGoals));

        if (Phase != TournamentPhase.GroupStage && homeGoals == awayGoals)
            throw new InvalidOperationException(DrawInKnockoutError);
    }

    private static void ValidateSingleGoalValue(int goals, string paramName)
    {
        if (goals < MinGoals)
            throw new ArgumentOutOfRangeException(paramName, NegativeGoalsError);
    }

    private static bool AreSameTeam(Team homeTeam, Team awayTeam) =>
        ReferenceEquals(homeTeam, awayTeam)
        || string.Equals(homeTeam.Name, awayTeam.Name, StringComparison.OrdinalIgnoreCase);

    private static string? NormalizeMatchCode(string? matchCode)
    {
        if (string.IsNullOrWhiteSpace(matchCode))
            return null;

        return matchCode.Trim().ToUpperInvariant();
    }

    private Team? DetermineWinner(int homeGoals, int awayGoals)
    {
        if (homeGoals > awayGoals) return HomeTeam;
        if (awayGoals > homeGoals) return AwayTeam;
        return null;
    }
}
