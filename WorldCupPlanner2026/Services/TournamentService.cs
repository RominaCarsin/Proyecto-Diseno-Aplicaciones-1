using Domain;
using DTOs;
using IRepository;
using IServices;
using Services.Mappers;

namespace Services;

public class TournamentService : ITournamentService
{
    private const int RequiredGroupCount = 12;
    private const int MinTeamsPerGroup = 3;
    private const int BestThirdsCount = 8;

    private const string InvalidTeamCountErrorMessage = "Se requieren exactamente 48 equipos para generar los bombos.";
    private const string GroupNotFoundErrorMessage = "No se encontró el grupo.";
    private const string MatchNotFoundErrorMessage = "No se encontró el partido.";
    private const string MatchNotFoundInGroupErrorMessage = "No se encontró el partido en el grupo.";
    private const string KnockoutNotInitializedErrorMessage = "La fase eliminatoria no ha sido inicializada.";
    private const string FixtureAlreadyGeneratedErrorMessage = "El fixture ya fue generado.";
    private const string NotEnoughTeamsErrorMessage = "Se necesitan 48 equipos para generar el fixture.";
    private const string NotEnoughStadiumsErrorMessage = "Se necesitan al menos 4 estadios para generar el fixture.";
    private const string MatchRequestRequiredErrorMessage = "La solicitud de partido no puede ser nula.";
    private const string StadiumRequestRequiredErrorMessage = "La solicitud de estadio no puede ser nula.";

    private const string InvalidIncidenceTypeErrorMessage = "El tipo de incidencia es inválido.";

    private readonly IRepository<Group> _groupRepository;
    private readonly IRepository<Match> _matchRepository;
    private readonly IRepository<Stadium> _stadiumRepository;
    private readonly IRepository<Team> _teamRepository;
    private readonly IRepository<Pot> _potRepository;
    private readonly IAuditService _auditService;
    private readonly ISessionService _sessionService;
    private readonly IUpdateRankingService _updateRankingService;
    private readonly INotificationService _notificationService;

    public TournamentService(
        IRepository<Group> groupRepository,
        IRepository<Match> matchRepository,
        IRepository<Stadium> stadiumRepository,
        IRepository<Team> teamRepository,
        IRepository<Pot> potRepository,
        IAuditService auditService,
        ISessionService sessionService,
        IUpdateRankingService updateRankingService,
        INotificationService notificationService)
    {
        _groupRepository = groupRepository;
        _matchRepository = matchRepository;
        _stadiumRepository = stadiumRepository;
        _teamRepository = teamRepository;
        _potRepository = potRepository;
        _auditService = auditService;
        _sessionService = sessionService;
        _updateRankingService = updateRankingService;
        _notificationService = notificationService;
    }

    // ==================== GENERACIÓN DE FIXTURE ====================

    public void GenerateFixture(int seed, DateTime startDate)
    {
        ValidateFixtureCanBeGenerated();

        GeneratePots(seed);

        var pots = _potRepository.GetAll().ToList();
        var stadiums = _stadiumRepository.GetAll()
            .OrderBy(stadium => stadium.Name)
            .ToList();

        var groupPhase = new GroupPhase(pots, stadiums, startDate);
        groupPhase.Generate();

        foreach (var group in groupPhase.Groups)
            _groupRepository.Add(group);
    }

    public void GenerateFixture(int seed) =>
        GenerateFixture(seed, new DateTime(2026, 6, 1));

    public bool CanGenerateFixture() =>
        HasEnoughTeams() && HasEnoughStadiums() && !HasGroups();

    public int GetStadiumCount() => _stadiumRepository.GetAll().Count();

    // ==================== GRUPOS ====================

    public IEnumerable<GroupResponse> GetGroups() =>
        _groupRepository.GetAll().Select(GroupMapper.FromEntity);

    public GroupResponse GetGroupByLabel(char label) =>
        GroupMapper.FromEntity(FindGroupByLabelOrThrow(label));

    public IEnumerable<TeamResponse> GetGroupStandings(char groupLabel)
    {
        var group = FindGroupByLabelOrThrow(groupLabel);
        return group.GetOrderedTeamsByPosition()
            .Select(TeamMapper.FromEntity);
    }

    public IEnumerable<TeamResponse> GetQualifiedTeams()
    {
        var groups = _groupRepository.GetAll().ToList();

        if (groups.Count != RequiredGroupCount)
            throw new InvalidOperationException("Deben existir exactamente 12 grupos para determinar clasificados.");

        if (groups.Any(g => g.Teams.Count < MinTeamsPerGroup))
            throw new InvalidOperationException("Cada grupo debe tener al menos 3 equipos para determinar 1ro, 2do y 3ro.");

        var winners = groups.Select(g => g.GetTeamByPosition(1));
        var runnersUp = groups.Select(g => g.GetTeamByPosition(2));
        var thirds = groups.Select(g => g.GetTeamByPosition(3));

        var sortedWinners = SortTeams(winners);
        var sortedRunnersUp = SortTeams(runnersUp);
        var bestThirds = SortTeams(thirds).Take(BestThirdsCount);

        return sortedWinners
            .Concat(sortedRunnersUp)
            .Concat(bestThirds)
            .Select(TeamMapper.FromEntity);
    }

    // ==================== PARTIDOS ====================

    public IEnumerable<MatchResponse> GetMatches()
    {
        var allMatches = _matchRepository.GetAll().ToList();
        var knockoutExists = allMatches.Any(m => m.Phase != TournamentPhase.GroupStage);

        return allMatches.Select(m =>
            MatchMapper.ToResponse(m, isLocked: knockoutExists && m.Phase == TournamentPhase.GroupStage));
    }

    public MatchResponse GetMatchById(int matchId)
    {
        var match = FindMatchByIdOrThrow(matchId);
        return MatchMapper.ToResponse(match);
    }

    // ==================== RESULTADOS DE GRUPO ====================

    public void SetGroupMatchResult(char groupLabel, int matchId, int homeGoals, int awayGoals)
    {
        var group = FindGroupByLabelOrThrow(groupLabel);
        var match = FindMatchInGroupOrThrow(group, matchId);

        group.SetMatchResult(match, homeGoals, awayGoals);

        _groupRepository.Update(group);
        _matchRepository.Update(match);

        _notificationService.NotifyMatchResult(
            matchId, match.HomeTeam.Name, match.AwayTeam.Name, homeGoals, awayGoals);
    }

    public void FinishGroupMatch(char groupLabel, int matchId)
    {
        var group = FindGroupByLabelOrThrow(groupLabel);
        var match = FindMatchInGroupOrThrow(group, matchId);

        group.FinishMatch(match);
        _updateRankingService.UpdateAfterMatch(match);

        _groupRepository.Update(group);
        _matchRepository.Update(match);
    }

    // ==================== SIMULACIÓN DE GRUPOS ====================

    public void SimulateGroupStage(int seed) =>
        SimulateGroupStage(seed, SimulationEngineTypeDto.Probabilistic);

    public void SimulateGroupStage(int seed, SimulationEngineTypeDto simulationEngineType)
    {
        var groups = _groupRepository.GetAll().ToList();

        if (!groups.Any())
            throw new InvalidOperationException("No se ha generado la fase de grupos.");

        var domainEngineType = SimulationEngineTypeMapper.ToDomain(simulationEngineType);
        var simulationEngine = SimulationEngineFactory.Create(domainEngineType);

        var simulator = new GroupStageSimulator(seed, simulationEngine);
        simulator.Simulate(groups);

        foreach (var group in groups)
            _groupRepository.Update(group);

        LogAction(
            "Simulación de fase de grupos ejecutada",
            $"Simulación de fase de grupos ejecutada con semilla {seed} usando {simulationEngine.Name}");

        var matchCount = groups.SelectMany(g => g.Matches).Count();
        _notificationService.NotifySimulationCompleted("Fase de grupos", matchCount);
    }

    // ==================== FASE ELIMINATORIA ====================

    public void InitializeKnockoutStage(int simulationSeed, int crossesSeed = 0)
    {
        var groups = _groupRepository.GetAll().ToList();

        if (!groups.Any())
            throw new InvalidOperationException("No se ha generado la fase de grupos.");

        var allMatchesHaveResults = groups
            .SelectMany(g => g.Matches)
            .All(m => m.HasResult);

        if (!allMatchesHaveResults)
            throw new InvalidOperationException("La fase de grupos debe estar simulada.");

        if (_matchRepository.GetAll().Any(m => m.Phase != TournamentPhase.GroupStage))
            throw new InvalidOperationException("La fase eliminatoria ya fue inicializada.");

        var qualifiedTeamResponses = GetQualifiedTeams().ToList();
        var qualifiedTeamNames = qualifiedTeamResponses.Select(t => t.Name).ToHashSet();
        var qualifiedTeams = _teamRepository.GetAll()
            .Where(t => qualifiedTeamNames.Contains(t.Name))
            .ToList();

        // Obtener los estadios del repositorio
        var stadiums = _stadiumRepository.GetAll().ToList();

        var shuffler = new Shuffler(simulationSeed);
        var knockout = new Knockout(simulationSeed, shuffler, stadiums, crossesSeed);

        knockout.SetQualifiedTeams(qualifiedTeams);
        knockout.InitializeRoundOf32();

        foreach (var match in knockout.Matches)
            _matchRepository.Add(match);
    }

    public void SetKnockoutMatchResult(string matchCode, int homeGoals, int awayGoals)
    {
        var allMatches = _matchRepository.GetAll().ToList();
        var stadiums = _stadiumRepository.GetAll().ToList();
        var knockout = CreateKnockoutFromExistingMatches(allMatches, stadiums);

        knockout.SetMatchResult(matchCode, homeGoals, awayGoals);

        var updatedMatch = allMatches.Single(m => m.MatchCode == matchCode);
        _matchRepository.Update(updatedMatch);

        _notificationService.NotifyMatchResult(
            updatedMatch.MatchId, updatedMatch.HomeTeam.Name, updatedMatch.AwayTeam.Name,
            homeGoals, awayGoals);
    }

    public void FinishKnockoutMatch(string matchCode)
    {
        var match = FindKnockoutMatchByCodeOrThrow(matchCode);
        FinishAndRecalculate(match);
        _matchRepository.Update(match);

        LogAction(
            "Partido de eliminación finalizado",
            $"Partido de eliminación {matchCode} marcado como finalizado");
    }

    public void FinishAllKnockoutMatches()
    {
        var knockoutMatches = _matchRepository.GetAll()
            .Where(m => m.Phase != TournamentPhase.GroupStage)
            .ToList();

        if (!knockoutMatches.Any())
            throw new InvalidOperationException(KnockoutNotInitializedErrorMessage);

        var matchesToFinish = knockoutMatches
            .Where(m => m.HasResult && m.Status != MatchStatus.Finished)
            .ToList();

        foreach (var match in matchesToFinish)
        {
            FinishAndRecalculate(match);
            _matchRepository.Update(match);
        }

        LogAction(
            "Todos los partidos de eliminación finalizados",
            $"{matchesToFinish.Count} partidos de eliminación finalizados y rankings recalculados");
    }

    public IEnumerable<MatchResponse> GetKnockoutMatches() =>
        _matchRepository.GetAll()
            .Where(m => m.Phase != TournamentPhase.GroupStage)
            .Select(m => MatchMapper.ToResponse(m))
            .ToList();

    // ==================== INICIALIZACIÓN DE RONDAS ====================

    public void InitializeRoundOf16()
    {
        var knockout = GetLoadedKnockout();
        knockout.InitializeRoundOf16();
        AddNewMatchesByPhase(knockout, TournamentPhase.RoundOf16);
    }

    public void InitializeQuarterFinals()
    {
        var knockout = GetLoadedKnockout();
        knockout.InitializeQuarterFinals();
        AddNewMatchesByPhase(knockout, TournamentPhase.QuarterFinals);
    }

    public void InitializeSemiFinals()
    {
        var knockout = GetLoadedKnockout();
        knockout.InitializeSemiFinals();
        AddNewMatchesByPhase(knockout, TournamentPhase.SemiFinals);
    }

    public void InitializeThirdPlace()
    {
        var knockout = GetLoadedKnockout();
        knockout.InitializeThirdPlace();
        AddNewMatchesByPhase(knockout, TournamentPhase.ThirdPlace);
    }

    public void InitializeFinal()
    {
        var knockout = GetLoadedKnockout();
        knockout.InitializeFinal();
        AddNewMatchesByPhase(knockout, TournamentPhase.Final);
    }

    // ==================== SIMULACIÓN DE ELIMINATORIA ====================

    public void SimulateKnockoutStage(int seed) =>
        SimulateKnockoutStage(seed, SimulationEngineTypeDto.Probabilistic);

    public void SimulateKnockoutStage(int seed, SimulationEngineTypeDto simulationEngineType)
    {
        var knockoutMatches = _matchRepository.GetAll()
            .Where(m => m.Phase != TournamentPhase.GroupStage)
            .ToList();

        if (!knockoutMatches.Any())
            throw new InvalidOperationException(KnockoutNotInitializedErrorMessage);

        var domainEngineType = SimulationEngineTypeMapper.ToDomain(simulationEngineType);
        var simulationEngine = SimulationEngineFactory.Create(domainEngineType);

        var simulator = new KnockoutSimulator(seed, simulationEngine);
        simulator.Simulate(knockoutMatches);

        foreach (var match in knockoutMatches)
            _matchRepository.Update(match);

        LogAction(
            "Simulación de fase eliminatoria ejecutada",
            $"Simulación de fase eliminatoria ejecutada con semilla {seed} usando {simulationEngine.Name}");

        _notificationService.NotifySimulationCompleted("Fase eliminatoria", knockoutMatches.Count);
    }

    // ==================== MÉTODOS AUXILIARES PRIVADOS ====================

    private void ValidateFixtureCanBeGenerated()
    {
        if (!HasEnoughTeams())
            throw new InvalidOperationException(NotEnoughTeamsErrorMessage);

        if (!HasEnoughStadiums())
            throw new InvalidOperationException(NotEnoughStadiumsErrorMessage);

        if (HasGroups())
            throw new InvalidOperationException(FixtureAlreadyGeneratedErrorMessage);
    }

    private bool HasEnoughTeams() =>
        _teamRepository.GetAll().Count() >= GroupPhase.RequiredTeamCount;

    private bool HasEnoughStadiums() =>
        _stadiumRepository.GetAll().Count() >= GroupPhase.MinStadiumCount;

    private bool HasGroups() =>
        _groupRepository.GetAll().Any();

    private void GeneratePots(int seed)
    {
        var teams = _teamRepository.GetAll().ToList();

        if (teams.Count != GroupPhase.RequiredTeamCount)
            throw new ArgumentException(InvalidTeamCountErrorMessage);

        var sortedTeams = SortByFifaRankingDescending(teams);
        new PotDraw().ShuffleByRanking(sortedTeams, seed);

        for (int potNumber = Pot.MinNumber; potNumber <= Pot.MaxNumber; potNumber++)
        {
            var potTeams = sortedTeams
                .Skip((potNumber - 1) * Pot.MaxTeams)
                .Take(Pot.MaxTeams)
                .ToList();

            _potRepository.Add(new Pot(potNumber, potTeams));
        }
    }

    private static List<Team> SortByFifaRankingDescending(IEnumerable<Team> teams) =>
        teams.OrderByDescending(t => t.RankingFifa).ToList();

    private Group FindGroupByLabelOrThrow(char label) =>
        _groupRepository.GetAll()
            .FirstOrDefault(g => g.Label == label)
            ?? throw new ArgumentException(GroupNotFoundErrorMessage);

    private Match FindMatchByIdOrThrow(int matchId) =>
        _matchRepository.GetAll()
            .FirstOrDefault(m => m.MatchId == matchId)
            ?? throw new ArgumentException(MatchNotFoundErrorMessage);

    private static Match FindMatchInGroupOrThrow(Group group, int matchId) =>
        group.Matches
            .FirstOrDefault(m => m.MatchId == matchId)
            ?? throw new ArgumentException(MatchNotFoundInGroupErrorMessage);

    private static IEnumerable<Team> SortTeams(IEnumerable<Team> teams) =>
        teams.OrderByDescending(t => t.Points)
            .ThenByDescending(t => t.GoalDifference)
            .ThenByDescending(t => t.GoalsInFavor);

    private Knockout GetLoadedKnockout()
    {
        var allMatches = _matchRepository.GetAll().ToList();

        if (!allMatches.Any(m => m.Phase != TournamentPhase.GroupStage))
            throw new InvalidOperationException(KnockoutNotInitializedErrorMessage);

        var stadiums = _stadiumRepository.GetAll().ToList();
        return CreateKnockoutFromExistingMatches(allMatches, stadiums);
    }

    private Knockout CreateKnockoutFromExistingMatches(IEnumerable<Match> matches, IEnumerable<Stadium> stadiums)
    {
        var shuffler = new Shuffler(0);
        var knockout = new Knockout(0, shuffler, stadiums.ToList());
        knockout.LoadExistingMatches(matches.ToList());
        return knockout;
    }

    private void AddNewMatchesByPhase(Knockout knockout, TournamentPhase phase)
    {
        var existingMatchCodes = _matchRepository.GetAll()
            .Select(m => m.MatchCode)
            .ToHashSet();

        var newMatches = knockout.Matches
            .Where(m => m.Phase == phase)
            .Where(m => !existingMatchCodes.Contains(m.MatchCode))
            .ToList();

        foreach (var match in newMatches)
            _matchRepository.Add(match);
    }

    private Match FindKnockoutMatchByCodeOrThrow(string matchCode) =>
        _matchRepository.GetAll()
            .FirstOrDefault(m => m.Phase != TournamentPhase.GroupStage && m.MatchCode == matchCode)
            ?? throw new ArgumentException(MatchNotFoundErrorMessage);

    private void FinishAndRecalculate(Match match)
    {
        match.Finish();
        _updateRankingService.UpdateAfterMatch(match);
    }

    // ==================== MÉTODOS PÚBLICOS ADICIONALES (CRUD) ====================

    public void AddMatch(MatchRequest matchRequest)
    {
        ValidateNotNull(matchRequest, MatchRequestRequiredErrorMessage);

        var match = MatchMapper.ToEntity(matchRequest);

        ValidateStadiumAvailability(null, match.Stadium, match.Date);

        _matchRepository.Add(match);

        if (matchRequest.Group.HasValue)
            AddMatchToGroup(match, matchRequest.Group.Value);

        LogAction(
            "Partido creado",
            $"Partido agregado: {match.HomeTeam.Name} vs {match.AwayTeam.Name} en {match.Stadium.Name} el {match.Date}");
    }

    public void SetMatchResult(int matchId, int homeGoals, int awayGoals)
    {
        var match = FindMatchByIdOrThrow(matchId);

        match.SetResult(homeGoals, awayGoals);

        _matchRepository.Update(match);

        _notificationService.NotifyMatchResult(
            matchId, match.HomeTeam.Name, match.AwayTeam.Name, homeGoals, awayGoals);

        LogAction(
            "Resultado de partido establecido",
            $"Resultado del partido {matchId} ({match.HomeTeam.Name} vs {match.AwayTeam.Name}) establecido a {homeGoals}-{awayGoals}");
    }

    public void UpdateMatchDate(int matchId, DateTime newDate)
    {
        var match = FindMatchByIdOrThrow(matchId);

        ValidateStadiumAvailability(matchId, match.Stadium, newDate);

        var oldDate = match.Date;

        match.UpdateDate(newDate);

        _matchRepository.Update(match);

        LogAction(
            "Fecha de partido actualizada",
            $"Fecha del partido {matchId} cambiada de {oldDate} a {newDate}");
    }

    public void UpdateMatchStadium(int matchId, StadiumRequest newStadiumRequest)
    {
        ValidateNotNull(newStadiumRequest, StadiumRequestRequiredErrorMessage);

        var match = FindMatchByIdOrThrow(matchId);
        var stadium = StadiumMapper.ToEntity(newStadiumRequest);

        ValidateStadiumAvailability(matchId, stadium, match.Date);

        var oldStadiumName = match.Stadium.Name;

        match.UpdateStadium(stadium);

        _matchRepository.Update(match);

        LogAction(
            "Estadio de partido actualizado",
            $"Estadio del partido {matchId} cambiado de {oldStadiumName} a {stadium.Name}");
    }

    public void FinishMatch(int matchId)
    {
        var match = FindMatchByIdOrThrow(matchId);

        FinishAndRecalculate(match);

        _matchRepository.Update(match);

        LogAction("Partido finalizado", $"Partido {matchId} marcado como finalizado");
    }

    public IEnumerable<StadiumRequest> GetAvailableStadiumsForMatch(
        int matchId,
        IEnumerable<StadiumRequest> candidateStadiums)
    {
        var match = FindMatchByIdOrThrow(matchId);

        return candidateStadiums.Where(stadiumRequest =>
        {
            try
            {
                var stadium = StadiumMapper.ToEntity(stadiumRequest);
                ValidateStadiumAvailability(matchId, stadium, match.Date);
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        });
    }

    public void RegisterMatchIncidence(int matchId, string incidenceType, bool isHomeTeam, int minute)
    {
        var match = FindMatchByIdOrThrow(matchId);
        var team = isHomeTeam ? match.HomeTeam : match.AwayTeam;

        var incidence = BuildIncidence(incidenceType, team, minute);
        match.RegisterIncidence(incidence);

        _matchRepository.Update(match);
    }

    public void FinishAllMatches()
    {
        var finishedCount = 0;

        // Finalizar partidos de grupo
        foreach (var group in _groupRepository.GetAll())
        {
            var finishedGroupMatches = group.FinishPlayedMatches().ToList();

            foreach (var match in finishedGroupMatches)
                _updateRankingService.UpdateAfterMatch(match);

            if (finishedGroupMatches.Any())
                _groupRepository.Update(group);

            finishedCount += finishedGroupMatches.Count;
        }

        // Finalizar partidos sueltos (eliminatoria, etc.)
        var standaloneMatches = _matchRepository.GetAll()
            .Where(m => m.HasResult && m.Status != MatchStatus.Finished)
            .ToList();

        foreach (var match in standaloneMatches)
        {
            FinishAndRecalculate(match);
            _matchRepository.Update(match);
        }

        finishedCount += standaloneMatches.Count;

        LogAction("Todos los partidos finalizados", $"{finishedCount} partidos finalizados y rankings recalculados");
    }

    // ==================== MÉTODOS PRIVADOS DE APOYO ====================

    private void AddMatchToGroup(Match match, char groupLabel)
    {
        var group = _groupRepository.GetAll()
            .FirstOrDefault(g => g.Label == groupLabel)
            ?? throw new ArgumentException(GroupNotFoundErrorMessage);

        group.AddMatch(match);
        _groupRepository.Update(group);
    }

    private void ValidateStadiumAvailability(int? excludeMatchId, Stadium stadium, DateTime matchDate)
    {
        var scheduledMatches = _matchRepository.GetAll()
            .Where(m => excludeMatchId == null || m.MatchId != excludeMatchId.Value);

        StadiumAvailability.Validate(scheduledMatches, stadium, matchDate);
    }

    private static void ValidateNotNull<T>(T? value, string errorMessage)
    {
        if (value is null)
            throw new ArgumentException(errorMessage);
    }

    private static MatchIncidence BuildIncidence(string incidenceType, Team team, int minute) =>
        incidenceType switch
        {
            "Yellow" => new YellowCard(team, minute),
            "Red" => new RedCard(team, minute),
            _ => throw new ArgumentException(InvalidIncidenceTypeErrorMessage)
        };

    private void LogAction(string action, string details)
    {
        var actorEmail = _sessionService.GetCurrentUserEmail() ?? "system";
        _auditService.Log(actorEmail, action, details);
    }
}