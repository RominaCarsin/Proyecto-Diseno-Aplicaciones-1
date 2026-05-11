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

    public FixtureService(
        ITeamService teamService,
        IStadiumService stadiumService,
        IGroupService groupService,
        IRepository<Group> groupRepository)
    {
        _teamService = teamService;
        _stadiumService = stadiumService;
        _groupService = groupService;
        _groupRepository = groupRepository;
    }

    
    public void GenerateFixture(int seed, DateTime startDate)
    {
       ValidateFixtureCanBeGenerated();
       var teams = GetTeamsOrdered(seed);
       var stadiums = GetStadiumsNormalized();
       var groups = CreateGroups();
       AssignTeamsToGroups(teams, groups);
       GenerateMatchesForGroups(groups, stadiums, startDate);
       SaveGroups(groups);
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
            var pairings = new List<(int, int)> { (0,3),(1,2),(0,2),(1,3),(0,1),(2,3) };

            for (int round = 0; round < 3; round++)
            {
                var roundDate = startDate.AddDays(round * 3);
                var roundPairings = pairings.Skip(round * 2).Take(2).ToList();

                for (int i = 0; i < roundPairings.Count; i++)
                {
                    var (homeIndex, awayIndex) = roundPairings[i];
                    var matchDate = round == 2
                        ? roundDate.Date.AddHours(14)
                        : roundDate.Date.AddHours(14).AddHours(i * 4);

                    var stadium = BuildStadium(stadiums[matchCounter % stadiums.Count]);
                    group.AddMatch(new Match(matchDate, stadium, teamList[homeIndex], teamList[awayIndex]));
                    matchCounter++;
                }
            }
        }
    }

    private static Stadium BuildStadium(StadiumResponse r) =>
        new Stadium(r.Name, r.City, r.Description, r.Capacity);
    
    private List<Team> GetTeamsOrdered(int seed)
    {
        return _teamService.GetTeamsOrdered(seed)
            .Select(t => new Team(t.Name, t.ConfederationName, t.RankingFifa))
            .ToList();
    }

    private static void AssignTeamsToGroups(List<Team> teams, List<Group> groups)
    {
        for (int i = 0; i < teams.Count; i++)
            groups[i % 12].AddTeam(teams[i]);
    }
    
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
            _groupRepository.Add(group);
    }
    
    public bool CanGenerateFixture()
    {
        var teamCount = _teamService.GetTeams().Count();
        var stadiumCount = _stadiumService.GetAllStadiums().Count();
        var groupCount = _groupService.GetGroups().Count();


        return teamCount >= 48  && stadiumCount >= 4 && groupCount == 0;
    }

    public int GetStadiumCount()
    {
        return 0;
    }}