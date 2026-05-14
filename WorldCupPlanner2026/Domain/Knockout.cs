namespace Domain;

public class Knockout
{
    private const int RequiredQualifiedTeams = 32;
    private const int BestFirstPlacesStart = 0;
    private const int BestFirstPlacesCount = 4;
    private const int WorstFirstPlacesStart = 4;
    private const int WorstFirstPlacesCount = 8;
    private const int BestSecondPlacesStart = 12;
    private const int BestSecondPlacesCount = 8;
    private const int WorstSecondPlacesStart = 20;
    private const int WorstSecondPlacesCount = 4;
    private const int ThirdPlacesStart = 24;
    private const int ThirdPlacesCount = 8;
    private const int HalfRemainingSecondsCount = 4;
    private const int FirstMatchNumber = 1;
    private const int RemainingSecondsFirstMatchNumber = 5;

    private static readonly DateTime RoundOf32DefaultDate = new(2026, 7, 1);

    private readonly List<Team> _qualifiedTeams = new();
    private readonly List<Match> _matches = new();

    public int SimulationSeed { get; private set; }
    public int SemillaCrucesFase { get; private set; }
    public IReadOnlyList<Team> QualifiedTeams => _qualifiedTeams;
    public IReadOnlyList<Match> Matches => _matches;

    public Knockout(int simulationSeed, int semillaCrucesFase = 0)
    {
        SimulationSeed = simulationSeed;
        SemillaCrucesFase = semillaCrucesFase;
    }

    public void UpdateSimulationSeed(int newSeed)
    {
        SimulationSeed = newSeed;
    }

    public void UpdateSemillaCrucesFase(int newSeed)
    {
        SemillaCrucesFase = newSeed;
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
        ValidateCanInitializeRoundOf32();

        var worstFirstPlaces = GetQualifiedTeamsSegment(WorstFirstPlacesStart, WorstFirstPlacesCount);
        var thirdPlaces = GetQualifiedTeamsSegment(ThirdPlacesStart, ThirdPlacesCount);
        var bestFirstPlaces = GetQualifiedTeamsSegment(BestFirstPlacesStart, BestFirstPlacesCount);
        var worstSecondPlaces = GetQualifiedTeamsSegment(WorstSecondPlacesStart, WorstSecondPlacesCount);
        var bestSecondPlaces = GetQualifiedTeamsSegment(BestSecondPlacesStart, BestSecondPlacesCount);

        AddMatches(PairTeams(worstFirstPlaces, thirdPlaces, "A", FirstMatchNumber));
        AddMatches(PairTeams(bestFirstPlaces, worstSecondPlaces, "B", FirstMatchNumber));
        AddMatches(PairRemainingSecondPlaces(bestSecondPlaces));
    }

    public void AddMatch(Match match)
    {
        if (match is null)
            throw new ArgumentException("Match cannot be null.");

        ValidatePreviousPhaseFinished(match.Phase);

        _matches.Add(match);
    }

    private void ValidateCanInitializeRoundOf32()
    {
        if (_qualifiedTeams.Count != RequiredQualifiedTeams)
            throw new InvalidOperationException($"Knockout phase requires exactly {RequiredQualifiedTeams} qualified teams.");

        if (_matches.Any(match => match.Phase == TournamentPhase.RoundOf32))
            throw new InvalidOperationException("Round of 32 has already been initialized.");
    }

    private List<Team> GetQualifiedTeamsSegment(int start, int count) =>
        _qualifiedTeams
            .Skip(start)
            .Take(count)
            .ToList();

    private IEnumerable<Match> PairRemainingSecondPlaces(List<Team> remainingSecondPlaces)
    {
        Shuffle(remainingSecondPlaces, SemillaCrucesFase);

        var homePot = remainingSecondPlaces
            .Take(HalfRemainingSecondsCount)
            .ToList();

        var awayPot = remainingSecondPlaces
            .Skip(HalfRemainingSecondsCount)
            .Take(HalfRemainingSecondsCount)
            .ToList();

        return PairTeams(homePot, awayPot, "B", RemainingSecondsFirstMatchNumber);
    }

    private List<Match> PairTeams(List<Team> homePot, List<Team> awayPot, string prefix, int startNumber)
    {
        Shuffle(homePot, SemillaCrucesFase);
        Shuffle(awayPot, SemillaCrucesFase);

        var matches = new List<Match>();

        for (int i = 0; i < homePot.Count; i++)
        {
            var homeTeam = homePot[i];
            var awayTeam = TakeFirstAvailableOpponent(homeTeam, awayPot);
            var matchCode = $"{prefix}{startNumber + i}";

            matches.Add(CreateRoundOf32Match(homeTeam, awayTeam, matchCode));
        }

        return matches;
    }

    private static Team TakeFirstAvailableOpponent(Team homeTeam, List<Team> awayPot)
    {
        int opponentIndex = awayPot.FindIndex(awayTeam => !AreFromSameGroup(homeTeam, awayTeam));

        if (opponentIndex < 0)
            opponentIndex = 0;

        var opponent = awayPot[opponentIndex];
        awayPot.RemoveAt(opponentIndex);

        return opponent;
    }

    private static bool AreFromSameGroup(Team homeTeam, Team awayTeam) =>
        !string.IsNullOrWhiteSpace(homeTeam.GroupLabel)
        && string.Equals(homeTeam.GroupLabel, awayTeam.GroupLabel, StringComparison.OrdinalIgnoreCase);

    private void AddMatches(IEnumerable<Match> matches)
    {
        foreach (var match in matches)
            AddMatch(match);
    }

    private static Match CreateRoundOf32Match(Team homeTeam, Team awayTeam, string matchCode) =>
        new(
            RoundOf32DefaultDate,
            BuildDefaultStadium(),
            homeTeam,
            awayTeam,
            group: null,
            phase: TournamentPhase.RoundOf32,
            matchCode: matchCode);

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

    private static void Shuffle<T>(List<T> list, int seed)
    {
        var random = new Random(seed);

        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
