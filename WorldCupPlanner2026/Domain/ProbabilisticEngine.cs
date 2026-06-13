namespace Domain;

public sealed class ProbabilisticEngine : ISimulationEngine
{
    public string Name => "Probabilístico";

    public (int homeGoals, int awayGoals) SimulateResult(
        int homeRanking, int awayRanking, bool allowDraw, Random random)
        => (random.Next(0, 5), random.Next(0, 5));
    
    
}