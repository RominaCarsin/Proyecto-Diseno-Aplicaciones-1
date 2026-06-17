namespace Domain.Test;

[TestClass]
public class StadiumTests
{
    private const string ValidName = "CENTENARIO";
    private const string ValidCity = "MONTEVIDEO";
    private const string ValidDescription = "Historic stadium";
    private const int ValidCapacity = 60000;

    // ─── Constructor ──────────────────────────────────────────────────────────

    [TestMethod]
    public void CreateStadium_WithValidData_ShouldCreateStadiumWithExpectedProperties()
    {
        var stadium = CreateValidStadium();

        Assert.AreEqual(ValidName, stadium.Name);
        Assert.AreEqual(ValidCity, stadium.City);
        Assert.AreEqual(ValidDescription, stadium.Description);
        Assert.AreEqual(ValidCapacity, stadium.Capacity);
    }

    // ─── Nombre ───────────────────────────────────────────────────────────────

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
        Assert.ThrowsException<ArgumentException>(() =>
            new Stadium("", ValidCity, ValidDescription, ValidCapacity));
    }

    [TestMethod]
    public void CreateStadium_WithNullName_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Stadium(null, ValidCity, ValidDescription, ValidCapacity));
    }

    [TestMethod]
    public void CreateStadium_WithWhitespaceOnlyName_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Stadium("   ", ValidCity, ValidDescription, ValidCapacity));
    }

    [TestMethod]
    public void CreateStadium_WithNameOneAboveMaxLength_ShouldThrowException()
    {
        var nameOneAboveMaxLength = new string('A', 81);

        Assert.ThrowsException<ArgumentException>(() =>
            new Stadium(nameOneAboveMaxLength, ValidCity, ValidDescription, ValidCapacity));
    }

    // ─── Ciudad ───────────────────────────────────────────────────────────────

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
        Assert.ThrowsException<ArgumentException>(() =>
            new Stadium(ValidName, "", ValidDescription, ValidCapacity));
    }

    [TestMethod]
    public void CreateStadium_WithNullCity_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Stadium(ValidName, null, ValidDescription, ValidCapacity));
    }

    [TestMethod]
    public void CreateStadium_WithWhitespaceOnlyCity_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Stadium(ValidName, "   ", ValidDescription, ValidCapacity));
    }

    [TestMethod]
    public void CreateStadium_WithCityOneAboveMaxLength_ShouldThrowException()
    {
        var cityOneAboveMaxLength = new string('A', 61);

        Assert.ThrowsException<ArgumentException>(() =>
            new Stadium(ValidName, cityOneAboveMaxLength, ValidDescription, ValidCapacity));
    }

    // ─── Descripción ──────────────────────────────────────────────────────────

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

        Assert.ThrowsException<ArgumentException>(() =>
            new Stadium(ValidName, ValidCity, descriptionOneAboveMaxLength, ValidCapacity));
    }

    // ─── Capacidad ────────────────────────────────────────────────────────────

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
        Assert.ThrowsException<ArgumentException>(() =>
            new Stadium(ValidName, ValidCity, ValidDescription, 19999));
    }

    [TestMethod]
    public void CreateStadium_WithCapacityOfOne_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Stadium(ValidName, ValidCity, ValidDescription, 1));
    }

    // ─── Normalización combinada ──────────────────────────────────────────────

    [TestMethod]
    public void CreateStadium_WithLowercaseAndSpacesInNameAndCity_ShouldNormalizeBothFields()
    {
        var stadium = new Stadium("  san juan  ", "  buenos aires  ", ValidDescription, ValidCapacity);

        Assert.AreEqual("SAN JUAN", stadium.Name);
        Assert.AreEqual("BUENOS AIRES", stadium.City);
    }

    // ─── Update Name ─────────────────────────────────────────────────────────

    [TestMethod]
    public void UpdateName_WithValidName_ShouldUpdateName()
    {
        var stadium = CreateValidStadium();

        stadium.UpdateName("CAMPEON DEL SIGLO");

        Assert.AreEqual("CAMPEON DEL SIGLO", stadium.Name);
    }

    [TestMethod]
    public void UpdateName_WithLowercaseAndSpaces_ShouldNormalizeName()
    {
        var stadium = CreateValidStadium();

        stadium.UpdateName("  gran parque central  ");

        Assert.AreEqual("GRAN PARQUE CENTRAL", stadium.Name);
    }

    [TestMethod]
    public void UpdateName_WithEmptyName_ShouldThrowException()
    {
        var stadium = CreateValidStadium();

        Assert.ThrowsException<ArgumentException>(() =>
            stadium.UpdateName(""));
    }

    [TestMethod]
    public void UpdateName_WithNullName_ShouldThrowException()
    {
        var stadium = CreateValidStadium();

        Assert.ThrowsException<ArgumentException>(() =>
            stadium.UpdateName(null));
    }

    [TestMethod]
    public void UpdateName_WithNameOneAboveMaxLength_ShouldThrowException()
    {
        var stadium = CreateValidStadium();
        var nameOneAboveMaxLength = new string('A', 81);

        Assert.ThrowsException<ArgumentException>(() =>
            stadium.UpdateName(nameOneAboveMaxLength));
    }
    
    // ─── Update City ─────────────────────────────────────────────────────────

    [TestMethod]
    public void UpdateCity_WithValidCity_ShouldUpdateCity()
    {
        var stadium = CreateValidStadium();

        stadium.UpdateCity("MALDONADO");

        Assert.AreEqual("MALDONADO", stadium.City);
    }

    [TestMethod]
    public void UpdateCity_WithLowercaseAndSpaces_ShouldNormalizeCity()
    {
        var stadium = CreateValidStadium();

        stadium.UpdateCity("  punta del este  ");

        Assert.AreEqual("PUNTA DEL ESTE", stadium.City);
    }

    [TestMethod]
    public void UpdateCity_WithEmptyCity_ShouldThrowException()
    {
        var stadium = CreateValidStadium();

        Assert.ThrowsException<ArgumentException>(() =>
            stadium.UpdateCity(""));
    }

    [TestMethod]
    public void UpdateCity_WithNullCity_ShouldThrowException()
    {
        var stadium = CreateValidStadium();

        Assert.ThrowsException<ArgumentException>(() =>
            stadium.UpdateCity(null));
    }

    [TestMethod]
    public void UpdateCity_WithCityOneAboveMaxLength_ShouldThrowException()
    {
        var stadium = CreateValidStadium();
        var cityOneAboveMaxLength = new string('A', 61);

        Assert.ThrowsException<ArgumentException>(() =>
            stadium.UpdateCity(cityOneAboveMaxLength));
    }
    
    // ─── Update Description ──────────────────────────────────────────────────

    [TestMethod]
    public void UpdateDescription_WithValidDescription_ShouldUpdateDescription()
    {
        var stadium = CreateValidStadium();

        stadium.UpdateDescription("Updated description");

        Assert.AreEqual("Updated description", stadium.Description);
    }

    [TestMethod]
    public void UpdateDescription_WithNullDescription_ShouldUpdateDescriptionToNull()
    {
        var stadium = CreateValidStadium();

        stadium.UpdateDescription(null);

        Assert.IsNull(stadium.Description);
    }

    [TestMethod]
    public void UpdateDescription_WithEmptyDescription_ShouldUpdateDescriptionToEmpty()
    {
        var stadium = CreateValidStadium();

        stadium.UpdateDescription("");

        Assert.AreEqual("", stadium.Description);
    }

    [TestMethod]
    public void UpdateDescription_WithDescriptionExactlyAtMaxLength_ShouldUpdateDescriptionSuccessfully()
    {
        var stadium = CreateValidStadium();
        var descriptionWithMaxLength = new string('A', 400);

        stadium.UpdateDescription(descriptionWithMaxLength);

        Assert.AreEqual(descriptionWithMaxLength, stadium.Description);
    }

    [TestMethod]
    public void UpdateDescription_WithDescriptionOneAboveMaxLength_ShouldThrowException()
    {
        var stadium = CreateValidStadium();
        var descriptionOneAboveMaxLength = new string('A', 401);

        Assert.ThrowsException<ArgumentException>(() =>
            stadium.UpdateDescription(descriptionOneAboveMaxLength));
    }
    
    // ─── Update Capacity ─────────────────────────────────────────────────────

    [TestMethod]
    public void UpdateCapacity_WithValidCapacity_ShouldUpdateCapacity()
    {
        var stadium = CreateValidStadium();

        stadium.UpdateCapacity(70000);

        Assert.AreEqual(70000, stadium.Capacity);
    }

    [TestMethod]
    public void UpdateCapacity_WithCapacityExactlyAtMinimum_ShouldUpdateCapacitySuccessfully()
    {
        var stadium = CreateValidStadium();

        stadium.UpdateCapacity(20000);

        Assert.AreEqual(20000, stadium.Capacity);
    }

    [TestMethod]
    public void UpdateCapacity_WithCapacityOneBelowMinimum_ShouldThrowException()
    {
        var stadium = CreateValidStadium();

        Assert.ThrowsException<ArgumentException>(() =>
            stadium.UpdateCapacity(19999));
    }
    
    // ─── StadiumId ────────────────────────────────────────────────────────────

    [TestMethod]
    public void CreateStadium_StadiumId_ShouldDefaultToZero()
    {
        var stadium = CreateValidStadium();

        Assert.AreEqual(0, stadium.StadiumId);
    }

    // ─── Update boundary tests ────────────────────────────────────────────────

    [TestMethod]
    public void UpdateName_WithNameExactlyAtMaxLength_ShouldUpdateNameSuccessfully()
    {
        var stadium = CreateValidStadium();
        var nameWithMaxLength = new string('A', 80);

        stadium.UpdateName(nameWithMaxLength);

        Assert.AreEqual(nameWithMaxLength, stadium.Name);
    }

    [TestMethod]
    public void UpdateCity_WithCityExactlyAtMaxLength_ShouldUpdateCitySuccessfully()
    {
        var stadium = CreateValidStadium();
        var cityWithMaxLength = new string('A', 60);

        stadium.UpdateCity(cityWithMaxLength);

        Assert.AreEqual(cityWithMaxLength, stadium.City);
    }

    [TestMethod]
    public void UpdateCapacity_WithVeryHighCapacity_ShouldUpdateCapacitySuccessfully()
    {
        var stadium = CreateValidStadium();

        stadium.UpdateCapacity(200000);

        Assert.AreEqual(200000, stadium.Capacity);
    }

    [TestMethod]
    public void UpdateDescription_WithWhitespaceDescription_ShouldStoreWhitespace()
    {
        var stadium = CreateValidStadium();

        stadium.UpdateDescription("   ");

        Assert.AreEqual("   ", stadium.Description);
    }

    // ─── Builders ─────────────────────────────────────────────────────────────

    private static Stadium CreateValidStadium() =>
        new Stadium(ValidName, ValidCity, ValidDescription, ValidCapacity);
}