using Domain;
using DTOs;
using IRepository;
using IServices;

namespace Services;

public class FixtureService : IFixtureService
{
    private const int RequiredTeamCount = 48;
    private const int RequiredMinimumStadiumCount = 4;
    private const int TotalGroupCount = 12;
    private const int TeamsPerGroup = 4;
    private const int RoundsPerGroup = 3;
    private const int MatchesPerRound = 2;
    private const int DaysBetweenRounds = 3;
    private const int DaysBetweenGroupStarts = 7;
    private const int FirstMatchHour = 14;
    private const int HoursBetweenMatches = 4;

    private const string NotEnoughTeamsErrorMessage = "Se requieren 48 equipos para generar el fixture.";
    private const string NotEnoughStadiumsErrorMessage = "Se requieren al menos 4 estadios para generar el fixture.";
    private const string FixtureAlreadyGeneratedErrorMessage = "El fixture ya fue generado.";
    private const string FixtureNotGeneratedErrorMessage = "El fixture aún no fue generado.";
    private const string TeamCouldNotBeAssignedErrorMessage = "El equipo '{0}' no pudo ser asignado a ningún grupo.";

    private static readonly List<(int homeIndex, int awayIndex)> GroupStagePairings = new()
    {
        (0, 3), (1, 2),
        (0, 2), (1, 3),
        (0, 1), (2, 3)
    };

    private readonly ITeamService _teamService;
    private readonly IStadiumService _stadiumService;
    private readonly IGroupService _groupService;
    private readonly IRepository<Group> _groupRepository;
    private readonly IRepository<Match> _matchRepository;
    private readonly IPotService _potService;

    public FixtureService(
        ITeamService teamService,
        IStadiumService stadiumService,
        IGroupService groupService,
        IRepository<Group> groupRepository,
        IRepository<Match> matchRepository,
        IPotService potService)
    {
        _teamService = teamService;
        _stadiumService = stadiumService;
        _groupService = groupService;
        _groupRepository = groupRepository;
        _matchRepository = matchRepository;
        _potService = potService;
    }

    public void GenerateFixture(int seed)
    {
        GenerateFixture(seed, new DateTime(2026, 6, 1));
    }

    public void GenerateFixture(int seed, DateTime startDate)
    {
        ValidateFixtureCanBeGenerated();

        _potService.GeneratePots(seed);

        var pots = _potService.GetPots().ToList();
        var stadiums = GetStadiumsSortedByName();
        var groups = CreateGroups();

        AssignTeamsToGroupsFromPots(pots, groups);
        GenerateMatchesForGroups(groups, stadiums, startDate);
        SaveGroupsAndMatches(groups);
    }

    public void SimulateGroupStage(int seed)
    {
        var groups = _groupRepository.GetAll().ToList();

        if (!groups.Any())
            throw new InvalidOperationException(FixtureNotGeneratedErrorMessage);

        foreach (var group in groups)
            group.SimulateMatches(seed);
    }

    public bool CanGenerateFixture()
    {
        return HasEnoughTeams()
            && HasEnoughStadiums()
            && FixtureHasNotBeenGenerated();
    }

    public int GetStadiumCount() => _stadiumService.GetAllStadiums().Count();
    
    private void ValidateFixtureCanBeGenerated()
    {
        if (!HasEnoughTeams())
            throw new InvalidOperationException(NotEnoughTeamsErrorMessage);

        if (!HasEnoughStadiums())
            throw new InvalidOperationException(NotEnoughStadiumsErrorMessage);

        if (!FixtureHasNotBeenGenerated())
            throw new InvalidOperationException(FixtureAlreadyGeneratedErrorMessage);
    }

    private bool HasEnoughTeams() =>
        _teamService.GetTeams().Count() >= RequiredTeamCount;

    private bool HasEnoughStadiums() =>
        _stadiumService.GetAllStadiums().Count() >= RequiredMinimumStadiumCount;

    private bool FixtureHasNotBeenGenerated() =>
        !_groupService.GetGroups().Any();
    
    private static List<Group> CreateGroups() =>
        Enumerable.Range(0, TotalGroupCount)
            .Select(i => new Group((char)('A' + i)))
            .ToList();

    private static void AssignTeamsToGroupsFromPots(List<Pot> pots, List<Group> groups)
    {
        foreach (var pot in pots)
        {
            foreach (var team in pot.Teams)
            {
                int preferredGroupIndex = FindGroupIndexWithFewestSameConfederationTeams(team, groups);
                AssignTeamToGroupStartingFrom(team, groups, preferredGroupIndex);
            }
        }
    }

    private static int FindGroupIndexWithFewestSameConfederationTeams(Team team, List<Group> groups)
    {
        return groups
            .Select((group, index) => (group, index))
            .Where(x => x.group.Teams.Count < TeamsPerGroup)
            .OrderBy(x => x.group.Teams.Count(t =>
                t.ConfederationName.Equals(team.ConfederationName, StringComparison.OrdinalIgnoreCase)))
            .ThenBy(x => x.group.Teams.Count)
            .Select(x => x.index)
            .FirstOrDefault(0);
    }

    private static void AssignTeamToGroupStartingFrom(Team team, List<Group> groups, int startIndex)
    {
        for (int offset = 0; offset < groups.Count; offset++)
        {
            var group = groups[(startIndex + offset) % groups.Count];
            try
            {
                group.AddTeam(team);
                return;
            }
            catch (ArgumentException)
            {
            }
        }

        throw new InvalidOperationException(
            string.Format(TeamCouldNotBeAssignedErrorMessage, team.Name));
    }
    
    private static void GenerateMatchesForGroups(
        List<Group> groups,
        List<StadiumResponse> stadiums,
        DateTime startDate)
    {
        int matchCounter = 0;

        for (int groupIndex = 0; groupIndex < groups.Count; groupIndex++)
        {
            var group = groups[groupIndex];
            var groupTeams = group.Teams.ToList();
            var groupStartDate = startDate.Date.AddDays(groupIndex * DaysBetweenGroupStarts);

            for (int round = 0; round < RoundsPerGroup; round++)
            {
                var roundDate = groupStartDate.AddDays(round * DaysBetweenRounds);
                var roundPairings = GroupStagePairings
                    .Skip(round * MatchesPerRound)
                    .Take(MatchesPerRound)
                    .ToList();

                for (int matchIndex = 0; matchIndex < roundPairings.Count; matchIndex++)
                {
                    var (homeIndex, awayIndex) = roundPairings[matchIndex];
                    var matchDate = CalculateMatchDate(roundDate, round, matchIndex);
                    var stadium = BuildStadiumFromResponse(stadiums[matchCounter % stadiums.Count]);

                    group.AddMatch(new Match(
                        matchDate,
                        stadium,
                        groupTeams[homeIndex],
                        groupTeams[awayIndex],
                        group: group,
                        phase: TournamentPhase.GroupStage
                    ));

                    matchCounter++;
                }
            }
        }
    }

    private static DateTime CalculateMatchDate(DateTime roundDate, int round, int matchIndex)
    {
        bool isLastRound = round == RoundsPerGroup - 1;

        return isLastRound
            ? roundDate.AddHours(FirstMatchHour)
            : roundDate.AddHours(FirstMatchHour + matchIndex * HoursBetweenMatches);
    }

    private static Stadium BuildStadiumFromResponse(StadiumResponse response) =>
        new Stadium(response.Name, response.City, response.Description, response.Capacity);
    
    private List<StadiumResponse> GetStadiumsSortedByName() =>
        _stadiumService.GetAllStadiums()
            .OrderBy(s => s.Name.ToUpperInvariant())
            .ToList();

    private void SaveGroupsAndMatches(List<Group> groups)
    {
        foreach (var group in groups)
        {
            _groupRepository.Add(group);

            foreach (var match in group.Matches)
                _matchRepository.Add(match);
        }
    }
}