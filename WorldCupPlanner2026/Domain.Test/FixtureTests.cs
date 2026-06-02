namespace Domain.Tests;

[TestClass]
public class FixtureTests
{
    private static readonly DateTime StartDate = new(2026, 6, 1);

    private static List<Team> BuildTeams(string confederation, string prefix, int count, int startRanking)
    {
        var teams = new List<Team>(count);

        for (int i = 0; i < count; i++)
            teams.Add(new Team($"{prefix}{i:00}", confederation, startRanking + i * 7));

        return teams;
    }

    private static List<Pot> BuildValidPots() => new()
    {
        new Pot(1, BuildTeams("UEFA", "UEA-", 12, 600)),
        new Pot(2, BuildTeams("UEFA", "UEB-", 12, 800)),
        new Pot(3, BuildTeams("CONMEBOL", "CON-", 12, 1000)),
        new Pot(4, BuildTeams("AFC", "AFC-", 12, 1200)),
    };

    private static List<Stadium> BuildStadiums(int count)
    {
        var stadiums = new List<Stadium>(count);

        for (int i = 0; i < count; i++)
            stadiums.Add(new Stadium($"Stadium {i:00}", "City", "desc", 30000));

        return stadiums;
    }

    [TestMethod]
    public void CreateFixture_WithValidPotsStadiumsAndStartDate_ShouldCreateWithExpectedProperties()
    {
        var fixture = new Fixture(BuildValidPots(), BuildStadiums(4), StartDate);

        Assert.IsNotNull(fixture);
        Assert.IsNotNull(fixture.Groups);
        Assert.AreEqual(0, fixture.Groups.Count);
    }
    
    [TestMethod]
    public void Generate_WithValidData_ShouldCreateTwelveGroups()
    {
        var fixture = new Fixture(BuildValidPots(), BuildStadiums(4), StartDate);

        fixture.Generate();

        Assert.AreEqual(12, fixture.Groups.Count);
    }
    
    [TestMethod]
    public void Generate_WithLessThanFortyEightTeams_ShouldThrowException()
    {
        var fixture = new Fixture(BuildPotsWithLessThanRequiredTeams(), BuildStadiums(4), StartDate);

        var exception = Assert.ThrowsException<InvalidOperationException>(() => fixture.Generate());

        Assert.AreEqual("Se necesitan 48 equipos para generar el fixture.", exception.Message);
    }
    
    private static List<Pot> BuildPotsWithLessThanRequiredTeams() => new()
    {
        new Pot(1, BuildTeams("UEFA", "A-", 12, 600)),
        new Pot(2, BuildTeams("CONMEBOL", "B-", 12, 800)),
        new Pot(3, BuildTeams("AFC", "C-", 12, 1000)),
    };
    
    [TestMethod]
    public void Generate_WithLessThanFourStadiums_ShouldThrowException()
    {
        var fixture = new Fixture(BuildValidPots(), BuildStadiums(3), StartDate);

        var exception = Assert.ThrowsException<InvalidOperationException>(() => fixture.Generate());

        Assert.AreEqual("Se necesitan al menos 4 estadios para generar el fixture.", exception.Message);
    }
}