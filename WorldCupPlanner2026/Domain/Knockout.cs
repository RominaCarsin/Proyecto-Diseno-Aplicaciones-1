using System.ComponentModel.DataAnnotations;

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

    private const string NullQualifiedTeamsErrorMessage = "Qualified teams cannot be null.";
    private const string NullMatchErrorMessage = "Match cannot be null.";
    private const string RoundOf32AlreadyInitErrorMessage = "Round of 32 has already been initialized.";
    private const string PhaseNotFinishedErrorMessage = "Previous knockout phase must be finished before adding matches for the next phase.";
    private const string MatchNotFoundErrorMessage = "No se encontró un partido con el código '{0}'.";
    private const string KnockoutDrawErrorMessage = "Los partidos de knockout no pueden terminar en empate.";
    private const string QualifiedTeamsCountErrorMessage = "Knockout phase must receive exactly {0} qualified teams.";
    private const string QualifiedTeamsCountInitErrorMessage = "Knockout phase requires exactly {0} qualified teams.";

    private static readonly DateTime RoundOf32DefaultDate = new(2026, 7, 1);
    private static readonly DateTime KnockoutPhaseDefaultDate = new(2026, 7, 1);

    private readonly List<Team> _qualifiedTeams = new();
    private readonly List<Match> _matches = new();
    private readonly IShuffleService _shuffleService;
    
    public int KnockoutId { get; private set; }
    public int SimulationSeed { get; private set; }
    public int SemillaCrucesFase { get; private set; }
    public IReadOnlyList<Team> QualifiedTeams => _qualifiedTeams;
    public IReadOnlyList<Match> Matches => _matches;

    private Knockout() { }
    public Knockout(int simulationSeed, IShuffleService shuffleService, int semillaCrucesFase = 0)
    {
        SimulationSeed = simulationSeed;
        _shuffleService = shuffleService;
        SemillaCrucesFase = semillaCrucesFase;
    }

    public void UpdateSimulationSeed(int newSeed) => SimulationSeed = newSeed;

    public void SetQualifiedTeams(IEnumerable<Team> qualifiedTeams)
    {
        if (qualifiedTeams is null)
            throw new ArgumentException(NullQualifiedTeamsErrorMessage);

        var teams = qualifiedTeams.ToList();

        if (teams.Count != RequiredQualifiedTeams)
            throw new ArgumentException(string.Format(QualifiedTeamsCountErrorMessage, RequiredQualifiedTeams));

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
            throw new ArgumentException(NullMatchErrorMessage);

        ValidatePreviousPhaseFinished(match.Phase);
        _matches.Add(match);
    }

    private void ValidateCanInitializeRoundOf32()
    {
        if (_qualifiedTeams.Count != RequiredQualifiedTeams)
            throw new InvalidOperationException(string.Format(QualifiedTeamsCountInitErrorMessage, RequiredQualifiedTeams));

        if (_matches.Any(match => match.Phase == TournamentPhase.RoundOf32))
            throw new InvalidOperationException(RoundOf32AlreadyInitErrorMessage);
    }

    private List<Team> GetQualifiedTeamsSegment(int start, int count) =>
        _qualifiedTeams
            .Skip(start)
            .Take(count)
            .ToList();

    private IEnumerable<Match> PairRemainingSecondPlaces(List<Team> remainingSecondPlaces)
    {
        _shuffleService.Shuffle(remainingSecondPlaces);

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
        _shuffleService.Shuffle(homePot);
        _shuffleService.Shuffle(awayPot);

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
            throw new InvalidOperationException(PhaseNotFinishedErrorMessage);
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



    public void InitializeRoundOf16()
    {
        ValidateCanInitializePhase(TournamentPhase.RoundOf16);

        var roundOf16Matches = new List<Match>();

        for (int i = 1; i <= 8; i++)
        {
            var matchA = _matches.Single(m => m.MatchCode == $"A{i}");
            var matchB = _matches.Single(m => m.MatchCode == $"B{i}");

            var homeTeam = matchA.Winner ?? throw new InvalidOperationException($"Match A{i} has no winner");
            var awayTeam = matchB.Winner ?? throw new InvalidOperationException($"Match B{i} has no winner");

            var match = CreateKnockoutMatch(
                homeTeam,
                awayTeam,
                $"C{i}",
                TournamentPhase.RoundOf16);

            roundOf16Matches.Add(match);
        }

        AddMatches(roundOf16Matches);
    }

    public void InitializeQuarterFinals()
    {
        ValidateCanInitializePhase(TournamentPhase.QuarterFinals);

        var pairings = new[] { (1, 2), (3, 4), (5, 6), (7, 8) };
        var quarterFinalMatches = new List<Match>();

        for (int d = 0; d < pairings.Length; d++)
        {
            var (c1, c2) = pairings[d];
            var matchC1 = _matches.Single(m => m.MatchCode == $"C{c1}");
            var matchC2 = _matches.Single(m => m.MatchCode == $"C{c2}");

            var homeTeam = matchC1.Winner ?? throw new InvalidOperationException($"Match C{c1} has no winner");
            var awayTeam = matchC2.Winner ?? throw new InvalidOperationException($"Match C{c2} has no winner");

            var match = CreateKnockoutMatch(
                homeTeam,
                awayTeam,
                $"D{d + 1}",
                TournamentPhase.QuarterFinals);

            quarterFinalMatches.Add(match);
        }

        AddMatches(quarterFinalMatches);
    }

    public void InitializeSemiFinals()
    {
        ValidateCanInitializePhase(TournamentPhase.SemiFinals);

        var semiFinalMatches = new List<Match>();

        var matchD1 = _matches.Single(m => m.MatchCode == "D1");
        var matchD2 = _matches.Single(m => m.MatchCode == "D2");

        var homeTeamS1 = matchD1.Winner ?? throw new InvalidOperationException("Match D1 has no winner");
        var awayTeamS1 = matchD2.Winner ?? throw new InvalidOperationException("Match D2 has no winner");

        semiFinalMatches.Add(CreateKnockoutMatch(homeTeamS1, awayTeamS1, "S1", TournamentPhase.SemiFinals));

        var matchD3 = _matches.Single(m => m.MatchCode == "D3");
        var matchD4 = _matches.Single(m => m.MatchCode == "D4");

        var homeTeamS2 = matchD3.Winner ?? throw new InvalidOperationException("Match D3 has no winner");
        var awayTeamS2 = matchD4.Winner ?? throw new InvalidOperationException("Match D4 has no winner");

        semiFinalMatches.Add(CreateKnockoutMatch(homeTeamS2, awayTeamS2, "S2", TournamentPhase.SemiFinals));

        AddMatches(semiFinalMatches);
    }

    public void InitializeThirdPlace()
    {
        ValidateCanInitializePhase(TournamentPhase.ThirdPlace);

        var matchS1 = _matches.Single(m => m.MatchCode == "S1");
        var matchS2 = _matches.Single(m => m.MatchCode == "S2");

        var homeTeam = matchS1.Loser ?? throw new InvalidOperationException("Match S1 has no loser");
        var awayTeam = matchS2.Loser ?? throw new InvalidOperationException("Match S2 has no loser");

        var thirdPlaceMatch = CreateKnockoutMatch(homeTeam, awayTeam, "T1", TournamentPhase.ThirdPlace);

        AddMatch(thirdPlaceMatch);
    }

    public void InitializeFinal()
    {
        ValidateCanInitializePhase(TournamentPhase.Final);

        var matchS1 = _matches.Single(m => m.MatchCode == "S1");
        var matchS2 = _matches.Single(m => m.MatchCode == "S2");

        var homeTeam = matchS1.Winner ?? throw new InvalidOperationException("Match S1 has no winner");
        var awayTeam = matchS2.Winner ?? throw new InvalidOperationException("Match S2 has no winner");

        var finalMatch = CreateKnockoutMatch(homeTeam, awayTeam, "F1", TournamentPhase.Final);

        AddMatch(finalMatch);
    }

    private void ValidateCanInitializePhase(TournamentPhase phase)
    {
        if (_matches.Any(m => m.Phase == phase))
            throw new InvalidOperationException($"{phase} has already been initialized.");

        var previousPhase = GetPreviousPhase(phase);

        if (previousPhase != null && !PhaseIsFinished(previousPhase.Value))
            throw new InvalidOperationException($"Previous phase {previousPhase} must be finished before initializing {phase}.");
    }

    private bool PhaseIsFinished(TournamentPhase phase)
    {
        var phaseMatches = _matches.Where(m => m.Phase == phase).ToList();
        return phaseMatches.Any() && phaseMatches.All(m => m.Status == MatchStatus.Finished);
    }

    public void LoadExistingMatches(IEnumerable<Match> matches)
    {
        _matches.Clear();
        _matches.AddRange(matches);
    }

    public void SetMatchResult(string matchCode, int homeGoals, int awayGoals)
    {
        var match = _matches.SingleOrDefault(m => m.MatchCode == matchCode);

        if (match is null)
            throw new ArgumentException(string.Format(MatchNotFoundErrorMessage, matchCode));

        if (homeGoals == awayGoals)
            throw new ArgumentException(KnockoutDrawErrorMessage);

        match.SetResult(homeGoals, awayGoals);
        match.Finish();
    }

    public void SimulateMatches(int seed)
    {
        var random = new Random(seed);

        foreach (var match in _matches)
        {
            if (match.Status == MatchStatus.Finished)
                continue;

            var (homeGoals, awayGoals) = SimulateKnockoutResult(
                match.HomeTeam.RankingFifa,
                match.AwayTeam.RankingFifa,
                random);

            match.SetResult(homeGoals, awayGoals);
            match.Finish();
        }
    }

    private static (int homeGoals, int awayGoals) SimulateKnockoutResult(
        int homeRanking, int awayRanking, Random random)
    {
        int total = homeRanking + awayRanking;
        double homeProbability = (double)homeRanking / total;

        if (random.NextDouble() < homeProbability)
            return (SimulateGoals(random, 2, 3), SimulateGoals(random, 0, 1));
        else
            return (SimulateGoals(random, 0, 1), SimulateGoals(random, 2, 3));
    }

    private static int SimulateGoals(Random random, int min, int max) =>
        random.Next(min, max + 1);

    private static Match CreateKnockoutMatch(Team homeTeam, Team awayTeam, string matchCode, TournamentPhase phase) =>
        new(KnockoutPhaseDefaultDate, BuildDefaultStadium(), homeTeam, awayTeam, group: null, phase: phase, matchCode: matchCode);
}
