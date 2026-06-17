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
    private static readonly DateTime KnockoutPhaseDefaultDate = new(2026, 7, 1);
    
    private const string NullQualifiedTeamsErrorMessage = "Los equipos clasificados no pueden ser nulos.";
    private const string NullMatchErrorMessage = "El partido no puede ser nulo.";
    private const string RoundOf32AlreadyInitErrorMessage = "Los dieciseisavos de final ya fueron inicializados.";
    private const string PhaseNotFinishedErrorMessage = "La fase eliminatoria anterior debe estar finalizada antes de agregar partidos para la siguiente fase.";
    private const string MatchNotFoundErrorMessage = "No se encontró un partido con el código '{0}'.";
    private const string KnockoutDrawErrorMessage = "Los partidos de knockout no pueden terminar en empate.";
    private const string QualifiedTeamsCountErrorMessage = "La fase eliminatoria debe recibir exactamente {0} equipos clasificados.";
    private const string QualifiedTeamsCountInitErrorMessage = "La fase eliminatoria requiere exactamente {0} equipos clasificados.";
    private const string NoStadiumsAvailableErrorMessage = "No hay estadios disponibles para asignar a los partidos.";

    private readonly List<Team> _qualifiedTeams = new();
    private readonly List<Match> _matches = new();
    private readonly IShuffler _shuffler;
    private readonly List<Stadium> _stadiums = new();

    private IShuffler Shuffler =>
        _shuffler ?? throw new InvalidOperationException("El servicio de sorteo no está inicializado.");
    
    public int SimulationSeed { get; private set; }
    public int CrossesSeed { get; private set; }
    public IReadOnlyList<Team> QualifiedTeams => _qualifiedTeams;
    public IReadOnlyList<Match> Matches => _matches;
    
    public Knockout(
        int simulationSeed,
        IShuffler shuffler,
        List<Stadium> stadiums,
        int crossesSeed = 0)
    {
        if (stadiums is null || !stadiums.Any())
            throw new ArgumentException(NoStadiumsAvailableErrorMessage, nameof(stadiums));
        
        if (shuffler is null)
            throw new ArgumentNullException(nameof(shuffler));
        
        SimulationSeed = simulationSeed;
        _shuffler = shuffler;
        CrossesSeed = crossesSeed;
        _stadiums.AddRange(stadiums);
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

    public void InitializeRoundOf16()
    {
        ValidateCanInitializePhase(TournamentPhase.RoundOf16);

        var roundOf16Matches = new List<Match>();

        for (int i = 1; i <= 8; i++)
        {
            var matchA = _matches.Single(m => m.MatchCode == $"A{i}");
            var matchB = _matches.Single(m => m.MatchCode == $"B{i}");

            var homeTeam = matchA.Winner ?? throw new InvalidOperationException($"El partido A{i} no tiene ganador");
            var awayTeam = matchB.Winner ?? throw new InvalidOperationException($"El partido B{i} no tiene ganador");

            var match = CreateKnockoutMatch(homeTeam, awayTeam, TournamentPhase.RoundOf16, $"C{i}");
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

            var homeTeam = matchC1.Winner ?? throw new InvalidOperationException($"El partido C{c1} no tiene ganador");
            var awayTeam = matchC2.Winner ?? throw new InvalidOperationException($"El partido C{c2} no tiene ganador");

            var match = CreateKnockoutMatch(homeTeam, awayTeam, TournamentPhase.QuarterFinals, $"D{d + 1}");
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

        var homeTeamS1 = matchD1.Winner ?? throw new InvalidOperationException("El partido D1 no tiene ganador");
        var awayTeamS1 = matchD2.Winner ?? throw new InvalidOperationException("El partido D2 no tiene ganador");

        semiFinalMatches.Add(CreateKnockoutMatch(homeTeamS1, awayTeamS1, TournamentPhase.SemiFinals, "S1"));

        var matchD3 = _matches.Single(m => m.MatchCode == "D3");
        var matchD4 = _matches.Single(m => m.MatchCode == "D4");

        var homeTeamS2 = matchD3.Winner ?? throw new InvalidOperationException("El partido D3 no tiene ganador");
        var awayTeamS2 = matchD4.Winner ?? throw new InvalidOperationException("El partido D4 no tiene ganador");

        semiFinalMatches.Add(CreateKnockoutMatch(homeTeamS2, awayTeamS2, TournamentPhase.SemiFinals, "S2"));

        AddMatches(semiFinalMatches);
    }

    public void InitializeThirdPlace()
    {
        ValidateCanInitializePhase(TournamentPhase.ThirdPlace);

        var matchS1 = _matches.Single(m => m.MatchCode == "S1");
        var matchS2 = _matches.Single(m => m.MatchCode == "S2");

        var homeTeam = matchS1.Loser ?? throw new InvalidOperationException("El partido S1 no tiene perdedor");
        var awayTeam = matchS2.Loser ?? throw new InvalidOperationException("El partido S2 no tiene perdedor");

        var thirdPlaceMatch = CreateKnockoutMatch(homeTeam, awayTeam, TournamentPhase.ThirdPlace, "T1");
        AddMatch(thirdPlaceMatch);
    }

    public void InitializeFinal()
    {
        ValidateCanInitializePhase(TournamentPhase.Final);

        var matchS1 = _matches.Single(m => m.MatchCode == "S1");
        var matchS2 = _matches.Single(m => m.MatchCode == "S2");

        var homeTeam = matchS1.Winner ?? throw new InvalidOperationException("El partido S1 no tiene ganador");
        var awayTeam = matchS2.Winner ?? throw new InvalidOperationException("El partido S2 no tiene ganador");

        var finalMatch = CreateKnockoutMatch(homeTeam, awayTeam, TournamentPhase.Final, "F1");
        AddMatch(finalMatch);
    }

    public void AddMatch(Match match)
    {
        if (match is null)
            throw new ArgumentException(NullMatchErrorMessage);

        var previousPhase = GetPreviousPhase(match.Phase);

        if (previousPhase is not null
            && _matches.Any(m => m.Phase == previousPhase.Value)
            && !PhaseIsFinished(previousPhase.Value))
        {
            throw new InvalidOperationException(PhaseNotFinishedErrorMessage);
        }

        _matches.Add(match);
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
    }
    
    public void FinishMatch(string matchCode)
    {
        var match = _matches.SingleOrDefault(m => m.MatchCode == matchCode);

        if (match is null)
            throw new ArgumentException(string.Format(MatchNotFoundErrorMessage, matchCode));

        match.Finish();
    }
    
    private void ValidateCanInitializeRoundOf32()
    {
        if (_qualifiedTeams.Count != RequiredQualifiedTeams)
            throw new InvalidOperationException(string.Format(QualifiedTeamsCountInitErrorMessage, RequiredQualifiedTeams));

        if (_matches.Any(match => match.Phase == TournamentPhase.RoundOf32))
            throw new InvalidOperationException(RoundOf32AlreadyInitErrorMessage);
    }

    private void ValidateCanInitializePhase(TournamentPhase phase)
    {
        if (_matches.Any(m => m.Phase == phase))
            throw new InvalidOperationException($"La fase {phase} ya fue inicializada.");

        var previousPhase = GetPreviousPhase(phase);

        if (previousPhase != null && !PhaseIsFinished(previousPhase.Value))
            throw new InvalidOperationException($"La fase anterior {previousPhase} debe estar finalizada antes de inicializar {phase}.");
    }

    private bool PhaseIsFinished(TournamentPhase phase)
    {
        var phaseMatches = _matches.Where(m => m.Phase == phase).ToList();
        return phaseMatches.Any() && phaseMatches.All(m => m.Status == MatchStatus.Finished);
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

    private List<Team> GetQualifiedTeamsSegment(int start, int count) =>
        _qualifiedTeams.Skip(start).Take(count).ToList();

    private IEnumerable<Match> PairRemainingSecondPlaces(List<Team> remainingSecondPlaces)
    {
        Shuffler.Shuffle(remainingSecondPlaces);

        var homePot = remainingSecondPlaces.Take(HalfRemainingSecondsCount).ToList();
        var awayPot = remainingSecondPlaces.Skip(HalfRemainingSecondsCount).Take(HalfRemainingSecondsCount).ToList();

        return PairTeams(homePot, awayPot, "B", RemainingSecondsFirstMatchNumber);
    }

    private List<Match> PairTeams(List<Team> homePot, List<Team> awayPot, string prefix, int startNumber)
    {
        Shuffler.Shuffle(homePot);
        Shuffler.Shuffle(awayPot);

        var matches = new List<Match>();

        for (int i = 0; i < homePot.Count; i++)
        {
            var homeTeam = homePot[i];
            var awayTeam = TakeFirstAvailableOpponent(homeTeam, awayPot);
            var matchCode = $"{prefix}{startNumber + i}";

            matches.Add(CreateKnockoutMatch(homeTeam, awayTeam, TournamentPhase.RoundOf32, matchCode));
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

    private Match CreateKnockoutMatch(Team homeTeam, Team awayTeam, TournamentPhase phase, string matchCode) =>
        new(KnockoutPhaseDefaultDate, PickStadium(matchCode), homeTeam, awayTeam, group: null, phase: phase, matchCode: matchCode);
    
    private Stadium PickStadium(string matchCode)
    {
        if (_stadiums.Count == 0)
            throw new InvalidOperationException(NoStadiumsAvailableErrorMessage);

        var index = Math.Abs(matchCode.GetHashCode()) % _stadiums.Count;
        return _stadiums[index];
    }
}