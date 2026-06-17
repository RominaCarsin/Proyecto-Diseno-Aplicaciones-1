namespace Domain;

public static class CardRegistration
{
    public static void RegisterCards(Match match, CardGenerator cardGenerator, SimulationResult result)
    {
        foreach (var card in cardGenerator.GenerateYellowCards(match.HomeTeam, result.HomeYellowCards))
            match.RegisterIncidence(card);
        foreach (var card in cardGenerator.GenerateYellowCards(match.AwayTeam, result.AwayYellowCards))
            match.RegisterIncidence(card);
        foreach (var card in cardGenerator.GenerateRedCards(match.HomeTeam, result.HomeRedCards))
            match.RegisterIncidence(card);
        foreach (var card in cardGenerator.GenerateRedCards(match.AwayTeam, result.AwayRedCards))
            match.RegisterIncidence(card);
    }
}
