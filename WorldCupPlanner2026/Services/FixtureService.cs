using Domain;
using DTOs;
using IRepository;
using IServices;

namespace Services;

public class FixtureService : IFixtureService
{
    private readonly ITeamService _teamService;
    private readonly IStadiumService _stadiumService;
    private readonly IGroupService _groupService;
    private readonly IRepository<Group> _groupRepository;
    private readonly IRepository<Match> _matchRepository;
    private readonly IPotService _potService;
    
    private const int MaxMatchesPerDay = 3;
    private const int FirstMatchHour = 14;
    private const int HoursBetweenMatches = 4;

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
        // Cada pot tiene 12 equipos, uno por grupo
        // Pot 1 → grupo A,B,...,L (posición 0 de cada grupo)
        // Pot 2 → grupo A,B,...,L (posición 1 de cada grupo)
        // Pot 3 → grupo A,B,...,L (posición 2 de cada grupo)
        // Pot 4 → grupo A,B,...,L (posición 3 de cada grupo)
        foreach (var pot in pots)
        {
            var teams = pot.Teams.ToList();
            for (int i = 0; i < groups.Count; i++)
                groups[i].AddTeam(teams[i]);
        }
    }

    private List<StadiumResponse> GetStadiumsNormalized()
    {
        return _stadiumService.GetAllStadiums()
            .OrderBy(s => s.Name.ToUpperInvariant())
            .ToList();
    }

    private static void GenerateMatchesForGroups(List<Group> groups, List<StadiumResponse> stadiums, DateTime startDate)
    {
        int matchCounter = 0;

        foreach (var group in groups)
        {
            var teamList = group.Teams.ToList();
            var pairings = new List<(int, int)> { (0,3), (1,2), (0,2), (1,3), (0,1), (2,3) };

            foreach (var (homeIndex, awayIndex) in pairings)
            {
                int dayOffset = matchCounter / MaxMatchesPerDay;
                int matchIndexInDay = matchCounter % MaxMatchesPerDay;

                var matchDate = startDate.Date
                    .AddDays(dayOffset)
                    .AddHours(FirstMatchHour)
                    .AddHours(matchIndexInDay * HoursBetweenMatches);

                var stadium = BuildStadium(stadiums[matchCounter % stadiums.Count]);

                group.AddMatch(new Match(
                    matchDate,
                    stadium,
                    teamList[homeIndex],
                    teamList[awayIndex],
                    group: group.Label,
                    phase: "GroupStage"
                ));

                matchCounter++;
            }
        }
    }

    private static Stadium BuildStadium(StadiumResponse r) =>
        new Stadium(r.Name, r.City, r.Description, r.Capacity);

    private void ValidateFixtureCanBeGenerated()
    {
        if (_teamService.GetTeams().Count() < 48)
            throw new InvalidOperationException("Se necesitan 48 equipos para generar el fixture.");

        if (_stadiumService.GetAllStadiums().Count() < 4)
            throw new InvalidOperationException("Se necesitan al menos 4 estadios para generar el fixture.");

        if (_groupService.GetGroups().Any())
            throw new InvalidOperationException("El fixture ya fue generado.");
    }

    private static List<Group> CreateGroups()
    {
        return Enumerable.Range(0, 12)
            .Select(i => new Group((char)('A' + i)))
            .ToList();
    }

    private void SaveGroups(List<Group> groups)
    {
        foreach (var group in groups)
        {
            _groupRepository.Add(group);
            foreach (var match in group.Matches)
                _matchRepository.Add(match);
        }
    }

    public bool CanGenerateFixture()
    {
        var teamCount = _teamService.GetTeams().Count();
        var stadiumCount = _stadiumService.GetAllStadiums().Count();
        var groupCount = _groupService.GetGroups().Count();

        return teamCount >= 48 && stadiumCount >= 4 && groupCount == 0;
    }

    public int GetStadiumCount() => _stadiumService.GetAllStadiums().Count();
}