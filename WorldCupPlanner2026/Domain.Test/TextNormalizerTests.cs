namespace Domain.Test;

[TestClass]
public class TextNormalizerTests
{
    [TestMethod]
    public void Normalize_WithLowercaseText_ShouldReturnUppercaseText()
    {
        string value = "uruguay";

        string result = TextNormalizer.Normalize(value);

        Assert.AreEqual("URUGUAY", result);
    }

    [TestMethod]
    public void Normalize_WithTextContainingSpaces_ShouldTrimText()
    {
        string value = "  uruguay  ";

        string result = TextNormalizer.Normalize(value);

        Assert.AreEqual("URUGUAY", result);
    }

    [TestMethod]
    public void Normalize_WithAlreadyUppercaseText_ShouldReturnSameText()
    {
        string value = "CONMEBOL";

        string result = TextNormalizer.Normalize(value);

        Assert.AreEqual("CONMEBOL", result);
    }

    [TestMethod]
    public void Normalize_WithMixedCaseText_ShouldReturnUppercaseText()
    {
        string value = "ConMeBol";

        string result = TextNormalizer.Normalize(value);

        Assert.AreEqual("CONMEBOL", result);
    }

    [TestMethod]
    public void Normalize_WithEmptyText_ShouldReturnEmptyText()
    {
        string value = "";

        string result = TextNormalizer.Normalize(value);

        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void Normalize_WithOnlySpaces_ShouldReturnEmptyText()
    {
        string value = "     ";

        string result = TextNormalizer.Normalize(value);

        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void Normalize_WithNullText_ShouldReturnEmptyText()
    {
        string? value = null;

        string result = TextNormalizer.Normalize(value!);

        Assert.AreEqual(string.Empty, result);
    }
}