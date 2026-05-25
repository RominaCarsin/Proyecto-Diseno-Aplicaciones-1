using Domain;
using Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Repository.Tests;

[TestClass]
public class RepositoryTests
{
    private AppDbContext _context;
    private Repository<Team> _repository;
    private Team _team;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _context = new AppDbContext(options);
        
        _repository = new Repository<Team>(_context);
        
        _team = new Team("Uruguay", "CONMEBOL", 500);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [TestMethod]
    public void Add_ShouldAddEntity()
    {
        _repository.Add(_team);

        Assert.AreEqual(1, _repository.GetAll().Count());
    }

    [TestMethod]
    public void Add_MultipleTimes_ShouldAddAll()
    {
        var team2 = new Team("Argentina", "CONMEBOL", 600);

        _repository.Add(_team);
        _repository.Add(team2);

        Assert.AreEqual(2, _repository.GetAll().Count());
    }

    [TestMethod]
    public void GetAll_WhenEmpty_ShouldReturnEmptyList()
    {
        var result = _repository.GetAll();
        Assert.AreEqual(0, result.Count());
    }

    [TestMethod]
    public void GetAll_ShouldReturnAllAddedEntities()
    {
        var team2 = new Team("Argentina", "CONMEBOL", 600);
        _repository.Add(_team);
        _repository.Add(team2);

        var result = _repository.GetAll().ToList();

        Assert.IsTrue(result.Any(t => t.Name == "Uruguay"));
        Assert.IsTrue(result.Any(t => t.Name == "Argentina"));
    }

    [TestMethod]
    public void Delete_ShouldOnlyRemoveTargetEntity()
    {
        var team2 = new Team("Argentina", "CONMEBOL", 600);
        _repository.Add(_team);
        _repository.Add(team2);

        _repository.Delete(_team);

        var result = _repository.GetAll().ToList();
        Assert.IsFalse(result.Any(t => t.Name == "Uruguay"));
        Assert.IsTrue(result.Any(t => t.Name == "Argentina"));
    }

    [TestMethod]
    public void Delete_WhenEntityExists_ShouldRemoveIt()
    {
        _repository.Add(_team);

        _repository.Delete(_team);

        Assert.AreEqual(0, _repository.GetAll().Count());
    }
    
}