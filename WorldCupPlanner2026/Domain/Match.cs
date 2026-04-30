namespace Domain;

using Domain.Exceptions;

public class Match
{
    private const int MinGoals = 0;

    private const string LockedMatchError = "Locked matches cannot be edited.";
    private const string InvalidDateError = "Match date must be valid.";
    private const string DifferentTeamsError = "Home team and away team must be different.";
    private const string RequiredGroupError = "Group is required for group stage matches.";
    private const string KnockoutGroupError = "Only group stage matches can have a group.";

    private static int _nextId = 1;

    public int Id { get; private set; }
    public DateTime Date { get; private set; }
    public Stadium Stadium { get; private set; }
    public Team HomeTeam { get; private set; }
    public Team AwayTeam { get; private set; }
    public Group? Group { get; private set; }
    public TournamentPhase Phase { get; private set; }
    public int? HomeGoals { get; private set; }
    public int? AwayGoals { get; private set; }
    public Team? Winner { get; private set; }
    public bool IsLocked { get; private set; }

    public Match(
        DateTime date,
        Stadium stadium,
        Team homeTeam,
        Team awayTeam,
        Group? group,
        TournamentPhase phase)
    {
        ValidateDate(date);
        ValidateStadium(stadium);
        ValidateHomeTeam(homeTeam);
        ValidateAwayTeam(awayTeam);
        ValidateTeamsAreDifferent(homeTeam, awayTeam);
        ValidateGroup(group, phase);

        Id = _nextId++;
        Date = date;
        Stadium = stadium;
        HomeTeam = homeTeam;
        AwayTeam = awayTeam;
        Group = group;
        Phase = phase;
    }

    public void SetResult(int homeGoals, int awayGoals)
    {
        ValidateNotLocked();
        ValidateGoals(homeGoals, awayGoals);

        HomeGoals = homeGoals;
        AwayGoals = awayGoals;
        Winner = CalculateWinner(homeGoals, awayGoals);
    }

    public void UpdateDate(DateTime newDate)
    {
        ValidateNotLocked();
        ValidateDate(newDate);
        Date = newDate;
    }

    public void UpdateStadium(Stadium newStadium)
    {
        ValidateNotLocked();
        ValidateStadium(newStadium);
        Stadium = newStadium;
    }

    public void LockEdition()
    {
        IsLocked = true;
    }

    private static void ValidateDate(DateTime date)
    {
        if (date == DateTime.MinValue)
        {
            throw new DomainException(InvalidDateError);
        }
    }

    private static void ValidateStadium(Stadium stadium)
    {
        if (stadium == null)
        {
            throw new DomainException("Stadium cannot be null.");
        }
    }

    private static void ValidateHomeTeam(Team homeTeam)
    {
        if (homeTeam == null)
        {
            throw new DomainException("Home team cannot be null.");
        }
    }

    private static void ValidateAwayTeam(Team awayTeam)
    {
        if (awayTeam == null)
        {
            throw new DomainException("Away team cannot be null.");
        }
    }

    private static void ValidateTeamsAreDifferent(Team homeTeam, Team awayTeam)
    {
        if (ReferenceEquals(homeTeam, awayTeam) ||
            string.Equals(homeTeam.Name, awayTeam.Name, StringComparison.OrdinalIgnoreCase))
        {
            throw new DomainException(DifferentTeamsError);
        }
    }

    private static void ValidateGroup(Group? group, TournamentPhase phase)
    {
        if (phase == TournamentPhase.GroupStage && group == null)
        {
            throw new DomainException(RequiredGroupError);
        }

        if (phase != TournamentPhase.GroupStage && group != null)
        {
            throw new DomainException(KnockoutGroupError);
        }
    }

    private void ValidateNotLocked()
    {
        if (IsLocked)
        {
            throw new DomainException(LockedMatchError);
        }
    }

    private static void ValidateGoals(int homeGoals, int awayGoals)
    {
        if (homeGoals < MinGoals)
        {
            throw new DomainException("Home goals cannot be negative.");
        }

        if (awayGoals < MinGoals)
        {
            throw new DomainException("Away goals cannot be negative.");
        }
    }

    private Team? CalculateWinner(int homeGoals, int awayGoals)
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