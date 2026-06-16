namespace Domain;

public class KnockoutSimulator
{
    private readonly int _seed;
    private readonly ISimulationEngine _simulationEngine;

    public KnockoutSimulator(int seed, ISimulationEngine simulationEngine)
    {
        _seed = seed;
        _simulationEngine = simulationEngine;
    }

    public void Simulate(IEnumerable<Match> matches)
    {
        var random = new Random(_seed);

        foreach (var match in matches)
        {
            if (match.Status == MatchStatus.Finished)
                continue;

            var result = _simulationEngine.Simulate(
                match.HomeTeam,
                match.AwayTeam,
                match.Phase,
                random);

            match.ApplySimulationResult(result);

            var cardGenerator = new CardGenerator(random);
            match.ClearCards();
            RegisterCards(match, cardGenerator, result);
        }
    }

    private static void RegisterCards(Match match, CardGenerator cardGenerator, SimulationResult result)
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
