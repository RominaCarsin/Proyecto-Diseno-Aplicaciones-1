namespace IServices;

public interface ISimulationService
{
    void SimulateGroupMatch(char groupLabel, int matchId, int seed);
}