using Domain;
using DTOs;
using IRepository;
using IServices;
using Services.Mappers;

namespace Services;

public class FixtureService : IFixtureService
{
    private const int RequiredTeamCount = 48;
    private const int MinStadiumCount = 4;
    private const int GroupCount = 12;
    private const int MaxTeamsPerGroup = 4;
    private const int FirstMatchHour = 14;
    private const int HoursBetweenMatches = 4;
    private const int RoundDaysOffset = 3;
    private const int MatchesPerRound = 2;

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
        var stadiums = GetStadiumsNormalized();
        var groups = CreateGroups();
        AssignTeamsToGroupsFromPots(pots, groups);
        GenerateMatchesForGroups(groups, stadiums, startDate);
        SaveGroups(groups);
    }

    private static void AssignTeamsToGroupsFromPots(List<Pot> pots, List<Group> groups)
    {
        foreach (var pot in pots)
        {
            var teams = pot.Teams;

            for (int i = 0; i < teams.Count; i++)
            {
                int bestStart = FindBestStartIndex(teams[i], groups);
                AddTeamToNextAvailableGroup(teams[i], groups, bestStart);
            }
        }
    }

    private static int FindBestStartIndex(Team team, List<Group> groups) =>
        groups
            .Select((g, index) => (group: g, index))
            .Where(x => x.group.Teams.Count < MaxTeamsPerGroup)
            .OrderBy(x => x.group.Teams.Count(t => t.ConfederationName == team.ConfederationName))
            .ThenBy(x => x.group.Teams.Count)
            .Select(x => x.index)
            .FirstOrDefault(0);

    private static void AddTeamToNextAvailableGroup(Team team, List<Group> groups, int startGroupIndex)
    {
        int count = groups.Count;

        for (int i = 0; i < count; i++)
        {
            var group = groups[(startGroupIndex + i) % count];
            try
            {
                group.AddTeam(team);
                return;
            }
            catch (ArgumentException)
            {
            }
        }

        throw new InvalidOperationException($"No se pudo asignar el equipo {team.Name} a ningún grupo.");
    }

    private List<Stadium> GetStadiumsNormalized() =>
        _stadiumRepository.GetAll()
            .OrderBy(s => s.Name)
            .ToList();

    private static void GenerateMatchesForGroups(List<Group> groups, List<Stadium> stadiums, DateTime startDate)
    {
        int matchCounter = 0;

        for (int groupIndex = 0; groupIndex < groups.Count; groupIndex++)
        {
            var group = groups[groupIndex];
            var teamList = group.Teams.ToList();

            var pairings = new List<(int, int)>
            {
                (0, 3), (1, 2),
                (0, 2), (1, 3),
                (0, 1), (2, 3)
            };

            int groupStartDayOffset = groupIndex * RoundDaysOffset;

            for (int round = 0; round < 3; round++)
            {
                var roundDate = startDate.Date.AddDays(groupStartDayOffset + round * RoundDaysOffset);

                var roundPairings = pairings
                    .Skip(round * MatchesPerRound)
                    .Take(MatchesPerRound)
                    .ToList();

                for (int matchIndex = 0; matchIndex < roundPairings.Count; matchIndex++)
                {
                    var matchDate = round == 2
                        ? roundDate.AddHours(FirstMatchHour)
                        : roundDate.AddHours(FirstMatchHour).AddHours(matchIndex * HoursBetweenMatches);

                    AddMatchToGroup(group, teamList, roundPairings[matchIndex], matchDate, stadiums, ref matchCounter);
                }
            }
        }
    }

    private void ValidateFixtureCanBeGenerated()
    {
        if (_teamService.GetTeams().Count() < RequiredTeamCount)
            throw new InvalidOperationException("Se necesitan 48 equipos para generar el fixture.");

        if (_stadiumService.GetAllStadiums().Count() < MinStadiumCount)
            throw new InvalidOperationException("Se necesitan al menos 4 estadios para generar el fixture.");

        if (_groupService.GetGroups().Any())
            throw new InvalidOperationException("El fixture ya fue generado.");
    }

    private static List<Group> CreateGroups() =>
        Enumerable.Range(0, GroupCount).Select(i => new Group((char)('A' + i))).ToList();

    private void SaveGroups(List<Group> groups)
    {
        foreach (var group in groups)
        {
            _groupRepository.Add(group);
        }
    }

    public bool CanGenerateFixture()
    {
        var teamCount = _teamService.GetTeams().Count();
        var stadiumCount = _stadiumService.GetAllStadiums().Count();
        var groupCount = _groupService.GetGroups().Count();

        return teamCount >= RequiredTeamCount && stadiumCount >= MinStadiumCount && groupCount == 0;
    }

    public int GetStadiumCount() => _stadiumService.GetAllStadiums().Count();

    public void GenerateFixture(int seed) =>
        GenerateFixture(seed, new DateTime(2026, 6, 1));

    public void SimulateGroupStage(int seed)
    {
        var groups = _groupRepository.GetAll().ToList();

        if (!groups.Any())
            throw new InvalidOperationException("No se ha generado el fixture todavía.");

        foreach (var group in groups)
            group.SimulateMatches(seed);

        foreach (var group in groups)
            _groupRepository.Update(group);
    }

    private static void AddMatchToGroup(
        Group group,
        List<Team> teamList,
        (int homeIndex, int awayIndex) pairing,
        DateTime matchDate,
        List<Stadium> stadiums,
        ref int matchCounter)
    {
        var stadium = stadiums[matchCounter % stadiums.Count];

        group.AddMatch(new Match(
            matchDate,
            stadium,
            teamList[pairing.homeIndex],
            teamList[pairing.awayIndex],
            group: group.Label,
            phase: TournamentPhase.GroupStage
        ));

        matchCounter++;
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
