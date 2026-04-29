namespace Domain.Test;

[TestClass]
public class StadiumTests
{
    private const string ValidName = "CENTENARIO";
    private const string ValidCity = "MONTEVIDO";
    private const string ValidDescription = "Historic stadium";
    private const int ValidCapacity = 60000;

    [TestMethod]
    public void CreateStadium_WithValidData_ShouldSucceed()
    {
        var stadium = CreateValidStadium();

        Assert.IsNotNull(stadium);
        Assert.AreEqual(ValidName, stadium.Name);
        Assert.AreEqual(ValidCity, stadium.City);
        Assert.AreEqual(ValidDescription, stadium.Description);
        Assert.AreEqual(ValidCapacity, stadium.Capacity);
    }

    [TestMethod]
    public void CreateStadium_WithSpacesInNameAndCity_ShouldSucceed()
    {
        var stadium = new Stadium(" CENTENARIO", " MONTEVIDEO", ValidDescription, ValidCapacity);
        
        Assert.AreEqual("CENTENARIO", stadium.Name);
        Assert.AreEqual("MONTEVIDEO", stadium.City);
    }
    
    [TestMethod]
    public void CreateStadium_WithLowerCaseNameAndCity_ShouldStoreInUpperCase()
    {
        var stadium = new Stadium("Centenario", " Montevideo", ValidDescription, ValidCapacity);
        
        Assert.AreEqual("CENTENARIO", stadium.Name);
        Assert.AreEqual("MONTEVIDEO", stadium.City);
    }

    [TestMethod]
    public void CreateStadium_WithEmptyName_ShouldThrowException()
    {
        Assert.ThrowsException<Exception>(() => new Stadium("", ValidCity, ValidDescription, ValidCapacity));
    }

    [TestMethod]
    public void CreateStadium_WithNullName_ShouldThrowException()
    {
        Assert.ThrowsException<Exception>(() => new Stadium(null, ValidCity, ValidDescription, ValidCapacity));
    }

    [TestMethod]
    public void CreateStadium_WithWhitespaceName_ShouldThrowException()
    {
        Assert.ThrowsException<Exception>(() => new Stadium("   ", ValidCity, ValidDescription, ValidCapacity));
    }

    [TestMethod]
    public void CreateStadium_WithNameLongerThan80Characters_ShouldThrowException()
    {
        var longName = new string('A', 81);

        Assert.ThrowsException<Exception>(() => new Stadium(longName, ValidCity, ValidDescription, ValidCapacity));
    }

    [TestMethod]
    public void CreateStadium_WithNameExactly80Characters_ShouldSucceed()
    {
        var nameWith80Chars = new string('A', 80);

        var stadium = new Stadium(nameWith80Chars, ValidCity, ValidDescription, ValidCapacity);

        Assert.IsNotNull(stadium);
        Assert.AreEqual(nameWith80Chars, stadium.Name);
    }

    [TestMethod]
    public void CreateStadium_WithEmptyCity_ShouldThrowException()
    {
        Assert.ThrowsException<Exception>(() => new Stadium(ValidName, "", ValidDescription, ValidCapacity));
    }

    [TestMethod]
    public void CreateStadium_WithNullCity_ShouldThrowException()
    {
        Assert.ThrowsException<Exception>(() => new Stadium(ValidName, null, ValidDescription, ValidCapacity));
    }

    [TestMethod]
    public void CreateStadium_WithWhitespaceCity_ShouldThrowException()
    {
        Assert.ThrowsException<Exception>(() => new Stadium(ValidName, "   ", ValidDescription, ValidCapacity));
    }

    [TestMethod]
    public void CreateStadium_WithCityLongerThan60Characters_ShouldThrowException()
    {
        var longCity = new string('A', 61);

        Assert.ThrowsException<Exception>(() => new Stadium(ValidName, longCity, ValidDescription, ValidCapacity));
    }

    [TestMethod]
    public void CreateStadium_WithCityExactly60Characters_ShouldSucceed()
    {
        var cityWith60Chars = new string('A', 60);

        var stadium = new Stadium(ValidName, cityWith60Chars, ValidDescription, ValidCapacity);

        Assert.IsNotNull(stadium);
        Assert.AreEqual(cityWith60Chars, stadium.City);
    }

    [TestMethod]
    public void CreateStadium_WithCapacityLowerThan20000_ShouldThrowException()
    {
        Assert.ThrowsException<Exception>(() => new Stadium(ValidName, ValidCity, ValidDescription, 19999));
    }

    [TestMethod]
    public void CreateStadium_WithCapacityOf1_ShouldThrowException()
    {
        Assert.ThrowsException<Exception>(() => new Stadium(ValidName, ValidCity, ValidDescription, 1));
    }

    [TestMethod]
    public void CreateStadium_WithCapacityExactly20000_ShouldSucceed()
    {
        var stadium = new Stadium(ValidName, ValidCity, ValidDescription, 20000);

        Assert.IsNotNull(stadium);
        Assert.AreEqual(20000, stadium.Capacity);
    }

    [TestMethod]
    public void CreateStadium_WithNullDescription_ShouldSucceed()
    {
        var stadium = new Stadium(ValidName, ValidCity, null, ValidCapacity);

        Assert.IsNotNull(stadium);
        Assert.IsNull(stadium.Description);
    }

    [TestMethod]
    public void CreateStadium_WithEmptyDescription_ShouldSucceed()
    {
        var stadium = new Stadium(ValidName, ValidCity, "", ValidCapacity);

        Assert.IsNotNull(stadium);
        Assert.AreEqual("", stadium.Description);
    }

    [TestMethod]
    public void CreateStadium_WithDescriptionLongerThan400Characters_ShouldThrowException()
    {
        var longDescription = new string('A', 401);

        Assert.ThrowsException<Exception>(() => new Stadium(ValidName, ValidCity, longDescription, ValidCapacity));
    }

    [TestMethod]
    public void CreateStadium_WithDescriptionExactly400Characters_ShouldSucceed()
    {
        var description = new string('A', 400);

        var stadium = new Stadium(ValidName, ValidCity, description, ValidCapacity);

        Assert.IsNotNull(stadium);
        Assert.AreEqual(description, stadium.Description);
    }

    private static Stadium CreateValidStadium()
    {
        return new Stadium(ValidName, ValidCity, ValidDescription, ValidCapacity);
    }
}