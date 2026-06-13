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
        throw new NotImplementedException();
    }
}