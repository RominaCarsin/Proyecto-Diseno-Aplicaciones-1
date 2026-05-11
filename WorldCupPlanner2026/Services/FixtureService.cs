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

    public bool CanGenerateFixture() => throw new NotImplementedException();
    public int GetStadiumCount() => throw new NotImplementedException();

    public void GenerateFixture(int seed, DateTime startDate)
    {
       ValidateFixtureCanBeGenerated();
       var teams = GetTeamsOrdered(seed);
       var groups = CreateGroups();
       AssignTeamsToGroups(teams, groups);
       SaveGroups(groups);
    }
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
}