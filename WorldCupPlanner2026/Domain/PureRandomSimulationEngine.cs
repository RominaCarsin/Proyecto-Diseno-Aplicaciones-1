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
        if (phase == TournamentPhase.GroupStage)
        {
            return new SimulationResult(
                random.Next(0, 5),
                random.Next(0, 5));
        }

        bool homeWins = random.Next(0, 2) == 0;

        if (homeWins)
            return new SimulationResult(
                random.Next(2, 4),
                random.Next(0, 2));

        return new SimulationResult(
            random.Next(0, 2),
            random.Next(2, 4));
    }
}