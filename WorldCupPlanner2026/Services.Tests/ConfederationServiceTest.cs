namespace Services.Tests;

[TestClass]
public class ConfederationServiceTest
{
    private ConfederationService _service;

    [TestInitialize]
    public void Setup()
    {
        _service = new ConfederationService();
    }

    [TestMethod]
    public void GetAll_ShouldContainAllConfederationsWithCorrectSlots()
    {
        var expected = new Dictionary<string, int>
        {
            { "UEFA",     16 },
            { "CAF",       9 },
            { "AFC",       8 },
            { "CONCACAF",  7 },
            { "CONMEBOL",  7 },
            { "OFC",       1 },
        };

        var result = _service.GetAll().ToList();

        foreach (var (name, slots) in expected)
        {
            var conf = result.FirstOrDefault(c => c.Name == name);
            Assert.IsNotNull(conf, $"Confederation {name} not found");
            Assert.AreEqual(slots, conf.Slots, $"{name} should have {slots} slots");
        }
    }
}