namespace Domain;

public interface ISimulationEngine
{
    string Name { get; }
    (int homeGoals, int awayGoals) SimulateResult(
        int homeRanking, int awayRanking, bool allowDraw, Random random);
}