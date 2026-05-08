using System.Text.RegularExpressions;
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
    [TestMethod]
    public void AddGroup_ShouldAddGroupToRepository()
    {
        IRepository<Domain.Group> repository = new Repository<Domain.Group>();
        IGroupService service = new GroupService(repository);

        var request = new GroupRequest
        {
            Label = 'A'
        };

        service.AddGroup(request);

        var groups = service.GetGroups().ToList();

        Assert.AreEqual(1, groups.Count);
        Assert.AreEqual('A', groups[0].Label);
    }
    
    [TestMethod]
    public void GetGroupByLabel_ShouldReturnCorrectGroup()
    {
        IRepository<Domain.Group> repository = new Repository<Domain.Group>();
        IGroupService service = new GroupService(repository);

        service.AddGroup(new GroupRequest { Label = 'A' });

        var group = service.GetGroupByLabel('A');

        Assert.AreEqual('A', group.Label);
    }
    
    [TestMethod]
    public void DeleteGroup_ShouldRemoveGroup()
    {
        IRepository<Domain.Group> repository = new Repository<Domain.Group>();
        IGroupService service = new GroupService(repository);

        service.AddGroup(new GroupRequest { Label = 'A' });

        service.DeleteGroup('A');

        Assert.AreEqual(0, service.GetGroups().Count());
    }
    
    [TestMethod]
    public void AddTeamToGroup_ShouldAddTeamToGroup()
    {
        IRepository<Domain.Group> repository = new Repository<Domain.Group>();
        IGroupService service = new GroupService(repository);

        service.AddGroup(new GroupRequest { Label = 'A' });

        TeamRequest teamRequest = new TeamRequest
        {
            Name = "Uruguay",
            ConfederationName = "CONMEBOL",
            RankingFifa = 1500
        };

        service.AddTeamToGroup('A', teamRequest);

        GroupResponse group = service.GetGroupByLabel('A');

        Assert.AreEqual(1, group.Teams.Count);
    }
    
    [TestMethod]
    public void GetGroupStandings_ShouldReturnTeamsOrderedByPoints()
    {
        IRepository<Domain.Group> repository = new Repository<Domain.Group>();
        IGroupService service = new GroupService(repository);

        service.AddGroup(new GroupRequest { Label = 'A' });

        var uruguay = new TeamRequest
        {
            Name = "Uruguay",
            ConfederationName = "CONMEBOL",
            RankingFifa = 1500
        };

        var argentina = new TeamRequest
        {
            Name = "Argentina",
            ConfederationName = "CONMEBOL",
            RankingFifa = 1800
        };

        service.AddTeamToGroup('A', uruguay);
        service.AddTeamToGroup('A', argentina);

        var standings = service.GetGroupStandings('A').ToList();

        Assert.AreEqual("URUGUAY", standings[0].Name);
        Assert.AreEqual("ARGENTINA", standings[1].Name);
    }
    
    [TestMethod]
    public void GetTeamByPosition_ShouldReturnTeamInGivenPosition()
    {
        IRepository<Domain.Group> repository = new Repository<Domain.Group>();
        IGroupService service = new GroupService(repository);

        service.AddGroup(new GroupRequest { Label = 'A' });

        service.AddTeamToGroup('A', new TeamRequest
        {
            Name = "Uruguay",
            ConfederationName = "CONMEBOL",
            RankingFifa = 1500
        });

        TeamResponse team = service.GetTeamByPosition('A', 1);

        Assert.AreEqual("URUGUAY", team.Name);
    }
}