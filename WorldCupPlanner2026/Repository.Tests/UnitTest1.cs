using Domain;
using Repository;

namespace Repository.Tests;

[TestClass]
public class RepositoryTests
{
    private Repository<Team> _repository;
    private Team _team;

    [TestInitialize]
    public void Setup()
    {
        _repository = new Repository<Team>();
        _team = new Team("Uruguay", "CONMEBOL", 500);
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

        Assert.IsTrue(result.Contains(_team));
        Assert.IsTrue(result.Contains(team2));
    }
    

    [TestMethod]
    public void Delete_WhenEntityExists_ShouldRemoveIt()
    {
        _repository.Add(_team);

        _repository.Delete(_team);

        Assert.AreEqual(0, _repository.GetAll().Count());
    }

    [TestMethod]
    public void Delete_ShouldOnlyRemoveTargetEntity()
    {
        var team2 = new Team("Argentina", "CONMEBOL", 600);
        _repository.Add(_team);
        _repository.Add(team2);

        _repository.Delete(_team);

        var result = _repository.GetAll().ToList();
        Assert.IsFalse(result.Contains(_team));
        Assert.IsTrue(result.Contains(team2));
    }

    [TestMethod]
    public void Delete_WhenEntityNotExists_ShouldNotThrow()
    {
        _repository.Add(_team);
        var nonExistent = new Team("Brazil", "CONMEBOL", 700);

        _repository.Delete(nonExistent);

        Assert.AreEqual(1, _repository.GetAll().Count());
    }
}