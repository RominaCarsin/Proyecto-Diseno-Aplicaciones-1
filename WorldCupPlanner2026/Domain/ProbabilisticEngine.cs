namespace Domain;

public sealed class ProbabilisticEngine : ISimulationEngine
{
    public string Name => "Probabilístico";

    public (int homeGoals, int awayGoals) SimulateResult(
        int homeRanking, int awayRanking, bool allowDraw, Random random)
        => throw new NotImplementedException();
}