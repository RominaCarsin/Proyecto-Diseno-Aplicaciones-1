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

    public void UpdateLabel(char label)
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

    public void RemoveTeam(Team? team)
    {
        ValidateRequiredTeam(team);

        if (!_teams.Remove(team!))
            throw new ArgumentException(TeamNotFoundErrorMessage);
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
        return GetStandings().ElementAt(position - 1);
    }
    public IEnumerable<Team> GetStandings()
    {
        return _teams
            .OrderByDescending(t => t.Points)
            .ThenByDescending(t => t.GoalDifference)
            .ThenByDescending(t => t.GoalsInFavor)
            .ToList();
    }
    
    public void SimulateMatches(int seed)
    {
        var random = new Random(seed + Label);

        foreach (var match in _matches)
        {
            var (homeGoals, awayGoals) = SimulateResult(
                match.HomeTeam.RankingFifa,
                match.AwayTeam.RankingFifa,
                random);

            SetMatchResult(match, homeGoals, awayGoals);
        }
    }
    
    private static (int homeGoals, int awayGoals) SimulateResult(
        int homeRanking,
        int awayRanking,
        Random random)
    {
        if (homeRanking == awayRanking)
        {
            int drawGoals = SimulateGoals(random, 0, 2);
            return (drawGoals, drawGoals);
        }
        
        int total = homeRanking + awayRanking;
        double homeProbability = (double)homeRanking / total;

        double roll = random.NextDouble();

        if (roll < homeProbability - DrawZone)
            return (SimulateGoals(random, 2, 4), SimulateGoals(random, 0, 2));

        if (roll > homeProbability + DrawZone)
            return (SimulateGoals(random, 0, 2), SimulateGoals(random, 2, 4));
        
        int finalDrawGoals = SimulateGoals(random, 0, 2);
        return (finalDrawGoals, finalDrawGoals);
    }
    
    private static int SimulateGoals(Random random, int min, int max)
    {
        return random.Next(min, max + 1);
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