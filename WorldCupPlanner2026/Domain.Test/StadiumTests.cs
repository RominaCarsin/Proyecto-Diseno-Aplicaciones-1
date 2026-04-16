
namespace Domain.Test;

[TestClass]
public class StadiumTests
{
    [TestMethod]
    public void CreatesStadium_WhitValidData_ShouldSucceed()
    {
        var stadium = new Stadium("Centenario", "Montevideo", "Historic stadium", 60000);

        Assert.IsNotNull(stadium);
        Assert.AreEqual("Centenario", stadium.Name);
        Assert.AreEqual("Montevideo", stadium.City);
        Assert.AreEqual("Historic stadium", stadium.Description);
        Assert.AreEqual(60000, stadium.Capacity);
    }

    [TestMethod]
    public void CreateStadium_WithEmptyName_ShouldThrowException()
    {
        Assert.ThrowsException<Exception>(() => new Stadium("", "Montevideo", "Historic stadium", 60000));
    }
    
    [TestMethod]
    public void CreateStadium_WithNameLongerThan80Characters_ShouldThrowException()
    {
        string longName = new string('A', 81);

        Assert.ThrowsException<Exception>(() => new Stadium(longName, "Montevideo", "Historic stadium", 60000));
    }
    
    [TestMethod]
    public void CreateStadium_WithEmptyCity_ShouldThrowException()
    {
        Assert.ThrowsException<Exception>(() =>
        {
            new Stadium("Centenario", "", "Historic stadium", 60000);
        });
    }
    
    [TestMethod]
    public void CreateStadium_WithCityLongerThan60Characters_ShouldThrowException()
    {
        string longCity = new string('A', 61);

        Assert.ThrowsException<Exception>(() =>
        {
            new Stadium("Centenario", longCity, "Historic stadium", 60000);
        });
    }
    
    [TestMethod]
    public void CreateStadium_WithCapacityLowerThan20000_ShouldThrowException()
    {
        Assert.ThrowsException<Exception>(() =>
        {
            new Stadium("Centenario", "Montevideo", "Historic stadium", 19999);
        });
    }
}