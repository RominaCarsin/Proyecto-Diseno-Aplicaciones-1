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
}