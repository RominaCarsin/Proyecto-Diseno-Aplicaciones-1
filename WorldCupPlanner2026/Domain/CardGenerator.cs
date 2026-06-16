namespace Domain;

public class CardGenerator
{
    private const string NullRandomErrorMessage = "El generador de números aleatorios no puede ser nulo.";
    private const string NegativeCardsErrorMessage = "Las tarjetas no pueden ser negativas.";

    private readonly Random _random;

    public CardGenerator(Random? random)
    {
        if (random is null)
            throw new ArgumentNullException(nameof(random), NullRandomErrorMessage);
        _random = random;
    }

    public YellowCard[] GenerateYellowCards(Team team, int count)
    {
        ValidateCount(count);
        var cards = new YellowCard[count];
        for (int i = 0; i < count; i++)
            cards[i] = new YellowCard(team, _random.Next(MatchIncidence.MinMinute, MatchIncidence.MaxMinute + 1));
        return cards;
    }

    public RedCard[] GenerateRedCards(Team team, int count)
    {
        ValidateCount(count);
        var cards = new RedCard[count];
        for (int i = 0; i < count; i++)
            cards[i] = new RedCard(team, _random.Next(MatchIncidence.MinMinute, MatchIncidence.MaxMinute + 1));
        return cards;
    }

    private static void ValidateCount(int count)
    {
        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count), NegativeCardsErrorMessage);
    }
}
