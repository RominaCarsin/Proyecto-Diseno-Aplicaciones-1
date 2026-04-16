
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
        Assert.ThrowsException<Exception>(() => new Stadium("Centenario", "Montevideo", "Historic stadium", 19999));
    }
    
    [TestMethod]
    public void CreateStadium_WithNullName_ShouldThrowException()
    {
        Assert.ThrowsException<Exception>(() => new Stadium(null, "Montevideo", "Historic stadium", 60000));
    }
    
    [TestMethod]
    public void CreateStadium_WithWhitespaceCity_ShouldThrowException()
    {
        Assert.ThrowsException<Exception>(() => new Stadium("Centenario", "   ", "Historic stadium", 60000));
    }
    
    [TestMethod]
    public void CreateStadium_WithNullCity_ShouldThrowException()
    {
        Assert.ThrowsException<Exception>(() => new Stadium("Centenario", null, "Historic stadium", 60000));
    }
    
    [TestMethod]
    public void CreateStadium_WithNameExactly80Characters_ShouldSucceed()
    {
        string nameWith80Chars = new string('A', 80);
        var stadium = new Stadium(nameWith80Chars, "Montevideo", "Historic stadium", 60000);
    
        Assert.IsNotNull(stadium);
        Assert.AreEqual(nameWith80Chars, stadium.Name);
    }
    
    [TestMethod]
    public void CreateStadium_WithCityExactly60Characters_ShouldSucceed()
    {
        string cityWith60Chars = new string('A', 60);
        var stadium = new Stadium("Centenario", cityWith60Chars, "Historic stadium", 60000);
    
        Assert.IsNotNull(stadium);
        Assert.AreEqual(cityWith60Chars, stadium.City);
    }
    
    [TestMethod]
    public void CreateStadium_WithCapacityExactly20000_ShouldSucceed()
    {
        var stadium = new Stadium("Centenario", "Montevideo", "Historic stadium", 20000);
    
        Assert.IsNotNull(stadium);
        Assert.AreEqual(20000, stadium.Capacity);
    }
    
    [TestMethod]
    public void CreateStadium_WithNullDescription_ShouldSucceed()
    {
        var stadium = new Stadium("Centenario", "Montevideo", null, 60000);
    
        Assert.IsNotNull(stadium);
        Assert.IsNull(stadium.Description);
    }
    
    [TestMethod]
    public void CreateStadium_WithEmptyDescription_ShouldSucceed()
    {
        var stadium = new Stadium("Centenario", "Montevideo", "", 60000);
    
        Assert.IsNotNull(stadium);
        Assert.AreEqual("", stadium.Description);
    }
    
    [TestMethod]
    public void CreateStadium_WithDescriptionLongerThan400Characters_ShouldThrowException()
    {
        string longDescription = new string('A', 401);

        Assert.ThrowsException<Exception>(() =>
        {
            new Stadium("Centenario", "Montevideo", longDescription, 60000);
        });
    }
    
    
}