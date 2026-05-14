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
    private const string DrawInKnockoutErrorMessage = "Knockout matches cannot end in a draw.";
    private const string KnockoutGroupErrorMessage = "Knockout matches cannot have a group assigned.";

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
    public Group? Group { get; private set; }
    public TournamentPhase Phase { get; private set; }

    public bool HasResult => HomeGoals.HasValue && AwayGoals.HasValue;

    public Match(
        DateTime date,
        Stadium? stadium,
        Team? homeTeam,
        Team? awayTeam,
        Group? group = null,
        TournamentPhase phase = TournamentPhase.GroupStage)
    {
        ValidateDate(date);
        ValidateRequired(stadium, RequiredStadiumErrorMessage);
        ValidateRequired(homeTeam, RequiredHomeTeamErrorMessage);
        ValidateRequired(awayTeam, RequiredAwayTeamErrorMessage);
        ValidateTeamsAreDifferent(homeTeam!, awayTeam!);
        ValidateGroupIsNotAssignedToKnockoutPhase(group, phase);

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
        ValidateIsEditable();
        ValidateGoalsAreNotNegative(homeGoals, awayGoals);
        ValidateKnockoutMatchIsNotDraw(homeGoals, awayGoals);

        if (HasResult)
            RevertPreviousResultFromTeams();

        HomeGoals = homeGoals;
        AwayGoals = awayGoals;
        Winner = DetermineWinner(homeGoals, awayGoals);

        ApplyResultToTeams();
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
        ValidateRequired(newStadium, RequiredStadiumErrorMessage);

        Stadium = newStadium!;
    }

    public void LockEdition()
    {
        IsLocked = true;
    }
    
    private void ValidateIsEditable()
    {
        ValidateIsNotFinished();
        ValidateIsNotLocked();
    }

    private void ValidateIsNotFinished()
    {
        if (Status == MatchStatus.Finished)
            throw new InvalidOperationException(FinishedMatchErrorMessage);
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

    private static void ValidateGroupIsNotAssignedToKnockoutPhase(Group? group, TournamentPhase phase)
    {
        if (phase != TournamentPhase.GroupStage && group is not null)
            throw new InvalidOperationException(KnockoutGroupErrorMessage);
    }

    private static void ValidateGoalsAreNotNegative(int homeGoals, int awayGoals)
    {
        ValidateSingleGoalValue(homeGoals, nameof(homeGoals));
        ValidateSingleGoalValue(awayGoals, nameof(awayGoals));
    }

    private void ValidateKnockoutMatchIsNotDraw(int homeGoals, int awayGoals)
    {
        if (Phase != TournamentPhase.GroupStage && homeGoals == awayGoals)
            throw new InvalidOperationException(DrawInKnockoutErrorMessage);
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

    private void RevertPreviousResultFromTeams() =>
        ApplyResultOperationToTeams(
            HomeGoals!.Value,
            AwayGoals!.Value,
            onHomeWin: (hg, ag) => { HomeTeam.RevertWin(hg, ag); AwayTeam.RevertLoss(ag, hg); },
            onAwayWin: (hg, ag) => { AwayTeam.RevertWin(ag, hg); HomeTeam.RevertLoss(hg, ag); },
            onDraw: (hg, ag) => { HomeTeam.RevertDraw(hg); AwayTeam.RevertDraw(ag); }
        );

    private void ApplyResultToTeams() =>
        ApplyResultOperationToTeams(
            HomeGoals!.Value,
            AwayGoals!.Value,
            onHomeWin: (hg, ag) => { HomeTeam.RecordWin(hg, ag); AwayTeam.RecordLoss(ag, hg); },
            onAwayWin: (hg, ag) => { AwayTeam.RecordWin(ag, hg); HomeTeam.RecordLoss(hg, ag); },
            onDraw: (hg, ag) => { HomeTeam.RecordDraw(hg); AwayTeam.RecordDraw(ag); }
        );

    private static void ApplyResultOperationToTeams(
        int homeGoals,
        int awayGoals,
        Action<int, int> onHomeWin,
        Action<int, int> onAwayWin,
        Action<int, int> onDraw)
    {
        if (homeGoals > awayGoals) onHomeWin(homeGoals, awayGoals);
        else if (awayGoals > homeGoals) onAwayWin(homeGoals, awayGoals);
        else onDraw(homeGoals, awayGoals);
    }
}