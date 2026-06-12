using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Group
{
    private const char MinLabel = 'A';
    private const char MaxLabel = 'L';
    private const int MaxTeams = 4;
    private const int MaxUEFATeamsPerGroup = 2;
    private const int MaxOtherConfederationTeamsPerGroup = 1;
    private const double DrawZone = 0.10;
    
    private const string TooManyTeamsFromSameConfederationErrorMessage = "El grupo no puede tener más de 2 equipos de la misma confederación.";
    private const string LabelOutOfRangeErrorMessage = "La etiqueta debe ser una letra mayúscula entre 'A' y 'L'.";
    private const string NullTeamErrorMessage = "El equipo no puede ser nulo.";
    private const string FullGroupErrorMessage = "El grupo no puede tener más de 4 equipos.";
    private const string TeamNotFoundErrorMessage = "El equipo no existe en el grupo.";
    private const string InvalidPositionErrorMessage = "La posición es inválida.";
    private const string NullMatchErrorMessage = "El partido no puede ser nulo.";

    private readonly List<Team> _teams = new();
    private readonly List<Match> _matches = new();
    
    public int GroupId { get; private set; }
    public char Label { get; private set; }
    public IReadOnlyList<Team> Teams => _teams;
    public IReadOnlyList<Match> Matches => _matches;

    private Group() { }
    public Group(char label)
    {
        UpdateLabel(label);
    }

    private void UpdateLabel(char label)
    {
        var normalizedLabel = NormalizeLabel(label);
        ValidateLabel(normalizedLabel);
        Label = normalizedLabel;
    }

    public void AddTeam(Team? team)
    {
        ValidateRequiredTeam(team);
        ValidateGroupHasAvailableSlot();
        ValidateConfederationLimit(team!);

        _teams.Add(team!);
    }
    
    public void FinishMatch(Match match) => match.Finish();

    private void ValidateConfederationLimit(Team team)
    {
        int teamsFromSameConfederation = _teams.Count(t =>
            t.ConfederationName == team.ConfederationName);

        bool isUefa = team.ConfederationName == "UEFA";

        int limit = isUefa
            ? MaxUEFATeamsPerGroup
            : MaxOtherConfederationTeamsPerGroup;

        if (teamsFromSameConfederation >= limit)
            throw new ArgumentException(TooManyTeamsFromSameConfederationErrorMessage);
    }


    public void AddMatch(Match? match)
    {
        ValidateRequiredMatch(match);
        _matches.Add(match!);
    }

    public void SetMatchResult(Match match, int homeGoals, int awayGoals) =>
        match.SetResult(homeGoals, awayGoals);

    public Team GetTeamByPosition(int position)
    {
        ValidatePosition(position);
        return GetOrderedTeamsByPosition().ElementAt(position - 1);
    }
    public IEnumerable<Team> GetOrderedTeamsByPosition()
    {
        return _teams
            .OrderByDescending(t => t.Points)
            .ThenByDescending(t => t.GoalDifference)
            .ThenByDescending(t => t.GoalsInFavor)
            .ToList();
    }
    
    
    public IEnumerable<Match> FinishPlayedMatches()
    {
        var playedMatches = _matches
            .Where(match => match.HasResult && match.Status != MatchStatus.Finished)
            .ToList();

        foreach (var match in playedMatches)
            match.Finish();

        return playedMatches;
    }

    private static char NormalizeLabel(char label) => char.ToUpperInvariant(label);

    private static void ValidateLabel(char label)
    {
        if (label < MinLabel || label > MaxLabel)
            throw new ArgumentException(LabelOutOfRangeErrorMessage, nameof(label));
    }

    private static void ValidateRequiredTeam(Team? team)
    {
        if (team is null)
            throw new ArgumentException(NullTeamErrorMessage);
    }

    private static void ValidateRequiredMatch(Match? match)
    {
        if (match is null)
            throw new ArgumentException(NullMatchErrorMessage);
    }

    private void ValidateGroupHasAvailableSlot()
    {
        if (_teams.Count >= MaxTeams)
            throw new ArgumentException(FullGroupErrorMessage);
    }

    private void ValidatePosition(int position)
    {
        if (position < 1 || position > _teams.Count)
            throw new ArgumentException(InvalidPositionErrorMessage);
    }
}