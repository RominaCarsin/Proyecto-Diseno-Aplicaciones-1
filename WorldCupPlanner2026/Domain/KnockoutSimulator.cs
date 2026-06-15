namespace Domain;

public class KnockoutSimulator
{
    private readonly int _seed;
    private readonly ISimulationEngine _simulationEngine;

    public KnockoutSimulator(int seed)
        : this(seed, SimulationEngineFactory.Create(SimulationEngineType.Probabilistic))
    {
    }

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

            match.SetSimulationResult(result, random);
        }
    }
}