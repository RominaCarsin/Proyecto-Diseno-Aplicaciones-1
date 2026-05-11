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
        if (_teamService.GetTeams().Count() < 48)
            throw new InvalidOperationException("Se necesitan 48 equipos para generar el fixture.");
        if (_stadiumService.GetAllStadiums().Count() < 4)
            throw new InvalidOperationException("Se necesitan al menos 4 estadios para generar el fixture.");

    }
    
    
}