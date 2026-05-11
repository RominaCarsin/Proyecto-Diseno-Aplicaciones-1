using DTOs;
using IServices;
using Domain;
using IRepository;
using Repository;
using Services;

namespace Services.Tests;

[TestClass]
public class GroupServiceTests
{
    private IGroupService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        IRepository<Domain.Group> repository = new Repository<Domain.Group>();
        _service = new GroupService(repository);
    }

    [TestMethod]
    public void AddGroup_ShouldAddGroupToRepository()
    {
        _service.AddGroup(new GroupRequest { Label = 'A' });

        var groups = _service.GetGroups().ToList();

        Assert.AreEqual(1, groups.Count);
        Assert.AreEqual('A', groups[0].Label);
    }

    [TestMethod]
    public void GetGroupByLabel_ShouldReturnCorrectGroup()
    {
        _service.AddGroup(new GroupRequest { Label = 'A' });

        var group = _service.GetGroupByLabel('A');

        Assert.AreEqual('A', group.Label);
    }

    [TestMethod]
    public void DeleteGroup_ShouldRemoveGroup()
    {
        _service.AddGroup(new GroupRequest { Label = 'A' });

        _service.DeleteGroup('A');

        Assert.AreEqual(0, _service.GetGroups().Count());
    }

    [TestMethod]
    public void AddTeamToGroup_ShouldAddTeamToGroup()
    {
        _service.AddGroup(new GroupRequest { Label = 'A' });

        _service.AddTeamToGroup('A', CreateTeamRequest("Uruguay"));

        var group = _service.GetGroupByLabel('A');

        Assert.AreEqual(1, group.Teams.Count);
    }

    [TestMethod]
    public void GetTeamByPosition_ShouldReturnTeamInGivenPosition()
    {
        _service.AddGroup(new GroupRequest { Label = 'A' });
        _service.AddTeamToGroup('A', CreateTeamRequest("Uruguay"));
        _service.AddTeamToGroup('A', CreateTeamRequest("Argentina"));
        _service.AddMatchToGroup('A', CreateValidMatchRequest('A'));

        var group = _service.GetGroupByLabel('A');
        _service.SetMatchResult('A', group.Matches[0].Id, 3, 0);

        var team = _service.GetTeamByPosition('A', 1);

        Assert.AreEqual("URUGUAY", team.Name);
    }

    [TestMethod]
    public void AddMatchToGroup_WithValidMatch_ShouldAddMatchToGroup()
    {
        _service.AddGroup(new GroupRequest { Label = 'A' });
        _service.AddTeamToGroup('A', CreateTeamRequest("Uruguay"));
        _service.AddTeamToGroup('A', CreateTeamRequest("Argentina"));

        _service.AddMatchToGroup('A', CreateValidMatchRequest('A'));

        var group = _service.GetGroupByLabel('A');

        Assert.AreEqual(1, group.Matches.Count);
    }

    private static TeamRequest CreateTeamRequest(string name) => new()
    {
        Name = name,
        ConfederationName = "CONMEBOL",
        RankingFifa = 1500
    };

    private static MatchRequest CreateValidMatchRequest(char group) => new()
    {
        Date = new DateTime(2026, 6, 1),
        Stadium = new StadiumRequest
        {
            Name = "Centenario",
            City = "Montevideo",
            Description = "Historic stadium",
            Capacity = 60000
        },
        HomeTeam = CreateTeamRequest("Uruguay"),
        AwayTeam = CreateTeamRequest("Argentina"),
        Group = group,
        Phase = "GroupStage"
    };
}