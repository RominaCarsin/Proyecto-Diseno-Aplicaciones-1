namespace Domain;

public class Match
{
    private const int MinGoals = 0;

    private const string InvalidDateErrorMessage = "Match date must be valid.";
    private const string RequiredStadiumErrorMessage = "Stadium cannot be null.";
    private const string RequiredHomeTeamErrorMessage = "Home team cannot be null.";
    private const string RequiredAwayTeamErrorMessage = "Away team cannot be null.";
    private const string SameTeamsErrorMessage = "Home team and away team must be different.";
    private const string LockedMatchErrorMessage = "Locked matches cannot be edited.";
    private const string NegativeGoalsErrorMessage = "Goals cannot be negative.";
    private const string FinishedMatchErrorMessage = "Finished matches cannot be edited.";
    
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
    public string Phase { get; private set; } = "GroupStage";
    
    public bool HasResult => HomeGoals.HasValue && AwayGoals.HasValue;

    public Match(
        DateTime date,
        Stadium? stadium,
        Team? homeTeam,
        Team? awayTeam,
        char? group = null, 
        string phase = "GroupStage")
    {
        ValidateDate(date);
        ValidateRequired(stadium, RequiredStadiumErrorMessage);
        ValidateRequired(homeTeam, RequiredHomeTeamErrorMessage);
        ValidateRequired(awayTeam, RequiredAwayTeamErrorMessage);
        ValidateTeamsAreDifferent(homeTeam!, awayTeam!);

        Id = _nextId++;
        Date = date;
        Stadium = stadium!;
        HomeTeam = homeTeam!;
        AwayTeam = awayTeam!;
        Group = group;       
        Phase = phase;
        Status = MatchStatus.NotFinished;
    }

    public void SetResult(int homeGoals, int awayGoals)
    {
        ValidateIsNotFinished();
        ValidateGoals(homeGoals, awayGoals);

        if (HasResult)
            RevertPreviousResult();
        
        HomeGoals = homeGoals;
        AwayGoals = awayGoals;
        Winner = DetermineWinner(homeGoals, awayGoals);

        NotifyTeams();
        
    }
    
    private void RevertPreviousResult()
    {
        if (HomeGoals > AwayGoals)
        {
            HomeTeam.RevertWin(HomeGoals.Value, AwayGoals.Value);
            AwayTeam.RevertLoss(AwayGoals.Value, HomeGoals.Value);
        }
        else if (AwayGoals > HomeGoals)
        {
            AwayTeam.RevertWin(AwayGoals.Value, HomeGoals.Value);
            HomeTeam.RevertLoss(HomeGoals.Value, AwayGoals.Value);
        }
        else
        {
            HomeTeam.RevertDraw(HomeGoals.Value);
            AwayTeam.RevertDraw(AwayGoals.Value);
        }
    }
    
    private void NotifyTeams()
    {
        if (HomeGoals > AwayGoals)
        {
            HomeTeam.RecordWin(HomeGoals.Value, AwayGoals.Value);
            AwayTeam.RecordLoss(AwayGoals.Value, HomeGoals.Value);
        }
        else if (AwayGoals > HomeGoals)
        {
            AwayTeam.RecordWin(AwayGoals.Value, HomeGoals.Value);
            HomeTeam.RecordLoss(HomeGoals.Value, AwayGoals.Value);
        }
        else
        {
            HomeTeam.RecordDraw(HomeGoals.Value);
            AwayTeam.RecordDraw(AwayGoals.Value);
        }
    }
    
    public void Finish()
    {
        ValidateIsNotFinished();
        Status = MatchStatus.Finished;
        IsLocked = true;
    }
    
    private void ValidateIsNotFinished()
    {
        if (Status == MatchStatus.Finished)
            throw new InvalidOperationException(FinishedMatchErrorMessage);
    }

    public void UpdateDate(DateTime newDate)
    {
        ValidateIsNotLocked();
        ValidateDate(newDate);

        Date = newDate;
    }

    public void UpdateStadium(Stadium? newStadium)
    {
        ValidateIsNotLocked();
        ValidateRequired(newStadium, RequiredStadiumErrorMessage);

        Stadium = newStadium!;
    }

    public void LockEdition()
    {
        IsLocked = true;
    }
    
    
    private void ValidateIsNotLocked()
    {
        if (IsLocked)
            throw new InvalidOperationException(LockedMatchErrorMessage);
    }

    private static void ValidateDate(DateTime date)
    {
        if (date == DateTime.MinValue)
            throw new ArgumentException(InvalidDateErrorMessage, nameof(date));
    }

    private static void ValidateRequired<T>(T? value, string errorMessage)
    {
        if (value is null)
            throw new ArgumentException(errorMessage);
    }

    private static void ValidateTeamsAreDifferent(Team homeTeam, Team awayTeam)
    {
        if (AreSameTeam(homeTeam, awayTeam))
            throw new ArgumentException(SameTeamsErrorMessage);
    }

    private static void ValidateGoals(int homeGoals, int awayGoals)
    {
        ValidateSingleGoalValue(homeGoals, nameof(homeGoals));
        ValidateSingleGoalValue(awayGoals, nameof(awayGoals));
    }

    private static void ValidateSingleGoalValue(int goals, string paramName)
    {
        if (goals < MinGoals)
            throw new ArgumentOutOfRangeException(paramName, NegativeGoalsErrorMessage);
    }


    private static bool AreSameTeam(Team homeTeam, Team awayTeam) =>
        ReferenceEquals(homeTeam, awayTeam)
        || string.Equals(homeTeam.Name, awayTeam.Name, StringComparison.OrdinalIgnoreCase);

    private Team? DetermineWinner(int homeGoals, int awayGoals)
    {
        if (homeGoals > awayGoals) return HomeTeam;
        if (awayGoals > homeGoals) return AwayTeam;
        return null;
    }
}