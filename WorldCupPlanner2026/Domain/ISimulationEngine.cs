namespace Domain;

public interface ISimulationEngine
{
    string Name { get; }

    SimulationResult Simulate(
        Team homeTeam,
        Team awayTeam,
        TournamentPhase phase,
        Random random);
}