namespace Domain;

public class PureRandomSimulationEngine : ISimulationEngine
{
    public string Name => "Aleatorio Puro";

    public SimulationResult Simulate(
        Team homeTeam,
        Team awayTeam,
        TournamentPhase phase,
        Random random)
    {
        return new SimulationResult(
            random.Next(0, 5),
            random.Next(0, 5));
    }
}