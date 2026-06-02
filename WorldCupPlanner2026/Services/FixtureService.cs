using Domain;
using DTOs;
using IRepository;
using IServices;
using Services.Mappers;

namespace Services;

public class FixtureService : IFixtureService
{
    private readonly ITeamService _teamService;
    private readonly IStadiumService _stadiumService;
    private readonly IGroupService _groupService;
    private readonly IRepository<Group> _groupRepository;
    private readonly IRepository<Match> _matchRepository;
    private readonly IRepository<Stadium> _stadiumRepository;
    private readonly IRepository<Team> _teamRepository;
    private readonly IPotService _potService;
    private Knockout? _knockout;

    public FixtureService(
        ITeamService teamService,
        IStadiumService stadiumService,
        IGroupService groupService,
        IRepository<Group> groupRepository,
        IRepository<Match> matchRepository,
        IRepository<Stadium> stadiumRepository,
        IRepository<Team> teamRepository,
        IPotService potService)
    {
        _teamService = teamService;
        _stadiumService = stadiumService;
        _groupService = groupService;
        _groupRepository = groupRepository;
        _matchRepository = matchRepository;
        _stadiumRepository = stadiumRepository;
        _teamRepository = teamRepository;
        _potService = potService;
    }

    public void GenerateFixture(int seed, DateTime startDate)
    {
        ValidateFixtureCanBeGenerated();

        _potService.GeneratePots(seed);
        var pots = _potService.GetPots().ToList();
        var stadiums = _stadiumRepository.GetAll().OrderBy(s => s.Name).ToList();

        var fixture = new Fixture(pots, stadiums, startDate);
        fixture.Generate();

        foreach (var group in fixture.Groups)
            _groupRepository.Add(group);
    }

    public void GenerateFixture(int seed) =>
        GenerateFixture(seed, new DateTime(2026, 6, 1));

    public void SimulateGroupStage(int seed)
    {
        var groups = _groupRepository.GetAll().ToList();

        var fixture = Fixture.FromExistingGroups(groups);
        fixture.SimulateGroupStage(seed);

        foreach (var group in fixture.Groups)
            _groupRepository.Update(group);
    }

    public bool CanGenerateFixture()
    {
        var teamCount = _teamService.GetTeams().Count();
        var stadiumCount = _stadiumService.GetAllStadiums().Count();
        var groupCount = _groupService.GetGroups().Count();

        return teamCount >= Fixture.RequiredTeamCount
            && stadiumCount >= Fixture.MinStadiumCount
            && groupCount == 0;
    }

    public int GetStadiumCount() => _stadiumService.GetAllStadiums().Count();

    private void ValidateFixtureCanBeGenerated()
    {
        if (_teamService.GetTeams().Count() < Fixture.RequiredTeamCount)
            throw new InvalidOperationException("Se necesitan 48 equipos para generar el fixture.");

        if (_stadiumService.GetAllStadiums().Count() < Fixture.MinStadiumCount)
            throw new InvalidOperationException("Se necesitan al menos 4 estadios para generar el fixture.");

        if (_groupService.GetGroups().Any())
            throw new InvalidOperationException("El fixture ya fue generado.");
    }

    public void InitializeKnockoutStage(int simulationSeed, int semillaCrucesFase = 0)
    {
        var groups = _groupRepository.GetAll().ToList();

        if (!groups.Any())
            throw new InvalidOperationException("No se ha generado la fase de grupos.");

        var allMatchesHaveResults = groups.SelectMany(g => g.Matches).All(m => m.HasResult);
        if (!allMatchesHaveResults)
            throw new InvalidOperationException("La fase de grupos debe estar simulada.");

        if (_matchRepository.GetAll().Any(m => m.Phase != TournamentPhase.GroupStage))
            throw new InvalidOperationException("La fase eliminatoria ya fue inicializada.");

        var qualifiedTeamResponses = _groupService.GetQualifiedTeams().ToList();
        var qualifiedTeamNames = qualifiedTeamResponses.Select(t => t.Name).ToHashSet();
        var qualifiedTeams = _teamRepository.GetAll()
            .Where(t => qualifiedTeamNames.Contains(t.Name))
            .ToList();

        var shuffleService = new ShuffleService(simulationSeed);
        var knockout = new Knockout(simulationSeed, shuffleService, semillaCrucesFase);
        knockout.SetQualifiedTeams(qualifiedTeams);
        knockout.InitializeRoundOf32();

        foreach (var match in knockout.Matches)
            _matchRepository.Add(match);

        _knockout = knockout;
    }

    public void SetKnockoutMatchResult(string matchCode, int homeGoals, int awayGoals)
    {
        var allMatches = _matchRepository.GetAll().ToList();
        var shuffleService = new ShuffleService(0);
        var knockout = new Knockout(0, shuffleService);
        knockout.LoadExistingMatches(allMatches);
        knockout.SetMatchResult(matchCode, homeGoals, awayGoals);
        
        var updatedMatch = allMatches.Single(m => m.MatchCode == matchCode);
        _matchRepository.Update(updatedMatch);
    }

    public IEnumerable<MatchResponse> GetKnockoutMatches() =>
        _matchRepository.GetAll()
            .Where(m => m.Phase != TournamentPhase.GroupStage)
            .Select(MatchMapper.ToResponse)
            .ToList();

    public void InitializeRoundOf16()
    {
        if (_knockout is null)
            throw new InvalidOperationException("La fase eliminatoria no ha sido inicializada.");

        var allMatches = _matchRepository.GetAll().ToList();
        _knockout.LoadExistingMatches(allMatches);
        _knockout.InitializeRoundOf16();

        var newMatches = _knockout.Matches.Where(m => m.Phase == TournamentPhase.RoundOf16).ToList();
        foreach (var match in newMatches)
            _matchRepository.Add(match);
    }

    public void InitializeQuarterFinals()
    {
        if (_knockout is null)
            throw new InvalidOperationException("La fase eliminatoria no ha sido inicializada.");

        var allMatches = _matchRepository.GetAll().ToList();
        _knockout.LoadExistingMatches(allMatches);
        _knockout.InitializeQuarterFinals();

        var newMatches = _knockout.Matches.Where(m => m.Phase == TournamentPhase.QuarterFinals).ToList();
        foreach (var match in newMatches)
            _matchRepository.Add(match);
    }

    public void InitializeSemiFinals()
    {
        if (_knockout is null)
            throw new InvalidOperationException("La fase eliminatoria no ha sido inicializada.");

        var allMatches = _matchRepository.GetAll().ToList();
        _knockout.LoadExistingMatches(allMatches);
        _knockout.InitializeSemiFinals();

        var newMatches = _knockout.Matches.Where(m => m.Phase == TournamentPhase.SemiFinals).ToList();
        foreach (var match in newMatches)
            _matchRepository.Add(match);
    }

    public void InitializeThirdPlace()
    {
        if (_knockout is null)
            throw new InvalidOperationException("La fase eliminatoria no ha sido inicializada.");

        var allMatches = _matchRepository.GetAll().ToList();
        _knockout.LoadExistingMatches(allMatches);
        _knockout.InitializeThirdPlace();

        var newMatches = _knockout.Matches.Where(m => m.Phase == TournamentPhase.ThirdPlace).ToList();
        foreach (var match in newMatches)
            _matchRepository.Add(match);
    }

    public void SimulateKnockoutStage(int seed)
    {
        if (_knockout is null)
            throw new InvalidOperationException("La fase eliminatoria no ha sido inicializada.");

        var allMatches = _matchRepository.GetAll().ToList();
        _knockout.LoadExistingMatches(allMatches);
        _knockout.SimulateMatches(seed);

        foreach (var match in _knockout.Matches)
            _matchRepository.Update(match);
    }

    public void InitializeFinal()
    {
        if (_knockout is null)
            throw new InvalidOperationException("La fase eliminatoria no ha sido inicializada.");

        var allMatches = _matchRepository.GetAll().ToList();
        _knockout.LoadExistingMatches(allMatches);
        _knockout.InitializeFinal();

        var newMatches = _knockout.Matches.Where(m => m.Phase == TournamentPhase.Final).ToList();
        foreach (var match in newMatches)
            _matchRepository.Add(match);
    }
}