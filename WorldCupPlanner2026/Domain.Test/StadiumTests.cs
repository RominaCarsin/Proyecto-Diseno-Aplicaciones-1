namespace Domain.Test;

[TestClass]
public class StadiumTests
{
    private const string ValidName = "CENTENARIO";
    private const string ValidCity = "MONTEVIDEO";
    private const string ValidDescription = "Historic stadium";
    private const int ValidCapacity = 60000;


    [TestMethod]
    public void CreateStadium_WithValidData_ShouldCreateStadiumWithExpectedProperties()
    {
        var stadium = CreateValidStadium();

        Assert.AreEqual(ValidName, stadium.Name);
        Assert.AreEqual(ValidCity, stadium.City);
        Assert.AreEqual(ValidDescription, stadium.Description);
        Assert.AreEqual(ValidCapacity, stadium.Capacity);
    }


    [TestMethod]
    public void CreateStadium_WithLowercaseName_ShouldStoreNameInUpperCase()
    {
        var stadium = new Stadium("centenario", ValidCity, ValidDescription, ValidCapacity);

        Assert.AreEqual("CENTENARIO", stadium.Name);
    }

    [TestMethod]
    public void CreateStadium_WithMixedCaseName_ShouldStoreNameInUpperCase()
    {
        var stadium = new Stadium("cenTENario", ValidCity, ValidDescription, ValidCapacity);

        Assert.AreEqual("CENTENARIO", stadium.Name);
    }

    [TestMethod]
    public void CreateStadium_WithLeadingAndTrailingSpacesInName_ShouldStoreTrimmedName()
    {
        var stadium = new Stadium("  CENTENARIO  ", ValidCity, ValidDescription, ValidCapacity);

        Assert.AreEqual("CENTENARIO", stadium.Name);
    }

    [TestMethod]
    public void CreateStadium_WithNameExactlyAtMaxLength_ShouldStoreNameSuccessfully()
    {
        var nameWithMaxLength = new string('A', 80);

        var stadium = new Stadium(nameWithMaxLength, ValidCity, ValidDescription, ValidCapacity);

        Assert.AreEqual(nameWithMaxLength, stadium.Name);
    }

    [TestMethod]
    public void CreateStadium_WithNameOneBelowMaxLength_ShouldStoreNameSuccessfully()
    {
        var nameOneBelowMaxLength = new string('A', 79);

        var stadium = new Stadium(nameOneBelowMaxLength, ValidCity, ValidDescription, ValidCapacity);

        Assert.AreEqual(nameOneBelowMaxLength, stadium.Name);
    }

    [TestMethod]
    public void CreateStadium_WithEmptyName_ShouldThrowException()
    {
        Assert.ThrowsException<Exception>(() =>
            new Stadium("", ValidCity, ValidDescription, ValidCapacity));
    }

    [TestMethod]
    public void CreateStadium_WithNullName_ShouldThrowException()
    {
        Assert.ThrowsException<Exception>(() =>
            new Stadium(null, ValidCity, ValidDescription, ValidCapacity));
    }

    [TestMethod]
    public void CreateStadium_WithWhitespaceOnlyName_ShouldThrowException()
    {
        Assert.ThrowsException<Exception>(() =>
            new Stadium("   ", ValidCity, ValidDescription, ValidCapacity));
    }

    [TestMethod]
    public void CreateStadium_WithNameOneAboveMaxLength_ShouldThrowException()
    {
        var nameOneAboveMaxLength = new string('A', 81);

        Assert.ThrowsException<Exception>(() =>
            new Stadium(nameOneAboveMaxLength, ValidCity, ValidDescription, ValidCapacity));
    }


    [TestMethod]
    public void CreateStadium_WithLowercaseCity_ShouldStoreCityInUpperCase()
    {
        var stadium = new Stadium(ValidName, "montevideo", ValidDescription, ValidCapacity);

        Assert.AreEqual("MONTEVIDEO", stadium.City);
    }

    [TestMethod]
    public void CreateStadium_WithMixedCaseCity_ShouldStoreCityInUpperCase()
    {
        var stadium = new Stadium(ValidName, "monTeVidEo", ValidDescription, ValidCapacity);

        Assert.AreEqual("MONTEVIDEO", stadium.City);
    }

    [TestMethod]
    public void CreateStadium_WithLeadingAndTrailingSpacesInCity_ShouldStoreTrimmedCity()
    {
        var stadium = new Stadium(ValidName, "  MONTEVIDEO  ", ValidDescription, ValidCapacity);

        Assert.AreEqual("MONTEVIDEO", stadium.City);
    }

    [TestMethod]
    public void CreateStadium_WithCityExactlyAtMaxLength_ShouldStoreCitySuccessfully()
    {
        var cityWithMaxLength = new string('A', 60);

        var stadium = new Stadium(ValidName, cityWithMaxLength, ValidDescription, ValidCapacity);

        Assert.AreEqual(cityWithMaxLength, stadium.City);
    }

    [TestMethod]
    public void CreateStadium_WithCityOneBelowMaxLength_ShouldStoreCitySuccessfully()
    {
        var cityOneBelowMaxLength = new string('A', 59);

        var stadium = new Stadium(ValidName, cityOneBelowMaxLength, ValidDescription, ValidCapacity);

        Assert.AreEqual(cityOneBelowMaxLength, stadium.City);
    }

    [TestMethod]
    public void CreateStadium_WithEmptyCity_ShouldThrowException()
    {
        Assert.ThrowsException<Exception>(() =>
            new Stadium(ValidName, "", ValidDescription, ValidCapacity));
    }

    [TestMethod]
    public void CreateStadium_WithNullCity_ShouldThrowException()
    {
        Assert.ThrowsException<Exception>(() =>
            new Stadium(ValidName, null, ValidDescription, ValidCapacity));
    }

    [TestMethod]
    public void CreateStadium_WithWhitespaceOnlyCity_ShouldThrowException()
    {
        Assert.ThrowsException<Exception>(() =>
            new Stadium(ValidName, "   ", ValidDescription, ValidCapacity));
    }

    [TestMethod]
    public void CreateStadium_WithCityOneAboveMaxLength_ShouldThrowException()
    {
        var cityOneAboveMaxLength = new string('A', 61);

        Assert.ThrowsException<Exception>(() =>
            new Stadium(ValidName, cityOneAboveMaxLength, ValidDescription, ValidCapacity));
    }
    

    [TestMethod]
    public void CreateStadium_WithNullDescription_ShouldStoreNullDescription()
    {
        var stadium = new Stadium(ValidName, ValidCity, null, ValidCapacity);

        Assert.IsNull(stadium.Description);
    }

    [TestMethod]
    public void CreateStadium_WithEmptyDescription_ShouldStoreEmptyDescription()
    {
        var stadium = new Stadium(ValidName, ValidCity, "", ValidCapacity);

        Assert.AreEqual("", stadium.Description);
    }

    [TestMethod]
    public void CreateStadium_WithDescriptionExactlyAtMaxLength_ShouldStoreDescriptionSuccessfully()
    {
        var descriptionWithMaxLength = new string('A', 400);

        var stadium = new Stadium(ValidName, ValidCity, descriptionWithMaxLength, ValidCapacity);

        Assert.AreEqual(descriptionWithMaxLength, stadium.Description);
    }

    [TestMethod]
    public void CreateStadium_WithDescriptionOneBelowMaxLength_ShouldStoreDescriptionSuccessfully()
    {
        var descriptionOneBelowMaxLength = new string('A', 399);

        var stadium = new Stadium(ValidName, ValidCity, descriptionOneBelowMaxLength, ValidCapacity);

        Assert.AreEqual(descriptionOneBelowMaxLength, stadium.Description);
    }

    [TestMethod]
    public void CreateStadium_WithDescriptionOneAboveMaxLength_ShouldThrowException()
    {
        var descriptionOneAboveMaxLength = new string('A', 401);

        Assert.ThrowsException<Exception>(() =>
            new Stadium(ValidName, ValidCity, descriptionOneAboveMaxLength, ValidCapacity));
    }
    

    [TestMethod]
    public void CreateStadium_WithCapacityExactlyAtMinimum_ShouldStoreCapacitySuccessfully()
    {
        var stadium = new Stadium(ValidName, ValidCity, ValidDescription, 20000);

        Assert.AreEqual(20000, stadium.Capacity);
    }

    [TestMethod]
    public void CreateStadium_WithCapacityOneAboveMinimum_ShouldStoreCapacitySuccessfully()
    {
        var stadium = new Stadium(ValidName, ValidCity, ValidDescription, 20001);

        Assert.AreEqual(20001, stadium.Capacity);
    }

    [TestMethod]
    public void CreateStadium_WithVeryHighCapacity_ShouldStoreCapacitySuccessfully()
    {
        var stadium = new Stadium(ValidName, ValidCity, ValidDescription, 200000);

        Assert.AreEqual(200000, stadium.Capacity);
    }

    [TestMethod]
    public void CreateStadium_WithCapacityOneBelowMinimum_ShouldThrowException()
    {
        Assert.ThrowsException<Exception>(() =>
            new Stadium(ValidName, ValidCity, ValidDescription, 19999));
    }

    [TestMethod]
    public void CreateStadium_WithCapacityOfOne_ShouldThrowException()
    {
        Assert.ThrowsException<Exception>(() =>
            new Stadium(ValidName, ValidCity, ValidDescription, 1));
    }
    

    [TestMethod]
    public void CreateStadium_WithLowercaseAndSpacesInNameAndCity_ShouldNormalizeBothFields()
    {
        var stadium = new Stadium("  san juan  ", "  buenos aires  ", ValidDescription, ValidCapacity);

        Assert.AreEqual("SAN JUAN", stadium.Name);
        Assert.AreEqual("BUENOS AIRES", stadium.City);
    }
    

    private static Stadium CreateValidStadium() =>
        new Stadium(ValidName, ValidCity, ValidDescription, ValidCapacity);
}