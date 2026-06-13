namespace Domain;

public static class SimulationEngineFactory
{
    public static ISimulationEngine Create(SimulationEngineType type)
    {
        if (type == SimulationEngineType.PureRandom)
            return new PureRandomSimulationEngine();

        return new ProbabilisticSimulationEngine();
    }
}