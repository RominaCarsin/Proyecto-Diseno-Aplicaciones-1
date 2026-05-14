namespace Domain;

public class Knockout
{
    private const int RequiredQualifiedTeams = 32;
    private static readonly DateTime RoundOf32DefaultDate = new(2026, 7, 1);

    private readonly List<Team> _qualifiedTeams = new();
    private readonly List<Match> _matches = new();

    public int SimulationSeed { get; private set; }
    public IReadOnlyList<Team> QualifiedTeams => _qualifiedTeams;
    public IReadOnlyList<Match> Matches => _matches;

    public Knockout(int simulationSeed)
    {
        SimulationSeed = simulationSeed;
    }

    public void UpdateSimulationSeed(int newSeed)
    {
        SimulationSeed = newSeed;
    }

    public void SetQualifiedTeams(IEnumerable<Team> qualifiedTeams)
    {
        if (qualifiedTeams is null)
            throw new ArgumentException("Qualified teams cannot be null.");

        var teams = qualifiedTeams.ToList();

        if (teams.Count != RequiredQualifiedTeams)
            throw new ArgumentException($"Knockout phase must receive exactly {RequiredQualifiedTeams} qualified teams.");

        _qualifiedTeams.Clear();
        _qualifiedTeams.AddRange(teams);
    }

    public void InitializeRoundOf32()
    {
        if (_qualifiedTeams.Count != RequiredQualifiedTeams)
            throw new InvalidOperationException($"Knockout phase requires exactly {RequiredQualifiedTeams} qualified teams.");

        if (_matches.Any(match => match.Phase == TournamentPhase.RoundOf32))
            throw new InvalidOperationException("Round of 32 has already been initialized.");

        var shuffledTeams = _qualifiedTeams.ToList();
        Shuffle(shuffledTeams, SimulationSeed);

        for (int i = 0; i < RequiredQualifiedTeams; i += 2)
            AddMatch(CreateRoundOf32Match(shuffledTeams[i], shuffledTeams[i + 1]));
    }

    public void AddMatch(Match match)
    {
        if (match is null)
            throw new ArgumentException("Match cannot be null.");

        ValidatePreviousPhaseFinished(match.Phase);

        _matches.Add(match);
    }

    private static Match CreateRoundOf32Match(Team homeTeam, Team awayTeam) =>
        new(
            RoundOf32DefaultDate,
            BuildDefaultStadium(),
            homeTeam,
            awayTeam,
            group: null,
            phase: TournamentPhase.RoundOf32);

    private static Stadium BuildDefaultStadium() =>
        new("KNOCKOUT STADIUM", "TBD", "Default knockout stadium", 60000);

    private void ValidatePreviousPhaseFinished(TournamentPhase phase)
    {
        var previousPhase = GetPreviousPhase(phase);

        if (previousPhase is null)
            return;

        bool previousPhaseHasMatches = _matches.Any(match => match.Phase == previousPhase.Value);
        bool previousPhaseIsFinished = _matches
            .Where(match => match.Phase == previousPhase.Value)
            .All(match => match.Status == MatchStatus.Finished);

        if (previousPhaseHasMatches && !previousPhaseIsFinished)
            throw new InvalidOperationException("Previous knockout phase must be finished before adding matches for the next phase.");
    }

    private static TournamentPhase? GetPreviousPhase(TournamentPhase phase) =>
        phase switch
        {
            TournamentPhase.RoundOf16 => TournamentPhase.RoundOf32,
            TournamentPhase.QuarterFinals => TournamentPhase.RoundOf16,
            TournamentPhase.SemiFinals => TournamentPhase.QuarterFinals,
            TournamentPhase.ThirdPlace => TournamentPhase.SemiFinals,
            TournamentPhase.Final => TournamentPhase.SemiFinals,
            _ => null
        };

    private static void Shuffle<T>(IList<T> items, int seed)
    {
        var random = new Random(seed);

        for (int i = items.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (items[i], items[j]) = (items[j], items[i]);
        }
    }
}
