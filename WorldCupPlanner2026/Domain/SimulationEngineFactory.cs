namespace Domain;

public static class SimulationEngineFactory
{
    public static ISimulationEngine Create(SimulationEngineType type)
    {
        return new ProbabilisticSimulationEngine();
    }
}