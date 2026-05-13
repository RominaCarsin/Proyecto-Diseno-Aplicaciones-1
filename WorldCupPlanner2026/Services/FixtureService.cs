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

    private static int FindBestStartIndex(Team team, List<Group> groups)
    {
        // Preferir el grupo con menos equipos de la misma confederación
        // y que tenga lugar disponible
        return groups
            .Select((g, index) => (group: g, index))
            .Where(x => x.group.Teams.Count < 4)
            .OrderBy(x => x.group.Teams.Count(t => t.ConfederationName == team.ConfederationName))
            .ThenBy(x => x.group.Teams.Count)
            .Select(x => x.index)
            .FirstOrDefault(0);
    }


    private static void AddTeamToNextAvailableGroup(Team team, List<Group> groups, int startGroupIndex)
    {
        int startIndex = startGroupIndex;
        int count = groups.Count;

        for (int i = 0; i < count; i++)
        {
            var group = groups[(startIndex + i) % count];
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

    private List<StadiumResponse> GetStadiumsNormalized()
    {
        return _stadiumService.GetAllStadiums()
            .OrderBy(s => s.Name.ToUpperInvariant())
            .ToList();
    }

    private static void GenerateMatchesForGroups(List<Group> groups, List<StadiumResponse> stadiums, DateTime startDate)
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

            for (int round = 0; round < 3; round++)
            {
                var groupStartDate = startDate.Date.AddDays(groupIndex*7);
                var roundDate = groupStartDate.AddDays(round * 3);

                var roundPairings = pairings.Skip(round * 2).Take(2).ToList();

                for (int matchIndexInRound = 0; matchIndexInRound < roundPairings.Count; matchIndexInRound++)
                {
                    var (homeIndex, awayIndex) = roundPairings[matchIndexInRound];

                    var matchDate = round == 2
                        ? roundDate.AddHours(FirstMatchHour)
                        : roundDate.AddHours(FirstMatchHour)
                            .AddHours(matchIndexInRound * HoursBetweenMatches);

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
    
    public void GenerateFixture(int seed)
    {
        GenerateFixture(seed, new DateTime(2026, 6, 1));
    }
    
    public void SimulateGroupStage(int seed)
    {
        var groups = _groupRepository.GetAll().ToList();

        if (!groups.Any())
            throw new InvalidOperationException("No se ha generado el fixture todavía.");

        foreach (var group in groups)
            group.SimulateMatches(seed);
    }
    
}