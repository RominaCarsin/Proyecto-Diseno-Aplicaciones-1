namespace Domain.Test;

[TestClass]
public class CardGeneratorTests
{
    private static Team BuildTeam(string name) =>
        new(name, "CONMEBOL", 1800);

    [TestMethod]
    public void Constructor_WithNullRandom_ShouldThrowArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => new CardGenerator(null));
    }

    [TestMethod]
    public void Constructor_WithValidRandom_ShouldNotThrow()
    {
        new CardGenerator(new Random(42));
    }

    [TestMethod]
    public void GenerateYellowCards_WithPositiveCount_ShouldCreateCorrectNumberOfCards()
    {
        var team = BuildTeam("URUGUAY");
        var generator = new CardGenerator(new Random(42));

        var cards = generator.GenerateYellowCards(team, 3);

        Assert.AreEqual(3, cards.Length);
        Assert.IsTrue(cards.All(c => c is YellowCard));
        Assert.IsTrue(cards.All(c => ReferenceEquals(c.Team, team)));
    }

    [TestMethod]
    public void GenerateYellowCards_WithZeroCount_ShouldReturnEmpty()
    {
        var generator = new CardGenerator(new Random(42));

        var cards = generator.GenerateYellowCards(BuildTeam("URUGUAY"), 0);

        Assert.AreEqual(0, cards.Length);
    }

    [TestMethod]
    public void GenerateYellowCards_WithNegativeCount_ShouldThrowArgumentOutOfRangeException()
    {
        var generator = new CardGenerator(new Random(42));

        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            generator.GenerateYellowCards(BuildTeam("URUGUAY"), -1));
    }

    [TestMethod]
    public void GenerateRedCards_WithPositiveCount_ShouldCreateCorrectNumberOfCards()
    {
        var team = BuildTeam("URUGUAY");
        var generator = new CardGenerator(new Random(42));

        var cards = generator.GenerateRedCards(team, 3);

        Assert.AreEqual(3, cards.Length);
        Assert.IsTrue(cards.All(c => c is RedCard));
        Assert.IsTrue(cards.All(c => ReferenceEquals(c.Team, team)));
    }

    [TestMethod]
    public void GenerateRedCards_WithZeroCount_ShouldReturnEmpty()
    {
        var generator = new CardGenerator(new Random(42));

        var cards = generator.GenerateRedCards(BuildTeam("URUGUAY"), 0);

        Assert.AreEqual(0, cards.Length);
    }

    [TestMethod]
    public void GenerateRedCards_WithNegativeCount_ShouldThrowArgumentOutOfRangeException()
    {
        var generator = new CardGenerator(new Random(42));

        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            generator.GenerateRedCards(BuildTeam("URUGUAY"), -1));
    }

    [TestMethod]
    public void GenerateYellowCards_MinutesShouldBeWithinValidRange()
    {
        var team = BuildTeam("URUGUAY");
        var generator = new CardGenerator(new Random(42));

        var cards = generator.GenerateYellowCards(team, 50);

        Assert.IsTrue(cards.All(c => c.Minute >= MatchIncidence.MinMinute && c.Minute <= MatchIncidence.MaxMinute));
    }

    [TestMethod]
    public void GenerateRedCards_MinutesShouldBeWithinValidRange()
    {
        var team = BuildTeam("URUGUAY");
        var generator = new CardGenerator(new Random(42));

        var cards = generator.GenerateRedCards(team, 50);

        Assert.IsTrue(cards.All(c => c.Minute >= MatchIncidence.MinMinute && c.Minute <= MatchIncidence.MaxMinute));
    }

    [TestMethod]
    public void GenerateCards_WithSameSeed_ShouldProduceDeterministicMinutes()
    {
        var team = BuildTeam("URUGUAY");

        var generator1 = new CardGenerator(new Random(42));
        var cards1 = generator1.GenerateYellowCards(team, 5);

        var generator2 = new CardGenerator(new Random(42));
        var cards2 = generator2.GenerateYellowCards(team, 5);

        for (int i = 0; i < 5; i++)
            Assert.AreEqual(cards1[i].Minute, cards2[i].Minute);
    }

    [TestMethod]
    public void GenerateCards_WithDifferentSeed_ShouldProduceDifferentMinutes()
    {
        var team = BuildTeam("URUGUAY");

        var generator1 = new CardGenerator(new Random(42));
        var cards1 = generator1.GenerateYellowCards(team, 10);

        var generator2 = new CardGenerator(new Random(99));
        var cards2 = generator2.GenerateYellowCards(team, 10);

        var minutes1 = cards1.Select(c => c.Minute).ToList();
        var minutes2 = cards2.Select(c => c.Minute).ToList();
        CollectionAssert.AreNotEqual(minutes1, minutes2);
    }
}
